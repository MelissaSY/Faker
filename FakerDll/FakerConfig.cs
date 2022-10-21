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
                    else
                    {
                        if (generatorsConstraits[type].ContainsKey(member))
                        {
                            generatorsConstraits[type].Remove(member);
                        }
                    }
                    generatorsConstraits[type].Add(member, generator);
                }
            }
        }
        /// <summary>
        /// Removes specified constraits for generating fields and properties of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="lambda"></param>
        public void Remove<T, E>(Expression<Func<T, E?>> lambda)
        {
            Type type = lambda.Parameters[0].Type;
            MemberExpression? operation = lambda.Body as MemberExpression;
            if (operation != null)
            {
                if (generatorsConstraits.ContainsKey(type))
                {
                    MemberInfo member = operation.Member;
                    if (generatorsConstraits[type].ContainsKey(member))
                    {
                        generatorsConstraits[type].Remove(member);
                    }
                    if(generatorsConstraits[type].Count == 0)
                    {
                        generatorsConstraits.Remove(type);
                    }
                }
            }
        }
        /// <summary>
        /// Removes all constraits for generating type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Remove<T>()
        {
            Remove(typeof(T));
        }
        /// <summary>
        /// Removes all constraits for generating type T
        /// </summary>
        /// <param name="T"></param>
        public void Remove(Type T)
        {
            if (generatorsConstraits.ContainsKey(T))
            {
                generatorsConstraits.Remove(T);
            }
        }
    }
}
