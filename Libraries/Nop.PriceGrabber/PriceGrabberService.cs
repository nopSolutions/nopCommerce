using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Utils.Html;

namespace NopSolutions.NopCommerce.PriceGrabber
{
    /// <summary>
    /// PriceGrabber service
    /// </summary>
    public static partial class PriceGrabberService
    {
        /// <summary>
        /// Generate PriceGrabber feed
        /// </summary>
        /// <param name="stream">Stream</param>
        public static void GenerateFeed(Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine("Unique Retailer SKU;Manufacturer Name;Manufacturer Part Number;Product Title;Categorization;Product URL;Image URL;Detailed Description;Selling Price;Condition;Availability");

                foreach(Product p in ProductManager.GetAllProducts(false))
                {
                    foreach (ProductVariant pv in ProductManager.GetProductVariantsByProductId(p.ProductId, false))
                    {
                        string sku = pv.ProductVariantId.ToString();
                        string manufacturerName = p.ProductManufacturers.Count > 0 ? p.ProductManufacturers[0].Manufacturer.Name : String.Empty;
                        string manufacturerPartNumber = pv.ManufacturerPartNumber;
                        string productTitle = pv.FullProductName;
                        string productUrl = SEOHelper.GetProductUrl(p);

                        string imageUrl = string.Empty;
                        var pictures = PictureManager.GetPicturesByProductId(p.ProductId, 1);
                        if (pictures.Count > 0)
                            imageUrl = PictureManager.GetPictureUrl(pictures[0], SettingManager.GetSettingValueInteger("PromotionProvider.PriceGrabber.ProductThumbnailImageSize"), true);
                        else
                            imageUrl = PictureManager.GetDefaultPictureUrl(PictureTypeEnum.Entity, SettingManager.GetSettingValueInteger("PromotionProvider.PriceGrabber.ProductThumbnailImageSize"));

                        string description = pv.Description;
                        string price = pv.Price.ToString(new CultureInfo("en-US", false).NumberFormat);
                        string availability = pv.StockQuantity > 0 ? "Yes" : "No";
                        string categorization = "no category";

                        if (String.IsNullOrEmpty(description))
                        {
                            description = p.FullDescription;
                        }
                        if (String.IsNullOrEmpty(description))
                        {
                            description = p.ShortDescription;
                        }
                        if (String.IsNullOrEmpty(description))
                        {
                            description = p.Name;
                        }

                        var productCategories = p.ProductCategories;
                        if (productCategories.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (Category cat in CategoryManager.GetBreadCrumb(productCategories[0].CategoryId))
                            {
                                sb.AppendFormat("{0}>", cat.Name);
                            }
                            sb.Length -= 1;
                            categorization = sb.ToString();
                        }

                        productTitle = RemoveSpecChars(productTitle);
                        manufacturerPartNumber = RemoveSpecChars(manufacturerPartNumber);
                        manufacturerName = RemoveSpecChars(manufacturerName);
                        description = HtmlHelper.StripTags(description);
                        description = RemoveSpecChars(description);
                        categorization = RemoveSpecChars(categorization);

                        writer.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};New;{9}", 
                            sku, 
                            manufacturerName, 
                            manufacturerPartNumber, 
                            productTitle, 
                            categorization, 
                            productUrl, 
                            imageUrl, 
                            description, 
                            price, 
                            availability);
                    }
                }
            }
        }

        private static string RemoveSpecChars(string s)
        {
            s = s.Replace(';', ',');
            s = s.Replace('\r', ' ');
            s = s.Replace('\n', ' ');
            return s;
        }
    }
}
