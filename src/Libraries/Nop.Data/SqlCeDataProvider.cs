using System.Data.Common;
using System.Data.SqlClient;
using Nop.Core.Data;
#if EF6
using Nop.Data.Initializers;
#endif

namespace Nop.Data
{
    /// <summary>
    /// Represents SQL Server Compact data provider
    /// </summary>
    public partial class SqlCeDataProvider : IDataProvider
    {
        #region Methods

#if EF6
        /// <summary>
        /// Initialize database
        /// </summary>
        public virtual void InitializeDatabase()
        {
            var initializer = new CreateCeDatabaseIfNotExists<NopObjectContext>();
            Database.SetInitializer(initializer);
        }
#else
        /// <summary>
        /// Initialize database
        /// </summary>
        public virtual void InitializeDatabase()
        {
        }
#endif

        /// <summary>
        /// Get a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        public virtual DbParameter GetParameter()
        {
            return new SqlParameter();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this data provider supports stored procedures
        /// </summary>
        public virtual bool StoredProceduresSupported => false;

        /// <summary>
        /// Gets a value indicating whether this data provider supports backup
        /// </summary>
        public virtual bool BackupSupported => false;

        /// <summary>
        /// Gets a maximum length of the data for HASHBYTES functions, returns 0 if HASHBYTES function is not supported
        /// </summary>
        public virtual int SupportedLengthOfBinaryHash => 0;

        #endregion
    }
}