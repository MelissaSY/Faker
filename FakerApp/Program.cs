using FakerDll;
namespace FakerApp
{
    public struct person
    {
        public person(int Age)
        {
            this.Age = Age;
            this.Name = null;
        }
        public string? Name { get; set; }
        public int Age { get; set; }
    }
    public class A
    {
        public B? b;
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

            Faker faker = new Faker();
            person Person = faker.Create<person>();
            int i = faker.Create<int>();
            faker.Create<List<person>>();
            Console.ReadLine();
        }
    }
}