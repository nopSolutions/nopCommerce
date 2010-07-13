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
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Security;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager
    {
        #region Methods
        /// <summary>
        /// Import string resources and message templates from XML
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="content">XML content</param>
        public static void ImportResources(int languageId, string content)
        {
            LocalizationManager.LanguagePackImport(languageId, content);
        }

        /// <summary>
        /// Import customer list from XLS file
        /// </summary>
        /// <param name="filePath">Excel file path</param>
        public static void ImportCustomersFromXls(string filePath)
        {
            using (ExcelHelper excelHelper = new ExcelHelper(filePath))
            {
                excelHelper.Hdr = "YES";
                excelHelper.Imex = "1";

                DataTable dt = excelHelper.ReadTable("Customers");
                foreach (DataRow dr in dt.Rows)
                {
                    int customerId = Convert.ToInt32(dr["CustomerId"]);
                    Guid customerGuid = new Guid(dr["CustomerGuid"].ToString());
                    string email = dr["Email"].ToString();
                    string username = dr["Username"].ToString();
                    string passwordHash = dr["PasswordHash"].ToString();
                    string saltKey = dr["SaltKey"].ToString();
                    int affiliateId = Convert.ToInt32(dr["AffiliateId"]);
                    int billingAddressId = 0;
                    int shippingAddressId = 0;
                    int lastPaymentMethodId = 0;
                    string lastAppliedCouponCode = string.Empty;
                    int languageId = Convert.ToInt32(dr["LanguageId"]);
                    int currencyId = Convert.ToInt32(dr["CurrencyId"]);
                    int taxDisplayTypeId = Convert.ToInt32(dr["TaxDisplayTypeId"]);
                    bool isTaxExempt = Convert.ToBoolean(dr["IsTaxExempt"]);
                    bool isAdmin = Convert.ToBoolean(dr["IsAdmin"]);
                    bool isGuest = Convert.ToBoolean(dr["IsGuest"]);
                    bool isForumModerator = Convert.ToBoolean(dr["IsForumModerator"]);
                    int totalForumPosts = Convert.ToInt32(dr["TotalForumPosts"]);
                    string signature = dr["Signature"].ToString();
                    string adminComment = dr["AdminComment"].ToString();
                    bool active = Convert.ToBoolean(dr["Active"]);
                    bool deleted = Convert.ToBoolean(dr["Deleted"]);
                    DateTime registrationDate = DateTime.FromOADate(Convert.ToDouble(dr["RegistrationDate"]));
                    string timeZoneId = dr["TimeZoneId"].ToString();
                    int avatarId = Convert.ToInt32(dr["AvatarId"]);

                    //custom properties
                    string gender = dr["Gender"].ToString();
                    string firstName = dr["FirstName"].ToString();
                    string lastName = dr["LastName"].ToString();
                    string company = dr["Company"].ToString();
                    string streetAddress = dr["StreetAddress"].ToString();
                    string streetAddress2 = dr["StreetAddress2"].ToString();
                    string zipPostalCode = dr["ZipPostalCode"].ToString();
                    string city = dr["City"].ToString();
                    string phoneNumber = dr["PhoneNumber"].ToString();
                    string faxNumber = dr["FaxNumber"].ToString();
                    int countryId = Convert.ToInt32(dr["CountryId"]);
                    int stateProvinceId = Convert.ToInt32(dr["StateProvinceId"]);
                    bool receiveNewsletter = Convert.ToBoolean(dr["ReceiveNewsletter"]);


                    var customer = CustomerManager.GetCustomerByEmail(email);
                    if (customer == null)
                    {
                        //no customers found
                        customer = CustomerManager.AddCustomerForced(customerGuid, email, username,
                            passwordHash, saltKey, affiliateId, billingAddressId, shippingAddressId, lastPaymentMethodId,
                            lastAppliedCouponCode, string.Empty, string.Empty,
                            languageId, currencyId, (TaxDisplayTypeEnum)taxDisplayTypeId, isTaxExempt,
                            isAdmin, isGuest, isForumModerator, totalForumPosts, signature,
                            adminComment, active, deleted, registrationDate, timeZoneId, avatarId);
                    }
                    else
                    {
                        if (!customer.IsGuest)
                        {
                            //customer is not a guest
                            customer = CustomerManager.UpdateCustomer(customer.CustomerId, customer.CustomerGuid,
                                email, username, passwordHash, saltKey, affiliateId, billingAddressId,
                                shippingAddressId, lastPaymentMethodId, lastAppliedCouponCode,
                                string.Empty, string.Empty, languageId, currencyId,
                                (TaxDisplayTypeEnum)taxDisplayTypeId, isTaxExempt, isAdmin, isGuest,
                                isForumModerator, totalForumPosts, signature, adminComment,
                                active, deleted, registrationDate, timeZoneId, avatarId);
                        }
                        else
                        {
                            //customer is a guest
                            customer = CustomerManager.GetCustomerByGuid(customerGuid);
                            if (customer == null)
                            {
                                customer = CustomerManager.AddCustomerForced(customerGuid, email, username,
                                    passwordHash, saltKey, affiliateId, billingAddressId, shippingAddressId, lastPaymentMethodId,
                                    lastAppliedCouponCode, string.Empty,
                                    string.Empty, languageId, currencyId,
                                    (TaxDisplayTypeEnum)taxDisplayTypeId, isTaxExempt,
                                    isAdmin, isGuest, isForumModerator, totalForumPosts, signature,
                                    adminComment, active, deleted, registrationDate, timeZoneId, avatarId);
                            }
                            else
                            {
                                customer = CustomerManager.UpdateCustomer(customer.CustomerId, customer.CustomerGuid,
                                    email, username, passwordHash, saltKey, affiliateId, billingAddressId,
                                    shippingAddressId, lastPaymentMethodId, lastAppliedCouponCode,
                                    string.Empty, string.Empty, languageId, currencyId,
                                    (TaxDisplayTypeEnum)taxDisplayTypeId, isTaxExempt, isAdmin, isGuest,
                                    isForumModerator, totalForumPosts, signature, adminComment,
                                    active, deleted, registrationDate, timeZoneId, avatarId);
                            }
                        }
                    }

                    if (CustomerManager.FormFieldGenderEnabled)
                        customer.Gender = gender;
                    customer.FirstName = firstName;
                    customer.LastName = lastName;
                    if (CustomerManager.FormFieldCompanyEnabled)
                        customer.Company = company;
                    if (CustomerManager.FormFieldStreetAddressEnabled)
                        customer.StreetAddress = streetAddress;
                    if (CustomerManager.FormFieldStreetAddress2Enabled)
                        customer.StreetAddress2 = streetAddress2;
                    if (CustomerManager.FormFieldPostCodeEnabled)
                        customer.ZipPostalCode = zipPostalCode;
                    if (CustomerManager.FormFieldCityEnabled)
                        customer.City = city;
                    if (CustomerManager.FormFieldPhoneEnabled)
                        customer.PhoneNumber = phoneNumber;
                    if (CustomerManager.FormFieldFaxEnabled)
                        customer.FaxNumber = faxNumber;
                    if (CustomerManager.FormFieldCountryEnabled)
                        customer.CountryId = countryId;
                    if (CustomerManager.FormFieldStateEnabled)
                        customer.StateProvinceId = stateProvinceId;
                    customer.ReceiveNewsletter = receiveNewsletter;
                }
            }
        }

        /// <summary>
        /// Import products from XLS file
        /// </summary>
        /// <param name="filePath">Excel file path</param>
        public static void ImportProductsFromXls(string filePath)
        {
            using (ExcelHelper excelHelper = new ExcelHelper(filePath))
            {
                excelHelper.Hdr = "YES";
                excelHelper.Imex = "1";

                DataTable dt = excelHelper.ReadTable("Products");
                foreach (DataRow dr in dt.Rows)
                {
                    string Name = dr["Name"].ToString();
                    string ShortDescription = dr["ShortDescription"].ToString();
                    string FullDescription = dr["FullDescription"].ToString();
                    int TemplateId = Convert.ToInt32(dr["TemplateId"]);
                    bool ShowOnHomePage = Convert.ToBoolean(dr["ShowOnHomePage"]);
                    string MetaKeywords = dr["MetaKeywords"].ToString();
                    string MetaDescription = dr["MetaDescription"].ToString();
                    string MetaTitle = dr["MetaTitle"].ToString();
                    bool AllowCustomerReviews = Convert.ToBoolean(dr["AllowCustomerReviews"]);
                    bool AllowCustomerRatings = Convert.ToBoolean(dr["AllowCustomerRatings"]);
                    bool Published = Convert.ToBoolean(dr["Published"]);
                    string SKU = dr["SKU"].ToString();
                    string ManufacturerPartNumber = dr["ManufacturerPartNumber"].ToString();
                    bool IsGiftCard = Convert.ToBoolean(dr["IsGiftCard"]);
                    int GiftCardType = Convert.ToInt32(dr["GiftCardType"]);
                    bool IsDownload = Convert.ToBoolean(dr["IsDownload"]);
                    int DownloadId = Convert.ToInt32(dr["DownloadId"]);
                    bool UnlimitedDownloads = Convert.ToBoolean(dr["UnlimitedDownloads"]);
                    int MaxNumberOfDownloads = Convert.ToInt32(dr["MaxNumberOfDownloads"]);
                    bool HasSampleDownload = Convert.ToBoolean(dr["HasSampleDownload"]);
                    int DownloadActivationType = Convert.ToInt32(dr["DownloadActivationType"]);
                    int SampleDownloadId = Convert.ToInt32(dr["SampleDownloadId"]);
                    bool HasUserAgreement = Convert.ToBoolean(dr["HasUserAgreement"]);
                    string UserAgreementText = dr["UserAgreementText"].ToString();
                    bool IsRecurring = Convert.ToBoolean(dr["IsRecurring"]);
                    int CycleLength = Convert.ToInt32(dr["CycleLength"]);
                    int CyclePeriod = Convert.ToInt32(dr["CyclePeriod"]);
                    int TotalCycles = Convert.ToInt32(dr["TotalCycles"]);
                    bool IsShipEnabled = Convert.ToBoolean(dr["IsShipEnabled"]);
                    bool IsFreeShipping = Convert.ToBoolean(dr["IsFreeShipping"]);
                    decimal AdditionalShippingCharge = Convert.ToDecimal(dr["AdditionalShippingCharge"]);
                    bool IsTaxExempt = Convert.ToBoolean(dr["IsTaxExempt"]);
                    int TaxCategoryId = Convert.ToInt32(dr["TaxCategoryId"]);
                    int ManageInventory = Convert.ToInt32(dr["ManageInventory"]);
                    int StockQuantity = Convert.ToInt32(dr["StockQuantity"]);
                    bool DisplayStockAvailability = Convert.ToBoolean(dr["DisplayStockAvailability"]);
                    bool DisplayStockQuantity = Convert.ToBoolean(dr["DisplayStockQuantity"]);
                    int MinStockQuantity = Convert.ToInt32(dr["MinStockQuantity"]);
                    int LowStockActivityId = Convert.ToInt32(dr["LowStockActivityId"]);
                    int NotifyAdminForQuantityBelow = Convert.ToInt32(dr["NotifyAdminForQuantityBelow"]);
                    int Backorders = Convert.ToInt32(dr["Backorders"]);
                    int OrderMinimumQuantity = Convert.ToInt32(dr["OrderMinimumQuantity"]);
                    int OrderMaximumQuantity = Convert.ToInt32(dr["OrderMaximumQuantity"]);
                    bool DisableBuyButton = Convert.ToBoolean(dr["DisableBuyButton"]);
                    decimal Price = Convert.ToDecimal(dr["Price"]);
                    decimal OldPrice = Convert.ToDecimal(dr["OldPrice"]);
                    decimal ProductCost = Convert.ToDecimal(dr["ProductCost"]);
                    bool CustomerEntersPrice = Convert.ToBoolean(dr["CustomerEntersPrice"]);
                    decimal MinimumCustomerEnteredPrice = Convert.ToDecimal(dr["MinimumCustomerEnteredPrice"]);
                    decimal MaximumCustomerEnteredPrice = Convert.ToDecimal(dr["MaximumCustomerEnteredPrice"]);
                    decimal Weight = Convert.ToDecimal(dr["Weight"]);
                    decimal Length = Convert.ToDecimal(dr["Length"]);
                    decimal Width = Convert.ToDecimal(dr["Width"]);
                    decimal Height = Convert.ToDecimal(dr["Height"]);
                    DateTime CreatedOn = DateTime.FromOADate(Convert.ToDouble(dr["CreatedOn"]));
                    
                    var productVariant = ProductManager.GetProductVariantBySKU(SKU);
                    if (productVariant != null)
                    {
                        var product = ProductManager.GetProductById(productVariant.ProductId);
                        product = ProductManager.UpdateProduct(product.ProductId, Name, ShortDescription,
                            FullDescription, product.AdminComment,
                            TemplateId, ShowOnHomePage, MetaKeywords, MetaDescription,
                            MetaTitle, product.SEName, AllowCustomerReviews, AllowCustomerRatings,
                            product.RatingSum, product.TotalRatingVotes,
                            Published, product.Deleted, CreatedOn, DateTime.UtcNow);

                        productVariant = ProductManager.UpdateProductVariant(productVariant.ProductVariantId,
                            productVariant.ProductId, productVariant.Name, SKU,
                            productVariant.Description, productVariant.AdminComment,
                            ManufacturerPartNumber, IsGiftCard, GiftCardType, IsDownload, DownloadId,
                            UnlimitedDownloads, MaxNumberOfDownloads, productVariant.DownloadExpirationDays,
                            (DownloadActivationTypeEnum)DownloadActivationType, HasSampleDownload,
                            SampleDownloadId, HasUserAgreement, UserAgreementText, IsRecurring,
                            CycleLength, CyclePeriod, TotalCycles, IsShipEnabled,
                            IsFreeShipping, AdditionalShippingCharge, IsTaxExempt,
                            TaxCategoryId, ManageInventory, StockQuantity,
                            DisplayStockAvailability, DisplayStockQuantity, MinStockQuantity,
                            (LowStockActivityEnum)LowStockActivityId, NotifyAdminForQuantityBelow,
                            Backorders, OrderMinimumQuantity,
                            OrderMaximumQuantity, productVariant.WarehouseId, DisableBuyButton,
                            Price, OldPrice, ProductCost, CustomerEntersPrice, 
                            MinimumCustomerEnteredPrice, MaximumCustomerEnteredPrice,
                            Weight, Length, Width, Height,
                            productVariant.PictureId, productVariant.AvailableStartDateTime,
                            productVariant.AvailableEndDateTime, productVariant.Published,
                            productVariant.Deleted, productVariant.DisplayOrder, CreatedOn, DateTime.UtcNow);
                    }
                    else
                    {
                        var product = ProductManager.InsertProduct(Name, ShortDescription, FullDescription,
                            string.Empty, TemplateId, ShowOnHomePage, MetaKeywords, MetaDescription,
                            MetaTitle, string.Empty, AllowCustomerReviews, AllowCustomerRatings, 0, 0,
                            Published, false, CreatedOn, DateTime.UtcNow);

                        productVariant = ProductManager.InsertProductVariant(product.ProductId,
                            string.Empty, SKU, string.Empty, string.Empty, ManufacturerPartNumber,
                            IsGiftCard, GiftCardType, IsDownload, DownloadId,
                            UnlimitedDownloads, MaxNumberOfDownloads, null, (DownloadActivationTypeEnum)DownloadActivationType,
                            HasSampleDownload, SampleDownloadId, HasUserAgreement, UserAgreementText, IsRecurring, CycleLength, CyclePeriod, TotalCycles,
                            IsShipEnabled, IsFreeShipping, AdditionalShippingCharge, IsTaxExempt,
                            TaxCategoryId, ManageInventory, StockQuantity,
                            DisplayStockAvailability, DisplayStockQuantity, MinStockQuantity,
                            (LowStockActivityEnum)LowStockActivityId, NotifyAdminForQuantityBelow,
                            Backorders, OrderMinimumQuantity,
                            OrderMaximumQuantity, 0, DisableBuyButton,
                            Price, OldPrice, ProductCost, CustomerEntersPrice,
                            MinimumCustomerEnteredPrice, MaximumCustomerEnteredPrice, 
                            Weight, Length, Width, Height, 0, null, null,
                            true, false, 1, CreatedOn, DateTime.UtcNow);
                    }
                }
            }
        }
        #endregion
    }
}
