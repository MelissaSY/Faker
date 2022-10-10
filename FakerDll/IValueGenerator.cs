using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public interface IValueGenerator
    {
        object? Generate(Type t, GeneratorContext context);
        bool CanGenerate(Type t);
    }
}
