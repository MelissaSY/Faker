using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class BoolGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(bool);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            int num = context.Random.Next(0, 2);
            bool value = num == 1;
            return value;
        }
    }
}
