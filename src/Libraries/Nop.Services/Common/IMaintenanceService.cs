using System.Collections.Generic;
using System.IO;
using Nop.Core;

namespace Nop.Services.Common
{
    /// <summary>
    ///  Maintenance service interface
    /// </summary>
    public partial interface IMaintenanceService
    {
        /// <summary>
        /// Get the current ident value
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns>Integer ident; null if cannot get the result</returns>
        int? GetTableIdent<T>() where T : BaseEntity;

        /// <summary>
        /// Set table ident (is supported)
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="ident">Ident value</param>
        void SetTableIdent<T>(int ident) where T : BaseEntity;

        /// <summary>
        /// Gets all backup files
        /// </summary>
        /// <returns>Backup file collection</returns>
        IList<FileInfo> GetAllBackupFiles();

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
    }
}
