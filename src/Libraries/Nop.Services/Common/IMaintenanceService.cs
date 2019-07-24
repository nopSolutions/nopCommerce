using System.Collections.Generic;
using Nop.Core;

namespace Nop.Services.Common
{
    /// <summary>
    ///  Maintenance service interface
    /// </summary>
    public partial interface IMaintenanceService
    {
        /// <summary>
        /// Get the current identity value
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns>Integer identity; null if cannot get the result</returns>
        int? GetTableIdent<T>() where T : BaseEntity;

        /// <summary>
        /// Set table identity (is supported)
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="ident">Identity value</param>
        void SetTableIdent<T>(int ident) where T : BaseEntity;

        /// <summary>
        /// Gets all backup files
        /// </summary>
        /// <returns>Backup file collection</returns>
        IList<string> GetAllBackupFiles();

        /// <summary>
        /// Creates a backup of the database
        /// </summary>
        void BackupDatabase();

        /// <summary>
        /// Restores the database from a backup
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        void RestoreDatabase(string backupFileName);

        /// <summary>
        /// Returns the path to the backup file
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        /// <returns>The path to the backup file</returns>
        string GetBackupPath(string backupFileName);

        /// <summary>
        /// Re-index database tables
        /// </summary>
        void ReIndexTables();
    }
}
