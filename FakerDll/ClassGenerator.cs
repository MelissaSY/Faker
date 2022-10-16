using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class ClassGenerator : IValueGenerator
    {
        //_constraits may be redused after calling SetGenerators method
        private Dictionary<MemberInfo, IValueGenerator>? _constraits;
        //_fields and _properties will be initialized after method SetGenerators(Type) 
        private Dictionary<PropertyInfo, IValueGenerator?> _properties;
        private Dictionary<FieldInfo, IValueGenerator?> _fields;
        //_parameters will be initialized after calling method SelectConstructor
        private Dictionary<ParameterInfo, IValueGenerator?> _parameters;  
        public ClassGenerator()
        {
            _properties = new Dictionary<PropertyInfo, IValueGenerator?>();
            _fields = new Dictionary<FieldInfo, IValueGenerator?>();
            _parameters = new Dictionary<ParameterInfo, IValueGenerator?>();
        }
        public bool CanGenerate(Type t)
        {
            return true;
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            _properties.Clear();
            _fields.Clear();
            _parameters.Clear();
            if (context.Faker.Types[t] > 3)
            {
                return null;
            }
            object? newObj = null;
            Type parameterType;
            ConstructorInfo? constructor;
            List<ConstructorInfo> constructors = t.GetConstructors(BindingFlags.Public | BindingFlags.Instance).ToList();
            constructors.Sort(new MaxParametersComparerer());
            context.Faker.Config?.generatorsConstraits.TryGetValue(t, out _constraits);

            SetGenerators(t);

            while (constructors.Count > 0 && newObj == null)
            {
                constructor = SelectConstructor(constructors);

                if (_parameters.Count > 0)
                {
                    object?[] objParameters = new object[_parameters.Count];
                    int param = -1;
                    foreach(ParameterInfo parameter in _parameters.Keys)
                    {
                        param++;
                        parameterType = parameter.ParameterType;
                        context.Generator = _parameters[parameter];
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
                foreach (FieldInfo field in _fields.Keys)
                {
                    parameterType = field.FieldType;
                    context.Generator = _fields[field];
                    field.SetValue(newObj, context.Faker.Create(parameterType));
                }
                foreach (PropertyInfo property in _properties.Keys)
                {
                    parameterType = property.PropertyType;
                    context.Generator = _properties[property];
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
        private void SetGenerators(Type t)
        {
            FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            IValueGenerator? generator;
            foreach(FieldInfo field in fields)
            {
                generator = FindGenerator(field);
                _fields.Add(field, null);
            }
            foreach(PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    generator = FindGenerator(property);
                    _properties.Add(property, generator);
                }
            }
            if(this._constraits?.Count < 1)
            {
                this._constraits = null;
            }
        }
        /// <summary>
        /// searches generator in private field _constraits and if found removes from dictionary
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private IValueGenerator? FindGenerator(MemberInfo member)
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
        private (Dictionary<ParameterInfo, IValueGenerator?>, int unsatisfied) SetGenerators(List<ParameterInfo> parameters)
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
        private ConstructorInfo? SelectConstructor(List<ConstructorInfo> constructors)
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
            (_parameters, unsatisfied) = SetGenerators(constructor.GetParameters().ToList());
            if (this._constraits != null)
            {
                _parameters.Clear();
                constructor = null;
                int miss = this._constraits.Count;
                for (int i = 0; i < constructors.Count && miss > 0; i++)
                {
                    unsatisfied = this._constraits.Count;
                    parameters = constructors[i].GetParameters().ToList();
                    (constructorParamters, unsatisfied) = SetGenerators(parameters);

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
