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
        MeasureSearchModel PrepareMeasureSearchModel(MeasureSearchModel searchModel);

        /// <summary>
        /// Prepare measure dimension search model
        /// </summary>
        /// <param name="searchModel">Measure dimension search model</param>
        /// <returns>Measure dimension search model</returns>
        MeasureDimensionSearchModel PrepareMeasureDimensionSearchModel(MeasureDimensionSearchModel searchModel);

        /// <summary>
        /// Prepare paged measure dimension list model
        /// </summary>
        /// <param name="searchModel">Measure dimension search model</param>
        /// <returns>Measure dimension list model</returns>
        MeasureDimensionListModel PrepareMeasureDimensionListModel(MeasureDimensionSearchModel searchModel);

        /// <summary>
        /// Prepare measure weight search model
        /// </summary>
        /// <param name="searchModel">Measure weight search model</param>
        /// <returns>Measure weight search model</returns>
        MeasureWeightSearchModel PrepareMeasureWeightSearchModel(MeasureWeightSearchModel searchModel);

        /// <summary>
        /// Prepare paged measure weight list model
        /// </summary>
        /// <param name="searchModel">Measure weight search model</param>
        /// <returns>Measure weight list model</returns>
        MeasureWeightListModel PrepareMeasureWeightListModel(MeasureWeightSearchModel searchModel);
    }
}