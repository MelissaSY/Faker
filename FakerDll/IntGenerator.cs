using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class IntGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(int);
        }

        public object Generate(Type t, GeneratorContext context)
        {
            byte[] data = new byte[4];
            context.Random.NextBytes(data);
            return BitConverter.ToInt32(data);
        }
    }
}
