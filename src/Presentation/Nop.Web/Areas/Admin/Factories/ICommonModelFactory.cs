using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents common models factory
/// </summary>
public partial interface ICommonModelFactory
{
    /// <summary>
    /// Prepare system info model
    /// </summary>
    /// <param name="model">System info model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the system info model
    /// </returns>
    Task<SystemInfoModel> PrepareSystemInfoModelAsync(SystemInfoModel model);

    /// <summary>
    /// Prepare system warning models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of system warning models
    /// </returns>
    Task<IList<SystemWarningModel>> PrepareSystemWarningModelsAsync();

    /// <summary>
    /// Prepare maintenance model
    /// </summary>
    /// <param name="model">Maintenance model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the maintenance model
    /// </returns>
    Task<MaintenanceModel> PrepareMaintenanceModelAsync(MaintenanceModel model);

    /// <summary>
    /// Prepare paged backup file list model
    /// </summary>
    /// <param name="searchModel">Backup file search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the backup file list model
    /// </returns>
    Task<BackupFileListModel> PrepareBackupFileListModelAsync(BackupFileSearchModel searchModel);

    /// <summary>
    /// Prepare URL record search model
    /// </summary>
    /// <param name="searchModel">URL record search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the uRL record search model
    /// </returns>
    Task<UrlRecordSearchModel> PrepareUrlRecordSearchModelAsync(UrlRecordSearchModel searchModel);

    /// <summary>
    /// Prepare paged URL record list model
    /// </summary>
    /// <param name="searchModel">URL record search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the uRL record list model
    /// </returns>
    Task<UrlRecordListModel> PrepareUrlRecordListModelAsync(UrlRecordSearchModel searchModel);

    /// <summary>
    /// Prepare language selector model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language selector model
    /// </returns>
    Task<LanguageSelectorModel> PrepareLanguageSelectorModelAsync();

    /// <summary>
    /// Prepare popular search term search model
    /// </summary>
    /// <param name="searchModel">Popular search term search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the popular search term search model
    /// </returns>
    Task<PopularSearchTermSearchModel> PreparePopularSearchTermSearchModelAsync(PopularSearchTermSearchModel searchModel);

    /// <summary>
    /// Prepare paged popular search term list model
    /// </summary>
    /// <param name="searchModel">Popular search term search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the popular search term list model
    /// </returns>
    Task<PopularSearchTermListModel> PreparePopularSearchTermListModelAsync(PopularSearchTermSearchModel searchModel);

    /// <summary>
    /// Prepare common statistics model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the common statistics model
    /// </returns>
    Task<CommonStatisticsModel> PrepareCommonStatisticsModelAsync();

    /// <summary>
    /// Prepare plugins warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PreparePluginsWarningModelAsync(IList<SystemWarningModel> models);

    /// <summary>
    /// Prepare multistore preview models
    /// </summary>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <param name="model">Entity model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of multistore preview models
    /// </returns>
    Task<IList<MultistorePreviewModel>> PrepareMultistorePreviewModelsAsync<TModel>(TModel model) where TModel : BaseNopEntityModel;
}