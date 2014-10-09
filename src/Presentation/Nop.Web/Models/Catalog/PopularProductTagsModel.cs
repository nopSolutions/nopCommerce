using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class PopularProductTagsModel : BaseNopModel
    {
        public PopularProductTagsModel()
        {
            Tags = new List<ProductTagModel>();
        }

        #region Utilities

        protected virtual int GetFontSize(double weight, double mean, double stdDev)
        {
            double factor = (weight - mean);

            if (factor != 0 && stdDev != 0) factor /= stdDev;

            return (factor > 2) ? 150 :
                (factor > 1) ? 120 :
                (factor > 0.5) ? 100 :
                (factor > -0.5) ? 90 :
                (factor > -1) ? 85 :
                (factor > -2) ? 80 :
                75;
        }

        protected virtual double Mean(IEnumerable<double> values)
        {
            double sum = 0;
            int count = 0;

            foreach (double d in values)
            {
                sum += d;
                count++;
            }

            return sum / count;
        }

        protected virtual double StdDev(IEnumerable<double> values, out double mean)
        {
            mean = Mean(values);
            double sumOfDiffSquares = 0;
            int count = 0;

            foreach (double d in values)
            {
                double diff = (d - mean);
                sumOfDiffSquares += diff * diff;
                count++;
            }

            return Math.Sqrt(sumOfDiffSquares / count);
        }

        #endregion

        #region Methods

        public virtual int GetFontSize(ProductTagModel productTag)
        {
            var itemWeights = new List<double>();
            foreach (var tag in Tags)
                itemWeights.Add(tag.ProductCount);
            double mean;
            double stdDev = StdDev(itemWeights, out mean);

            return GetFontSize(productTag.ProductCount, mean, stdDev);
        }

        #endregion

        #region Properties

        public int TotalTags { get; set; }

        public IList<ProductTagModel> Tags { get; set; }

        #endregion
    }
}