using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerTests
{
    public class TestGenerator : IValueGenerator
    {
        public static string[] _strings = { "Test", "UnitTest" };
        public bool CanGenerate(Type t)
        {
            return t == typeof(string);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            int stringNum = context.Random.Next(0, _strings.Length);
            return _strings[stringNum];
        }
    }
}
