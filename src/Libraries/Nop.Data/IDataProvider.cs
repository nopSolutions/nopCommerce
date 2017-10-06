
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Nop.Data
{
    /// <summary>
    /// Data provider interface
    /// </summary>
    public interface IDataProvider
    {
        DbContextOptions<DbContext> GetOptions();

        /// <summary>
        /// Set database initializer
        /// </summary>
        void SetDatabaseInitializer(IDbContext context);

        /// <summary>
        /// A value indicating whether this data provider supports stored procedures
        /// </summary>
        bool StoredProceduredSupported { get; }

        /// <summary>
        /// A value indicating whether this data provider supports backup
        /// </summary>
        bool BackupSupported { get; }

        /// <summary>
        /// Gets a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        DbParameter GetParameter();

        /// <summary>
        /// Maximum length of the data for HASHBYTES functions
        /// returns 0 if HASHBYTES function is not supported
        /// </summary>
        /// <returns>Length of the data for HASHBYTES functions</returns>
        int SupportedLengthOfBinaryHash();
    }
}
