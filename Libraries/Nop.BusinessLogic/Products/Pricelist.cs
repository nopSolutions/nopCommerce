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
using System.Collections.Specialized;
using System.IO;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
	/// <summary>
	/// This object represents the properties and methods of a Pricelist.
	/// </summary>
    public partial class Pricelist : BaseEntity
    {
        #region Utilities

        private string CreatePricelistContents()
        {
            string strContents = string.Empty;

            var productVariants = new List<ProductVariant>();
            bool blnOverrideAdjustment = this.OverrideIndivAdjustment;

            switch (this.ExportMode)
            {
                case PriceListExportModeEnum.All:
                    {
                        blnOverrideAdjustment = true;
                        int totalRecords = 0;
                        productVariants = IoC.Resolve<IProductService>().GetAllProductVariants(0, 0, string.Empty, int.MaxValue, 0, out totalRecords);
                    }
                    break;
                case PriceListExportModeEnum.AssignedProducts:
                    {
                        productVariants = IoC.Resolve<IProductService>().GetProductVariantsByPricelistId(this.PricelistId);
                    }
                    break;
                default:
                    break;
            }

            // create new file
            // write header, if provided
            if (!String.IsNullOrEmpty(this.Header))
            {
                strContents += this.Header;
                if (!this.Header.EndsWith("\n"))
                    strContents += "\n";
            }

            // write body
            foreach (ProductVariant productVariant in productVariants)
            {
                // calculate price adjustments
                decimal newPrice = decimal.Zero;

                // if export mode is all, then calculate price
                if (blnOverrideAdjustment)
                {
                    newPrice = GetAdjustedPrice(productVariant.Price, this.PriceAdjustmentType, PriceAdjustment);
                }
                else
                {
                    ProductVariantPricelist productVariantPricelist = IoC.Resolve<IProductService>().GetProductVariantPricelist(productVariant.ProductVariantId, this.PricelistId);
                    if (productVariantPricelist != null)
                    {
                        newPrice = GetAdjustedPrice(productVariant.Price, productVariantPricelist.PriceAdjustmentType, productVariantPricelist.PriceAdjustment);
                    }
                }
                strContents += replaceMessageTemplateTokens(productVariant, this.Body,
                    this.FormatLocalization, new System.Collections.Specialized.NameValueCollection(), AffiliateId, newPrice);
                if (!this.Body.EndsWith("\n"))
                    strContents += "\n";
            }

            // write footer, if provided
            if (!String.IsNullOrEmpty(this.Footer))
            {
                strContents += this.Header;
                if (!this.Footer.EndsWith("\n"))
                    strContents += "\n";
            }

            return strContents;
        }

        private bool CheckCache(int cacheTime, string cachePath)
        {
            // search for youngest file
            string[] files = System.IO.Directory.GetFiles(cachePath, this.ShortName + "*.txt");

            DateTime youngestFileDate = new DateTime(2000, 01, 01);

            foreach (string file in files)
            {
                if (new System.IO.FileInfo(file).CreationTime > youngestFileDate)
                {
                    youngestFileDate = new System.IO.FileInfo(file).CreationTime;
                }
            }

            if (DateTime.Now.AddMinutes(-CacheTime) > youngestFileDate)
                return false;
            else
                return true;
        }

        private string RetrieveFromCache(string cachePath)
        {
            // search for youngest file
            string[] files = System.IO.Directory.GetFiles(cachePath, this.ShortName + "*.txt");

            string youngestFileName = "";
            DateTime youngestFileDate = new DateTime(2000, 01, 01);

            foreach (string file in files)
            {
                if (new System.IO.FileInfo(file).CreationTime > youngestFileDate)
                {
                    youngestFileName = file;
                    youngestFileDate = new System.IO.FileInfo(file).CreationTime;
                }
            }

            using (FileStream fs = new FileStream(youngestFileName, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                return sr.ReadToEnd();
            }
        }

        private void SaveToCache(string cachePath, string contents)
        {
            string saveFilePath = string.Format("{0}{1}_{2:yyyyMMdd_HHmmss}.txt", cachePath, this.ShortName, DateTime.Now);

            if (File.Exists(saveFilePath))
            {
                using (FileStream fs = new FileStream(saveFilePath, FileMode.CreateNew, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                    sw.Write(contents);
            }
        }

        private string GetProductUrlWithPricelistProvider(int productId, int affiliateId)
        {
            string url = SEOHelper.GetProductUrl(productId);
            if (AffiliateId != 0)
                url = CommonHelper.ModifyQueryString(url, "AffiliateID=" + AffiliateId.ToString(), null);
            return url;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="productVariant">Product variant instance</param>
        /// <param name="template">Template</param>
        /// <param name="localFormat">Localization Provider Short name (en-US, de-DE, etc.)</param>
        /// <param name="additionalKeys">Additional keys</param>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <param name="price">Price</param>
        /// <returns>New template</returns>
        protected string replaceMessageTemplateTokens(ProductVariant productVariant, 
            string template, string localFormat, NameValueCollection additionalKeys,
            int affiliateId, decimal price)
        {
            NameValueCollection tokens = new NameValueCollection();

            IFormatProvider locProvider = new System.Globalization.CultureInfo(localFormat);

            string strHelper = template;

            while (strHelper.Contains("%"))
            {
                strHelper = strHelper.Substring(strHelper.IndexOf("%") + 1);
                string strToken = strHelper.Substring(0, strHelper.IndexOf("%"));
                string strFormat = "";
                strHelper = strHelper.Substring(strHelper.IndexOf("%") + 1);

                if (strToken.Contains(":"))
                {
                    strFormat = strToken.Substring(strToken.IndexOf(":"));
                    strToken = strToken.Substring(0, strToken.IndexOf(":"));
                }

                if (tokens.Get(strToken + strFormat) == null)
                {
                    switch (strToken.ToLower())
                    {
                        case "store.name":
                            {
                                tokens.Add(strToken + strFormat, String.Format(locProvider, "{0" + strFormat + "}", IoC.Resolve<ISettingManager>().StoreName));
                            }
                            break;
                        case "product.pictureurl":
                            {
                                var pictures = productVariant.Product.ProductPictures;
                                if (pictures.Count > 0)
                                {
                                    tokens.Add(strToken + strFormat, String.Format(locProvider, "{0" + strFormat + "}", IoC.Resolve<IPictureService>().GetPictureUrl(pictures[0].PictureId)));
                                }
                                else
                                {
                                    tokens.Add(strToken + strFormat, String.Format(locProvider, "{0" + strFormat + "}", string.Empty));
                                }
                            }
                            break;
                        case "pv.producturl":
                            {
                                tokens.Add(strToken + strFormat, String.Format(locProvider, "{0" + strFormat + "}", GetProductUrlWithPricelistProvider(productVariant.ProductId, AffiliateId)));
                            }
                            break;
                        case "pv.price":
                            {
                            tokens.Add(strToken + strFormat, String.Format(locProvider, "{0" + strFormat + "}", price));
                            }
                            break;
                        case "pv.name":
                            {
                                tokens.Add(strToken + strFormat, String.Format(locProvider, "{0" + strFormat + "}", productVariant.FullProductName));
                            }
                            break;
                        case "pv.description":
                            {
                                tokens.Add(strToken + strFormat, String.Format(locProvider, "{0" + strFormat + "}", productVariant.Description));
                            }
                            break;
                        case "product.description":
                            {
                                tokens.Add(strToken + strFormat, String.Format(locProvider, "{0" + strFormat + "}", productVariant.Product.FullDescription));
                            }
                            break;
                        case "product.shortdescription":
                            {
                                tokens.Add(strToken + strFormat, String.Format(locProvider, "{0" + strFormat + "}", productVariant.Product.ShortDescription));
                            }
                            break;
                        case "pv.partnumber":
                            {
                                tokens.Add(strToken + strFormat, String.Format(locProvider, "{0" + strFormat + "}", productVariant.ManufacturerPartNumber));
                            }
                            break;
                        case "product.manufacturer":
                            {
                                string mans = string.Empty;
                                var productManufacturers = productVariant.Product.ProductManufacturers;
                                foreach (ProductManufacturer pm in productManufacturers)
                                {
                                    mans += ", " + pm.Manufacturer.Name;
                                }
                                if (mans.Length != 0)
                                    mans = mans.Substring(2);

                                if (productManufacturers.Count > 0)
                                    tokens.Add(strToken + strFormat, String.Format(locProvider, "{0" + strFormat + "}", mans));
                            }
                            break;
                        case "product.category":
                            {
                                string cats = string.Empty;
                                var productCategories = productVariant.Product.ProductCategories;
                                foreach (ProductCategory pc in productCategories)
                                {
                                    cats += ", " + pc.Category.Name;
                                }
                                if (cats.Length != 0)
                                    cats = cats.Substring(2);

                                if (productCategories.Count > 0)
                                    tokens.Add(strToken + strFormat, String.Format(locProvider, "{0" + strFormat + "}", cats));
                            }
                            break;
                        case "product.shippingcosts":
                            {
                            }
                            break;
                        default:
                            {
                            tokens.Add(strToken + strFormat, strToken + strFormat);
                            }
                            break;
                    }
                }

            }

            foreach (string token in tokens.Keys)
                template = template.Replace(string.Format(@"%{0}%", token), tokens[token]);

            foreach (string token in additionalKeys.Keys)
                template = template.Replace(string.Format(@"%{0}%", token), additionalKeys[token]);

            return template;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates price list
        /// </summary>
        /// <param name="cachePath">Cache path where cached price list can be stored</param>
        /// <returns>Created price list</returns>
        public string CreatePricelist(string cachePath)
        {
            string strContents = string.Empty;

            if (CheckCache(this.CacheTime, cachePath))
            {
                return RetrieveFromCache(cachePath);
            }
            else
            {
                strContents = CreatePricelistContents();
                SaveToCache(cachePath, strContents);
                return strContents;
            }
        }

        /// <summary>
        /// Gets list of allowed tokens
        /// </summary>
        /// <returns>List of allowed tokens</returns>
        public static string[] GetListOfAllowedTokens()
        {
            List<string> allowedTokens = new List<string>();
            allowedTokens.Add("%store.name%");
            allowedTokens.Add("%product.pictureurl%");
            allowedTokens.Add("%pv.producturl%");
            allowedTokens.Add("%pv.price%");
            allowedTokens.Add("%pv.name%");
            allowedTokens.Add("%pv.description%");
            allowedTokens.Add("%product.description%");
            allowedTokens.Add("%product.shortdescription%");
            allowedTokens.Add("%pv.partnumber%");
            allowedTokens.Add("%product.manufacturer%");
            allowedTokens.Add("%product.category%");
            allowedTokens.Add("%product.shippingcosts%");
            return allowedTokens.ToArray();
        }

        /// <summary>
        /// Calculate a price adjustment according to adjustment type
        /// </summary>
        /// <param name="price">Price to adjust</param>
        /// <param name="priceAdjustmentType">The type of price adjustment calculation (e.g. absolute adjustment)</param>
        /// <param name="priceAdjustment">The value for price adjustment calculation</param>
        /// <returns>Adjusted price</returns>
        public decimal GetAdjustedPrice(decimal price, 
            PriceAdjustmentTypeEnum priceAdjustmentType, decimal priceAdjustment)
        {
            decimal result = price;

            switch (PriceAdjustmentType)
            {
                case PriceAdjustmentTypeEnum.AbsoluteAdjustment:
                    {
                        result -= PriceAdjustment;
                    }
                    break;
                case PriceAdjustmentTypeEnum.AbsolutePrice:
                    {
                    }
                    break;
                case PriceAdjustmentTypeEnum.RelativeAdjustment:
                    {
                        result = result * ((100 - PriceAdjustment) / 100);
                    }
                    break;
                default:
                    break;
            }

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Pricelist identifier
        /// </summary>
        public int PricelistId { get; set; }

        /// <summary>
        /// Gets or sets the Mode of list creation (Export all, assigned only, assigned only with special price)
        /// </summary>
        public int ExportModeId { get; set; }

        /// <summary>
        /// Gets or sets the CSV or XML
        /// </summary>
        public int ExportTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Affiliate connected to this pricelist (optional), links will be created with AffiliateId
        /// </summary>
        public int AffiliateId { get; set; }

        /// <summary>
        /// Gets or sets the Displayedname
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the shortname to identify the pricelist
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier to get pricelist "anonymous"
        /// </summary>
        public string PricelistGuid { get; set; }

        /// <summary>
        /// Gets or sets the how long will the pricelist be in cached before new creation
        /// </summary>
        public int CacheTime { get; set; }

        /// <summary>
        /// Gets or sets the what localization will be used (numeric formats, etc.) en-US, de-DE etc.
        /// </summary>
        public string FormatLocalization { get; set; }

        /// <summary>
        /// Gets or sets the Displayed description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Admin can put some notes here, not displayed in public
        /// </summary>
        public string AdminNotes { get; set; }

        /// <summary>
        /// Gets or sets the Headerline of the exported file (plain text)
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the template for an exportet productvariant, uses delimiters and replacement strings
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the Footer line of the exportet file (plain text)
        /// </summary>
        public string Footer { get; set; }

        /// <summary>
        /// Gets or sets the type of price adjustment (if used) (relative or absolute)
        /// </summary>
        public int PriceAdjustmentTypeId { get; set; }

        /// <summary>
        /// Gets or sets the price will be adjusted by this amount (in accordance with PriceAdjustmentType)
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the use individual adjustment, if available, or override
        /// </summary>
        public bool OverrideIndivAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the when was this record originally created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the last time this recordset was updated
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the export mode
        /// </summary>
        public PriceListExportModeEnum ExportMode
        {
            get
            {
                return (PriceListExportModeEnum)this.ExportModeId;
            }
            set
            {
                this.ExportModeId = (int)value;
            }
        }

        /// <summary>
        /// Gets the log type
        /// </summary>
        public PriceListExportTypeEnum ExportType
        {
            get
            {
                return (PriceListExportTypeEnum)this.ExportTypeId;
            }
            set
            {
                this.ExportTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets the price adjustment type
        /// </summary>
        public PriceAdjustmentTypeEnum PriceAdjustmentType
        {
            get
            {
                return (PriceAdjustmentTypeEnum)this.PriceAdjustmentTypeId;
            }
            set
            {
                this.PriceAdjustmentTypeId = (int)value;
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the product variant pricelists
        /// </summary>
        public virtual List<ProductVariantPricelist> NpProductVariantPricelists { get; set; }

        #endregion
        
    }
}

