using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the measures model factory implementation
/// </summary>
public partial class MeasureModelFactory : IMeasureModelFactory
{
    #region Fields

    protected readonly IMeasureService _measureService;
    protected readonly MeasureSettings _measureSettings;

    #endregion

    #region Ctor

    public MeasureModelFactory(IMeasureService measureService,
        MeasureSettings measureSettings)
    {
        _measureService = measureService;
        _measureSettings = measureSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare measure dimension search model
    /// </summary>
    /// <param name="searchModel">Measure dimension search model</param>
    /// <returns>Measure dimension search model</returns>
    protected virtual MeasureDimensionSearchModel PrepareMeasureDimensionSearchModel(MeasureDimensionSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare measure weight search model
    /// </summary>
    /// <param name="searchModel">Measure weight search model</param>
    /// <returns>Measure weight search model</returns>
    protected virtual MeasureWeightSearchModel PrepareMeasureWeightSearchModel(MeasureWeightSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare measure search model
    /// </summary>
    /// <param name="searchModel">Measure search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the measure search model
    /// </returns>
    public virtual Task<MeasureSearchModel> PrepareMeasureSearchModelAsync(MeasureSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare nested search models
        PrepareMeasureDimensionSearchModel(searchModel.MeasureDimensionSearchModel);
        PrepareMeasureWeightSearchModel(searchModel.MeasureWeightSearchModel);

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged measure dimension list model
    /// </summary>
    /// <param name="searchModel">Measure dimension search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the measure dimension list model
    /// </returns>
    public virtual async Task<MeasureDimensionListModel> PrepareMeasureDimensionListModelAsync(MeasureDimensionSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get dimensions
        var dimensions = (await _measureService.GetAllMeasureDimensionsAsync()).ToPagedList(searchModel);

        //prepare list model
        var model = new MeasureDimensionListModel().PrepareToGrid(searchModel, dimensions, () =>
        {
            return dimensions.Select(dimension =>
            {
                //fill in model values from the entity
                var dimensionModel = dimension.ToModel<MeasureDimensionModel>();

                //fill in additional values (not existing in the entity)
                dimensionModel.IsPrimaryDimension = dimension.Id == _measureSettings.BaseDimensionId;

                return dimensionModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged measure weight list model
    /// </summary>
    /// <param name="searchModel">Measure weight search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the measure weight list model
    /// </returns>
    public virtual async Task<MeasureWeightListModel> PrepareMeasureWeightListModelAsync(MeasureWeightSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get weights
        var weights = (await _measureService.GetAllMeasureWeightsAsync()).ToPagedList(searchModel);

        //prepare list model
        var model = new MeasureWeightListModel().PrepareToGrid(searchModel, weights, () =>
        {
            return weights.Select(weight =>
            {
                //fill in model values from the entity
                var weightModel = weight.ToModel<MeasureWeightModel>();

                //fill in additional values (not existing in the entity)
                weightModel.IsPrimaryWeight = weight.Id == _measureSettings.BaseWeightId;

                return weightModel;
            });
        });

        return model;
    }

    #endregion
}