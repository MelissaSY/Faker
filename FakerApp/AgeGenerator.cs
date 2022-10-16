namespace FakerDll
{
    public class AgeGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(int);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            return context.Random.Next(0, 100);
        }
    }
}