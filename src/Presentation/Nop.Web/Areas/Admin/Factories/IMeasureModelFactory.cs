using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Models.Directory;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the measures model factory
    /// </summary>
    public partial interface IMeasureModelFactory
    {
        /// <summary>
        /// Prepare measure search model
        /// </summary>
        /// <param name="searchModel">Measure search model</param>
        /// <returns>Measure search model</returns>
        Task<MeasureSearchModel> PrepareMeasureSearchModelAsync(MeasureSearchModel searchModel);

        /// <summary>
        /// Prepare paged measure dimension list model
        /// </summary>
        /// <param name="searchModel">Measure dimension search model</param>
        /// <returns>Measure dimension list model</returns>
        Task<MeasureDimensionListModel> PrepareMeasureDimensionListModelAsync(MeasureDimensionSearchModel searchModel);

        /// <summary>
        /// Prepare paged measure weight list model
        /// </summary>
        /// <param name="searchModel">Measure weight search model</param>
        /// <returns>Measure weight list model</returns>
        Task<MeasureWeightListModel> PrepareMeasureWeightListModelAsync(MeasureWeightSearchModel searchModel);
    }
}