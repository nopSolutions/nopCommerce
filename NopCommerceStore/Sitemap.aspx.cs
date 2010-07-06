using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Content.Topics;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Audit;

namespace NopSolutions.NopCommerce.Web
{
    public partial class Sitemap : BaseNopPage
    {
        protected override void OnPreRender(EventArgs e)
        {
            BindData();
            base.OnPreRender(e);
        }

        protected void BindData()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                if (SettingManager.GetSettingValueBoolean("Sitemap.IncludeCategories", true))
                {
                    sb.Append("<li>");
                    sb.AppendFormat("<span>{0}</span>", GetLocaleResourceString("Sitemap.Categories"));
                    sb.Append("<ul>");
                    WriteCategories(sb, CategoryManager.GetAllCategories(0));
                    sb.Append("</ul>");
                    sb.Append("</li>");
                }
                if (SettingManager.GetSettingValueBoolean("Sitemap.IncludeManufacturers", true))
                {
                    sb.Append("<li>");
                    sb.AppendFormat("<span>{0}</span>", GetLocaleResourceString("Sitemap.Manufacturers"));
                    sb.Append("<ul>");
                    WriteManufacturers(sb, ManufacturerManager.GetAllManufacturers());
                    sb.Append("</ul>");
                    sb.Append("</li>");
                }
                if (SettingManager.GetSettingValueBoolean("Sitemap.IncludeProducts", true))
                {
                    sb.Append("<li>");
                    sb.AppendFormat("<span>{0}</span>", GetLocaleResourceString("Sitemap.Products"));
                    sb.Append("<ul>");
                    WriteProducts(sb, ProductManager.GetAllProducts());
                    sb.Append("</ul>");
                    sb.Append("</li>");
                }
                if (SettingManager.GetSettingValueBoolean("Sitemap.IncludeTopics", true))
                {
                    sb.Append("<li>");
                    sb.AppendFormat("<span>{0}</span>", GetLocaleResourceString("Sitemap.Topics"));
                    sb.Append("<ul>");
                    WriteTopics(sb, TopicManager.GetAllTopics());
                    sb.Append("</ul>");
                    sb.Append("</li>");
                }
                string otherPages = SettingManager.GetSettingValue("Sitemap.OtherPages");
                if (!String.IsNullOrEmpty(otherPages))
                {
                    sb.Append("<li>");
                    sb.AppendFormat("<span>{0}</span>", GetLocaleResourceString("Sitemap.OtherPages"));
                    sb.Append("<ul>");
                    WriteOtherPages(sb, otherPages.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    sb.Append("</ul>");
                    sb.Append("</li>");
                }

                lSitemapContent.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                lSitemapContent.Text = ex.Message;
                LogManager.InsertLog(LogTypeEnum.CommonError, ex.Message, ex);
            }
        }

        private void WriteCategories(StringBuilder sb, List<Category> categoryCollection)
        {
            foreach(Category category in categoryCollection)
            {
                sb.Append("<li>");
                sb.AppendFormat("<a href=\"{0}\">{1}</a>", SEOHelper.GetCategoryUrl(category), Server.HtmlEncode(category.LocalizedName));
                var childCategoryCollection = CategoryManager.GetAllCategories(category.CategoryId);
                if(childCategoryCollection.Count > 0)
                {
                    sb.Append("<ul>");
                    WriteCategories(sb, childCategoryCollection);
                    sb.Append("</ul>");
                }
                sb.Append("</li>");
            }
        }

        private void WriteManufacturers(StringBuilder sb, List<Manufacturer> manufacturerCollection)
        {
            foreach(Manufacturer manufacturer in manufacturerCollection)
            {
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", SEOHelper.GetManufacturerUrl(manufacturer), Server.HtmlEncode(manufacturer.LocalizedName));
            }
        }

        private void WriteProducts(StringBuilder sb, List<Product> productCollection)
        {
            foreach(Product product in productCollection)
            {
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", SEOHelper.GetProductUrl(product), Server.HtmlEncode(product.LocalizedName));
            }
        }

        private void WriteTopics(StringBuilder sb, List<Topic> topicCollection)
        {
            foreach(Topic topic in topicCollection)
            {
                LocalizedTopic localizedTopic = TopicManager.GetLocalizedTopic(topic.TopicId, NopContext.Current.WorkingLanguage.LanguageId);
                if(localizedTopic != null)
                {
                    sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", SEOHelper.GetTopicUrl(localizedTopic.TopicId, localizedTopic.Title), (String.IsNullOrEmpty(localizedTopic.Title) ? localizedTopic.TopicId.ToString() : Server.HtmlEncode(localizedTopic.Title)));
                }
            }
        }

        private void WriteOtherPages(StringBuilder sb, string[] pages)
        {
            foreach(string page in pages)
            {
                sb.AppendFormat("<li><a href=\"{0}{1}\">{2}</a></li>", CommonHelper.GetStoreLocation(), page.Trim(), Server.HtmlEncode(page));
            }
        }
    }
}
