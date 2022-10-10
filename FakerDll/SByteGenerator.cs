using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class SByteGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(sbyte);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            return context.Random.Next(sbyte.MinValue, sbyte.MaxValue + 1);
        }
    }
}
