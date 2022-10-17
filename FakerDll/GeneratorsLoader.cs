using System.Reflection;

namespace FakerDll
{
    public class GeneratorsLoader
    {
        public List<IValueGenerator> LoadGenerators()
        {
            List<IValueGenerator> generators = new List<IValueGenerator>();
            List<Type> types = new List<Type>();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Generators");
            FileInfo[] dllGenerators;
            if (Directory.Exists(path))
            {
                DirectoryInfo GeneratorsInfo = new DirectoryInfo(path);
                dllGenerators = GeneratorsInfo.GetFiles("*.dll");
                foreach (FileInfo dllGenerator in dllGenerators)
                {
                    Assembly assembly = Assembly.LoadFile(dllGenerator.FullName);
                    types.AddRange(assembly.GetExportedTypes());
                }
                foreach (Type type in types)
                {
                    if (typeof(IValueGenerator).IsAssignableFrom(type))
                    {
                        IValueGenerator? generator = (IValueGenerator?)Activator.CreateInstance(type);
                        if (generator != null)
                        {
                            generators.Add(generator);
                        }
                    }
                }
            }
            return generators;
        }
    }
}
