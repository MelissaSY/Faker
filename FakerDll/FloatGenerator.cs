using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class FloatGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(float);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            return context.Random.NextSingle() * (float.MaxValue - float.MinValue) + float.MinValue;
        }
    }
}
