using System;
using LinqToDB;

namespace Nop.Data.Extensions
{
    public static class SQLFunctions
    {
        /// <summary>
        /// Gets a data hash from database side
        /// </summary>
        /// <param name="binaryData">Array for a hashing function</param>
        /// <param name="limit">Allowed limit input value</param>
        /// <returns>Data hash</returns>
        /// <remarks>
        /// For SQL Server 2014 (12.x) and earlier, allowed input values are limited to 8000 bytes. 
        /// https://docs.microsoft.com/en-us/sql/t-sql/functions/hashbytes-transact-sql
        /// </remarks>
        [Sql.Expression("CONVERT(VARCHAR(128), HASHBYTES('SHA2_512', SUBSTRING({0}, 0, {1})), 2)", ServerSideOnly = true, Configuration = ProviderName.SqlServer)]
        [Sql.Expression("SHA2({0}, 512)", ServerSideOnly = true, Configuration = ProviderName.MySql)]
        public static string Hash(byte[] binaryData, int limit)
            => throw new InvalidOperationException("This function should be used only in database code");
    }
}