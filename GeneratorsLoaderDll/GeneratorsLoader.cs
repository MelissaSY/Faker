using FakerDll;
using System.Reflection;
namespace GeneratorsLoaderDll
{
    public class GeneratorsLoader
    {
        public List<IValueGenerator> LoadGenerators()
        {
            List<IValueGenerator> generators = new List<IValueGenerator>();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Generators");
            FileInfo[]? dllGenerators; 
            if(Directory.Exists(path))
            {
                DirectoryInfo GeneratorsInfo = new DirectoryInfo(path);
                dllGenerators = GeneratorsInfo.GetFiles("*.dll");
                
            }
            return generators;
        }
    }
}