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
        public person(int age, string name)
        {
            this.Surname = "";
            this.Age = age;
            this.Name = name;
        }
        public person()
        {
            this.Age = 0;
            this.Surname = "";
            this.Name = "";
        }
        public string Name { get; }
        public int Age { get; }
        public string Surname { get; set; }
        public override string ToString()
        {
            string personInfo = $"Name: {this.Name}\n";
            personInfo += $"Surname: {this.Surname}\n";
            personInfo += $"Age: {this.Age}\n";
            return personInfo;
        }
    }
    public class A
    {
        public List<A> _as;
        public B? b;
        private string? _name;
        public int Age;
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
    public class Foo
    {
        public bool boo;
        public byte b;
        public double d;
        public float f;
        public int i;
        public string s;
        public List<List<person>> people;
        public short sh;
        public sbyte sb;   
        public uint u;
        public ulong ul;
        public ushort us;
        public long l;

        private A? _a;
        private char _c;
        public Foo(A a, List<List<person>> people, char c)
        {
            _a = a;
            _c = c;
            this.people = people;
            s = "";
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            FakerConfig config = new FakerConfig();
            config.Add<person, int, AgeGenerator>(p => p.Age);
            config.Add<person, string, NameGenerator>(p => p.Name);
            config.Add<person, string, SurnameGenerator>(p => p.Surname);

            config.Add<A, int, AgeGenerator>(a => a.Age);

            Faker faker = new Faker(config);

            GeneratorsLoader loader = new GeneratorsLoader();
            List<IValueGenerator> generators = loader.LoadGenerators();
            foreach(IValueGenerator generator in generators)
            {
                faker.AddGenerator(generator);
            }

            person Person = faker.Create<person>();
            DateTime dt = faker.Create<DateTime>();
            Foo foo = faker.Create<Foo>();
            char c = faker.Create<char>();
            decimal? dec = faker.Create<decimal>();
            Faker? fc = faker.Create<Faker>();
            
            double doub = faker.Create<double>();
            bool b = faker.Create<bool>();
         //   List<List<person>> people = faker.Create<List<List<person>>>();
            List<person> people2 = faker.Create<List<person>>();
            foreach(person person in people2)
            {
                Console.WriteLine(person);
            }
            Console.WriteLine(Person);
            Console.WriteLine(dt);
            Console.WriteLine(c);
            Console.WriteLine(dec);
            Console.WriteLine(doub);
            Console.WriteLine(dec);
            Console.ReadLine();
        }
    }
}