using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs
{
    public partial record BlogPostTagListModel : BaseNopModel
    {
        public BlogPostTagListModel()
        {
            Tags = new List<BlogPostTagModel>();
        }

        public int GetFontSize(BlogPostTagModel blogPostTag)
        {
            if (blogPostTag == null)
                throw new ArgumentNullException(nameof(blogPostTag));

            var itemWeights = new List<double>();
            foreach (var tag in Tags)
                itemWeights.Add(tag.BlogPostCount);

            var stdDev = StdDev(itemWeights, out var mean);
            return GetFontSize(blogPostTag.BlogPostCount, mean, stdDev);
        }

        protected int GetFontSize(double weight, double mean, double stdDev)
        {
            var factor = (weight - mean);

            if (factor != 0 && stdDev != 0)
                factor /= stdDev;

            return (factor > 2) ? 150 :
                (factor > 1) ? 120 :
                (factor > 0.5) ? 100 :
                (factor > -0.5) ? 90 :
                (factor > -1) ? 85 :
                (factor > -2) ? 80 :
                75;
        }

        protected double Mean(IEnumerable<double> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            double sum = 0;
            var count = 0;

            foreach (var d in values)
            {
                sum += d;
                count++;
            }

            if (count == 0)
                return 0;
            return sum / count;
        }

        protected double StdDev(IEnumerable<double> values, out double mean)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            mean = Mean(values);

            double sumOfDiffSquares = 0;
            var count = 0;

            foreach (var d in values)
            {
                var diff = (d - mean);
                sumOfDiffSquares += diff * diff;
                count++;
            }

            if (count == 0)
                return 0;
            return Math.Sqrt(sumOfDiffSquares / count);
        }


        public List<BlogPostTagModel> Tags { get; set; }
    }
}