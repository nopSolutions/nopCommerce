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


using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Controls;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;


namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class PopularTagsControl: BaseNopFrontendUserControl
    {
        double _mean;
        double _stdDev;
        protected override void OnPreRender(EventArgs e)
        {
            BindData();
            base.OnPreRender(e);
        }

        protected void BindData()
        {
            //get all tags
            int maxItems = 15;
            var productTags = IoC.Resolve<IProductService>().GetAllProductTags();
            List<ProductTag> cloudItems = new List<ProductTag>();
            for (int i = 0; i < productTags.Count; i++)
            {
                ProductTag productTag = productTags[i];
                if (i < maxItems)
                {
                    cloudItems.Add(productTag);
                }
            }

            //calculate weights
            _mean = 0;
            List<double> itemWeights = new List<double>();
            foreach (var productTag in cloudItems)
            {
                itemWeights.Add(productTag.ProductCount);
            }
            _stdDev = StdDev(itemWeights, out _mean);
            
            //sorting
            cloudItems.Sort(new ProductTagComparer());

            //binding
            if (cloudItems.Count > 0)
            {
                lvTagCloud.DataSource = cloudItems;
                lvTagCloud.DataBind();
            }
            else
                this.Visible = false;
        }

        protected int GetFontSize(double weight, double mean, double stdDev)
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

        protected int GetFontSize(int productCount)
        {
            int result = GetFontSize(productCount, _mean, _stdDev);
            return result;
        }

        protected double Mean(IEnumerable<double> values)
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

        protected double StdDev(IEnumerable<double> values, out double mean)
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

        protected double StdDev(IEnumerable<double> values)
        {
            double mean;
            return StdDev(values, out mean);
        }	

        protected class ProductTagComparer : IComparer<ProductTag>
        {
            public int Compare(ProductTag x, ProductTag y)
            {
                if (y == null || String.IsNullOrEmpty(y.Name))
                    return -1;
                if (x == null || String.IsNullOrEmpty(x.Name))
                    return 1;
                return x.Name.CompareTo(y.Name);
            }
        }
    }
}