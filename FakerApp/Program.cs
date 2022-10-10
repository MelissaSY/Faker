using FakerDll;
namespace FakerApp
{
    public struct person
    {
        public person(int Age, string Name)
        {
            this.Age = Age;
            this.Name = Name;
        }
        public string? Name { get; }
        public int Age { get; }
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

    
    internal class Program
    {
        static void Main(string[] args)
        {
            FakerConfig config = new FakerConfig();
            config.Add<person, int, IntGenerator>(p => p.Age);
            config.Add<person, person, StructGenerator>(p => p);

            Faker faker = new Faker(config);
            person Person = faker.Create<person>();
            A? a = faker.Create<A>();
            int i = faker.Create<int>();
            List<person> people = faker.Create<List<person>>();
            Console.ReadLine();
        }
    }
}