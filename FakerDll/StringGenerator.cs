using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class StringGenerator : IValueGenerator
    {
        private string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=`~";
        public bool CanGenerate(Type t)
        {
            return t == typeof(string);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            int stringLength = context.Random.Next(1, 20);
            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < stringLength; i++)
            {
                int j = context.Random.Next(0, alphabet.Length);
                string a  = alphabet[j].ToString();
                if(context.Random.Next(0, 2) == 1)
                {
                    a = a.ToLower();
                }
                stringBuilder.Append(a);
            }
            return stringBuilder.ToString();
        }
    }
}
