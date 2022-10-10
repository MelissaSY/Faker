using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

namespace FakerDll
{
    public class ListGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            bool isList = false;
            if(t.IsGenericType)
            {
                Type E = t.GetGenericTypeDefinition();
                isList = E == typeof(List<>);
            }
            return isList; 
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            Random random = new Random();
            int num = random.Next(20);
            Type[] underTypes = t.GetGenericArguments();
            IList? newObj = (IList?)Activator.CreateInstance(t, num);
            
            if(newObj != null)
            {
                for (int i = 0; i < num; i++)
                {
                    newObj.Add(context.Faker.Create(underTypes[0]));
                }
            }
            return newObj;
        }
    }
}
