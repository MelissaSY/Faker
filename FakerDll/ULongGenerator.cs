using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class ULongGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(ulong);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            byte[] data = new byte[8];
            context.Random.NextBytes(data);
            return BitConverter.ToUInt64(data);
        }
    }
}
