using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FakerDll
{
    public interface IFakerConfig
    {
        void Add<T, E, Generator>(Expression<Func<T, E>> lambda)
            where Generator : IValueGenerator, new();
    }
}
