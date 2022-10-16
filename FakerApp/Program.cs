using System.Reflection;
using FakerDll;

namespace FakerApp
{
    public struct person
    {
        public person(string Surname, string Name)
        {
            this.Surname = Surname;
            this.Name = Name;
            this.Age = 0;
        }
        public person(int age)
        {
            this.Age = age; 
            this.Surname = "";
            this.Name = "";
        }
        public string? Name { get; }
        public int Age { get; }
        public string Surname { get; set; }
    }
    public class A
    {
        public List<A> _as;
        public B? b;
        private string? _name;
        public int _age;
    }
    public class B
    {
        public C? c;
    }
    public class C
    {
        public A? a;
    }
    public class D
    {
        public B? b;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Assembly charGeneratorAssembly = Assembly.LoadFile(@"D:\5 сем\СПП\СПП\ЛР\лр2\CharGeneratorDll\bin\Debug\net6.0\CharGeneratorDll.dll");
            //Assembly assembly = Assembly.Load("CharGeneratorDll");
            // IValueGenerator ageGenearator = ageGeneratorAssembly.CreateInstance("AgeGeneratorDll.AgeGenerator");

            FakerConfig config = new FakerConfig();
            config.Add<person, int, AgeGenerator>(p => p.Age);
            config.Add<person, string, StringGenerator>(p => p.Name);
            config.Add<person, string, StringGenerator>(p => p.Surname);

            Faker faker = new Faker(config);
            faker.Create<DateTime>();
            Faker faker_1 = faker.Create<Faker>();
           // faker.AddGenerator(new DecimalGenerator());
//            faker.AddGenerator((IValueGenerator)charGeneratorAssembly.CreateInstance("CharGeneratorDll.CharGenerator"));

            /*char c = faker.Create<char>();
            decimal? dec = faker.Create<decimal>();*/

            person Person = faker.Create<person>();
           // D? a = faker.Create<D>();
            int i = faker.Create<int>();
            List<List<person>> people = faker.Create<List<List<person>>>();
            Console.ReadLine();
        }
    }
}