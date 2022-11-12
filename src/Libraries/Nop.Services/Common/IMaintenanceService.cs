<<<<<<< HEAD
﻿using System.Collections.Generic;

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

        /// <summary>
        /// Creates a path to a new database backup file
        /// </summary>
        /// <returns>Path to a new database backup file</returns>
        string CreateNewBackupFilePath();

        /// <summary>
        /// Returns the path to the backup file
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        /// <returns>The path to the backup file</returns>
        string GetBackupPath(string backupFileName);
    }
}
=======
﻿using System.Collections.Generic;

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

        /// <summary>
        /// Creates a path to a new database backup file
        /// </summary>
        /// <returns>Path to a new database backup file</returns>
        string CreateNewBackupFilePath();

        /// <summary>
        /// Returns the path to the backup file
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        /// <returns>The path to the backup file</returns>
        string GetBackupPath(string backupFileName);
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
