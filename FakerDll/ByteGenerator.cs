using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class ByteGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(byte);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            return (byte)context.Random.Next(byte.MinValue, byte.MaxValue + 1);
        }
    }
}
