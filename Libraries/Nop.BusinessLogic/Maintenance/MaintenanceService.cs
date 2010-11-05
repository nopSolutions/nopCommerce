//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Ionic.Zip;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Installation;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Maintenance
{
    /// <summary>
    /// Maintenance service
    /// </summary>
    public partial class MaintenanceService : IMaintenanceService
    {
        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public MaintenanceService(NopObjectContext context)
        {
            _context = context;
            _cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reindex tables
        /// </summary>
        public void Reindex()
        {
            
            _context.Sp_Maintenance_ReindexTables();
        }

        /// <summary>
        /// Backup database
        /// </summary>
        public void Backup()
        {
            string path = IoCFactory.Resolve<ISettingManager>().GetSettingValue("Maintenance.BackupPath").Trim();
            if (String.IsNullOrEmpty(path))
                path = string.Format("{0}{1}", HttpContext.Current.Request.PhysicalApplicationPath, "Administration\\backups\\");

            string fileName = string.Format(
                "{0}database_{1}_{2}.bak",
                path,
                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"),
                CommonHelper.GenerateRandomDigitCode(4));

            InstallerHelper.Backup(fileName);
        }

        /// <summary>
        /// Back up pictures
        /// </summary>
        public void BackupPictures()
        {
            string fileName = string.Format("{0}Administration\\backups\\images_{1}_{2}.zip", HttpContext.Current.Request.PhysicalApplicationPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
            using (ZipFile zipFile = new ZipFile())
            {
                zipFile.AddDirectory(IoCFactory.Resolve<IPictureService>().LocalImagePath);
                zipFile.Save(fileName);
            }
        }

        /// <summary>
        /// Delete files in \files\ExportImport folder (PDF, Excel etc)
        /// </summary>
        /// <param name="hours">Files to delete older than specified hours value</param>
        /// <returns>Number of deleted files</returns>
        public int DeleteOldExportImportFiles(int hours)
        {
            int num = 0;

            string path = string.Format("{0}\\files\\ExportImport\\", HttpContext.Current.Request.PhysicalApplicationPath);
            foreach (var fullPath in System.IO.Directory.GetFiles(path))
            {
                var fileName = Path.GetFileName(fullPath);
                if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var info = new FileInfo(fullPath);
                if (Math.Abs((DateTime.UtcNow - info.CreationTimeUtc).TotalHours) >= hours)
                {
                    File.Delete(fullPath);
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// Restore database
        /// </summary>
        /// <param name="fileName">Backup file name</param>
        public void RestoreBackup(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                return;

            if (File.Exists(fileName))
            {
                switch (Path.GetExtension(fileName).ToLowerInvariant())
                {
                    case ".zip":
                        if (Path.GetFileName(fileName).StartsWith("images"))
                        {
                            using (ZipFile zipFile = new ZipFile(fileName))
                            {
                                zipFile.ExtractAll(IoCFactory.Resolve<IPictureService>().LocalImagePath, ExtractExistingFileAction.OverwriteSilently);
                            }
                        }
                        break;
                    case ".bak":
                        InstallerHelper.RestoreBackup(fileName);
                        break;
                }
            }
            else
            {
                throw new NopException("File doesn't exist");
            }
        }

        /// <summary>
        /// Delete the backup file
        /// </summary>
        /// <param name="fileName">Backup file name</param>
        public void DeleteBackup(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        /// <summary>
        /// Gets all backup files
        /// </summary>
        /// <returns>Backup file collection</returns>
        public List<BackupFile> GetAllBackupFiles()
        {
            var collection = new List<BackupFile>();

            string path = string.Format("{0}Administration\\backups\\", HttpContext.Current.Request.PhysicalApplicationPath);
            foreach (var fullPath in System.IO.Directory.GetFiles(path))
            {
                var fileName = Path.GetFileName(fullPath);
                if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var info = new FileInfo(fullPath);
                collection.Add(new BackupFile()
                {
                    FullFileName = fullPath,
                    FileName = fileName,
                    FileSize = info.Length
                });
            }

            return collection;
        }

        #endregion
    }
}
