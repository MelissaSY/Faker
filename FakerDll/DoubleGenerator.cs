using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class DoubleGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(double);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            byte[] data = new byte[8];
            context.Random.NextBytes(data);
            return BitConverter.ToDouble(data);
        }
    }
}
