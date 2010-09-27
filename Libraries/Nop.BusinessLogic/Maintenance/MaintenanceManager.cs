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

namespace NopSolutions.NopCommerce.BusinessLogic.Maintenance
{
    /// <summary>
    /// Maintenance manager
    /// </summary>
    public partial class MaintenanceManager
    {
        #region Methods

        /// <summary>
        /// Reindex tables
        /// </summary>
        public static void Reindex()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            context.Sp_Maintenance_ReindexTables();
        }

        /// <summary>
        /// Backup database
        /// </summary>
        public static void Backup()
        {
            string path = SettingManager.GetSettingValue("Maintenance.BackupPath").Trim();
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
        public static void BackupPictures()
        {
            string fileName = string.Format("{0}Administration\\backups\\images_{1}_{2}.zip", HttpContext.Current.Request.PhysicalApplicationPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
            using (ZipFile zipFile = new ZipFile())
            {
                zipFile.AddDirectory(PictureManager.LocalImagePath);
                zipFile.Save(fileName);
            }
        }

        /// <summary>
        /// Restore database
        /// </summary>
        /// <param name="fileName">Backup file name</param>
        public static void RestoreBackup(string fileName)
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
                                zipFile.ExtractAll(PictureManager.LocalImagePath, ExtractExistingFileAction.OverwriteSilently);
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
        public static void DeleteBackup(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        /// <summary>
        /// Gets all backup files
        /// </summary>
        /// <returns>Backup file collection</returns>
        public static List<BackupFile> GetAllBackupFiles()
        {
            var collection = new List<BackupFile>();

            string path = string.Format("{0}Administration\\backups\\", HttpContext.Current.Request.PhysicalApplicationPath);
            foreach (var fullFileName in System.IO.Directory.GetFiles(path))
            {
                var fileName = Path.GetFileName(fullFileName);
                if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var info = new FileInfo(fullFileName);
                collection.Add(new BackupFile()
                {
                    FullFileName = fullFileName,
                    FileName = fileName,
                    FileSize = info.Length
                });
            }

            return collection;
        }

        #endregion
    }
}
