namespace FakerDll
{
    public class Faker: IFaker
    {
        private GeneratorContext generatorContext;
        private List<IValueGenerator> generators;
        private Random random;
        private int depth;
        private List<Type> types; 
        public FakerConfig? Config { get; }
        public Faker()
        {
            types = new List<Type>();
            depth = 0;
            random = new Random();
            generatorContext = new GeneratorContext(random, this);
            generators = new List<IValueGenerator>();
            generators.Add(new ByteGenerator());
            generators.Add(new SByteGenerator());
            generators.Add(new ShortGenerator());
            generators.Add(new UShortGenerator());
            generators.Add(new IntGenerator());
            generators.Add(new UIntGenerator());
            generators.Add(new StringGenerator());
            generators.Add(new StructGenerator());
            generators.Add(new FloatGenerator());
            generators.Add(new DoubleGenerator());
            generators.Add(new LongGenerator());
            generators.Add(new ULongGenerator());
            generators.Add(new ListGenerator());
            generators.Add(new ClassGenerator());
        }
        public Faker(FakerConfig config):this()
        {
            this.Config = config;
        }
        public T? Create<T>()
        {
            return (T?)Create(typeof(T));
        }
        public object? Create(Type T)
        {
            object? value = GetDefaultValue(T);
            int i;
            types.Add(T);
            if (T != types[0])
            {
                types.Remove(T);
            }
            if(types.Count <= 3)
            {
                for (i = 0; i < generators.Count && !generators[i].CanGenerate(T); i++) { }
                if (i < generators.Count)
                {
                    value = generators[i].Generate(T, generatorContext);
                }
            }
            if (T == types[0])
            {
                types.Remove(T);
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