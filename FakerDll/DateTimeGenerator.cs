using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class DateTimeGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(DateTime);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            long ticks = context.Random.NextInt64(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks);
            return new DateTime(ticks);
        }
    }
}
