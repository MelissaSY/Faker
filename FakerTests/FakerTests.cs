namespace FakerTests
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        public void PrimitiveTest<T>(Faker faker)
        {
            object? primitive = null;
            try
            {
                primitive = faker.Create<T>();
            }
            catch
            {
                Assert.Fail($"Could not generate type {typeof(T)}");
            }
            Assert.IsNotNull(primitive, $"Could not generate type {typeof(T)}");
        }
        [Test]
        public void PrimitivesTest()
        {
            Faker faker = new();
            PrimitiveTest<bool>(faker);
            PrimitiveTest<byte>(faker);
            PrimitiveTest<double>(faker);
            PrimitiveTest<float>(faker); 
            PrimitiveTest<int>(faker);
            PrimitiveTest<long>(faker);
            PrimitiveTest<sbyte>(faker);
            PrimitiveTest<short>(faker);

            PrimitiveTest<string>(faker);

            PrimitiveTest<uint>(faker);
            PrimitiveTest<ulong>(faker);
            PrimitiveTest<ushort>(faker);
        }
        [Test]
        public void DateTimeTest()
        {
            Faker faker = new();
            DateTime? dt = faker.Create<DateTime>();
            PrimitiveTest<DateTime>(faker);
           // Assert.IsNotNull(dt, $"Could not generate type {typeof(DateTime)}");
        }
        [Test]
        public void ListIntTest()
        {
            Faker faker = new();
            List<int>? ints = faker.Create<List<int>>();
            PrimitiveTest<List<int>>(faker);
          //  Assert.IsNotNull(ints, "Could not create List<int>");
        }
        [Test]
        public void DictionaryIntIntTest()
        {
            Faker faker = new();
            Dictionary<int, int>? intint = faker.Create<Dictionary<int, int>>();
            PrimitiveTest<Dictionary<int, int>>(faker);
           // Assert.IsNotNull(intint, "Could not create Dictionary<int, int>");
        }
        [Test]
        public void AddGeneratorTest()
        {
            Faker faker = new();
            TestGenerator generator = new TestGenerator();
            faker.AddGenerator(generator);
            Assert.IsTrue(faker.ContainsGenerator(generator), "Could not add generator to generators list");
        }
        [Test]
        public void ConfigTest()
        {
            FakerConfig config = new();
            config.Add<TestClass, string, TestGenerator>(test => test.TestString);
            Faker faker = new(config);
            TestClass? testClass = faker.Create<TestClass>();
            Assert.That(testClass, Is.Not.Null);
            Assert.IsTrue(TestGenerator._strings.Contains(testClass.TestString), "Could not use TestGenerator");
        }
        [Test]
        public void NoConfigPriorityTest()
        {
            Faker faker = new();
            TestClass? testClass = faker.Create<TestClass>();
            Assert.IsNotNull(testClass, "Could not create TestClass");
            Assert.IsNull(testClass.TestString2, "Used wrong constructor");
            Assert.IsNotNull(testClass.TestString, "Could not initilize TestClass.TestString");
            Assert.IsNotNull(testClass.TestString3, "Could not initilize TestClass.TestString3");
            Assert.IsNotNull(testClass.TestString4, "Could not initilize TestClass.TestString4");
        }
        [Test]
        public void ConfigPriorityTest()
        {
            FakerConfig config = new();
            config.Add<TestClass, string, TestGenerator>(test => test.TestString);
            config.Add<TestClass, string, TestGenerator>(test => test.TestString2);
            config.Add<TestClass, string, TestGenerator>(test => test.TestString3);
            config.Add<TestClass, string, TestGenerator>(test => test.TestString4);

            Faker faker = new(config);
            TestClass? testClass = faker.Create<TestClass>();
            Assert.IsNotNull(testClass, "Could not create TestClass");
            Assert.IsNotNull(testClass.TestString2, "Used wrong constructor");

            Assert.IsTrue(TestGenerator._strings.Contains(testClass.TestString), "Could not use TestGenerator");
            Assert.IsTrue(TestGenerator._strings.Contains(testClass.TestString2), "Could not use TestGenerator");
            Assert.IsTrue(TestGenerator._strings.Contains(testClass.TestString3), "Could not use TestGenerator");
            Assert.IsTrue(TestGenerator._strings.Contains(testClass.TestString4), "Could not use TestGenerator");

        }
        [Test]
        public void CyclicalClassesTest()
        {
            Faker faker = new();
            A? a = faker.Create<A>();
            Assert.IsNotNull(a);
            Assert.IsNotNull(a.b);
            Assert.IsNotNull(a.b.c);
            Assert.IsNotNull(a.b.c.a);
            Assert.IsNotNull(a.b.c.a.b);
            Assert.IsNotNull(a.b.c.a.b.c);
            Assert.IsNotNull(a.b.c.a.b.c.a);
            Assert.IsNotNull(a.b.c.a.b.c.a.b);
            Assert.IsNotNull(a.b.c.a.b.c.a.b.c);
            Assert.IsNotNull(a.b.c.a.b.c.a.b.c.a);
            Assert.IsNotNull(a.b.c.a.b.c.a.b.c.a.b);
            Assert.IsNotNull(a.b.c.a.b.c.a.b.c.a.b.c);
            Assert.IsNull(a.b.c.a.b.c.a.b.c.a.b.c.a);
        }
        
    }
}