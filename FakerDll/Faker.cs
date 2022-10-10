namespace FakerDll
{
    public class Faker: IFaker
    {
        private GeneratorContext generatorContext;
        private List<IValueGenerator> generators;
        private Random random;
        public Faker()
        {
            random = new Random();
            generatorContext = new GeneratorContext(random, this);
            generators = new List<IValueGenerator>();
            generators.Add(new IntGenerator());
            generators.Add(new StructGenerator());
            generators.Add(new ListGenerator());
        }
        public T? Create<T>()
        {
            return (T?)Create(typeof(T));
        }
        public object? Create(Type T)
        {
            object? value = GetDefaultValue(T);
            int i;
            for(i = 0; i < generators.Count && !generators[i].CanGenerate(T); i++) { }
            if(i < generators.Count)
            {
                value = generators[i].Generate(T, generatorContext);
            }
            return value;

        }
        private static object? GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);
            else
                return null;
        }
        public void AddGenerator(IValueGenerator generator)
        {
            if (!generators.Contains(generator))
            {
                generators.Add(generator);
            }
        }
    }
}