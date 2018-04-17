using System.Collections.Generic;
using Nop.Web.Areas.Admin.Models.Common;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents common models factory
    /// </summary>
    public partial interface ICommonModelFactory
    {
        /// <summary>
        /// Prepare system info model
        /// </summary>
        /// <param name="model">System info model</param>
        /// <returns>System info model</returns>
        SystemInfoModel PrepareSystemInfoModel(SystemInfoModel model);

        /// <summary>
        /// Prepare system warning models
        /// </summary>
        /// <returns>List of system warning models</returns>
        IList<SystemWarningModel> PrepareSystemWarningModels();

        /// <summary>
        /// Prepare maintenance model
        /// </summary>
        /// <param name="model">Maintenance model</param>
        /// <returns>Maintenance model</returns>
        MaintenanceModel PrepareMaintenanceModel(MaintenanceModel model);
        
        /// <summary>
        /// Prepare paged backup file list model
        /// </summary>
        /// <param name="searchModel">Backup file search model</param>
        /// <returns>Backup file list model</returns>
        BackupFileListModel PrepareBackupFileListModel(BackupFileSearchModel searchModel);

        /// <summary>
        /// Prepare URL record search model
        /// </summary>
        /// <param name="searchModel">URL record search model</param>
        /// <returns>URL record search model</returns>
        UrlRecordSearchModel PrepareUrlRecordSearchModel(UrlRecordSearchModel searchModel);

        /// <summary>
        /// Prepare paged URL record list model
        /// </summary>
        /// <param name="searchModel">URL record search model</param>
        /// <returns>URL record list model</returns>
        UrlRecordListModel PrepareUrlRecordListModel(UrlRecordSearchModel searchModel);

        /// <summary>
        /// Prepare language selector model
        /// </summary>
        /// <returns>Language selector model</returns>
        LanguageSelectorModel PrepareLanguageSelectorModel();

        /// <summary>
        /// Prepare popular search term search model
        /// </summary>
        /// <param name="searchModel">Popular search term search model</param>
        /// <returns>Popular search term search model</returns>
        PopularSearchTermSearchModel PreparePopularSearchTermSearchModel(PopularSearchTermSearchModel searchModel);

        /// <summary>
        /// Prepare paged popular search term list model
        /// </summary>
        /// <param name="searchModel">Popular search term search model</param>
        /// <returns>Popular search term list model</returns>
        PopularSearchTermListModel PreparePopularSearchTermListModel(PopularSearchTermSearchModel searchModel);

        /// <summary>
        /// Prepare common statistics model
        /// </summary>
        /// <returns>Common statistics model</returns>
        CommonStatisticsModel PrepareCommonStatisticsModel();
    }
}