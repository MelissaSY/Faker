using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class UIntGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(uint);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            byte[] data = new byte[4];
            context.Random.NextBytes(data);
            return BitConverter.ToUInt32(data);
        }
    }
}
