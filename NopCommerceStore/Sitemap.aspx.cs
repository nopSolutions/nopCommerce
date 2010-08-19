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
            string title = GetLocaleResourceString("PageTitle.Sitemap");
            SEOHelper.RenderTitle(this, title, true);

            BindData();
            base.OnPreRender(e);
        }

        protected void BindData()
        {
            //topics
            var sitemapTopics = new List<SitemapTopic>();
            var topics = TopicManager.GetAllTopics();
            topics = topics.FindAll(t => t.IncludeInSitemap);
            foreach (var topic in topics)
            {
                LocalizedTopic localizedTopic = TopicManager.GetLocalizedTopic(topic.TopicId, NopContext.Current.WorkingLanguage.LanguageId);
                if (localizedTopic != null && !String.IsNullOrEmpty(localizedTopic.Title))
                {
                    string topicURL = SEOHelper.GetTopicUrl(localizedTopic.TopicId, localizedTopic.Title);
                    string topicName = localizedTopic.Title;

                    sitemapTopics.Add(new SitemapTopic() { Name = topicName, Url = topicURL });
                }
            }

            //other pages
            string otherPages = SettingManager.GetSettingValue("Sitemap.OtherPages");
            if (!String.IsNullOrEmpty(otherPages))
            {
                string[] pages = otherPages.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string page in pages)
                {
                    sitemapTopics.Add(new SitemapTopic() { Name = page, Url = string.Format("{0}{1}", CommonHelper.GetStoreLocation(), page.Trim()) });
                }
            }
            if (sitemapTopics.Count > 0)
            {
                dlTopics.DataSource = sitemapTopics;
                dlTopics.DataBind();
            }
            else
            {
                dlTopics.Visible = false;
            }

            //categories
            if (SettingManager.GetSettingValueBoolean("Sitemap.IncludeCategories", true))
            {
                //root categories only here
                var categories = CategoryManager.GetAllCategoriesByParentCategoryId(0);
                if (categories.Count > 0)
                {
                    dlCategories.DataSource = categories;
                    dlCategories.DataBind();
                }
                else
                {
                    dlCategories.Visible = false;
                }
            }
            else
            {
                dlCategories.Visible = false;
            }

            //manufacturers
            if (SettingManager.GetSettingValueBoolean("Sitemap.IncludeManufacturers", true))
            {
                var manufacturers = ManufacturerManager.GetAllManufacturers();
                if (manufacturers.Count > 0)
                {
                    dlManufacturers.DataSource = ManufacturerManager.GetAllManufacturers();
                    dlManufacturers.DataBind();
                }
                else
                {
                    dlManufacturers.Visible = false;
                }
            }
            else
            {
                dlManufacturers.Visible = false;
            }

            //products
            if (SettingManager.GetSettingValueBoolean("Sitemap.IncludeProducts", true))
            {
                var products = ProductManager.GetAllProducts();
                if (products.Count > 0)
                {
                    dlProducts.DataSource = products;
                    dlProducts.DataBind();
                }
                else
                {
                    dlProducts.Visible = false;
                }
            }
            else
            {
                dlProducts.Visible = false;
            }
        }

        private void WriteProducts(StringBuilder sb, List<Product> productCollection)
        {
            foreach (Product product in productCollection)
            {
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", SEOHelper.GetProductUrl(product), Server.HtmlEncode(product.LocalizedName));
            }
        }

        protected void dlCategories_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var category = e.Item.DataItem as Category;

                var hlLink = e.Item.FindControl("hlLink") as HyperLink;
                if (hlLink != null)
                {
                    hlLink.NavigateUrl = SEOHelper.GetCategoryUrl(category);
                    hlLink.Text = Server.HtmlEncode(category.LocalizedName);
                }
            }
        }

        protected void dlManufacturers_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var manufacturer = e.Item.DataItem as Manufacturer;

                var hlLink = e.Item.FindControl("hlLink") as HyperLink;
                if (hlLink != null)
                {
                    hlLink.NavigateUrl = SEOHelper.GetManufacturerUrl(manufacturer);
                    hlLink.Text = Server.HtmlEncode(manufacturer.LocalizedName);
                }
            }
        }

        protected void dlProducts_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var product = e.Item.DataItem as Product;

                var hlLink = e.Item.FindControl("hlLink") as HyperLink;
                if (hlLink != null)
                {
                    hlLink.NavigateUrl = SEOHelper.GetProductUrl(product);
                    hlLink.Text = Server.HtmlEncode(product.LocalizedName);
                }
            }
        }

        protected void dlTopics_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var sitemapTopic = e.Item.DataItem as SitemapTopic;

                var hlLink = e.Item.FindControl("hlLink") as HyperLink;
                if (hlLink != null)
                {
                    hlLink.NavigateUrl = sitemapTopic.Url;
                    hlLink.Text = Server.HtmlEncode(sitemapTopic.Name);
                }
            }
        }

        private class SitemapTopic
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}
