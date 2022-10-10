using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class ShortGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(short);
        }

        public object? Generate(Type t, GeneratorContext context)
        { 
            return context.Random.Next(short.MinValue, short.MaxValue + 1);
        }
    }
}
