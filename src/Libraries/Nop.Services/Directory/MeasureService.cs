using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Services.Events;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Measure dimension service
    /// </summary>
    public partial class MeasureService : IMeasureService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<MeasureDimension> _measureDimensionRepository;
        private readonly IRepository<MeasureWeight> _measureWeightRepository;
        private readonly MeasureSettings _measureSettings;

        #endregion

        #region Ctor

        public MeasureService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<MeasureDimension> measureDimensionRepository,
            IRepository<MeasureWeight> measureWeightRepository,
            MeasureSettings measureSettings)
        {
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _measureDimensionRepository = measureDimensionRepository;
            _measureWeightRepository = measureWeightRepository;
            _measureSettings = measureSettings;
        }

        #endregion

        #region Methods

        #region Dimensions

        /// <summary>
        /// Deletes measure dimension
        /// </summary>
        /// <param name="measureDimension">Measure dimension</param>
        public virtual void DeleteMeasureDimension(MeasureDimension measureDimension)
        {
            if (measureDimension == null)
                throw new ArgumentNullException(nameof(measureDimension));

            _measureDimensionRepository.Delete(measureDimension);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.MeasureDimensionsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(measureDimension);
        }

        /// <summary>
        /// Gets a measure dimension by identifier
        /// </summary>
        /// <param name="measureDimensionId">Measure dimension identifier</param>
        /// <returns>Measure dimension</returns>
        public virtual MeasureDimension GetMeasureDimensionById(int measureDimensionId)
        {
            if (measureDimensionId == 0)
                return null;

            var key = string.Format(NopDirectoryDefaults.MeasureDimensionsByIdCacheKey, measureDimensionId);
            return _cacheManager.Get(key, () => _measureDimensionRepository.GetById(measureDimensionId));
        }

        /// <summary>
        /// Gets a measure dimension by system keyword
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <returns>Measure dimension</returns>
        public virtual MeasureDimension GetMeasureDimensionBySystemKeyword(string systemKeyword)
        {
            if (string.IsNullOrEmpty(systemKeyword))
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
        /// <returns>Measure dimensions</returns>
        public virtual IList<MeasureDimension> GetAllMeasureDimensions()
        {
            return _cacheManager.Get(NopDirectoryDefaults.MeasureDimensionsAllCacheKey, () =>
            {
                var query = from md in _measureDimensionRepository.Table
                            orderby md.DisplayOrder, md.Id
                            select md;
                var measureDimensions = query.ToList();
                return measureDimensions;
            });
        }

        /// <summary>
        /// Inserts a measure dimension
        /// </summary>
        /// <param name="measure">Measure dimension</param>
        public virtual void InsertMeasureDimension(MeasureDimension measure)
        {
            if (measure == null)
                throw new ArgumentNullException(nameof(measure));

            _measureDimensionRepository.Insert(measure);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.MeasureDimensionsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(measure);
        }

        /// <summary>
        /// Updates the measure dimension
        /// </summary>
        /// <param name="measure">Measure dimension</param>
        public virtual void UpdateMeasureDimension(MeasureDimension measure)
        {
            if (measure == null)
                throw new ArgumentNullException(nameof(measure));

            _measureDimensionRepository.Update(measure);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.MeasureDimensionsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(measure);
        }

        /// <summary>
        /// Converts dimension
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="sourceMeasureDimension">Source dimension</param>
        /// <param name="targetMeasureDimension">Target dimension</param>
        /// <param name="round">A value indicating whether a result should be rounded</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertDimension(decimal value,
            MeasureDimension sourceMeasureDimension, MeasureDimension targetMeasureDimension, bool round = true)
        {
            if (sourceMeasureDimension == null)
                throw new ArgumentNullException(nameof(sourceMeasureDimension));

            if (targetMeasureDimension == null)
                throw new ArgumentNullException(nameof(targetMeasureDimension));

            var result = value;
            if (result != decimal.Zero && sourceMeasureDimension.Id != targetMeasureDimension.Id)
            {
                result = ConvertToPrimaryMeasureDimension(result, sourceMeasureDimension);
                result = ConvertFromPrimaryMeasureDimension(result, targetMeasureDimension);
            }

            if (round)
                result = Math.Round(result, 2);

            return result;
        }

        /// <summary>
        /// Converts to primary measure dimension
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="sourceMeasureDimension">Source dimension</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertToPrimaryMeasureDimension(decimal value,
            MeasureDimension sourceMeasureDimension)
        {
            if (sourceMeasureDimension == null)
                throw new ArgumentNullException(nameof(sourceMeasureDimension));

            var result = value;
            var baseDimensionIn = GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            if (result == decimal.Zero || sourceMeasureDimension.Id == baseDimensionIn.Id) 
                return result;

            var exchangeRatio = sourceMeasureDimension.Ratio;
            if (exchangeRatio == decimal.Zero)
                throw new NopException($"Exchange ratio not set for dimension [{sourceMeasureDimension.Name}]");
            result = result / exchangeRatio;

            return result;
        }

        /// <summary>
        /// Converts from primary dimension
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetMeasureDimension">Target dimension</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertFromPrimaryMeasureDimension(decimal value,
            MeasureDimension targetMeasureDimension)
        {
            if (targetMeasureDimension == null)
                throw new ArgumentNullException(nameof(targetMeasureDimension));

            var result = value;
            var baseDimensionIn = GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            if (result == decimal.Zero || targetMeasureDimension.Id == baseDimensionIn.Id) 
                return result;

            var exchangeRatio = targetMeasureDimension.Ratio;
            if (exchangeRatio == decimal.Zero)
                throw new NopException($"Exchange ratio not set for dimension [{targetMeasureDimension.Name}]");
            result = result * exchangeRatio;

            return result;
        }

        #endregion

        #region Weights

        /// <summary>
        /// Deletes measure weight
        /// </summary>
        /// <param name="measureWeight">Measure weight</param>
        public virtual void DeleteMeasureWeight(MeasureWeight measureWeight)
        {
            if (measureWeight == null)
                throw new ArgumentNullException(nameof(measureWeight));

            _measureWeightRepository.Delete(measureWeight);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.MeasureWeightsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(measureWeight);
        }

        /// <summary>
        /// Gets a measure weight by identifier
        /// </summary>
        /// <param name="measureWeightId">Measure weight identifier</param>
        /// <returns>Measure weight</returns>
        public virtual MeasureWeight GetMeasureWeightById(int measureWeightId)
        {
            if (measureWeightId == 0)
                return null;

            var key = string.Format(NopDirectoryDefaults.MeasureWeightsByIdCacheKey, measureWeightId);
            return _cacheManager.Get(key, () => _measureWeightRepository.GetById(measureWeightId));
        }

        /// <summary>
        /// Gets a measure weight by system keyword
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <returns>Measure weight</returns>
        public virtual MeasureWeight GetMeasureWeightBySystemKeyword(string systemKeyword)
        {
            if (string.IsNullOrEmpty(systemKeyword))
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
        /// <returns>Measure weights</returns>
        public virtual IList<MeasureWeight> GetAllMeasureWeights()
        {
            return _cacheManager.Get(NopDirectoryDefaults.MeasureWeightsAllCacheKey, () =>
            {
                var query = from mw in _measureWeightRepository.Table
                            orderby mw.DisplayOrder, mw.Id
                            select mw;
                var measureWeights = query.ToList();
                return measureWeights;
            });
        }

        /// <summary>
        /// Inserts a measure weight
        /// </summary>
        /// <param name="measure">Measure weight</param>
        public virtual void InsertMeasureWeight(MeasureWeight measure)
        {
            if (measure == null)
                throw new ArgumentNullException(nameof(measure));

            _measureWeightRepository.Insert(measure);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.MeasureWeightsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(measure);
        }

        /// <summary>
        /// Updates the measure weight
        /// </summary>
        /// <param name="measure">Measure weight</param>
        public virtual void UpdateMeasureWeight(MeasureWeight measure)
        {
            if (measure == null)
                throw new ArgumentNullException(nameof(measure));

            _measureWeightRepository.Update(measure);

            _cacheManager.RemoveByPrefix(NopDirectoryDefaults.MeasureWeightsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(measure);
        }

        /// <summary>
        /// Converts weight
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="sourceMeasureWeight">Source weight</param>
        /// <param name="targetMeasureWeight">Target weight</param>
        /// <param name="round">A value indicating whether a result should be rounded</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertWeight(decimal value,
            MeasureWeight sourceMeasureWeight, MeasureWeight targetMeasureWeight, bool round = true)
        {
            if (sourceMeasureWeight == null)
                throw new ArgumentNullException(nameof(sourceMeasureWeight));

            if (targetMeasureWeight == null)
                throw new ArgumentNullException(nameof(targetMeasureWeight));

            var result = value;
            if (result != decimal.Zero && sourceMeasureWeight.Id != targetMeasureWeight.Id)
            {
                result = ConvertToPrimaryMeasureWeight(result, sourceMeasureWeight);
                result = ConvertFromPrimaryMeasureWeight(result, targetMeasureWeight);
            }

            if (round)
                result = Math.Round(result, 2);

            return result;
        }

        /// <summary>
        /// Converts to primary measure weight
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="sourceMeasureWeight">Source weight</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertToPrimaryMeasureWeight(decimal value, MeasureWeight sourceMeasureWeight)
        {
            if (sourceMeasureWeight == null)
                throw new ArgumentNullException(nameof(sourceMeasureWeight));

            var result = value;
            var baseWeightIn = GetMeasureWeightById(_measureSettings.BaseWeightId);
            if (result == decimal.Zero || sourceMeasureWeight.Id == baseWeightIn.Id)
                return result;

            var exchangeRatio = sourceMeasureWeight.Ratio;
            if (exchangeRatio == decimal.Zero)
                throw new NopException($"Exchange ratio not set for weight [{sourceMeasureWeight.Name}]");
            result = result / exchangeRatio;

            return result;
        }

        /// <summary>
        /// Converts from primary weight
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetMeasureWeight">Target weight</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertFromPrimaryMeasureWeight(decimal value,
            MeasureWeight targetMeasureWeight)
        {
            if (targetMeasureWeight == null)
                throw new ArgumentNullException(nameof(targetMeasureWeight));

            var result = value;
            var baseWeightIn = GetMeasureWeightById(_measureSettings.BaseWeightId);
            if (result == decimal.Zero || targetMeasureWeight.Id == baseWeightIn.Id) 
                return result;

            var exchangeRatio = targetMeasureWeight.Ratio;
            if (exchangeRatio == decimal.Zero)
                throw new NopException($"Exchange ratio not set for weight [{targetMeasureWeight.Name}]");
            result = result * exchangeRatio;

            return result;
        }

        #endregion

        #endregion
    }
}