using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nop.Core.Infrastructure;

namespace Nop.Core.Tests.Fakes
{
    public class FakeTypeFinder : ITypeFinder
    {
        public Assembly[] Assemblies { get; set; }
        public Type[] Types { get; set; }

        public FakeTypeFinder(Assembly assembly, params Type[] types)
        {
            Assemblies = new[] { assembly };
            Types = types;
        }
        public FakeTypeFinder(params Type[] types)
        {
            Assemblies = new Assembly[0];
            Types = types;
        }
        public FakeTypeFinder(params Assembly[] assemblies)
        {
            Assemblies = assemblies;
        }

        public IList<Assembly> GetAssemblies()
        {
            return Assemblies.ToList();
        }



        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return (from t in Types
                    where !t.IsInterface && assignTypeFrom.IsAssignableFrom(t) && (onlyConcreteClasses ? (t.IsClass && !t.IsAbstract) : true)
                    select t).ToList();
        }

        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(assignTypeFrom, onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), onlyConcreteClasses);
        }
    }
}
