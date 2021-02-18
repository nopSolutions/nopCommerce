using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a measure search model
    /// </summary>
    public partial record MeasureSearchModel : BaseSearchModel
    {
        #region Ctor

        public MeasureSearchModel()
        {
            MeasureDimensionSearchModel = new MeasureDimensionSearchModel();
            MeasureWeightSearchModel = new MeasureWeightSearchModel();
            AddMeasureDimension = new MeasureDimensionModel();
            AddMeasureWeight = new MeasureWeightModel();
        }

        #endregion

        #region Properties

        public MeasureDimensionSearchModel MeasureDimensionSearchModel { get; set; }

        public MeasureWeightSearchModel MeasureWeightSearchModel { get; set; }

        public MeasureDimensionModel AddMeasureDimension { get; set; }
        public MeasureWeightModel AddMeasureWeight { get; set; }
        #endregion
    }
}