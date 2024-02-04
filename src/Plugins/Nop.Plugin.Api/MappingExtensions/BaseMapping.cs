using System;
using System.Reflection;
using AutoMapper;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class BaseMapping
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> expression)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var sourceType = typeof(TSource);
            var destinationProperties = typeof(TDestination).GetProperties(flags);

            foreach (var property in destinationProperties)
            {
                if (sourceType.GetProperty(property.Name, flags) == null)
                {
                    expression.ForMember(property.Name, opt => opt.Ignore());
                }
            }
            return expression;
        }

        public static TResult GetWithDefault<TSource, TResult>(
            this TSource instance,
            Func<TSource, TResult> getter,
            TResult defaultValue = default(TResult))
            where TSource : class
        {
            return instance != null
                       ? getter(instance)
                       : defaultValue;
        }
    }
}
