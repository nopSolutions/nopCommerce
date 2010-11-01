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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Froogle
{
    /// <summary>
    /// Froogle service
    /// </summary>
    public static partial class FroogleService
    {
        /// <summary>
        /// Generate froogle feed
        /// </summary>
        /// <param name="stream">Stream</param>
        public static void GenerateFeed(Stream stream)
        {
            const string googleBaseNamespace = "http://base.google.com/ns/1.0";

            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8
            };
            using (var writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("rss");
                writer.WriteAttributeString("version", "2.0");
                writer.WriteAttributeString("xmlns", "g", null, googleBaseNamespace);
                writer.WriteStartElement("channel");
                writer.WriteElementString("title", string.Format("{0} Google Base", IoCFactory.Resolve<ISettingManager>().StoreName));
                writer.WriteElementString("link", "http://base.google.com/base/");
                writer.WriteElementString("description", "Information about products");


                var products = IoCFactory.Resolve<IProductManager>().GetAllProducts(false);
                foreach (var product in products)
                {
                    var productVariants = IoCFactory.Resolve<IProductManager>().GetProductVariantsByProductId(product.ProductId, false);

                    foreach (var productVariant in productVariants)
                    {
                        writer.WriteStartElement("item");
                        writer.WriteElementString("link", SEOHelper.GetProductUrl(product));
                        writer.WriteElementString("title", productVariant.FullProductName);
                        writer.WriteStartElement("description");
                        string description = productVariant.Description;
                        if (String.IsNullOrEmpty(description))
                            description = product.FullDescription;
                        if (String.IsNullOrEmpty(description))
                            description = product.ShortDescription;
                        if (String.IsNullOrEmpty(description))
                            description = product.Name;
                        writer.WriteCData(description);
                        writer.WriteEndElement(); // description
                        writer.WriteStartElement("g", "brand", googleBaseNamespace);
                        writer.WriteFullEndElement(); // g:brand
                        writer.WriteElementString("g", "condition", googleBaseNamespace, "new");
                        writer.WriteElementString("g", "expiration_date", googleBaseNamespace, DateTime.Now.AddDays(28).ToString("yyyy-MM-dd"));
                        writer.WriteElementString("g", "id", googleBaseNamespace, productVariant.ProductVariantId.ToString());
                        string imageUrl = string.Empty;
                        var pictures = IoCFactory.Resolve<IPictureManager>().GetPicturesByProductId(product.ProductId, 1);
                        if (pictures.Count > 0)
                            imageUrl = IoCFactory.Resolve<IPictureManager>().GetPictureUrl(pictures[0], IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("PromotionProvider.Froogle.ProductThumbnailImageSize"), true);
                        writer.WriteElementString("g", "image_link", googleBaseNamespace, imageUrl);
                        decimal price = IoCFactory.Resolve<ICurrencyManager>().ConvertCurrency(productVariant.Price, IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency, FroogleService.UsedCurrency);
                        writer.WriteElementString("g", "price", googleBaseNamespace, price.ToString(new CultureInfo("en-US", false).NumberFormat));
                        
                        //uncomment and set your product_type attribute
                        //writer.WriteStartElement("g", "product_type", googleBaseNamespace);
                        //writer.WriteCData("Clothing & Accessories > Clothing Accessories > Hair Accessories > Hair Pins & Clips");
                        //writer.WriteFullEndElement(); // g:brand


                        //if (productVariant.Weight != decimal.Zero)
                        //{
                        //    writer.WriteElementString("g", "weight", googleBaseNamespace, string.Format(CultureInfo.InvariantCulture, "{0} {1}", productVariant.Weight.ToString(new CultureInfo("en-US", false).NumberFormat), IoCFactory.Resolve<IMeasureManager>().BaseWeightIn.SystemKeyword));
                        //}
                        writer.WriteEndElement(); // item
                    }
                }

                writer.WriteEndElement(); // channel
                writer.WriteEndElement(); // rss
                writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Gets or sets the currency that is used to generate the feed
        /// </summary>
        public static Currency UsedCurrency
        {
            get
            {
                int currencyId = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("PromotionProvider.Froogle.Currency");
                var currency = IoCFactory.Resolve<ICurrencyManager>().GetCurrencyById(currencyId);
                if (currency == null || !currency.Published)
                    currency = IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency;
                return currency;
            }
            set
            {
                int id = 0;
                if (value != null)
                    id = value.CurrencyId;
                IoCFactory.Resolve<ISettingManager>().SetParam("PromotionProvider.Froogle.Currency", id.ToString());
            }
        }
    }
}
