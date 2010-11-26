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
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Topics;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.SEO.Sitemaps
{
    /// <summary>
    /// Represents a sitemap generator
    /// </summary>
    public partial class SitemapGenerator : BaseSitemapGenerator
    {
        #region Utilities

        /// <summary>
        /// Method that is overridden, that handles creation of child urls.
        /// Use the method WriteUrlLocation() within this method.
        /// </summary>
        protected override void GenerateUrlNodes()
        {
            bool IncludeCategories = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("SEO.Sitemaps.IncludeCategories", true);
            bool IncludeManufacturers = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("SEO.Sitemaps.IncludeManufacturers", false);
            bool IncludeProducts = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("SEO.Sitemaps.IncludeProducts", false);
            string OtherPages = IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Sitemaps.OtherPages").ToLowerInvariant();
            
            if (IncludeCategories)
            {
                WriteCategories(0);
            }

            if (IncludeManufacturers)
            {
                WriteManufacturers();
            }

            if (IncludeProducts)
            {
                WriteProducts();
            }

            WriteTopics();

            WriteOtherPages(OtherPages);
        }

        private void WriteCategories(int parentCategoryId)
        {
            var categories = IoC.Resolve<ICategoryService>().GetAllCategoriesByParentCategoryId(parentCategoryId, false);
            foreach (var category in categories)
            {
                var url = SEOHelper.GetCategoryUrl(category);
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = category.UpdatedOn;
                WriteUrlLocation(url, updateFrequency, updateTime);

                WriteCategories(category.CategoryId);
            }
        }

        private void WriteManufacturers()
        {
            var manufacturers = IoC.Resolve<IManufacturerService>().GetAllManufacturers(false);
            foreach (var manufacturer in manufacturers)
            {
                var url = SEOHelper.GetManufacturerUrl(manufacturer);
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = manufacturer.UpdatedOn;
                WriteUrlLocation(url, updateFrequency, updateTime);
            }
        }

        private void WriteProducts()
        {
            var products = IoC.Resolve<IProductService>().GetAllProducts(false);
            foreach (var product in products)
            {
                var url = SEOHelper.GetProductUrl(product);
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = product.UpdatedOn;
                WriteUrlLocation(url, updateFrequency, updateTime);
            }
        }

        private void WriteTopics()
        {
            var topics = IoC.Resolve<ITopicService>().GetAllTopics();
            topics = topics.FindAll(t => t.IncludeInSitemap);
            foreach (Topic topic in topics)
            {
                var localizedTopics = IoC.Resolve<ITopicService>().GetAllLocalizedTopics(topic.Name);
                if (localizedTopics.Count > 0)
                {
                    //UNDONE add topic of one language only (they have the same URL now)
                    var localizedTopic = localizedTopics[0];
                    var url = SEOHelper.GetTopicUrl(localizedTopic.TopicId, localizedTopic.Title);
                    var updateFrequency = UpdateFrequency.Weekly;
                    var updateTime = localizedTopic.UpdatedOn;
                    WriteUrlLocation(url, updateFrequency, updateTime);
                }
            }
        }

        private void WriteOtherPages(string otherPages)
        {
            if (String.IsNullOrEmpty(otherPages))
                return;

            string[] pages = otherPages.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string page in pages)
            {
                string url = CommonHelper.GetStoreLocation() + page.Trim();
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = DateTime.UtcNow;
                WriteUrlLocation(url, updateFrequency, updateTime);
            }
        }

        #endregion
    }
}
