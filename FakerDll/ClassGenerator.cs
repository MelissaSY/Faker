using System.Reflection;

namespace FakerDll
{
    internal class MaxParametersComparerer : IComparer<ConstructorInfo>
    {
        public int Compare(ConstructorInfo? x, ConstructorInfo? y)
        {
            int? result = -x?.GetParameters().Length + y?.GetParameters().Length;
            return (int)(result == null ? 0 : result);
        }
    }
    internal class SuitableConstructor : IComparer<ConstructorInfo>
    {
        private Dictionary<MemberInfo, IValueGenerator>? _constraits;
        private Dictionary<PropertyInfo, IValueGenerator?> _properties;
        private Dictionary<FieldInfo, IValueGenerator?> _fields;

        public SuitableConstructor(Dictionary<MemberInfo, IValueGenerator>? constraits, Dictionary<PropertyInfo, IValueGenerator?> properties, Dictionary<FieldInfo, IValueGenerator?> fields)
        {
            _constraits = constraits;
            _properties = properties;
            _fields = fields;
        }
        public int Compare(ConstructorInfo? x, ConstructorInfo? y)
        {
            int? result = -x?.GetParameters().Length + y?.GetParameters().Length;
            return (int)(result == null ? 0 : result);
        }
    }
    public class ClassGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return true;
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            //_constraits may be redused after calling SetGenerators method
            Dictionary<MemberInfo, IValueGenerator>? constraits = null;
            //_fields and _properties will be initialized after method SetGenerators(Type) 
            Dictionary<PropertyInfo, IValueGenerator?> properties;
            Dictionary<FieldInfo, IValueGenerator?> fields;
            //_parameters will be initialized after calling method SelectConstructor
            Dictionary<ParameterInfo, IValueGenerator?> parameters;

            properties = new Dictionary<PropertyInfo, IValueGenerator?>();
            fields = new Dictionary<FieldInfo, IValueGenerator?>();
            parameters = new Dictionary<ParameterInfo, IValueGenerator?>();
            if (context.Faker.Types[t] > 3)
            {
                return null;
            }
            object? newObj = null;
            Type parameterType;
            ConstructorInfo? constructor;
            List<ConstructorInfo> constructors = t.GetConstructors(BindingFlags.Public | BindingFlags.Instance).ToList();
            constructors.Sort(new MaxParametersComparerer());
            context.Faker.Config?.generatorsConstraits.TryGetValue(t, out constraits);

            SetGenerators(t, ref fields, ref properties, ref constraits);

            while (constructors.Count > 0 && newObj == null)
            {
                constructor = SelectConstructor(constructors, ref parameters, ref constraits);

                if (parameters.Count > 0)
                {
                    object?[] objParameters = new object[parameters.Count];
                    int param = -1;
                    foreach(ParameterInfo parameter in parameters.Keys)
                    {
                        param++;
                        parameterType = parameter.ParameterType;
                        context.Generator = parameters[parameter];
                        objParameters[param] = context.Faker.Create(parameterType);
                    }
                    try
                    {
                        newObj = Activator.CreateInstance(t, objParameters);
                    }
                    catch
                    {
                        if(constructor != null)
                        {
                            constructors.Remove(constructor);
                        }
                    }
                    finally { }
                }
                else
                {
                    newObj = Activator.CreateInstance(t);
                }
            };

            if(newObj == null)
            {
                try
                {
                    newObj = Activator.CreateInstance(t);
                }
                catch { }
                finally { }
            }

            if(newObj != null)
            {
                foreach (FieldInfo field in fields.Keys)
                {
                    parameterType = field.FieldType;
                    context.Generator = fields[field];
                    field.SetValue(newObj, context.Faker.Create(parameterType));
                }
                foreach (PropertyInfo property in properties.Keys)
                {
                    parameterType = property.PropertyType;
                    context.Generator = properties[property];
                    property.SetValue(newObj, context.Faker.Create(parameterType));
                }
            }
            return newObj;
        }
        /// <summary>
        /// sets generators for public fields and properties with public setters;
        /// calls method FindGenerator which removes generators from _constraits
        /// </summary>
        /// <param name="t"></param>
        private void SetGenerators(Type t, ref Dictionary<FieldInfo, IValueGenerator?> _fields, ref Dictionary<PropertyInfo, IValueGenerator?> _properties, ref Dictionary<MemberInfo, IValueGenerator>? _constraits)
        {
            FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            IValueGenerator? generator;
            foreach(FieldInfo field in fields)
            {
                generator = FindGenerator(field, ref _constraits);
                _fields.Add(field, null);
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
        private (Dictionary<ParameterInfo, IValueGenerator?>, int unsatisfied) SetGenerators(List<ParameterInfo> parameters, ref Dictionary<MemberInfo, IValueGenerator>? _constraits)
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
        private ConstructorInfo? SelectConstructor(List<ConstructorInfo> constructors, ref Dictionary<ParameterInfo, IValueGenerator?> _parameters, ref Dictionary<MemberInfo, IValueGenerator>? _constraits)
        {
            ConstructorInfo? constructor;
            List<ParameterInfo> parameters; 
            Dictionary<ParameterInfo, IValueGenerator?> constructorParamters;
            int unsatisfied = 0;
            if (constructors.Count < 0)
            {
                return null;
            }
            constructor = constructors[0];
            (_parameters, unsatisfied) = SetGenerators(constructor.GetParameters().ToList(), ref _constraits);
            if (_constraits != null)
            {
                _parameters.Clear();
                constructor = null;
                int miss = _constraits.Count;
                for (int i = 0; i < constructors.Count && miss > 0; i++)
                {
                    unsatisfied = _constraits.Count;
                    parameters = constructors[i].GetParameters().ToList();
                    (constructorParamters, unsatisfied) = SetGenerators(parameters, ref _constraits);

                    if (miss > unsatisfied)
                    {
                        miss = unsatisfied;
                        constructor = constructors[i];

                        _parameters = constructorParamters;
                    }
                }
            }
            return constructor;
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
    }
}
