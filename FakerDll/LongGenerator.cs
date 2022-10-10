using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class LongGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(long);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            byte[] data = new byte[8];
            context.Random.NextBytes(data);
            return BitConverter.ToInt64(data);
        }
    }
}
