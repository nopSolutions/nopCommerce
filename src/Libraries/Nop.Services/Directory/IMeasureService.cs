using System.Collections.Generic;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Measure dimension service interface
    /// </summary>
    public partial interface IMeasureService
    {
        /// <summary>
        /// Deletes measure dimension
        /// </summary>
        /// <param name="measureDimension">Measure dimension</param>
        void DeleteMeasureDimension(MeasureDimension measureDimension);

        /// <summary>
        /// Gets a measure dimension by identifier
        /// </summary>
        /// <param name="measureDimensionId">Measure dimension identifier</param>
        /// <returns>Measure dimension</returns>
        MeasureDimension GetMeasureDimensionById(int measureDimensionId);

        /// <summary>
        /// Gets a measure dimension by system keyword
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <returns>Measure dimension</returns>
        MeasureDimension GetMeasureDimensionBySystemKeyword(string systemKeyword);

        /// <summary>
        /// Gets all measure dimensions
        /// </summary>
        /// <returns>Measure dimensions</returns>
        IList<MeasureDimension> GetAllMeasureDimensions();

        /// <summary>
        /// Inserts a measure dimension
        /// </summary>
        /// <param name="measure">Measure dimension</param>
        void InsertMeasureDimension(MeasureDimension measure);

        /// <summary>
        /// Updates the measure dimension
        /// </summary>
        /// <param name="measure">Measure dimension</param>
        void UpdateMeasureDimension(MeasureDimension measure);

        /// <summary>
        /// Converts dimension
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="sourceMeasureDimension">Source dimension</param>
        /// <param name="targetMeasureDimension">Target dimension</param>
        /// <param name="round">A value indicating whether a result should be rounded</param>
        /// <returns>Converted value</returns>
        decimal ConvertDimension(decimal value,
            MeasureDimension sourceMeasureDimension, MeasureDimension targetMeasureDimension, bool round = true);

        /// <summary>
        /// Converts to primary measure dimension
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="sourceMeasureDimension">Source dimension</param>
        /// <returns>Converted value</returns>
        decimal ConvertToPrimaryMeasureDimension(decimal value,
            MeasureDimension sourceMeasureDimension);

        /// <summary>
        /// Converts from primary dimension
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetMeasureDimension">Target dimension</param>
        /// <returns>Converted value</returns>
        decimal ConvertFromPrimaryMeasureDimension(decimal value,
            MeasureDimension targetMeasureDimension);

        /// <summary>
        /// Deletes measure weight
        /// </summary>
        /// <param name="measureWeight">Measure weight</param>
        void DeleteMeasureWeight(MeasureWeight measureWeight);

        /// <summary>
        /// Gets a measure weight by identifier
        /// </summary>
        /// <param name="measureWeightId">Measure weight identifier</param>
        /// <returns>Measure weight</returns>
        MeasureWeight GetMeasureWeightById(int measureWeightId);

        /// <summary>
        /// Gets a measure weight by system keyword
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <returns>Measure weight</returns>
        MeasureWeight GetMeasureWeightBySystemKeyword(string systemKeyword);

        /// <summary>
        /// Gets all measure weights
        /// </summary>
        /// <returns>Measure weights</returns>
        IList<MeasureWeight> GetAllMeasureWeights();

        /// <summary>
        /// Inserts a measure weight
        /// </summary>
        /// <param name="measure">Measure weight</param>
        void InsertMeasureWeight(MeasureWeight measure);

        /// <summary>
        /// Updates the measure weight
        /// </summary>
        /// <param name="measure">Measure weight</param>
        void UpdateMeasureWeight(MeasureWeight measure);

        /// <summary>
        /// Converts weight
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="sourceMeasureWeight">Source weight</param>
        /// <param name="targetMeasureWeight">Target weight</param>
        /// <param name="round">A value indicating whether a result should be rounded</param>
        /// <returns>Converted value</returns>
        decimal ConvertWeight(decimal value,
            MeasureWeight sourceMeasureWeight, MeasureWeight targetMeasureWeight, bool round = true);

        /// <summary>
        /// Converts to primary measure weight
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="sourceMeasureWeight">Source weight</param>
        /// <returns>Converted value</returns>
        decimal ConvertToPrimaryMeasureWeight(decimal value, MeasureWeight sourceMeasureWeight);

        /// <summary>
        /// Converts from primary weight
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetMeasureWeight">Target weight</param>
        /// <returns>Converted value</returns>
        decimal ConvertFromPrimaryMeasureWeight(decimal value,
            MeasureWeight targetMeasureWeight);
    }
}