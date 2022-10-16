using FakerDll;

namespace CharGeneratorDll
{
    public class CharGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(char);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            byte[] value = new byte[2];
            context.Random.NextBytes(value);
            return BitConverter.ToChar(value);
        }
    }
}