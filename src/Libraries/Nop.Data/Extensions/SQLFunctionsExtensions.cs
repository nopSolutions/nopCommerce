using System;
using LinqToDB;

namespace Nop.Data.Extensions
{
    public static class SQLFunctions
    {
        // For SQL Server 2014 (12.x) and earlier, allowed input values are limited to 8000 bytes. 
        // https://docs.microsoft.com/en-us/sql/t-sql/functions/hashbytes-transact-sql
        [Sql.Expression(ProviderName.SqlServer, "HASHBYTES('SHA2_512', substring({0}, 0, {1}))", ServerSideOnly = true)]
        [Sql.Expression("SHA2({0}, 512)", ServerSideOnly = true)]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryData">Array for a hashing function</param>
        /// <param name="limit">Allowed limit input value</param>
        /// <returns></returns>
        public static string Hash(byte[] binaryData, int limit)
            => throw new InvalidOperationException("This function should be used only in database code");
    }
}