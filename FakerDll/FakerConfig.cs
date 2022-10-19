using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FakerDll
{
    public class FakerConfig : IFakerConfig
    {
        public Dictionary<Type, Dictionary<MemberInfo, IValueGenerator>> generatorsConstraits { get; private set; }
        public FakerConfig()
        {
            generatorsConstraits = new Dictionary<Type, Dictionary<MemberInfo, IValueGenerator>>();
        }
        public void Add<T, E, Generator>(Expression<Func<T, E?>> lambda)
            where Generator : IValueGenerator, new()
        {
            Type type = lambda.Parameters[0].Type;
            MemberExpression? operation = lambda.Body as MemberExpression;
            if (operation != null)
            {
                Generator generator = new Generator();
                MemberInfo member = operation.Member;   
                if (generator.CanGenerate(lambda.ReturnType))
                {
                    if (!generatorsConstraits.ContainsKey(type))
                    {
                        generatorsConstraits.Add(type, new Dictionary<MemberInfo, IValueGenerator>());
                    }
                    generatorsConstraits[type].Add(member, generator);
                }
            }
        }
    }
}
