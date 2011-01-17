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
        /// <returns>Measure dimension collection</returns>
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
        /// <param name="quantity">Quantity</param>
        /// <param name="sourceMeasureDimension">Source dimension</param>
        /// <param name="targetMeasureDimension">Target dimension</param>
        /// <returns>Converted value</returns>
        decimal ConvertDimension(decimal quantity,
            MeasureDimension sourceMeasureDimension, MeasureDimension targetMeasureDimension);

        /// <summary>
        /// Converts to primary measure dimension
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="sourceMeasureDimension">Source dimension</param>
        /// <returns>Converted value</returns>
        decimal ConvertToPrimaryMeasureDimension(decimal quantity,
            MeasureDimension sourceMeasureDimension);

        /// <summary>
        /// Converts from primary dimension
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="targetMeasureDimension">Target dimension</param>
        /// <returns>Converted value</returns>
        decimal ConvertFromPrimaryMeasureDimension(decimal quantity,
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
        /// <returns>Measure weight collection</returns>
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
        /// <param name="quantity">Quantity</param>
        /// <param name="sourceMeasureWeight">Source weight</param>
        /// <param name="targetMeasureWeight">Target weight</param>
        /// <returns>Converted value</returns>
        decimal ConvertWeight(decimal quantity,
            MeasureWeight sourceMeasureWeight, MeasureWeight targetMeasureWeight);

        /// <summary>
        /// Converts to primary measure weight
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="sourceMeasureWeight">Source weight</param>
        /// <returns>Converted value</returns>
        decimal ConvertToPrimaryMeasureWeight(decimal quantity, MeasureWeight sourceMeasureWeight);

        /// <summary>
        /// Converts from primary weight
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="targetMeasureWeight">Target weight</param>
        /// <returns>Converted value</returns>
        decimal ConvertFromPrimaryMeasureWeight(decimal quantity,
            MeasureWeight targetMeasureWeight);
    }
}