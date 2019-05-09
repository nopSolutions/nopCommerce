using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using NUnit.Framework;

namespace Nop.Tests
{
    public static class TestExtensions
    {
        public static T PropertiesShouldEqual<T>(this T actual, T expected, params string[] filters)
        {
            var properties = typeof (T).GetProperties().ToList();

            var filterByEntities = new List<string>();
            var values = new Dictionary<string, object>();

            foreach (var propertyInfo in properties.ToList())
            {
                //skip by filter
                if (filters.Any(f => f == propertyInfo.Name || f + "Id" == propertyInfo.Name) || propertyInfo.Name == "Id")
                    continue;
                var value = propertyInfo.GetValue(actual);
                values.Add(propertyInfo.Name, value);

                if(value == null)
                    continue;

                //skip array and System.Collections.Generic types
                if (value.GetType().IsArray || value.GetType().Namespace == "System.Collections.Generic")
                {
                    properties.Remove(propertyInfo);
                    continue;
                }
                
                if (!(value is BaseEntity))
                    continue;

                //skip BaseEntity types and entity Id
                filterByEntities.Add(propertyInfo.Name + "Id");
                properties.Remove(propertyInfo);
            }

            foreach (var propertyInfo in properties.Where(p=>values.ContainsKey(p.Name)))
            {
                if (filterByEntities.Any(f => f == propertyInfo.Name))
                    continue;
               
                Assert.AreEqual(values[propertyInfo.Name], propertyInfo.GetValue(expected), $"The property \"{typeof(T).Name}.{propertyInfo.Name}\" of these objects is not equal");
            }

            return actual;
        }
    }
}
