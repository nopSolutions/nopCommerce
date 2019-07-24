using System.Data.Common;

namespace Nop.Core.Data
{
    /// <summary>
    /// Represents a data provider
    /// </summary>
    public partial interface IDataProvider
    {
        #region Methods
        
        /// <summary>
        /// Initialize database
        /// </summary>
        void InitializeDatabase();

        /// <summary>
        /// Get a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        DbParameter GetParameter();

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this data provider supports backup
        /// </summary>
        bool BackupSupported { get; }

        /// <summary>
        /// Gets a maximum length of the data for HASHBYTES functions, returns 0 if HASHBYTES function is not supported
        /// </summary>
        int SupportedLengthOfBinaryHash { get; }

        #endregion
    }
}