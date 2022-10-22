namespace FakerDll
{
    public class Faker : IFaker
    {
        private GeneratorContext _generatorContext;
        private List<IValueGenerator> _generators;
        private IValueGenerator _generalGenerator;
        private Random _random;
        private int _recursionDepth;
        public int RecursionDepth
        {
            get
            {
                return _recursionDepth;
            }
            set
            {
                if (value <= RecursionController.MaxDepth && value >= RecursionController.MinDepth)
                {
                    _recursionDepth = value;
                }
            }
        }
        public FakerConfig Config { get; }
        public Faker()
        {
            _random = new Random();
            _recursionDepth = 3;
            _generatorContext = new GeneratorContext(_random, this);
            if(this.Config == null)
            {
                this.Config = new FakerConfig();
            }

            _generators = new List<IValueGenerator>();
            _generators.Add(new BoolGenerator());
            _generators.Add(new ByteGenerator());
            _generators.Add(new SByteGenerator());
            _generators.Add(new ShortGenerator());
            _generators.Add(new UShortGenerator());
            _generators.Add(new IntGenerator());
            _generators.Add(new UIntGenerator());
            _generators.Add(new StringGenerator());
            _generators.Add(new DateTimeGenerator());
            _generators.Add(new FloatGenerator());
            _generators.Add(new DoubleGenerator());
            _generators.Add(new LongGenerator());
            _generators.Add(new ULongGenerator());
            _generators.Add(new ListGenerator());
            _generalGenerator = new ClassGenerator();
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
            return FindGenerator(T).Generate(T, _generatorContext);
        }
        private IValueGenerator FindGenerator(Type T)
        {
            IValueGenerator generator;
            if(_generatorContext.Generator != null)
            {
                generator = _generatorContext.Generator;
                _generatorContext.Generator = null;
            }
            else
            {
                int i = 0;
                for (; i < _generators.Count && !_generators[i].CanGenerate(T); i++) { }

                if (i < _generators.Count)
                {
                    generator = _generators[i];
                }
                else
                {
                    generator = _generalGenerator;
                }
            }
            return generator;
        }
        public void AddGenerator(IValueGenerator generator)
        {
            if (!_generators.Contains(generator))
            {
                _generators.Add(generator);
            }
        }
        public void RemoveGenerator(IValueGenerator generator)
        {
            if (_generators.Contains(generator))
            {
                _generators.Remove(generator);
            }
        }
        public bool ContainsGenerator(IValueGenerator generator)
        {
            return _generators.Contains(generator);
        }
    }
}