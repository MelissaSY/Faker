using System.Reflection;

namespace FakerDll
{
    internal class MaxSatisfied : IComparer<ConstructorInfo>
    {
        Dictionary<ConstructorInfo, int> constructors;
        public MaxSatisfied(Dictionary<ConstructorInfo, int> constructors)
        {
            this.constructors = constructors;
        }
        public int Compare(ConstructorInfo? x, ConstructorInfo? y)
        {
            if (x == null || y == null)
            {
                return 0;
            }
            if(!constructors.ContainsKey(x) || !constructors.ContainsKey(y))
            {
                return 0;
            }
            int compareResult = constructors[x].CompareTo(constructors[y]);
            return compareResult == 0 ? (-x.GetParameters().Length + y.GetParameters().Length) : compareResult;
        }
    }
    public class ClassGenerator : IValueGenerator
    {
        private Dictionary<Type, int> Types { get; set; }
        public bool CanGenerate(Type t)
        {
            return true;
        }
        public ClassGenerator()
        {
            Types = new Dictionary<Type, int>();
        }
        public object? Generate(Type t, GeneratorContext context)
        {
            if (!Types.ContainsKey(t))
            {
                Types.Add(t, 0);
            }
            if (this.Types[t] > 3)
            {
                return GetDefaultValue(t);
            }
            Types[t]++;
            Dictionary<MemberInfo, IValueGenerator>? constraits = null;
            Dictionary<PropertyInfo, IValueGenerator?> properties = new Dictionary<PropertyInfo, IValueGenerator?>();
            Dictionary<FieldInfo, IValueGenerator?> fields = new Dictionary<FieldInfo, IValueGenerator?>();
            context.Faker.Config?.generatorsConstraits.TryGetValue(t, out constraits);
            constraits = CopyConstraits(constraits);
            SetGenerators(t, ref fields, ref properties, ref constraits);
            object? newObj = TryActivate(t, context, constraits);
            InitializeFieldsProoperties(newObj, fields, properties, context);
            this.Types[t]--;
            return newObj;
        }
        private Dictionary<MemberInfo, IValueGenerator>? CopyConstraits(Dictionary<MemberInfo, IValueGenerator>? constraits)
        {
            if (constraits == null)
                return null;
            Dictionary<MemberInfo, IValueGenerator>? newConstraits = new Dictionary<MemberInfo, IValueGenerator>();
            foreach(MemberInfo member in constraits.Keys)
            {
                newConstraits.Add(member, constraits[member]);
            }
            return newConstraits;
        }
        private void InitializeFieldsProoperties(object? newObj, 
            Dictionary<FieldInfo, IValueGenerator?> fields, Dictionary<PropertyInfo, IValueGenerator?> properties, 
            GeneratorContext context)
        {
            if (newObj != null)
            {
                foreach (FieldInfo field in fields.Keys)
                {
                    try
                    {
                        context.Generator = fields[field];
                        field.SetValue(newObj, context.Faker.Create(field.FieldType));
                    }
                    catch { }
                }
                foreach (PropertyInfo property in properties.Keys)
                {
                    try
                    {
                        if (property.SetMethod.IsPublic)
                        {
                            context.Generator = properties[property];
                            property.SetValue(newObj, context.Faker.Create(property.PropertyType));
                        }
                    }
                    catch { }
                }
            }
        }
        private object? TryActivate(Type t, GeneratorContext context, Dictionary<MemberInfo, IValueGenerator>? constraits)
        {
            object? newObj = null;
            ConstructorInfo? constructor;
            List<ConstructorInfo> constructors = t.GetConstructors(BindingFlags.Public | BindingFlags.Instance).ToList();
            Dictionary<ConstructorInfo, int> constructorUnsatisfied = new Dictionary<ConstructorInfo, int>();
            Dictionary<ConstructorInfo, Dictionary<ParameterInfo, IValueGenerator?>> constructorParameters =
                new Dictionary<ConstructorInfo, Dictionary<ParameterInfo, IValueGenerator?>>();
            Dictionary<ConstructorInfo, ParameterInfo[]> parameters = new Dictionary<ConstructorInfo, ParameterInfo[]>();
            foreach (ConstructorInfo c in constructors)
            {
                int unsatisfied;
                Dictionary<ParameterInfo, IValueGenerator?> generParams;
                parameters.Add(c, c.GetParameters());
                (generParams, unsatisfied) = SetGenerators(parameters[c].ToList(), constraits);
                constructorUnsatisfied.Add(c, unsatisfied);
                constructorParameters.Add(c, generParams);
            }
            constructors.Sort(new MaxSatisfied(constructorUnsatisfied));
            while (constructors.Count > 0 && newObj == null)
            {
                constructor = constructors.First();
                object?[] objParameters = new object[constructorParameters[constructor].Count];
                for(int i = 0; i < objParameters.Length; i++)
                {
                    context.Generator = constructorParameters[constructor][parameters[constructor][i]];
                    try
                    {
                        objParameters[i] = context.Faker.Create(parameters[constructor][i].ParameterType);
                    }
                    catch { }
                }
                try
                {
                    newObj = Activator.CreateInstance(t, objParameters);
                }
                catch
                {
                    if (constructor != null)
                    {
                        constructors.Remove(constructor);
                        constructorParameters.Remove(constructor);
                    }
                }
            }
            if (newObj == null)
            {
                try
                {
                    newObj = Activator.CreateInstance(t);
                }
                catch { }
            }
            return newObj;
        }
        /// <summary>
        /// sets generators for public fields and properties with public setters;
        /// calls method FindGenerator which removes generators from _constraits
        /// </summary>
        /// <param name="t"></param>
        private void SetGenerators(Type t, ref Dictionary<FieldInfo, IValueGenerator?> _fields, 
            ref Dictionary<PropertyInfo, IValueGenerator?> _properties, ref Dictionary<MemberInfo, IValueGenerator>? _constraits)
        {
            FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            IValueGenerator? generator;
            foreach(FieldInfo field in fields)
            {
                generator = FindGenerator(field, ref _constraits);
                _fields.Add(field, generator);
            }
            foreach(PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    generator = FindGenerator(property, ref _constraits);
                    _properties.Add(property, generator);
                }
            }
            if(_constraits?.Count < 1)
            {
                _constraits = null;
            }
        }
        /// <summary>
        /// searches generator in private field _constraits and if found removes from dictionary
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private IValueGenerator? FindGenerator(MemberInfo member, ref Dictionary<MemberInfo, IValueGenerator>? _constraits)
        {
            IValueGenerator? generator = null;
            _constraits?.TryGetValue(member, out generator);
            if(generator != null)
            {
                _constraits?.Remove(member);
            }
            return generator;
        }
        /// <summary>
        /// searches genrators in _constraits and sets generators for parameters;
        /// if not found in _constraits sets null as a IValueGenerator?
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private (Dictionary<ParameterInfo, IValueGenerator?>, int unsatisfied) SetGenerators(List<ParameterInfo> parameters, Dictionary<MemberInfo, IValueGenerator>? _constraits)
        {
            Dictionary<ParameterInfo, IValueGenerator?> parametersGenerators = new Dictionary<ParameterInfo, IValueGenerator?>();
            if (_constraits == null)
            {
                foreach (ParameterInfo parameter in parameters)
                {
                    parametersGenerators.Add(parameter, null);
                }
                return (parametersGenerators, 0);
            }
            int unsatisfied = _constraits.Count;
            bool satisfied;
            foreach (KeyValuePair<MemberInfo, IValueGenerator> constrait in _constraits)
            {
                satisfied = false;
                int j = 0;
                for (; j < parameters.Count && !satisfied; j++)
                {
                    satisfied = SameProperty(constrait.Key, parameters[j]);
                }
                j--;
                if (satisfied)
                {
                    unsatisfied--;
                    parametersGenerators.Add(parameters[j], constrait.Value);
                    parameters.RemoveAt(j);
                }
            }
            foreach(ParameterInfo parameter in parameters)
            {
                parametersGenerators.Add(parameter, null);
            }
            return (parametersGenerators, unsatisfied);

        }
        private bool SameProperty(MemberInfo member, ParameterInfo parameter)
        {
            bool sameType = false;
            PropertyInfo? prop = member as PropertyInfo;
            if(prop != null)
            {
                sameType = prop.PropertyType == parameter.ParameterType;
            }
            FieldInfo? field = member as FieldInfo;
            if(field != null)
            {
                sameType = field.FieldType == parameter.ParameterType;
            }
            return string.Compare(member.Name, parameter.Name, true) == 0 && sameType;
        }
        private static object? GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);
            else
                return null;
        }
    }
}
