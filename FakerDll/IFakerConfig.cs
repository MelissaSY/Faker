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
        void Add<T, Type, Generator>(Expression<Func<T, Type>> lambda);
    }
}
