using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Services.Common
{
    /// <summary>
    ///  Maintenance service
    /// </summary>
    public partial class MaintenanceService : IMaintenanceService
    {
        #region Fields
       
        protected INopFileProvider FileProvider { get; }

        #endregion

        #region Ctor

        public MaintenanceService(INopFileProvider fileProvider)
        {
            FileProvider = fileProvider;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get directory path for backs
        /// </summary>
        /// <param name="ensureFolderCreated">A value indicating whether a directory should be created if it doesn't exist</param>
        /// <returns></returns>
        protected virtual string GetBackupDirectoryPath(bool ensureFolderCreated = true)
        {
            var path = FileProvider.GetAbsolutePath(NopCommonDefaults.DbBackupsPath);
            if (ensureFolderCreated)
                FileProvider.CreateDirectory(path);
            return path;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all backup files
        /// </summary>
        /// <returns>Backup file collection</returns>
        public virtual IList<string> GetAllBackupFiles()
        {
            var path = GetBackupDirectoryPath();

            if (!FileProvider.DirectoryExists(path)) 
                throw new NopException("Backup directory not exists");

            return FileProvider.GetFiles(path, $"*.{NopCommonDefaults.DbBackupFileExtension}")
                .OrderByDescending(p => FileProvider.GetLastWriteTime(p)).ToList();
        }
        
        /// <summary>
        /// Returns the path to the backup file
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        /// <returns>The path to the backup file</returns>
        public virtual string GetBackupPath(string backupFileName)
        {
            return FileProvider.Combine(GetBackupDirectoryPath(), backupFileName);
        }

        /// <summary>
        /// Creates a path to a new database backup file
        /// </summary>
        /// <returns>Path to a new database backup file</returns>
        public virtual string CreateNewBackupFilePath()
        {
            return FileProvider.Combine(GetBackupDirectoryPath(), $"database_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(10)}.{NopCommonDefaults.DbBackupFileExtension}");
        }

        #endregion
    }
}