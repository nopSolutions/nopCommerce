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
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using System.Globalization;
using NopSolutions.NopCommerce.BusinessLogic.Localization;

namespace NopSolutions.NopCommerce.BusinessLogic.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager
    {
        #region Utilities

        private static void WriteCategories(XmlWriter xmlWriter, int parentCategoryId)
        {
            var categories = CategoryManager.GetAllCategories(parentCategoryId);
            if (categories.Count > 0)
            {
                foreach (var category in categories)
                {
                    xmlWriter.WriteStartElement("Category");
                    xmlWriter.WriteElementString("CategoryId", null, category.CategoryId.ToString());
                    xmlWriter.WriteElementString("Name", null, category.Name);
                    xmlWriter.WriteElementString("Description", null, category.Description);
                    xmlWriter.WriteElementString("TemplateId", null, category.TemplateId.ToString());
                    xmlWriter.WriteElementString("MetaKeywords", null, category.MetaKeywords);
                    xmlWriter.WriteElementString("MetaDescription", null, category.MetaDescription);
                    xmlWriter.WriteElementString("MetaTitle", null, category.MetaTitle);
                    xmlWriter.WriteElementString("SEName", null, category.SEName);
                    xmlWriter.WriteElementString("ParentCategoryId", null, category.ParentCategoryId.ToString());
                    xmlWriter.WriteElementString("PictureId", null, category.PictureId.ToString());
                    xmlWriter.WriteElementString("PageSize", null, category.PageSize.ToString());
                    xmlWriter.WriteElementString("PriceRanges", null, category.PriceRanges);
                    xmlWriter.WriteElementString("ShowOnHomePage", null, category.ShowOnHomePage.ToString());
                    xmlWriter.WriteElementString("Published", null, category.Published.ToString());
                    xmlWriter.WriteElementString("Deleted", null, category.Deleted.ToString());
                    xmlWriter.WriteElementString("DisplayOrder", null, category.DisplayOrder.ToString());
                    xmlWriter.WriteElementString("CreatedOn", null, category.CreatedOn.ToString());
                    xmlWriter.WriteElementString("UpdatedOn", null, category.UpdatedOn.ToString());

                    xmlWriter.WriteStartElement("Products");
                    var productCategories = category.ProductCategories;
                    foreach (var productCategory in productCategories)
                    {
                        var product = productCategory.Product;
                        if (product != null && !product.Deleted)
                        {
                            xmlWriter.WriteStartElement("ProductCategory");
                            xmlWriter.WriteElementString("ProductCategoryId", null, productCategory.ProductCategoryId.ToString());
                            xmlWriter.WriteElementString("ProductId", null, productCategory.ProductId.ToString());
                            xmlWriter.WriteElementString("IsFeaturedProduct", null, productCategory.IsFeaturedProduct.ToString());
                            xmlWriter.WriteElementString("DisplayOrder", null, productCategory.DisplayOrder.ToString());
                            xmlWriter.WriteEndElement();
                        }
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("SubCategories");
                    WriteCategories(xmlWriter, category.CategoryId);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Export all string resources and message templates as XML
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>XML content</returns>
        public static string ExportResources(int languageId)
        {
            return LocalizationManager.LanguagePackExport(languageId);

        }

        /// <summary>
        /// Export customer list to xml
        /// </summary>
        /// <param name="customers">Customers</param>
        /// <returns>Result in XML format</returns>
        public static string ExportCustomersToXml(List<Customer> customers)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter stringWriter = new StringWriter(sb);
            XmlWriter xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Customers");
            xmlWriter.WriteAttributeString("Version", SiteHelper.GetCurrentVersion());

            foreach (var customer in customers)
            {
                xmlWriter.WriteStartElement("Customer");
                xmlWriter.WriteElementString("CustomerId", null, customer.CustomerId.ToString());
                xmlWriter.WriteElementString("CustomerGuid", null, customer.CustomerGuid.ToString());
                xmlWriter.WriteElementString("Email", null, customer.Email);
                xmlWriter.WriteElementString("Username", null, customer.Username);
                xmlWriter.WriteElementString("PasswordHash", null, customer.PasswordHash);
                xmlWriter.WriteElementString("SaltKey", null, customer.SaltKey);
                xmlWriter.WriteElementString("AffiliateId", null, customer.AffiliateId.ToString());
                xmlWriter.WriteElementString("LanguageId", null, customer.LanguageId.ToString());
                xmlWriter.WriteElementString("CurrencyId", null, customer.CurrencyId.ToString());
                xmlWriter.WriteElementString("TaxDisplayTypeId", null, customer.TaxDisplayTypeId.ToString());
                xmlWriter.WriteElementString("IsTaxExempt", null, customer.IsTaxExempt.ToString());
                xmlWriter.WriteElementString("IsAdmin", null, customer.IsAdmin.ToString());
                xmlWriter.WriteElementString("IsGuest", null, customer.IsGuest.ToString());
                xmlWriter.WriteElementString("IsForumModerator", null, customer.IsForumModerator.ToString());
                xmlWriter.WriteElementString("TotalForumPosts", null, customer.TotalForumPosts.ToString());
                xmlWriter.WriteElementString("Active", null, customer.Active.ToString());
                xmlWriter.WriteElementString("Deleted", null, customer.Deleted.ToString());
                xmlWriter.WriteElementString("RegistrationDate", null, customer.RegistrationDate.ToString());
                xmlWriter.WriteElementString("TimeZoneId", null, customer.TimeZoneId);
                xmlWriter.WriteElementString("AvatarId", null, customer.AvatarId.ToString());


                xmlWriter.WriteElementString("Gender", null, customer.Gender);
                xmlWriter.WriteElementString("FirstName", null, customer.FirstName);
                xmlWriter.WriteElementString("LastName", null, customer.LastName);
                if (customer.DateOfBirth.HasValue)
                    xmlWriter.WriteElementString("DateOfBirth", null, customer.DateOfBirth.Value.ToBinary().ToString());
                xmlWriter.WriteElementString("Company", null, customer.Company);
                xmlWriter.WriteElementString("VatNumber", null, customer.VatNumber);
                xmlWriter.WriteElementString("VatNumberStatus", null, ((int)customer.VatNumberStatus).ToString());
                xmlWriter.WriteElementString("StreetAddress", null, customer.StreetAddress);
                xmlWriter.WriteElementString("StreetAddress2", null, customer.StreetAddress2);
                xmlWriter.WriteElementString("ZipPostalCode", null, customer.ZipPostalCode);
                xmlWriter.WriteElementString("City", null, customer.City);
                xmlWriter.WriteElementString("PhoneNumber", null, customer.PhoneNumber);
                xmlWriter.WriteElementString("FaxNumber", null, customer.FaxNumber);

                xmlWriter.WriteElementString("CountryId", null, customer.CountryId.ToString());
                var country = CountryManager.GetCountryById(customer.CountryId);
                xmlWriter.WriteElementString("Country", null, (country == null) ? string.Empty : country.Name);

                xmlWriter.WriteElementString("StateProvinceId", null, customer.StateProvinceId.ToString());
                var stateProvince = StateProvinceManager.GetStateProvinceById(customer.StateProvinceId);
                xmlWriter.WriteElementString("StateProvince", null, (stateProvince == null) ? string.Empty : stateProvince.Name);
                xmlWriter.WriteElementString("ReceiveNewsletter", null, customer.ReceiveNewsletter.ToString());

                var billingAddresses = customer.BillingAddresses;
                if (billingAddresses.Count > 0)
                {
                    xmlWriter.WriteStartElement("BillingAddresses");
                    foreach (var address in billingAddresses)
                    {
                        xmlWriter.WriteStartElement("Address");
                        xmlWriter.WriteElementString("AddressId", null, address.AddressId.ToString());
                        xmlWriter.WriteElementString("FirstName", null, address.FirstName);
                        xmlWriter.WriteElementString("LastName", null, address.LastName);
                        xmlWriter.WriteElementString("PhoneNumber", null, address.PhoneNumber);
                        xmlWriter.WriteElementString("Email", null, address.Email);
                        xmlWriter.WriteElementString("FaxNumber", null, address.FaxNumber);
                        xmlWriter.WriteElementString("Company", null, address.Company);
                        xmlWriter.WriteElementString("Address1", null, address.Address1);
                        xmlWriter.WriteElementString("Address2", null, address.Address2);
                        xmlWriter.WriteElementString("City", null, address.City);
                        xmlWriter.WriteElementString("StateProvinceId", null, address.StateProvinceId.ToString());
                        xmlWriter.WriteElementString("StateProvince", null, (address.StateProvince == null) ? string.Empty : address.StateProvince.Name);
                        xmlWriter.WriteElementString("ZipPostalCode", null, address.ZipPostalCode);
                        xmlWriter.WriteElementString("CountryId", null, address.CountryId.ToString());
                        xmlWriter.WriteElementString("Country", null, (address.Country == null) ? string.Empty : address.Country.Name);
                        xmlWriter.WriteElementString("CreatedOn", null, address.CreatedOn.ToString());
                        xmlWriter.WriteElementString("UpdatedOn", null, address.UpdatedOn.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }

                var shippingAddresses = customer.ShippingAddresses;
                if (shippingAddresses.Count > 0)
                {
                    xmlWriter.WriteStartElement("ShippingAddresses");
                    foreach (var address in shippingAddresses)
                    {
                        xmlWriter.WriteStartElement("Address");
                        xmlWriter.WriteElementString("AddressId", null, address.AddressId.ToString());
                        xmlWriter.WriteElementString("FirstName", null, address.FirstName);
                        xmlWriter.WriteElementString("LastName", null, address.LastName);
                        xmlWriter.WriteElementString("PhoneNumber", null, address.PhoneNumber);
                        xmlWriter.WriteElementString("Email", null, address.Email);
                        xmlWriter.WriteElementString("FaxNumber", null, address.FaxNumber);
                        xmlWriter.WriteElementString("Company", null, address.Company);
                        xmlWriter.WriteElementString("Address1", null, address.Address1);
                        xmlWriter.WriteElementString("Address2", null, address.Address2);
                        xmlWriter.WriteElementString("City", null, address.City);
                        xmlWriter.WriteElementString("StateProvinceId", null, address.StateProvinceId.ToString());
                        xmlWriter.WriteElementString("StateProvince", null, (address.StateProvince == null) ? string.Empty : address.StateProvince.Name);
                        xmlWriter.WriteElementString("ZipPostalCode", null, address.ZipPostalCode);
                        xmlWriter.WriteElementString("CountryId", null, address.CountryId.ToString());
                        xmlWriter.WriteElementString("Country", null, (address.Country == null) ? string.Empty : address.Country.Name);
                        xmlWriter.WriteElementString("CreatedOn", null, address.CreatedOn.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export customer list to XLS
        /// </summary>
        /// <param name="filePath">File path to use</param>
        /// <param name="customers">Customers</param>
        public static void ExportCustomersToXls(string filePath, List<Customer> customers)
        {
            using (ExcelHelper excelHelper = new ExcelHelper(filePath))
            {
                excelHelper.Hdr = "YES";
                excelHelper.Imex = "0";
                Dictionary<string, string> tableDefinition = new Dictionary<string,string>();
                tableDefinition.Add("CustomerId", "int");
                tableDefinition.Add("CustomerGuid", "uniqueidentifier");
                tableDefinition.Add("Email", "nvarchar(255)");
                tableDefinition.Add("Username", "nvarchar(255)");
                tableDefinition.Add("PasswordHash", "nvarchar(255)");
                tableDefinition.Add("SaltKey", "nvarchar(255)");
                tableDefinition.Add("AffiliateId", "int");
                tableDefinition.Add("LanguageId", "int");
                tableDefinition.Add("CurrencyId", "int");
                tableDefinition.Add("TaxDisplayTypeId", "int");
                tableDefinition.Add("IsTaxExempt", "nvarchar(5)");
                tableDefinition.Add("IsAdmin", "nvarchar(5)");
                tableDefinition.Add("IsGuest", "nvarchar(5)");
                tableDefinition.Add("IsForumModerator", "nvarchar(5)");
                tableDefinition.Add("TotalForumPosts", "int");
                tableDefinition.Add("Signature", "nvarchar(255)");
                tableDefinition.Add("AdminComment", "nvarchar(255)");
                tableDefinition.Add("Active", "nvarchar(5)");
                tableDefinition.Add("Deleted", "nvarchar(5)");
                tableDefinition.Add("RegistrationDate", "decimal");
                tableDefinition.Add("TimeZoneId", "nvarchar(200)");
                tableDefinition.Add("AvatarId", "int");
                tableDefinition.Add("Gender", "nvarchar(100)");
                tableDefinition.Add("FirstName", "nvarchar(100)");
                tableDefinition.Add("LastName", "nvarchar(100)");
                tableDefinition.Add("Company", "nvarchar(100)");
                tableDefinition.Add("VatNumber", "nvarchar(100)");
                tableDefinition.Add("VatNumberStatus", "int");
                tableDefinition.Add("StreetAddress", "nvarchar(100)");
                tableDefinition.Add("StreetAddress2", "nvarchar(100)");
                tableDefinition.Add("ZipPostalCode", "nvarchar(100)");
                tableDefinition.Add("City", "nvarchar(100)");
                tableDefinition.Add("PhoneNumber", "nvarchar(100)");
                tableDefinition.Add("FaxNumber", "nvarchar(100)");
                tableDefinition.Add("CountryId", "int");
                tableDefinition.Add("StateProvinceId", "int");
                tableDefinition.Add("ReceiveNewsletter", "nvarchar(5)");
                excelHelper.WriteTable("Customers", tableDefinition);

                string decimalQuoter = (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(",") ? "\"" : String.Empty);

                foreach (var customer in customers)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO [Customers] (CustomerId, CustomerGuid, Email, Username, PasswordHash, SaltKey, AffiliateId, LanguageId, CurrencyId, TaxDisplayTypeId, IsTaxExempt, IsAdmin, IsGuest, IsForumModerator, TotalForumPosts, Signature, AdminComment, Active, Deleted, RegistrationDate, TimeZoneId, AvatarId, Gender, FirstName, LastName, Company, VatNumber, VatNumberStatus, StreetAddress, StreetAddress2, ZipPostalCode, City, PhoneNumber, FaxNumber, CountryId, StateProvinceId, ReceiveNewsletter) VALUES (");
                    sb.Append(customer.CustomerId); sb.Append(",");
                    sb.Append('"'); sb.Append(customer.CustomerGuid); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.Email.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.Username); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.PasswordHash.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.SaltKey.Replace('"', '\'')); sb.Append("\",");
                    sb.Append(customer.AffiliateId); sb.Append(",");
                    sb.Append(customer.LanguageId); sb.Append(",");
                    sb.Append(customer.CurrencyId); sb.Append(",");
                    sb.Append(customer.TaxDisplayTypeId); sb.Append(',');
                    sb.Append('"'); sb.Append(customer.IsTaxExempt); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.IsAdmin); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.IsGuest); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.IsForumModerator); sb.Append("\",");
                    sb.Append(customer.TotalForumPosts); sb.Append(',');
                    sb.Append('"'); sb.Append(customer.Signature.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.AdminComment.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.Active); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.Deleted); sb.Append("\",");
                    sb.Append(decimalQuoter); sb.Append(customer.RegistrationDate.ToOADate()); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append('"'); sb.Append(customer.TimeZoneId); sb.Append("\",");
                    sb.Append(customer.AvatarId); sb.Append(',');

                    //custom properties
                    sb.Append('"'); sb.Append(customer.Gender); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.FirstName); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.LastName); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.Company); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.VatNumber); sb.Append("\",");
                    sb.Append(((int)customer.VatNumberStatus).ToString()); sb.Append(',');
                    sb.Append('"'); sb.Append(customer.StreetAddress); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.StreetAddress2); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.ZipPostalCode); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.City); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.PhoneNumber); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.FaxNumber); sb.Append("\",");
                    sb.Append(customer.CountryId); sb.Append(',');
                    sb.Append(customer.StateProvinceId); sb.Append(',');
                    sb.Append('"'); sb.Append(customer.ReceiveNewsletter); sb.Append("\"");
                    sb.Append(")");

                    excelHelper.ExecuteCommand(sb.ToString());
                }
            }
        }
       
        /// <summary>
        /// Export manufacturer list to xml
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        /// <returns>Result in XML format</returns>
        public static string ExportManufacturersToXml(List<Manufacturer> manufacturers)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter stringWriter = new StringWriter(sb);
            XmlWriter xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Manufacturers");
            xmlWriter.WriteAttributeString("Version", SiteHelper.GetCurrentVersion());

            foreach (var manufacturer in manufacturers)
            {
                xmlWriter.WriteStartElement("Manufacturer");
                xmlWriter.WriteElementString("ManufacturerId", null, manufacturer.ManufacturerId.ToString());
                xmlWriter.WriteElementString("Name", null, manufacturer.Name);
                xmlWriter.WriteElementString("Description", null, manufacturer.Description);
                xmlWriter.WriteElementString("TemplateId", null, manufacturer.TemplateId.ToString());
                xmlWriter.WriteElementString("MetaKeywords", null, manufacturer.MetaKeywords);
                xmlWriter.WriteElementString("MetaDescription", null, manufacturer.MetaDescription);
                xmlWriter.WriteElementString("MetaTitle", null, manufacturer.MetaTitle);
                xmlWriter.WriteElementString("SEName", null, manufacturer.SEName);
                xmlWriter.WriteElementString("PictureId", null, manufacturer.PictureId.ToString());
                xmlWriter.WriteElementString("PageSize", null, manufacturer.PageSize.ToString());
                xmlWriter.WriteElementString("PriceRanges", null, manufacturer.PriceRanges);
                xmlWriter.WriteElementString("Published", null, manufacturer.Published.ToString());
                xmlWriter.WriteElementString("Deleted", null, manufacturer.Deleted.ToString());
                xmlWriter.WriteElementString("DisplayOrder", null, manufacturer.DisplayOrder.ToString());
                xmlWriter.WriteElementString("CreatedOn", null, manufacturer.CreatedOn.ToString());
                xmlWriter.WriteElementString("UpdatedOn", null, manufacturer.UpdatedOn.ToString());

                xmlWriter.WriteStartElement("Products");
                var productManufacturers = manufacturer.ProductManufacturers;
                foreach (var productManufacturer in productManufacturers)
                {
                    var product = productManufacturer.Product;
                    if (product != null && !product.Deleted)
                    {
                        xmlWriter.WriteStartElement("ProductManufacturer");
                        xmlWriter.WriteElementString("ProductManufacturerId", null, productManufacturer.ProductManufacturerId.ToString());
                        xmlWriter.WriteElementString("ProductId", null, productManufacturer.ProductId.ToString());
                        xmlWriter.WriteElementString("IsFeaturedProduct", null, productManufacturer.IsFeaturedProduct.ToString());
                        xmlWriter.WriteElementString("DisplayOrder", null, productManufacturer.DisplayOrder.ToString());
                        xmlWriter.WriteEndElement();
                    }
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export category list to xml
        /// </summary>
        /// <returns>Result in XML format</returns>
        public static string ExportCategoriesToXml()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter stringWriter = new StringWriter(sb);
            XmlWriter xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Categories");
            xmlWriter.WriteAttributeString("Version", SiteHelper.GetCurrentVersion());
            WriteCategories(xmlWriter, 0);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export product list to xml
        /// </summary>
        /// <param name="products">Products</param>
        /// <returns>Result in XML format</returns>
        public static string ExportProductsToXml(List<Product> products)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter stringWriter = new StringWriter(sb);
            XmlWriter xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Products");
            xmlWriter.WriteAttributeString("Version", SiteHelper.GetCurrentVersion());

            foreach (var product in products)
            {
                xmlWriter.WriteStartElement("Product");
                xmlWriter.WriteElementString("ProductId", null, product.ProductId.ToString());
                xmlWriter.WriteElementString("Name", null, product.Name);
                xmlWriter.WriteElementString("ShortDescription", null, product.ShortDescription);
                xmlWriter.WriteElementString("FullDescription", null, product.FullDescription);
                xmlWriter.WriteElementString("AdminComment", null, product.AdminComment);
                xmlWriter.WriteElementString("TemplateId", null, product.TemplateId.ToString());
                xmlWriter.WriteElementString("ShowOnHomePage", null, product.ShowOnHomePage.ToString());
                xmlWriter.WriteElementString("MetaKeywords", null, product.MetaKeywords);
                xmlWriter.WriteElementString("MetaDescription", null, product.MetaDescription);
                xmlWriter.WriteElementString("MetaTitle", null, product.MetaTitle);
                xmlWriter.WriteElementString("SEName", null, product.SEName);
                xmlWriter.WriteElementString("AllowCustomerReviews", null, product.AllowCustomerReviews.ToString());
                xmlWriter.WriteElementString("AllowCustomerRatings", null, product.AllowCustomerRatings.ToString());
                xmlWriter.WriteElementString("RatingSum", null, product.RatingSum.ToString());
                xmlWriter.WriteElementString("TotalRatingVotes", null, product.TotalRatingVotes.ToString());
                xmlWriter.WriteElementString("Published", null, product.Published.ToString());
                xmlWriter.WriteElementString("Deleted", null, product.Deleted.ToString());
                xmlWriter.WriteElementString("CreatedOn", null, product.CreatedOn.ToString());
                xmlWriter.WriteElementString("UpdatedOn", null, product.UpdatedOn.ToString());

                xmlWriter.WriteStartElement("ProductVariants");
                var productVariants = ProductManager.GetProductVariantsByProductId(product.ProductId, true);
                foreach (var productVariant in productVariants)
                {
                    xmlWriter.WriteStartElement("ProductVariant");
                    xmlWriter.WriteElementString("ProductVariantId", null, productVariant.ProductVariantId.ToString());
                    xmlWriter.WriteElementString("ProductId", null, productVariant.ProductId.ToString());
                    xmlWriter.WriteElementString("Name", null, productVariant.Name);
                    xmlWriter.WriteElementString("SKU", null, productVariant.SKU);
                    xmlWriter.WriteElementString("Description", null, productVariant.Description);
                    xmlWriter.WriteElementString("AdminComment", null, productVariant.AdminComment);
                    xmlWriter.WriteElementString("ManufacturerPartNumber", null, productVariant.ManufacturerPartNumber);
                    xmlWriter.WriteElementString("IsGiftCard", null, productVariant.IsGiftCard.ToString());
                    xmlWriter.WriteElementString("GiftCardType", null, productVariant.GiftCardType.ToString());
                    xmlWriter.WriteElementString("IsDownload", null, productVariant.IsDownload.ToString());
                    xmlWriter.WriteElementString("DownloadId", null, productVariant.DownloadId.ToString());
                    xmlWriter.WriteElementString("UnlimitedDownloads", null, productVariant.UnlimitedDownloads.ToString());
                    xmlWriter.WriteElementString("MaxNumberOfDownloads", null, productVariant.MaxNumberOfDownloads.ToString());
                    if (productVariant.DownloadExpirationDays.HasValue)
                        xmlWriter.WriteElementString("DownloadExpirationDays", null, productVariant.DownloadExpirationDays.ToString());
                    else
                        xmlWriter.WriteElementString("DownloadExpirationDays", null, string.Empty);
                    xmlWriter.WriteElementString("DownloadActivationType", null, productVariant.DownloadActivationType.ToString());
                    xmlWriter.WriteElementString("HasSampleDownload", null, productVariant.HasSampleDownload.ToString());
                    xmlWriter.WriteElementString("SampleDownloadId", null, productVariant.SampleDownloadId.ToString());
                    xmlWriter.WriteElementString("HasUserAgreement", null, productVariant.HasUserAgreement.ToString());
                    xmlWriter.WriteElementString("UserAgreementText", null, productVariant.UserAgreementText);
                    xmlWriter.WriteElementString("IsRecurring", null, productVariant.IsRecurring.ToString());
                    xmlWriter.WriteElementString("CycleLength", null, productVariant.CycleLength.ToString());
                    xmlWriter.WriteElementString("CyclePeriod", null, productVariant.CyclePeriod.ToString());
                    xmlWriter.WriteElementString("TotalCycles", null, productVariant.TotalCycles.ToString());
                    xmlWriter.WriteElementString("IsShipEnabled", null, productVariant.IsShipEnabled.ToString());
                    xmlWriter.WriteElementString("IsFreeShipping", null, productVariant.IsFreeShipping.ToString());
                    xmlWriter.WriteElementString("AdditionalShippingCharge", null, productVariant.AdditionalShippingCharge.ToString());
                    xmlWriter.WriteElementString("IsTaxExempt", null, productVariant.IsTaxExempt.ToString());
                    xmlWriter.WriteElementString("TaxCategoryId", null, productVariant.TaxCategoryId.ToString());
                    xmlWriter.WriteElementString("ManageInventory", null, productVariant.ManageInventory.ToString());
                    xmlWriter.WriteElementString("StockQuantity", null, productVariant.StockQuantity.ToString());
                    xmlWriter.WriteElementString("DisplayStockAvailability", null, productVariant.DisplayStockAvailability.ToString());
                    xmlWriter.WriteElementString("DisplayStockQuantity", null, productVariant.DisplayStockQuantity.ToString());
                    xmlWriter.WriteElementString("MinStockQuantity", null, productVariant.MinStockQuantity.ToString());
                    xmlWriter.WriteElementString("LowStockActivityId", null, productVariant.LowStockActivityId.ToString());
                    xmlWriter.WriteElementString("NotifyAdminForQuantityBelow", null, productVariant.NotifyAdminForQuantityBelow.ToString());
                    xmlWriter.WriteElementString("Backorders", null, productVariant.Backorders.ToString());
                    xmlWriter.WriteElementString("OrderMinimumQuantity", null, productVariant.OrderMinimumQuantity.ToString());
                    xmlWriter.WriteElementString("OrderMaximumQuantity", null, productVariant.OrderMaximumQuantity.ToString());
                    xmlWriter.WriteElementString("WarehouseId", null, productVariant.WarehouseId.ToString());
                    xmlWriter.WriteElementString("DisableBuyButton", null, productVariant.DisableBuyButton.ToString());
                    xmlWriter.WriteElementString("Price", null, productVariant.Price.ToString());
                    xmlWriter.WriteElementString("OldPrice", null, productVariant.OldPrice.ToString());
                    xmlWriter.WriteElementString("ProductCost", null, productVariant.ProductCost.ToString());
                    xmlWriter.WriteElementString("CustomerEntersPrice", null, productVariant.CustomerEntersPrice.ToString());
                    xmlWriter.WriteElementString("MinimumCustomerEnteredPrice", null, productVariant.MinimumCustomerEnteredPrice.ToString());
                    xmlWriter.WriteElementString("MaximumCustomerEnteredPrice", null, productVariant.MaximumCustomerEnteredPrice.ToString());
                    xmlWriter.WriteElementString("Weight", null, productVariant.Weight.ToString());
                    xmlWriter.WriteElementString("Length", null, productVariant.Length.ToString());
                    xmlWriter.WriteElementString("Width", null, productVariant.Width.ToString());
                    xmlWriter.WriteElementString("Height", null, productVariant.Height.ToString());
                    xmlWriter.WriteElementString("PictureId", null, productVariant.PictureId.ToString());
                    xmlWriter.WriteElementString("Published", null, productVariant.Published.ToString());
                    xmlWriter.WriteElementString("Deleted", null, productVariant.Deleted.ToString());
                    xmlWriter.WriteElementString("DisplayOrder", null, productVariant.DisplayOrder.ToString());
                    xmlWriter.WriteElementString("CreatedOn", null, productVariant.CreatedOn.ToString());
                    xmlWriter.WriteElementString("UpdatedOn", null, productVariant.UpdatedOn.ToString());

                    xmlWriter.WriteStartElement("ProductDiscounts");
                    var discounts = productVariant.AllDiscounts;
                    foreach (var discount in discounts)
                    {
                        xmlWriter.WriteElementString("DiscountId", null, discount.DiscountId.ToString());
                    }
                    xmlWriter.WriteEndElement();


                    xmlWriter.WriteStartElement("TierPrices");
                    var tierPrices = productVariant.TierPrices;
                    foreach (var tierPrice in tierPrices)
                    {
                        xmlWriter.WriteElementString("TierPriceId", null, tierPrice.TierPriceId.ToString());
                        xmlWriter.WriteElementString("Quantity", null, tierPrice.Quantity.ToString());
                        xmlWriter.WriteElementString("Price", null, tierPrice.Price.ToString());
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("ProductAttributes");
                    var productVariantAttributes = productVariant.ProductVariantAttributes;
                    foreach (var productVariantAttribute in productVariantAttributes)
                    {
                        xmlWriter.WriteStartElement("ProductVariantAttribute");
                        xmlWriter.WriteElementString("ProductVariantAttributeId", null, productVariantAttribute.ProductVariantAttributeId.ToString());
                        xmlWriter.WriteElementString("ProductAttributeId", null, productVariantAttribute.ProductAttributeId.ToString());
                        xmlWriter.WriteElementString("TextPrompt", null, productVariantAttribute.TextPrompt);
                        xmlWriter.WriteElementString("IsRequired", null, productVariantAttribute.IsRequired.ToString());
                        xmlWriter.WriteElementString("AttributeControlTypeId", null, productVariantAttribute.AttributeControlTypeId.ToString());
                        xmlWriter.WriteElementString("DisplayOrder", null, productVariantAttribute.DisplayOrder.ToString());



                        xmlWriter.WriteStartElement("ProductVariantAttributeValues");
                        var productVariantAttributeValues = ProductAttributeManager.GetProductVariantAttributeValues(productVariantAttribute.ProductVariantAttributeId);
                        foreach (var productVariantAttributeValue in productVariantAttributeValues)
                        {
                            xmlWriter.WriteElementString("ProductVariantAttributeValueId", null, productVariantAttributeValue.ProductVariantAttributeValueId.ToString());
                            xmlWriter.WriteElementString("Name", null, productVariantAttributeValue.Name);
                            xmlWriter.WriteElementString("PriceAdjustment", null, productVariantAttributeValue.PriceAdjustment.ToString());
                            xmlWriter.WriteElementString("WeightAdjustment", null, productVariantAttributeValue.WeightAdjustment.ToString());
                            xmlWriter.WriteElementString("IsPreSelected", null, productVariantAttributeValue.IsPreSelected.ToString());
                            xmlWriter.WriteElementString("DisplayOrder", null, productVariantAttributeValue.DisplayOrder.ToString());
                        }
                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();



                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ProductPictures");
                var productPictures = product.ProductPictures;
                foreach (var productPicture in productPictures)
                {
                    xmlWriter.WriteStartElement("ProductPicture");
                    xmlWriter.WriteElementString("ProductPictureId", null, productPicture.ProductPictureId.ToString());
                    xmlWriter.WriteElementString("PictureId", null, productPicture.PictureId.ToString());
                    xmlWriter.WriteElementString("DisplayOrder", null, productPicture.DisplayOrder.ToString());
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("RelatedProducts");
                var relatedProducts = product.RelatedProducts;
                foreach (var relatedProduct in relatedProducts)
                {
                    xmlWriter.WriteStartElement("RelatedProduct");
                    xmlWriter.WriteElementString("RelatedProductId", null, relatedProduct.RelatedProductId.ToString());
                    xmlWriter.WriteElementString("ProductId1", null, relatedProduct.ProductId1.ToString());
                    xmlWriter.WriteElementString("ProductId2", null, relatedProduct.ProductId2.ToString());
                    xmlWriter.WriteElementString("DisplayOrder", null, relatedProduct.DisplayOrder.ToString());
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ProductCategories");
                var productCategories = product.ProductCategories;
                foreach (var productCategory in productCategories)
                {
                    xmlWriter.WriteStartElement("ProductCategory");
                    xmlWriter.WriteElementString("ProductCategoryId", null, productCategory.ProductCategoryId.ToString());
                    xmlWriter.WriteElementString("CategoryId", null, productCategory.CategoryId.ToString());
                    xmlWriter.WriteElementString("IsFeaturedProduct", null, productCategory.IsFeaturedProduct.ToString());
                    xmlWriter.WriteElementString("DisplayOrder", null, productCategory.DisplayOrder.ToString());
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ProductManufacturers");
                var productManufacturers = product.ProductManufacturers;
                foreach (var productManufacturer in productManufacturers)
                {
                    xmlWriter.WriteStartElement("ProductManufacturer");
                    xmlWriter.WriteElementString("ProductManufacturerId", null, productManufacturer.ProductManufacturerId.ToString());
                    xmlWriter.WriteElementString("ManufacturerId", null, productManufacturer.ManufacturerId.ToString());
                    xmlWriter.WriteElementString("IsFeaturedProduct", null, productManufacturer.IsFeaturedProduct.ToString());
                    xmlWriter.WriteElementString("DisplayOrder", null, productManufacturer.DisplayOrder.ToString());
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ProductSpecificationAttributes");
                var productSpecificationAttributes = SpecificationAttributeManager.GetProductSpecificationAttributesByProductId(product.ProductId);
                foreach (var productSpecificationAttribute in productSpecificationAttributes)
                {
                    xmlWriter.WriteStartElement("ProductSpecificationAttribute");
                    xmlWriter.WriteElementString("ProductSpecificationAttributeId", null, productSpecificationAttribute.ProductSpecificationAttributeId.ToString());
                    xmlWriter.WriteElementString("SpecificationAttributeOptionId", null, productSpecificationAttribute.SpecificationAttributeOptionId.ToString());
                    xmlWriter.WriteElementString("AllowFiltering", null, productSpecificationAttribute.AllowFiltering.ToString());
                    xmlWriter.WriteElementString("ShowOnProductPage", null, productSpecificationAttribute.ShowOnProductPage.ToString());
                    xmlWriter.WriteElementString("DisplayOrder", null, productSpecificationAttribute.DisplayOrder.ToString());
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();




                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export products to XLS
        /// </summary>
        /// <param name="filePath">File path to use</param>
        /// <param name="products">Products</param>
        public static void ExportProductsToXls(string filePath, List<Product> products)
        {
            using (ExcelHelper excelHelper = new ExcelHelper(filePath))
            {
                excelHelper.Hdr = "YES";
                excelHelper.Imex = "0";
                Dictionary<string, string> tableDefinition = new Dictionary<string, string>();
                tableDefinition.Add("Name", "ntext");
                tableDefinition.Add("ShortDescription", "ntext");
                tableDefinition.Add("FullDescription", "ntext");
                tableDefinition.Add("TemplateId", "int");
                tableDefinition.Add("ShowOnHomePage", "nvarchar(5)");
                tableDefinition.Add("MetaKeywords", "ntext");
                tableDefinition.Add("MetaDescription", "ntext");
                tableDefinition.Add("MetaTitle", "ntext");
                tableDefinition.Add("AllowCustomerReviews", "nvarchar(5)");
                tableDefinition.Add("AllowCustomerRatings", "nvarchar(5)");
                tableDefinition.Add("Published", "nvarchar(5)");
                tableDefinition.Add("SKU", "ntext");
                tableDefinition.Add("ManufacturerPartNumber", "ntext");
                tableDefinition.Add("IsGiftCard", "nvarchar(5)");
                tableDefinition.Add("GiftCardType", "int");
                tableDefinition.Add("IsDownload", "nvarchar(5)");
                tableDefinition.Add("DownloadId", "int");
                tableDefinition.Add("UnlimitedDownloads", "nvarchar(5)");
                tableDefinition.Add("MaxNumberOfDownloads", "int");
                tableDefinition.Add("DownloadActivationType", "int");                
                tableDefinition.Add("HasSampleDownload", "nvarchar(5)");
                tableDefinition.Add("SampleDownloadId", "int");
                tableDefinition.Add("HasUserAgreement", "nvarchar(5)");
                tableDefinition.Add("UserAgreementText", "ntext");
                tableDefinition.Add("IsRecurring", "nvarchar(5)");
                tableDefinition.Add("CycleLength", "int");
                tableDefinition.Add("CyclePeriod", "int");
                tableDefinition.Add("TotalCycles", "int");
                tableDefinition.Add("IsShipEnabled", "nvarchar(5)");
                tableDefinition.Add("IsFreeShipping", "nvarchar(5)");
                tableDefinition.Add("AdditionalShippingCharge", "decimal");
                tableDefinition.Add("IsTaxExempt", "nvarchar(5)");
                tableDefinition.Add("TaxCategoryId", "int");
                tableDefinition.Add("ManageInventory", "int");
                tableDefinition.Add("StockQuantity", "int");
                tableDefinition.Add("DisplayStockAvailability", "nvarchar(5)");
                tableDefinition.Add("DisplayStockQuantity", "nvarchar(5)");
                tableDefinition.Add("MinStockQuantity", "int");
                tableDefinition.Add("LowStockActivityId", "int");
                tableDefinition.Add("NotifyAdminForQuantityBelow", "int");
                tableDefinition.Add("Backorders", "int");
                tableDefinition.Add("OrderMinimumQuantity", "int");
                tableDefinition.Add("OrderMaximumQuantity", "int");
                tableDefinition.Add("DisableBuyButton", "nvarchar(5)");
                tableDefinition.Add("Price", "decimal");
                tableDefinition.Add("OldPrice", "decimal");
                tableDefinition.Add("ProductCost", "decimal");
                tableDefinition.Add("CustomerEntersPrice", "nvarchar(5)");
                tableDefinition.Add("MinimumCustomerEnteredPrice", "decimal");
                tableDefinition.Add("MaximumCustomerEnteredPrice", "decimal");
                tableDefinition.Add("Weight", "decimal");
                tableDefinition.Add("Length", "decimal");
                tableDefinition.Add("Width", "decimal");
                tableDefinition.Add("Height", "decimal");
                tableDefinition.Add("CreatedOn", "decimal");
                excelHelper.WriteTable("Products", tableDefinition);

                string decimalQuoter = (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(",") ? "\"" : String.Empty);

                foreach (var p in products)
                {
                    var productVariants = ProductManager.GetProductVariantsByProductId(p.ProductId, true);

                    foreach (var pv in productVariants)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("INSERT INTO [Products] (Name, ShortDescription,FullDescription,TemplateId,ShowOnHomePage,MetaKeywords,MetaDescription,MetaTitle,AllowCustomerReviews,AllowCustomerRatings,Published,SKU,ManufacturerPartNumber,IsGiftCard,GiftCardType,IsDownload,DownloadId,UnlimitedDownloads,MaxNumberOfDownloads,DownloadActivationType,HasSampleDownload,SampleDownloadId,HasUserAgreement,UserAgreementText,IsRecurring,CycleLength,CyclePeriod,TotalCycles,IsShipEnabled,IsFreeShipping,AdditionalShippingCharge,IsTaxExempt,TaxCategoryId,ManageInventory,StockQuantity,DisplayStockAvailability,DisplayStockQuantity,MinStockQuantity,LowStockActivityId,NotifyAdminForQuantityBelow,Backorders,OrderMinimumQuantity,OrderMaximumQuantity,DisableBuyButton,Price,OldPrice,ProductCost,CustomerEntersPrice,MinimumCustomerEnteredPrice,MaximumCustomerEnteredPrice,Weight, Length, Width, Height, CreatedOn) VALUES (");
                        sb.Append('"'); sb.Append(p.Name.Replace('"', '\'')); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.ShortDescription.Replace('"', '\'')); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.FullDescription.Replace('"', '\'')); sb.Append("\",");
                        sb.Append(p.TemplateId); sb.Append(",");
                        sb.Append('"'); sb.Append(p.ShowOnHomePage); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.MetaKeywords.Replace('"', '\'')); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.MetaDescription.Replace('"', '\'')); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.MetaTitle.Replace('"', '\'')); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.AllowCustomerReviews); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.AllowCustomerRatings); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.Published); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.SKU.Replace('"', '\'')); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.ManufacturerPartNumber.Replace('"', '\'')); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.IsGiftCard); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.GiftCardType); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.IsDownload); sb.Append("\",");
                        sb.Append(pv.DownloadId); sb.Append(",");
                        sb.Append('"'); sb.Append(pv.UnlimitedDownloads); sb.Append("\",");
                        sb.Append(pv.MaxNumberOfDownloads); sb.Append(",");
                        sb.Append(pv.DownloadActivationType); sb.Append(",");
                        sb.Append('"'); sb.Append(pv.HasSampleDownload); sb.Append("\",");
                        sb.Append(pv.SampleDownloadId); sb.Append(",");
                        sb.Append('"'); sb.Append(pv.HasUserAgreement); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.UserAgreementText.Replace('"', '\'')); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.IsRecurring); sb.Append("\",");
                        sb.Append(pv.CycleLength); sb.Append(",");
                        sb.Append(pv.CyclePeriod); sb.Append(",");
                        sb.Append(pv.TotalCycles); sb.Append(",");
                        sb.Append('"'); sb.Append(pv.IsShipEnabled); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.IsFreeShipping); sb.Append("\",");
                        sb.Append(decimalQuoter); sb.Append(pv.AdditionalShippingCharge); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append('"'); sb.Append(pv.IsTaxExempt); sb.Append("\",");
                        sb.Append(pv.TaxCategoryId); sb.Append(",");
                        sb.Append(pv.ManageInventory); sb.Append(",");
                        sb.Append(pv.StockQuantity); sb.Append(",");
                        sb.Append('"'); sb.Append(pv.DisplayStockAvailability); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.DisplayStockQuantity); sb.Append("\",");
                        sb.Append(pv.MinStockQuantity); sb.Append(",");
                        sb.Append(pv.LowStockActivityId); sb.Append(",");
                        sb.Append(pv.NotifyAdminForQuantityBelow); sb.Append(",");
                        sb.Append(pv.Backorders); sb.Append(",");
                        sb.Append(pv.OrderMinimumQuantity); sb.Append(",");
                        sb.Append(pv.OrderMaximumQuantity); sb.Append(",");
                        sb.Append('"'); sb.Append(pv.DisableBuyButton); sb.Append("\",");
                        sb.Append(decimalQuoter); sb.Append(pv.Price); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.OldPrice); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.ProductCost); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append('"'); sb.Append(pv.CustomerEntersPrice); sb.Append("\",");
                        sb.Append(decimalQuoter); sb.Append(pv.MinimumCustomerEnteredPrice); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.MaximumCustomerEnteredPrice); sb.Append(decimalQuoter); sb.Append(',');//decimal                        
                        sb.Append(decimalQuoter); sb.Append(pv.Weight); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.Length); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.Width); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.Height); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.CreatedOn.ToOADate()); sb.Append(decimalQuoter); 
                        sb.Append(")");

                        excelHelper.ExecuteCommand(sb.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Export order list to xml
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>Result in XML format</returns>
        public static string ExportOrdersToXml(List<Order> orders)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter stringWriter = new StringWriter(sb);
            XmlWriter xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Orders");
            xmlWriter.WriteAttributeString("Version", SiteHelper.GetCurrentVersion());

            foreach (var order in orders)
            {
                xmlWriter.WriteStartElement("Order");
                xmlWriter.WriteElementString("OrderId", null, order.OrderId.ToString());
                xmlWriter.WriteElementString("OrderGuid", null, order.OrderGuid.ToString());
                xmlWriter.WriteElementString("CustomerId", null, order.CustomerId.ToString());
                xmlWriter.WriteElementString("CustomerLanguageId", null, order.CustomerLanguageId.ToString());
                xmlWriter.WriteElementString("CustomerTaxDisplayTypeId", null, order.CustomerTaxDisplayTypeId.ToString());
                xmlWriter.WriteElementString("CustomerIP", null, order.CustomerIP);
                xmlWriter.WriteElementString("OrderSubtotalInclTax", null, order.OrderSubtotalInclTax.ToString());
                xmlWriter.WriteElementString("OrderSubtotalExclTax", null, order.OrderSubtotalExclTax.ToString());
                xmlWriter.WriteElementString("OrderShippingInclTax", null, order.OrderShippingInclTax.ToString());
                xmlWriter.WriteElementString("OrderShippingExclTax", null, order.OrderShippingExclTax.ToString());
                xmlWriter.WriteElementString("PaymentMethodAdditionalFeeInclTax", null, order.PaymentMethodAdditionalFeeInclTax.ToString());
                xmlWriter.WriteElementString("PaymentMethodAdditionalFeeExclTax", null, order.PaymentMethodAdditionalFeeExclTax.ToString());
                xmlWriter.WriteElementString("TaxRates", null, order.TaxRates);
                xmlWriter.WriteElementString("OrderTax", null, order.OrderTax.ToString());
                xmlWriter.WriteElementString("OrderTotal", null, order.OrderTotal.ToString());
                xmlWriter.WriteElementString("OrderDiscount", null, order.OrderDiscount.ToString());
                xmlWriter.WriteElementString("OrderSubtotalInclTaxInCustomerCurrency", null, order.OrderSubtotalInclTaxInCustomerCurrency.ToString());
                xmlWriter.WriteElementString("OrderSubtotalExclTaxInCustomerCurrency", null, order.OrderSubtotalExclTaxInCustomerCurrency.ToString());
                xmlWriter.WriteElementString("OrderShippingInclTaxInCustomerCurrency", null, order.OrderShippingInclTaxInCustomerCurrency.ToString());
                xmlWriter.WriteElementString("OrderShippingExclTaxInCustomerCurrency", null, order.OrderShippingExclTaxInCustomerCurrency.ToString());
                xmlWriter.WriteElementString("PaymentMethodAdditionalFeeInclTaxInCustomerCurrency", null, order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency.ToString());
                xmlWriter.WriteElementString("PaymentMethodAdditionalFeeExclTaxInCustomerCurrency", null, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency.ToString());
                xmlWriter.WriteElementString("TaxRatesInCustomerCurrency", null, order.TaxRatesInCustomerCurrency);
                xmlWriter.WriteElementString("OrderTaxInCustomerCurrency", null, order.OrderTaxInCustomerCurrency.ToString());
                xmlWriter.WriteElementString("OrderTotalInCustomerCurrency", null, order.OrderTotalInCustomerCurrency.ToString());
                xmlWriter.WriteElementString("OrderDiscountInCustomerCurrency", null, order.OrderDiscountInCustomerCurrency.ToString());
                xmlWriter.WriteElementString("CustomerCurrencyCode", null, order.CustomerCurrencyCode);
                xmlWriter.WriteElementString("OrderWeight", null, order.OrderWeight.ToString());
                xmlWriter.WriteElementString("AffiliateId", null, order.AffiliateId.ToString());
                xmlWriter.WriteElementString("OrderStatusId", null, order.OrderStatusId.ToString());
                xmlWriter.WriteElementString("AllowStoringCreditCardNumber", null, order.AllowStoringCreditCardNumber.ToString());
                xmlWriter.WriteElementString("CardType", null, order.CardType);
                xmlWriter.WriteElementString("CardName", null, order.CardName);
                xmlWriter.WriteElementString("CardNumber", null, order.CardNumber);
                xmlWriter.WriteElementString("MaskedCreditCardNumber", null, order.MaskedCreditCardNumber);
                xmlWriter.WriteElementString("CardCvv2", null, order.CardCvv2);
                xmlWriter.WriteElementString("CardExpirationMonth", null, order.CardExpirationMonth);
                xmlWriter.WriteElementString("CardExpirationYear", null, order.CardExpirationYear);
                xmlWriter.WriteElementString("PaymentMethodId", null, order.PaymentMethodId.ToString());
                xmlWriter.WriteElementString("PaymentMethodName", null, order.PaymentMethodName);
                xmlWriter.WriteElementString("AuthorizationTransactionId", null, order.AuthorizationTransactionId);
                xmlWriter.WriteElementString("AuthorizationTransactionCode", null, order.AuthorizationTransactionCode);
                xmlWriter.WriteElementString("AuthorizationTransactionResult", null, order.AuthorizationTransactionResult);
                xmlWriter.WriteElementString("CaptureTransactionId", null, order.CaptureTransactionId);
                xmlWriter.WriteElementString("CaptureTransactionResult", null, order.CaptureTransactionResult);
                xmlWriter.WriteElementString("SubscriptionTransactionId", null, order.SubscriptionTransactionId);
                xmlWriter.WriteElementString("PurchaseOrderNumber", null, order.PurchaseOrderNumber);
                xmlWriter.WriteElementString("PaymentStatusId", null, order.PaymentStatusId.ToString());
                xmlWriter.WriteElementString("PaidDate", null, (order.PaidDate == null) ? string.Empty : order.PaidDate.Value.ToString());
                xmlWriter.WriteElementString("BillingFirstName", null, order.BillingFirstName);
                xmlWriter.WriteElementString("BillingLastName", null, order.BillingLastName);
                xmlWriter.WriteElementString("BillingPhoneNumber", null, order.BillingPhoneNumber);
                xmlWriter.WriteElementString("BillingEmail", null, order.BillingEmail);
                xmlWriter.WriteElementString("BillingFaxNumber", null, order.BillingFaxNumber);
                xmlWriter.WriteElementString("BillingCompany", null, order.BillingCompany);
                xmlWriter.WriteElementString("BillingAddress1", null, order.BillingAddress1);
                xmlWriter.WriteElementString("BillingAddress2", null, order.BillingAddress2);
                xmlWriter.WriteElementString("BillingCity", null, order.BillingCity);
                xmlWriter.WriteElementString("BillingStateProvince", null, order.BillingStateProvince);
                xmlWriter.WriteElementString("BillingStateProvinceId", null, order.BillingStateProvinceId.ToString());
                xmlWriter.WriteElementString("BillingCountry", null, order.BillingCountry);
                xmlWriter.WriteElementString("BillingCountryId", null, order.BillingCountryId.ToString());
                xmlWriter.WriteElementString("BillingZipPostalCode", null, order.BillingZipPostalCode);
                xmlWriter.WriteElementString("ShippingStatusId", null, order.ShippingStatusId.ToString());
                xmlWriter.WriteElementString("ShippingFirstName", null, order.ShippingFirstName);
                xmlWriter.WriteElementString("ShippingLastName", null, order.ShippingLastName);
                xmlWriter.WriteElementString("ShippingPhoneNumber", null, order.ShippingPhoneNumber);
                xmlWriter.WriteElementString("ShippingEmail", null, order.ShippingEmail);
                xmlWriter.WriteElementString("ShippingFaxNumber", null, order.ShippingFaxNumber);
                xmlWriter.WriteElementString("ShippingCompany", null, order.ShippingCompany);
                xmlWriter.WriteElementString("ShippingAddress1", null, order.ShippingAddress1);
                xmlWriter.WriteElementString("ShippingAddress2", null, order.ShippingAddress2);
                xmlWriter.WriteElementString("ShippingCity", null, order.ShippingCity);
                xmlWriter.WriteElementString("ShippingStateProvince", null, order.ShippingStateProvince);
                xmlWriter.WriteElementString("ShippingStateProvinceId", null, order.ShippingStateProvinceId.ToString());
                xmlWriter.WriteElementString("ShippingCountry", null, order.ShippingCountry);
                xmlWriter.WriteElementString("ShippingCountryId", null, order.ShippingCountryId.ToString());
                xmlWriter.WriteElementString("ShippingZipPostalCode", null, order.ShippingZipPostalCode);
                xmlWriter.WriteElementString("ShippingMethod", null, order.ShippingMethod);
                xmlWriter.WriteElementString("ShippingRateComputationMethodId", null, order.ShippingRateComputationMethodId.ToString());
                xmlWriter.WriteElementString("ShippedDate", null, (order.ShippedDate == null) ? string.Empty : order.ShippedDate.Value.ToString());
                xmlWriter.WriteElementString("TrackingNumber", null, order.TrackingNumber);
                xmlWriter.WriteElementString("VatNumber", null, order.VatNumber);
                xmlWriter.WriteElementString("Deleted", null, order.Deleted.ToString());
                xmlWriter.WriteElementString("CreatedOn", null, order.CreatedOn.ToString());

                var orderProductVariants = order.OrderProductVariants;
                if (orderProductVariants.Count > 0)
                {
                    xmlWriter.WriteStartElement("OrderProductVariants");
                    foreach (var orderProductVariant in orderProductVariants)
                    {
                        xmlWriter.WriteStartElement("OrderProductVariant");
                        xmlWriter.WriteElementString("OrderProductVariantId", null, orderProductVariant.OrderProductVariantId.ToString());
                        xmlWriter.WriteElementString("ProductVariantId", null, orderProductVariant.ProductVariantId.ToString());

                        var productVariant = orderProductVariant.ProductVariant;
                        if (productVariant != null)
                            xmlWriter.WriteElementString("ProductVariantName", null, productVariant.FullProductName);


                        xmlWriter.WriteElementString("UnitPriceInclTax", null, orderProductVariant.UnitPriceInclTax.ToString());
                        xmlWriter.WriteElementString("UnitPriceExclTax", null, orderProductVariant.UnitPriceExclTax.ToString());
                        xmlWriter.WriteElementString("PriceInclTax", null, orderProductVariant.PriceInclTax.ToString());
                        xmlWriter.WriteElementString("PriceExclTax", null, orderProductVariant.PriceExclTax.ToString());
                        xmlWriter.WriteElementString("UnitPriceInclTaxInCustomerCurrency", null, orderProductVariant.UnitPriceInclTaxInCustomerCurrency.ToString());
                        xmlWriter.WriteElementString("UnitPriceExclTaxInCustomerCurrency", null, orderProductVariant.UnitPriceExclTaxInCustomerCurrency.ToString());
                        xmlWriter.WriteElementString("PriceInclTaxInCustomerCurrency", null, orderProductVariant.PriceInclTaxInCustomerCurrency.ToString());
                        xmlWriter.WriteElementString("PriceExclTaxInCustomerCurrency", null, orderProductVariant.PriceExclTaxInCustomerCurrency.ToString());
                        xmlWriter.WriteElementString("AttributeDescription", null, orderProductVariant.AttributeDescription);
                        xmlWriter.WriteElementString("AttributesXml", null, orderProductVariant.AttributesXml);
                        xmlWriter.WriteElementString("Quantity", null, orderProductVariant.Quantity.ToString());
                        xmlWriter.WriteElementString("DiscountAmountInclTax", null, orderProductVariant.DiscountAmountInclTax.ToString());
                        xmlWriter.WriteElementString("DiscountAmountExclTax", null, orderProductVariant.DiscountAmountExclTax.ToString());
                        xmlWriter.WriteElementString("DownloadCount", null, orderProductVariant.DownloadCount.ToString());
                        xmlWriter.WriteElementString("IsDownloadActivated", null, orderProductVariant.IsDownloadActivated.ToString());
                        xmlWriter.WriteElementString("LicenseDownloadId", null, orderProductVariant.LicenseDownloadId.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export orders to XLS
        /// </summary>
        /// <param name="filePath">File path to use</param>
        /// <param name="orders">Orders</param>
        public static void ExportOrdersToXls(string filePath, List<Order> orders)
        {
            using (ExcelHelper excelHelper = new ExcelHelper(filePath))
            {
                excelHelper.Hdr = "YES";
                excelHelper.Imex = "0";
                Dictionary<string, string> tableDefinition = new Dictionary<string, string>();
                tableDefinition.Add("OrderId", "int");
                tableDefinition.Add("OrderGuid", "uniqueidentifier");
                tableDefinition.Add("CustomerId", "int");
                tableDefinition.Add("OrderSubtotalInclTax", "decimal");
                tableDefinition.Add("OrderSubtotalExclTax", "decimal");
                tableDefinition.Add("OrderShippingInclTax", "decimal");
                tableDefinition.Add("OrderShippingExclTax", "decimal");
                tableDefinition.Add("PaymentMethodAdditionalFeeInclTax", "decimal");
                tableDefinition.Add("PaymentMethodAdditionalFeeExclTax", "decimal");
                tableDefinition.Add("TaxRates", "nvarchar(255)");
                tableDefinition.Add("OrderTax", "decimal");
                tableDefinition.Add("OrderTotal", "decimal");
                tableDefinition.Add("OrderDiscount", "decimal");
                tableDefinition.Add("OrderSubtotalInclTaxInCustomerCurrency", "decimal");
                tableDefinition.Add("OrderSubtotalExclTaxInCustomerCurrency", "decimal");
                tableDefinition.Add("OrderShippingInclTaxInCustomerCurrency", "decimal");
                tableDefinition.Add("OrderShippingExclTaxInCustomerCurrency", "decimal");
                tableDefinition.Add("PaymentMethodAdditionalFeeInclTaxInCustomerCurrency", "decimal");
                tableDefinition.Add("PaymentMethodAdditionalFeeExclTaxInCustomerCurrency", "decimal");
                tableDefinition.Add("TaxRatesInCustomerCurrency", "nvarchar(255)");
                tableDefinition.Add("OrderTaxInCustomerCurrency", "decimal");
                tableDefinition.Add("OrderTotalInCustomerCurrency", "decimal");
                tableDefinition.Add("OrderDiscountInCustomerCurrency", "decimal");
                tableDefinition.Add("CustomerCurrencyCode", "nvarchar(5)");
                tableDefinition.Add("OrderWeight", "decimal");
                tableDefinition.Add("AffiliateId", "int");
                tableDefinition.Add("OrderStatusId", "int");
                tableDefinition.Add("PaymentMethodId", "int");
                tableDefinition.Add("PaymentMethodName", "nvarchar(100)");
                tableDefinition.Add("PurchaseOrderNumber", "nvarchar(100)");
                tableDefinition.Add("PaymentStatusId", "int");
                tableDefinition.Add("BillingFirstName", "nvarchar(100)");
                tableDefinition.Add("BillingLastName", "nvarchar(100)");
                tableDefinition.Add("BillingPhoneNumber", "nvarchar(50)");
                tableDefinition.Add("BillingEmail", "nvarchar(255)");
                tableDefinition.Add("BillingFaxNumber", "nvarchar(50)");
                tableDefinition.Add("BillingCompany", "nvarchar(100)");
                tableDefinition.Add("BillingAddress1", "nvarchar(100)");
                tableDefinition.Add("BillingAddress2", "nvarchar(100)");
                tableDefinition.Add("BillingCity", "nvarchar(100)");
                tableDefinition.Add("BillingStateProvince", "nvarchar(100)");
                tableDefinition.Add("BillingZipPostalCode", "nvarchar(100)");
                tableDefinition.Add("BillingCountry", "nvarchar(100)");
                tableDefinition.Add("ShippingStatusId", "int");
                tableDefinition.Add("ShippingFirstName", "nvarchar(100)");
                tableDefinition.Add("ShippingLastName", "nvarchar(100)");
                tableDefinition.Add("ShippingPhoneNumber", "nvarchar(50)");
                tableDefinition.Add("ShippingEmail", "nvarchar(255)");
                tableDefinition.Add("ShippingFaxNumber", "nvarchar(50)");
                tableDefinition.Add("ShippingCompany", "nvarchar(100)");
                tableDefinition.Add("ShippingAddress1", "nvarchar(100)");
                tableDefinition.Add("ShippingAddress2", "nvarchar(100)");
                tableDefinition.Add("ShippingCity", "nvarchar(100)");
                tableDefinition.Add("ShippingStateProvince", "nvarchar(100)");
                tableDefinition.Add("ShippingZipPostalCode", "nvarchar(100)");
                tableDefinition.Add("ShippingCountry", "nvarchar(100)");
                tableDefinition.Add("ShippingMethod", "nvarchar(100)");
                tableDefinition.Add("ShippingRateComputationMethodId", "int");
                tableDefinition.Add("VatNumber", "nvarchar(100)");
                tableDefinition.Add("CreatedOn", "decimal");
                excelHelper.WriteTable("Orders", tableDefinition);
                
                string decimalQuoter = (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(",") ? "\"" : String.Empty);

                foreach (var order in orders)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO [Orders] (OrderId, OrderGuid, CustomerId, OrderSubtotalInclTax, OrderSubtotalExclTax, OrderShippingInclTax, OrderShippingExclTax, PaymentMethodAdditionalFeeInclTax, PaymentMethodAdditionalFeeExclTax, TaxRates, OrderTax, OrderTotal, OrderDiscount, OrderSubtotalInclTaxInCustomerCurrency, OrderSubtotalExclTaxInCustomerCurrency, OrderShippingInclTaxInCustomerCurrency, OrderShippingExclTaxInCustomerCurrency, PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, PaymentMethodAdditionalFeeExclTaxInCustomerCurrency, TaxRatesInCustomerCurrency, OrderTaxInCustomerCurrency, OrderTotalInCustomerCurrency, OrderDiscountInCustomerCurrency, CustomerCurrencyCode, OrderWeight, AffiliateId, OrderStatusId, PaymentMethodId, PaymentMethodName, PurchaseOrderNumber, PaymentStatusId, BillingFirstName, BillingLastName, BillingPhoneNumber, BillingEmail, BillingFaxNumber, BillingCompany, BillingAddress1, BillingAddress2, BillingCity, BillingStateProvince, BillingZipPostalCode, BillingCountry, ShippingStatusId,  ShippingFirstName, ShippingLastName, ShippingPhoneNumber, ShippingEmail, ShippingFaxNumber, ShippingCompany,  ShippingAddress1, ShippingAddress2, ShippingCity, ShippingStateProvince, ShippingZipPostalCode, ShippingCountry, ShippingMethod, ShippingRateComputationMethodId, VatNumber, CreatedOn) VALUES (");


                    sb.Append(order.OrderId); sb.Append(",");
                    sb.Append('"'); sb.Append(order.OrderGuid); sb.Append("\",");
                    sb.Append(order.CustomerId); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderSubtotalInclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderSubtotalExclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderShippingInclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderShippingExclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.PaymentMethodAdditionalFeeInclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.PaymentMethodAdditionalFeeExclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append('"'); sb.Append(order.TaxRates.Replace('"', '\'')); sb.Append("\",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderTotal); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderDiscount); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderSubtotalInclTaxInCustomerCurrency); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderSubtotalExclTaxInCustomerCurrency); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderShippingInclTaxInCustomerCurrency); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderShippingExclTaxInCustomerCurrency); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append('"'); sb.Append(order.TaxRatesInCustomerCurrency.Replace('"', '\'')); sb.Append("\",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderTaxInCustomerCurrency); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderTotalInCustomerCurrency); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderDiscountInCustomerCurrency); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append('"'); sb.Append(order.CustomerCurrencyCode.Replace('"', '\'')); sb.Append("\",");
                    sb.Append(order.OrderWeight); sb.Append(",");
                    sb.Append(order.AffiliateId); sb.Append(",");
                    sb.Append(order.OrderStatusId); sb.Append(",");
                    sb.Append(order.PaymentMethodId); sb.Append(",");
                    sb.Append('"'); sb.Append(order.PaymentMethodName.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.PurchaseOrderNumber.Replace('"', '\'')); sb.Append("\",");
                    sb.Append(order.PaymentStatusId); sb.Append(",");
                    sb.Append('"'); sb.Append(order.BillingFirstName.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.BillingLastName.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.BillingPhoneNumber.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.BillingEmail.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.BillingFaxNumber.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.BillingCompany.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.BillingAddress1.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.BillingAddress2.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.BillingCity.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.BillingStateProvince.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.BillingZipPostalCode.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.BillingCountry.Replace('"', '\'')); sb.Append("\",");
                    sb.Append(order.ShippingStatusId); sb.Append(",");
                    sb.Append('"'); sb.Append(order.ShippingFirstName.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingLastName.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingPhoneNumber.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingEmail.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingFaxNumber.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingCompany.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingAddress1.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingAddress2.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingCity.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingStateProvince.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingZipPostalCode.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingCountry.Replace('"', '\'')); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingMethod.Replace('"', '\'')); sb.Append("\",");
                    sb.Append(order.ShippingRateComputationMethodId); sb.Append(",");
                    sb.Append('"'); sb.Append(order.VatNumber.Replace('"', '\'')); sb.Append("\",");
                    sb.Append(decimalQuoter); sb.Append(order.CreatedOn.ToOADate()); sb.Append(decimalQuoter); 
                    sb.Append(")");

                    excelHelper.ExecuteCommand(sb.ToString());
                }
            }
        }

        /// <summary>
        /// Export message tokens to xml
        /// </summary>
        /// <returns>Result in XML format</returns>
        public static string ExportMessageTokensToXml()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter stringWriter = new StringWriter(sb);
            XmlWriter xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Tokens");
            string[] allowedTokens = MessageManager.GetListOfAllowedTokens();
            for (int i = 0; i < allowedTokens.Length; i++)
            {
                string token = allowedTokens[i];
                string tokenName = token.Replace("%", "");
                xmlWriter.WriteStartElement("Token");
                xmlWriter.WriteAttributeString("name", tokenName);
                xmlWriter.WriteAttributeString("value", token);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        #endregion
    }
}
