namespace FakerDll
{
    public class Faker : IFaker
    {
        // public IValueGenerator? PriorityGenerator { get; set; }
        private GeneratorContext generatorContext;
        private List<IValueGenerator> generators;
        private IValueGenerator generalGenerator;
        private Random random;
        public Dictionary<Type, int> Types { get; private set; }
        public FakerConfig? Config { get; }
        public Faker()
        {
            Types = new Dictionary<Type, int>();
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
            generators.Add(new DateTimeGenerator());
            generators.Add(new FloatGenerator());
            generators.Add(new DoubleGenerator());
            generators.Add(new LongGenerator());
            generators.Add(new ULongGenerator());
            generators.Add(new ListGenerator());
            generalGenerator = new ClassGenerator();
         //   generators.Add(new ClassGenerator());
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
            int i = 0;
            if (!Types.ContainsKey(T)) {
                Types.Add(T, 0);
            }
            Types[T]++;
            value = FindGenerator(T)?.Generate(T, generatorContext);
            Types[T]--;
            return value;
        }
        private IValueGenerator? FindGenerator(Type T)
        {
            IValueGenerator? generator = null;
            if(generatorContext.Generator != null)
            {
                generator = generatorContext.Generator;
                generatorContext.Generator = null;
            }
            else
            {
                int i = 0;
                for (; i < generators.Count && !generators[i].CanGenerate(T); i++) { }

                if (i < generators.Count)
                {
                    generator = generators[i];
                }
                else
                {
                    generator = generalGenerator;
                }
            }
            return generator;
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