using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// refs: https://www.elemarjr.com/pt/2017/06/licoes-de-performances-aprendidas-com-roslyn-1-objectpool-e-pooledstringbuilder/
    /// The usage is:
    ///        var inst = PooledStringBuilder.GetInstance();
    ///        var sb = inst.builder;
    ///        ... Do Stuff...
    ///        ... sb.ToString() ...
    ///        inst.Free();
    /// </summary>
    public static class PooledStringBuilder
    {
        private static readonly StringBuilderPooledObjectPolicy _pool = new StringBuilderPooledObjectPolicy();

        public static StringBuilder Create(int size = 32)
        {
            return _pool.Create();
        }

        public static bool Return(StringBuilder sb)
        {
            return _pool.Return(sb);
        }
    }

    public static class StringBuilderExtensions
    {
        /// <summary>
        /// return the string and free the resource back to the pool
        /// </summary>
        /// <param name="sb"></param>
        /// <returns></returns>
        public static string ToStringAndReturn(this StringBuilder sb)
        {
            string data = sb.ToString();
            PooledStringBuilder.Return(sb);
            return data;
        }
    }
}
