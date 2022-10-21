namespace FakerTests
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }
        public bool IsList(Type T)
        {
            bool isList = false;
            if (T.IsGenericType)
            {
                Type E = T.GetGenericTypeDefinition();
                isList = E == typeof(List<>);
            }
            return isList;
        }
        public void ListTest(IList ts)
        {
            int num = ts.Count;
            for(int i = 0; i < num; i++)
            {
                Assert.IsNotNull(ts[i]);
            }
        }
        public void PrimitiveTest<T>(Faker faker)
        {
            PrimitiveTest(typeof(T), faker);
        }
        public void PrimitiveTest(Type t, Faker faker)
        {
            object? primitive = null;
            try
            {
                primitive = faker.Create(t);
            }
            catch
            {
                Assert.Fail($"Could not generate type {t}");
            }
            Assert.IsNotNull(primitive, $"Could not generate type {t}");
        }
        [Test]
        public void PrimitivesTest()
        {
            Faker faker = new Faker();
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
            Faker faker = new Faker();
            PrimitiveTest<DateTime>(faker);
        }
        [Test]
        public void ListIntTest()
        {
            Faker faker = new Faker();
            List<int>? ints = faker.Create<List<int>>();
            PrimitiveTest<List<int>>(faker);
        }
        [Test]
        public void AddGeneratorTest()
        {
            Faker faker = new Faker();
            TestGenerator generator = new TestGenerator();
            faker.AddGenerator(generator);
            Assert.IsTrue(faker.ContainsGenerator(generator), "Could not add generator to generators list");
        }
        [Test]
        public void ConfigTest()
        {
            FakerConfig config = new FakerConfig();
            config.Add<PriorityTestClass, string, TestGenerator>(test => test.TestString3);
            Faker faker = new Faker(config);
            PriorityTestClass? testClass = faker.Create<PriorityTestClass>();
            Assert.That(testClass, Is.Not.Null);
            Assert.IsTrue(TestGenerator._strings.Contains(testClass.TestString3), "Could not use TestGenerator");
        }
        [Test]
        public void NoConfigPriorityTest()
        {
            Faker faker = new Faker();
            PriorityTestClass? testClass = faker.Create<PriorityTestClass>();
            Assert.IsNotNull(testClass, "Could not create PriorityTestClass");
            Assert.IsNull(testClass.TestString2, "Used wrong constructor");
            Assert.IsNotNull(testClass.TestString, "Could not initilize PriorityTestClass.TestString");
            Assert.IsNotNull(testClass.TestString3, "Could not initilize PriorityTestClass.TestString3");
            Assert.IsNotNull(testClass.TestString4, "Could not initilize PriorityTestClass.TestString4");
        }
        [Test]
        public void ConfigPriorityTest()
        {
            FakerConfig config = new FakerConfig();
            config.Add<PriorityTestClass, string, TestGenerator>(test => test.TestString);
            config.Add<PriorityTestClass, string, TestGenerator>(test => test.TestString2);
            config.Add<PriorityTestClass, string, TestGenerator>(test => test.TestString3);
            config.Add<PriorityTestClass, string, TestGenerator>(test => test.TestString4);

            Faker faker = new Faker(config);
            PriorityTestClass? testClass = faker.Create<PriorityTestClass>();
            Assert.IsNotNull(testClass, "Could not create PriorityTestClass");
            Assert.IsNotNull(testClass.TestString2, "Used wrong constructor");

            Assert.IsTrue(TestGenerator._strings.Contains(testClass.TestString), "Could not use TestGenerator");
            Assert.IsTrue(TestGenerator._strings.Contains(testClass.TestString2), "Could not use TestGenerator");
            Assert.IsTrue(TestGenerator._strings.Contains(testClass.TestString3), "Could not use TestGenerator");
            Assert.IsTrue(TestGenerator._strings.Contains(testClass.TestString4), "Could not use TestGenerator");

        }
        [Test]
        public void CyclicalClassesTest()
        {
            Faker faker = new Faker();
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
        [Test]
        public void RecursiondepthSetTest()
        {
            Faker faker = new Faker();
            faker.RecursionDepth = 0;
            A? a = faker.Create<A>();
            Assert.IsNotNull(a);
            Assert.IsNotNull(a.b);
            Assert.IsNotNull(a.b.c);
            Assert.IsNull(a.b.c.a);
            faker.RecursionDepth = 9;
            D? d = faker.Create<D>();
            Assert.IsNotNull(d);
            Assert.IsNotNull(d.d);
            Assert.IsNotNull(d.d.d);
            Assert.IsNotNull(d.d.d.d);
            Assert.IsNotNull(d.d.d.d.d);
            Assert.IsNotNull(d.d.d.d.d.d);
            Assert.IsNotNull(d.d.d.d.d.d.d);
            Assert.IsNotNull(d.d.d.d.d.d.d.d);
            Assert.IsNotNull(d.d.d.d.d.d.d.d.d);
            Assert.IsNotNull(d.d.d.d.d.d.d.d.d.d);
            Assert.IsNull(d.d.d.d.d.d.d.d.d.d.d);
        }
        [Test]
        public void PriviteConstructorTest()
        {
            Faker faker = new Faker();
            try
            {
                NoConstructorClasses? privateConstructorClass = faker.Create<NoConstructorClasses>();
            }
            catch
            {
                Assert.Fail("Could not create PrivateConstructorClass");
            }
        }
        [Test]
        public void ExceptionCostructorClassTest()
        {
            Faker faker = new Faker();
            try
            {
                ExceptionconstructorClass? exceptionconstructorClass = faker.Create<ExceptionconstructorClass>();
            }
            catch
            {
                Assert.Fail("Could not create ExceptionconstructorClasss");
            }
        }
        [Test]
        public void CyclicalListTest()
        {
            Faker faker = new Faker();
            CyclicalListClass? cyclicalListClass = faker.Create<CyclicalListClass>();
            Assert.IsNotNull(cyclicalListClass);
            Assert.IsNotNull(cyclicalListClass.cyclicals);
            ListTest(cyclicalListClass.cyclicals);
            foreach(CyclicalListClass c in cyclicalListClass.cyclicals)
            {
                Assert.IsNotNull(c.cyclicals, "List is not initiated");
                ListTest(c.cyclicals);
                foreach (CyclicalListClass c1 in c.cyclicals)
                {
                    Assert.IsNotNull(c1.cyclicals, "List is not initiated");
                    ListTest(c1.cyclicals);
                    foreach(CyclicalListClass c2 in c1.cyclicals)
                    {
                        Assert.IsNotNull(c2.cyclicals, "List is not initiated");
                        Assert.That(c2.cyclicals.Count, Is.EqualTo(0), "Cyclical list contains items but shouldn't on that level");
                    }
                } 
            }
        }
        [Test]
        public void RemoveConfigTest()
        {
            FakerConfig config = new FakerConfig();
            config.Add<PriorityTestClass, string, TestGenerator>(test => test.TestString);
            config.Add<PriorityTestClass, string, TestGenerator>(test => test.TestString2);
            Faker faker = new Faker(config);
            try
            {
                faker.Config.Remove<PriorityTestClass, string>(test => test.TestString);
                Assert.IsTrue(faker.Config.generatorsConstraits.ContainsKey(typeof(PriorityTestClass)), "At least one constrait should have remained");
                Assert.IsNotNull(faker.Config.generatorsConstraits[typeof(PriorityTestClass)], "Wrong implementation of removing operation");
                faker.Config.Remove<PriorityTestClass>();
                Assert.IsFalse(faker.Config.generatorsConstraits.ContainsKey(typeof(PriorityTestClass)), "There should be no remaining constraits for the type");
            }
            catch
            {
                Assert.Fail("Wrong implementation of removing operation (Exception occured)");
            }
        }
        [Test]
        public void GeneratorLoaderTest()
        {
            Faker faker = new Faker();
            PrimitiveTest<char>(faker);
            PrimitiveTest<decimal>(faker);
        }
    }
}