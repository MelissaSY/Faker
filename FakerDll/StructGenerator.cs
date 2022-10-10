using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace FakerDll
{
    public class StructGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t.IsValueType && !t.IsPrimitive && !t.IsEnum;
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            object? newObj;
            Type parameterType;
            ConstructorInfo[] constructors = t.GetConstructors();
            ParameterInfo[][] parameters = new ParameterInfo[constructors.Length][];
            FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            int maxParamNum = 0;
            int maxParamI = 0;
            for (int i = 0; i < constructors.Length; i++)
            {
                parameters[i] = constructors[i].GetParameters();
                if (parameters[i].Length > maxParamNum)
                {
                    maxParamNum = parameters[i].Length;
                    maxParamI = i;
                }
            }
            if(maxParamNum > 0)
            {
                ParameterInfo[] maxParameters = parameters[maxParamI];
                object?[] objParameters = new object[maxParameters.Length];
                for (int i = 0; i < maxParamNum; i++)
                {
                    parameterType = maxParameters[i].ParameterType;
                    objParameters[i] = context.Faker.Create(parameterType);

                }
                newObj = Activator.CreateInstance(t, objParameters);
            }
            else
            {
                newObj = Activator.CreateInstance(t);
            }
            foreach(FieldInfo field in fields)
            {
                parameterType = field.FieldType;
                field.SetValue(newObj, context.Faker.Create(parameterType));
            }
            foreach(PropertyInfo property in properties)
            {
                parameterType = property.PropertyType;
                property.SetValue(newObj, context.Faker.Create(parameterType));
            }
            return newObj;
        }
    }
}
