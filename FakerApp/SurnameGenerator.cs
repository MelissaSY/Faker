using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakerDll;

namespace FakerApp
{
    public class SurnameGenerator : IValueGenerator
    {
        private string[] _surnames = { "Арнгольц", "Лаптя", "Лодис", "Тимонович" };
        public bool CanGenerate(Type t)
        {
            return t == typeof(string);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            int nameNum = context.Random.Next(0, _surnames.Length);
            return _surnames[nameNum];
        }
    }
}
