
using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Directory;
using Nop.Data;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Measure dimension service
    /// </summary>
    public partial class MeasureService : IMeasureService
    {
        #region Constants
        private const string MEASUREDIMENSIONS_ALL_KEY = "Nop.measuredimension.all";
        private const string MEASUREDIMENSIONS_BY_ID_KEY = "Nop.measuredimension.id-{0}";
        private const string MEASUREWEIGHTS_ALL_KEY = "Nop.measureweight.all";
        private const string MEASUREWEIGHTS_BY_ID_KEY = "Nop.measureweight.id-{0}";
        private const string MEASUREDIMENSIONS_PATTERN_KEY = "Nop.measuredimension.";
        private const string MEASUREWEIGHTS_PATTERN_KEY = "Nop.measureweight.";
        #endregion

        #region Fields

        private readonly IRepository<MeasureDimension> _measureDimensionRepository;
        private readonly IRepository<MeasureWeight> _measureWeightRepository;
        private readonly ICacheManager _cacheManager;
        private readonly MeasureSettings _measureSettings;

        #endregion

        #region Ctor

       /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="measureDimensionRepository">Dimension repository</param>
        /// <param name="measureWeightRepository">Weight repository</param>
        /// <param name="measureSettings">Measure settings</param>
        public MeasureService(ICacheManager cacheManager,
            IRepository<MeasureDimension> measureDimensionRepository,
            IRepository<MeasureWeight> measureWeightRepository,
            MeasureSettings measureSettings)
        {
            this._cacheManager = cacheManager;
            this._measureDimensionRepository = measureDimensionRepository;
            this._measureWeightRepository = measureWeightRepository;
            this._measureSettings = measureSettings;
        }

        #endregion

        #region Methods

        #region Dimensions

        /// <summary>
        /// Deletes measure dimension
        /// </summary>
        /// <param name="measureDimension">Measure dimension</param>
        public void DeleteMeasureDimension(MeasureDimension measureDimension)
        {
            if (measureDimension == null)
                return;

            _measureDimensionRepository.Delete(measureDimension);

            _cacheManager.RemoveByPattern(MEASUREDIMENSIONS_PATTERN_KEY);
        }
        

        /// <summary>
        /// Gets a measure dimension by identifier
        /// </summary>
        /// <param name="measureDimensionId">Measure dimension identifier</param>
        /// <returns>Measure dimension</returns>
        public MeasureDimension GetMeasureDimensionById(int measureDimensionId)
        {
            if (measureDimensionId == 0)
                return null;

            string key = string.Format(MEASUREDIMENSIONS_BY_ID_KEY, measureDimensionId);
            return _cacheManager.Get(key, () =>
            {
                return _measureDimensionRepository.GetById(measureDimensionId);
            });
        }

        /// <summary>
        /// Gets a measure dimension by system keyword
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <returns>Measure dimension</returns>
        public MeasureDimension GetMeasureDimensionBySystemKeyword(string systemKeyword)
        {
            if (String.IsNullOrEmpty(systemKeyword))
                return null;

            var measureDimensions = GetAllMeasureDimensions();
            foreach (var measureDimension in measureDimensions)
                if (measureDimension.SystemKeyword.ToLowerInvariant() == systemKeyword.ToLowerInvariant())
                    return measureDimension;
            return null;
        }

        /// <summary>
        /// Gets all measure dimensions
        /// </summary>
        /// <returns>Measure dimension collection</returns>
        public IList<MeasureDimension> GetAllMeasureDimensions()
        {
            string key = MEASUREDIMENSIONS_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from md in _measureDimensionRepository.Table
                            orderby md.DisplayOrder
                            select md;
                var measureDimensions = query.ToList();
                return measureDimensions;

            });
        }

        /// <summary>
        /// Inserts a measure dimension
        /// </summary>
        /// <param name="measure">Measure dimension</param>
        public void InsertMeasureDimension(MeasureDimension measure)
        {
            if (measure == null)
                throw new ArgumentNullException("measure");

            _measureDimensionRepository.Insert(measure);

            _cacheManager.RemoveByPattern(MEASUREDIMENSIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the measure dimension
        /// </summary>
        /// <param name="measure">Measure dimension</param>
        public void UpdateMeasureDimension(MeasureDimension measure)
        {
            if (measure == null)
                throw new ArgumentNullException("measure");

            _measureDimensionRepository.Update(measure);

            _cacheManager.RemoveByPattern(MEASUREDIMENSIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Converts dimension
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="sourceMeasureDimension">Source dimension</param>
        /// <param name="targetMeasureDimension">Target dimension</param>
        /// <returns>Converted value</returns>
        public decimal ConvertDimension(decimal quantity,
            MeasureDimension sourceMeasureDimension, MeasureDimension targetMeasureDimension)
        {
            decimal result = quantity;
            if (sourceMeasureDimension.Id == targetMeasureDimension.Id)
                return result;
            if (result != decimal.Zero && sourceMeasureDimension.Id != targetMeasureDimension.Id)
            {
                result = ConvertToPrimaryMeasureDimension(result, sourceMeasureDimension);
                result = ConvertFromPrimaryMeasureDimension(result, targetMeasureDimension);
            }
            result = Math.Round(result, 2);
            return result;
        }

        /// <summary>
        /// Converts to primary measure dimension
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="sourceMeasureDimension">Source dimension</param>
        /// <returns>Converted value</returns>
        public decimal ConvertToPrimaryMeasureDimension(decimal quantity,
            MeasureDimension sourceMeasureDimension)
        {
            decimal result = quantity;
            var baseDimensionIn = GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            if (result != decimal.Zero && sourceMeasureDimension.Id != baseDimensionIn.Id)
            {
                decimal exchangeRatio = sourceMeasureDimension.Ratio;
                if (exchangeRatio == decimal.Zero)
                    throw new NopException(string.Format("Exchange ratio not set for dimension [{0}]", sourceMeasureDimension.Name));
                result = result / exchangeRatio;
            }
            return result;
        }

        /// <summary>
        /// Converts from primary dimension
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="targetMeasureDimension">Target dimension</param>
        /// <returns>Converted value</returns>
        public decimal ConvertFromPrimaryMeasureDimension(decimal quantity,
            MeasureDimension targetMeasureDimension)
        {
            decimal result = quantity;
            var baseDimensionIn = GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            if (result != decimal.Zero && targetMeasureDimension.Id != baseDimensionIn.Id)
            {
                decimal exchangeRatio = targetMeasureDimension.Ratio;
                if (exchangeRatio == decimal.Zero)
                    throw new NopException(string.Format("Exchange ratio not set for dimension [{0}]", targetMeasureDimension.Name));
                result = result * exchangeRatio;
            }
            return result;
        }

        #endregion

        #region Weights

        /// <summary>
        /// Deletes measure weight
        /// </summary>
        /// <param name="measureWeight">Measure weight</param>
        public void DeleteMeasureWeight(MeasureWeight measureWeight)
        {
            if (measureWeight == null)
                return;

            _measureWeightRepository.Delete(measureWeight);

            _cacheManager.RemoveByPattern(MEASUREWEIGHTS_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a measure weight by identifier
        /// </summary>
        /// <param name="measureWeightId">Measure weight identifier</param>
        /// <returns>Measure weight</returns>
        public MeasureWeight GetMeasureWeightById(int measureWeightId)
        {
            if (measureWeightId == 0)
                return null;

            string key = string.Format(MEASUREWEIGHTS_BY_ID_KEY, measureWeightId);
            return _cacheManager.Get(key, () =>
            {
                return _measureWeightRepository.GetById(measureWeightId);
            });
        }

        /// <summary>
        /// Gets a measure weight by system keyword
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <returns>Measure weight</returns>
        public MeasureWeight GetMeasureWeightBySystemKeyword(string systemKeyword)
        {
            if (String.IsNullOrEmpty(systemKeyword))
                return null;

            var measureWeights = GetAllMeasureWeights();
            foreach (var measureWeight in measureWeights)
                if (measureWeight.SystemKeyword.ToLowerInvariant() == systemKeyword.ToLowerInvariant())
                    return measureWeight;
            return null;
        }

        /// <summary>
        /// Gets all measure weights
        /// </summary>
        /// <returns>Measure weight collection</returns>
        public IList<MeasureWeight> GetAllMeasureWeights()
        {
            string key = MEASUREWEIGHTS_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from mw in _measureWeightRepository.Table
                            orderby mw.DisplayOrder
                            select mw;
                var measureWeights = query.ToList();
                return measureWeights;
            });
        }

        /// <summary>
        /// Inserts a measure weight
        /// </summary>
        /// <param name="measure">Measure weight</param>
        public void InsertMeasureWeight(MeasureWeight measure)
        {
            if (measure == null)
                throw new ArgumentNullException("measure");

            _measureWeightRepository.Insert(measure);

            _cacheManager.RemoveByPattern(MEASUREWEIGHTS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the measure weight
        /// </summary>
        /// <param name="measure">Measure weight</param>
        public void UpdateMeasureWeight(MeasureWeight measure)
        {
            if (measure == null)
                throw new ArgumentNullException("measure");

            _measureWeightRepository.Update(measure);

            _cacheManager.RemoveByPattern(MEASUREWEIGHTS_PATTERN_KEY);
        }

        /// <summary>
        /// Converts weight
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="sourceMeasureWeight">Source weight</param>
        /// <param name="targetMeasureWeight">Target weight</param>
        /// <returns>Converted value</returns>
        public decimal ConvertWeight(decimal quantity,
            MeasureWeight sourceMeasureWeight, MeasureWeight targetMeasureWeight)
        {
            decimal result = quantity;
            if (sourceMeasureWeight.Id == targetMeasureWeight.Id)
                return result;
            if (result != decimal.Zero && sourceMeasureWeight.Id != targetMeasureWeight.Id)
            {
                result = ConvertToPrimaryMeasureWeight(result, sourceMeasureWeight);
                result = ConvertFromPrimaryMeasureWeight(result, targetMeasureWeight);
            }
            result = Math.Round(result, 2);
            return result;
        }

        /// <summary>
        /// Converts to primary measure weight
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="sourceMeasureWeight">Source weight</param>
        /// <returns>Converted value</returns>
        public decimal ConvertToPrimaryMeasureWeight(decimal quantity, MeasureWeight sourceMeasureWeight)
        {
            decimal result = quantity;
            var baseWeightIn = GetMeasureWeightById(_measureSettings.BaseWeightId);
            if (result != decimal.Zero && sourceMeasureWeight.Id != baseWeightIn.Id)
            {
                decimal exchangeRatio = sourceMeasureWeight.Ratio;
                if (exchangeRatio == decimal.Zero)
                    throw new NopException(string.Format("Exchange ratio not set for weight [{0}]", sourceMeasureWeight.Name));
                result = result / exchangeRatio;
            }
            return result;
        }

        /// <summary>
        /// Converts from primary weight
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="targetMeasureWeight">Target weight</param>
        /// <returns>Converted value</returns>
        public decimal ConvertFromPrimaryMeasureWeight(decimal quantity,
            MeasureWeight targetMeasureWeight)
        {
            decimal result = quantity;
            var baseWeightIn = GetMeasureWeightById(_measureSettings.BaseWeightId);
            if (result != decimal.Zero && targetMeasureWeight.Id != baseWeightIn.Id)
            {
                decimal exchangeRatio = targetMeasureWeight.Ratio;
                if (exchangeRatio == decimal.Zero)
                    throw new NopException(string.Format("Exchange ratio not set for weight [{0}]", targetMeasureWeight.Name));
                result = result * exchangeRatio;
            }
            return result;
        }

        #endregion

        #endregion
    }
}