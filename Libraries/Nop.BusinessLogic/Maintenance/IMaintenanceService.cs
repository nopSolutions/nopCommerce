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
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Maintenance
{
    /// <summary>
    /// Maintenance service interface
    /// </summary>
    public partial interface IMaintenanceService
    {
        /// <summary>
        /// Reindex tables
        /// </summary>
        void Reindex();

        /// <summary>
        /// Backup database
        /// </summary>
        void Backup();

        /// <summary>
        /// Back up pictures
        /// </summary>
        void BackupPictures();

        /// <summary>
        /// Delete files in \files\ExportImport folder (PDF, Excel etc)
        /// </summary>
        /// <param name="hours">Files to delete older than specified hours value</param>
        /// <returns>Number of deleted files</returns>
        int DeleteOldExportImportFiles(int hours);

        /// <summary>
        /// Restore database
        /// </summary>
        /// <param name="fileName">Backup file name</param>
        void RestoreBackup(string fileName);

        /// <summary>
        /// Delete the backup file
        /// </summary>
        /// <param name="fileName">Backup file name</param>
        void DeleteBackup(string fileName);

        /// <summary>
        /// Gets all backup files
        /// </summary>
        /// <returns>Backup file collection</returns>
        List<BackupFile> GetAllBackupFiles();
    }
}
