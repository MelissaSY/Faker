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
            return context.Random.NextDouble() * (double.MaxValue - double.MinValue) + double.MinValue;
        }
    }
}
