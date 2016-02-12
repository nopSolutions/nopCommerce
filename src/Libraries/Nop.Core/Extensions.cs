﻿using System;
﻿using System.Xml;

namespace Nop.Core
{
    public static class Extensions
    {
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }  

        public static string ElText(this XmlNode node, string elName)
        {
            return node.SelectSingleNode(elName).InnerText;
        }

        public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failureValue)
            where TInput : class
        {
            return o == null ? failureValue : evaluator(o);
        }
    }
}
