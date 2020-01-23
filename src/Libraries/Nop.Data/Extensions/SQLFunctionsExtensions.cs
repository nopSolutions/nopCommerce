using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;

namespace Nop.Data.Extensions
{
    public static class SQLFunctionsExtensions
    {
        [Sql.Expression("SqlServer","HASHBYTES('sha1', substring({0}}, 0, {1}))", PreferServerSide = true)]
        public static string Hash<T>(this T x, int length) where T : IEnumerable<byte>
        {
            return BitConverter.ToString(x.ToArray()).Replace("-", string.Empty);
        }
    }
}