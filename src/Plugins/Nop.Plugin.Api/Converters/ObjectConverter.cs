using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nop.Plugin.Api.Converters
{
    public class ObjectConverter : IObjectConverter
    {
        private readonly IApiTypeConverter _apiTypeConverter;

        public ObjectConverter(IApiTypeConverter apiTypeConverter)
        {
            _apiTypeConverter = apiTypeConverter;
        }

        public T ToObject<T>(ICollection<KeyValuePair<string, string>> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            if (source != null)
            {
                foreach (var item in source)
                {
                    var itemKey = item.Key.Replace("_", string.Empty);
                    var currentProperty = someObjectType.GetProperty(itemKey,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (currentProperty != null)
                    {
                        currentProperty.SetValue(someObject, To(item.Value, currentProperty.PropertyType), null);
                    }
                }
            }

            return someObject;
        }

        private object To(string value, Type type)
        {
            if (type == typeof(DateTime?))
            {
                return _apiTypeConverter.ToUtcDateTimeNullable(value);
            }
            else if (type == typeof (int?))
            {
                return _apiTypeConverter.ToIntNullable(value);
            }
            else if (type == typeof(int))
            {
                return _apiTypeConverter.ToInt(value);
            }
            else if (type == typeof(List<int>))
            {
                return _apiTypeConverter.ToListOfInts(value);
            }
            else if(type == typeof(bool?))
            {
                // Because currently status is the only boolean and we need to accept published and unpublished statuses.
                return _apiTypeConverter.ToStatus(value);
            }
            else if (IsNullableEnum(type))
            {
                return _apiTypeConverter.ToEnumNullable(value, type);
            }

            // It should be the last resort, because it is not exception safe.
            return Convert.ChangeType(value, type);
        }

        private bool IsNullableEnum(Type t)
        {
            var u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }
    }
}