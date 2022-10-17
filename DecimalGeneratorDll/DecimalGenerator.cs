using FakerDll;

namespace DecimalGeneratorDll
{
    public class DecimalGenerator : IValueGenerator
    {
        public bool CanGenerate(Type t)
        {
            return t == typeof(decimal);
        }

        public object? Generate(Type t, GeneratorContext context)
        {
            decimal? value_1 = Convert.ToDecimal(context.Random.NextDouble());
            decimal? value_2 = value_1;
            value_1 *= decimal.MaxValue;
            value_1 += decimal.MinValue;
            value_2 *= decimal.MinValue;
            value_1 -= value_2;
            return value_1;
        }
    }
}