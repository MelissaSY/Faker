using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class UShortGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(ushort);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            return (ushort)context.Random.Next(ushort.MinValue, ushort.MaxValue + 1);   
        }
    }
}
