using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakerDll;

namespace FakerApp
{
    public class NameGenerator : IValueGenerator
    {
        private string[] _names = { "Соня", "Алексей", "Игорь", "Дмитрий"};
        public bool CanGenerate(Type t)
        {
            return t == typeof(string);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            int nameNum = context.Random.Next(0, _names.Length);
            return _names[nameNum];
        }
    }
}
