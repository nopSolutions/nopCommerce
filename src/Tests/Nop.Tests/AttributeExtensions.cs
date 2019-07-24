using System;
using System.Reflection;

namespace Nop.Tests
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// Will return true if the attributeTarget is decorated with an attribute of type TAttribute.
        /// Will return false if not.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="attributeTarget"></param>
        /// <returns></returns>
        public static bool IsDecoratedWith<TAttribute>(this ICustomAttributeProvider attributeTarget) where TAttribute : Attribute
        {
            return attributeTarget.GetCustomAttributes(typeof(TAttribute), false).Length > 0;
        }

        /// <summary>
        /// Will return true the first attribute of type TAttribute on the attributeTarget.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="attributeTarget"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider attributeTarget) where TAttribute : Attribute
        {
            return (TAttribute)attributeTarget.GetCustomAttributes(typeof(TAttribute), false)[0];
        }

    }
}
