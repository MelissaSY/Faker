using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public class GeneratorContext
    {
        public Random Random { get; }
        public Faker Faker { get; }
        public IValueGenerator? Generator { get; set; }
        public GeneratorContext(Random random, Faker faker)
        {
            Random = random;
            Faker = faker;
        }
    }
}
