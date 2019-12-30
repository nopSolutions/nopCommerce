using System.Collections.Generic;

namespace Nop.Services.Common
{
    /// <summary>
    ///  Maintenance service interface
    /// </summary>
    public partial interface IMaintenanceService
    {
        /// <summary>
        /// Gets all backup files
        /// </summary>
        /// <returns>Backup file collection</returns>
        IList<string> GetAllBackupFiles();

        string GetNewBackupFilePath();

        /// <summary>
        /// Returns the path to the backup file
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        /// <returns>The path to the backup file</returns>
        string GetBackupPath(string backupFileName);
    }
}
