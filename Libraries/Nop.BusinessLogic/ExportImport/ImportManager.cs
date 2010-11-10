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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager
    {
        #region Methods

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
                    string vatNumber = dr["VatNumber"].ToString();
                    VatNumberStatusEnum vatNumberStatus = (VatNumberStatusEnum)Convert.ToInt32(dr["VatNumberStatus"]);
                    string streetAddress = dr["StreetAddress"].ToString();
                    string streetAddress2 = dr["StreetAddress2"].ToString();
                    string zipPostalCode = dr["ZipPostalCode"].ToString();
                    string city = dr["City"].ToString();
                    string phoneNumber = dr["PhoneNumber"].ToString();
                    string faxNumber = dr["FaxNumber"].ToString();
                    int countryId = Convert.ToInt32(dr["CountryId"]);
                    int stateProvinceId = Convert.ToInt32(dr["StateProvinceId"]);
                    bool receiveNewsletter = Convert.ToBoolean(dr["ReceiveNewsletter"]);


                    var customer = IoCFactory.Resolve<ICustomerService>().GetCustomerByEmail(email);
                    if (customer == null)
                    {
                        //no customers found
                        customer = IoCFactory.Resolve<ICustomerService>().AddCustomerForced(customerGuid, email, username,
                            passwordHash, saltKey, affiliateId, billingAddressId, shippingAddressId, lastPaymentMethodId,
                            lastAppliedCouponCode, string.Empty, string.Empty,
                            languageId, currencyId, (TaxDisplayTypeEnum)taxDisplayTypeId, isTaxExempt,
                            isAdmin, isGuest, isForumModerator, totalForumPosts, signature,
                            adminComment, active, deleted, registrationDate, timeZoneId, avatarId, null);
                    }
                    else
                    {
                        if (!customer.IsGuest)
                        {
                            //customer is not a guest
                            customer.Email = email;
                            customer.Username = username;
                            customer.PasswordHash = passwordHash;
                            customer.SaltKey = saltKey;
                            customer.AffiliateId = affiliateId;
                            customer.BillingAddressId = billingAddressId;
                            customer.ShippingAddressId = shippingAddressId;
                            customer.LastPaymentMethodId = lastPaymentMethodId;
                            customer.LastAppliedCouponCode = lastAppliedCouponCode;
                            customer.LanguageId = languageId;
                            customer.CurrencyId = currencyId;
                            customer.TaxDisplayTypeId = taxDisplayTypeId;
                            customer.IsTaxExempt = isTaxExempt;
                            customer.IsAdmin = isAdmin;
                            customer.IsGuest = isGuest;
                            customer.IsForumModerator = isForumModerator;
                            customer.TotalForumPosts= totalForumPosts;
                            customer.Signature = signature;
                            customer.AdminComment = adminComment;
                            customer.Active = active;
                            customer.Deleted = deleted;
                            customer.RegistrationDate = registrationDate;
                            customer.TimeZoneId = timeZoneId;
                            customer.AvatarId = avatarId;
                            IoCFactory.Resolve<ICustomerService>().UpdateCustomer(customer);
                        }
                        else
                        {
                            //customer is a guest
                            customer = IoCFactory.Resolve<ICustomerService>().GetCustomerByGuid(customerGuid);
                            if (customer == null)
                            {
                                customer = IoCFactory.Resolve<ICustomerService>().AddCustomerForced(customerGuid, email, username,
                                    passwordHash, saltKey, affiliateId, billingAddressId, shippingAddressId, lastPaymentMethodId,
                                    lastAppliedCouponCode, string.Empty,
                                    string.Empty, languageId, currencyId,
                                    (TaxDisplayTypeEnum)taxDisplayTypeId, isTaxExempt,
                                    isAdmin, isGuest, isForumModerator, totalForumPosts, signature,
                                    adminComment, active, deleted, registrationDate, timeZoneId, avatarId, null);
                            }
                            else
                            {
                                customer.Email = email;
                                customer.Username = username;
                                customer.PasswordHash = passwordHash;
                                customer.SaltKey = saltKey;
                                customer.AffiliateId = affiliateId;
                                customer.BillingAddressId = billingAddressId;
                                customer.ShippingAddressId = shippingAddressId;
                                customer.LastPaymentMethodId = lastPaymentMethodId;
                                customer.LastAppliedCouponCode = lastAppliedCouponCode;
                                customer.LanguageId = languageId;
                                customer.CurrencyId = currencyId;
                                customer.TaxDisplayTypeId = taxDisplayTypeId;
                                customer.IsTaxExempt = isTaxExempt;
                                customer.IsAdmin = isAdmin;
                                customer.IsGuest = isGuest;
                                customer.IsForumModerator = isForumModerator;
                                customer.TotalForumPosts = totalForumPosts;
                                customer.Signature = signature;
                                customer.AdminComment = adminComment;
                                customer.Active = active;
                                customer.Deleted = deleted;
                                customer.RegistrationDate = registrationDate;
                                customer.TimeZoneId = timeZoneId;
                                customer.AvatarId = avatarId;

                                IoCFactory.Resolve<ICustomerService>().UpdateCustomer(customer);
                            }
                        }
                    }

                    if (IoCFactory.Resolve<ICustomerService>().FormFieldGenderEnabled)
                        customer.Gender = gender;
                    customer.FirstName = firstName;
                    customer.LastName = lastName;
                    if (IoCFactory.Resolve<ICustomerService>().FormFieldCompanyEnabled)
                        customer.Company = company;
                    if (IoCFactory.Resolve<ICustomerService>().FormFieldStreetAddressEnabled)
                        customer.StreetAddress = streetAddress;
                    if (IoCFactory.Resolve<ICustomerService>().FormFieldStreetAddress2Enabled)
                        customer.StreetAddress2 = streetAddress2;
                    if (IoCFactory.Resolve<ICustomerService>().FormFieldPostCodeEnabled)
                        customer.ZipPostalCode = zipPostalCode;
                    if (IoCFactory.Resolve<ICustomerService>().FormFieldCityEnabled)
                        customer.City = city;
                    if (IoCFactory.Resolve<ICustomerService>().FormFieldPhoneEnabled)
                        customer.PhoneNumber = phoneNumber;
                    if (IoCFactory.Resolve<ICustomerService>().FormFieldFaxEnabled)
                        customer.FaxNumber = faxNumber;
                    if (IoCFactory.Resolve<ICustomerService>().FormFieldCountryEnabled)
                        customer.CountryId = countryId;
                    if (IoCFactory.Resolve<ICustomerService>().FormFieldStateEnabled)
                        customer.StateProvinceId = stateProvinceId;
                    if (IoCFactory.Resolve<ICustomerService>().FormFieldNewsletterEnabled)
                        customer.ReceiveNewsletter = receiveNewsletter;

                    if (IoCFactory.Resolve<ITaxService>().EUVatEnabled)
                    {
                        customer.VatNumber = vatNumber;
                        customer.VatNumberStatus = vatNumberStatus;
                    }
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
                    bool CallForPrice = Convert.ToBoolean(dr["CallForPrice"]);
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
                    
                    var productVariant = IoCFactory.Resolve<IProductService>().GetProductVariantBySKU(SKU);
                    if (productVariant != null)
                    {
                        var product = IoCFactory.Resolve<IProductService>().GetProductById(productVariant.ProductId);
                        product.Name = Name;
                        product.ShortDescription = ShortDescription;
                        product.FullDescription = FullDescription;
                        product.TemplateId = TemplateId;
                        product.ShowOnHomePage = ShowOnHomePage;
                        product.MetaKeywords = MetaKeywords;
                        product.MetaDescription = MetaDescription;
                        product.MetaTitle = MetaTitle;
                        product.AllowCustomerReviews = AllowCustomerReviews;
                        product.AllowCustomerRatings = AllowCustomerRatings;
                        product.Published = Published;
                        product.CreatedOn = CreatedOn;
                        product.UpdatedOn = DateTime.UtcNow;

                        IoCFactory.Resolve<IProductService>().UpdateProduct(product);
                        
                        productVariant.SKU = SKU;
                        productVariant.ManufacturerPartNumber = ManufacturerPartNumber;
                        productVariant.IsGiftCard = IsGiftCard;
                        productVariant.GiftCardType = GiftCardType;
                        productVariant.IsDownload = IsDownload;
                        productVariant.DownloadId = DownloadId;
                        productVariant.UnlimitedDownloads = UnlimitedDownloads;
                        productVariant.MaxNumberOfDownloads = MaxNumberOfDownloads;
                        productVariant.DownloadActivationType = DownloadActivationType;
                        productVariant.HasSampleDownload = HasSampleDownload;
                        productVariant.SampleDownloadId = SampleDownloadId;
                        productVariant.HasUserAgreement = HasUserAgreement;
                        productVariant.UserAgreementText = UserAgreementText;
                        productVariant.IsRecurring = IsRecurring;
                        productVariant.CycleLength = CycleLength;
                        productVariant.CyclePeriod = CyclePeriod;
                        productVariant.TotalCycles = TotalCycles;
                        productVariant.IsShipEnabled = IsShipEnabled;
                        productVariant.IsFreeShipping = IsFreeShipping;
                        productVariant.AdditionalShippingCharge = AdditionalShippingCharge;
                        productVariant.IsTaxExempt = IsTaxExempt;
                        productVariant.TaxCategoryId = TaxCategoryId;
                        productVariant.ManageInventory = ManageInventory;
                        productVariant.StockQuantity = StockQuantity;
                        productVariant.DisplayStockAvailability = DisplayStockAvailability;
                        productVariant.DisplayStockQuantity = DisplayStockQuantity;
                        productVariant.MinStockQuantity = MinStockQuantity;
                        productVariant.LowStockActivityId = LowStockActivityId;
                        productVariant.NotifyAdminForQuantityBelow = NotifyAdminForQuantityBelow;
                        productVariant.Backorders = Backorders;
                        productVariant.OrderMinimumQuantity = OrderMinimumQuantity;
                        productVariant.OrderMaximumQuantity = OrderMaximumQuantity;
                        productVariant.DisableBuyButton = DisableBuyButton;
                        productVariant.CallForPrice = CallForPrice;
                        productVariant.Price = Price;
                        productVariant.OldPrice = OldPrice;
                        productVariant.ProductCost = ProductCost;
                        productVariant.CustomerEntersPrice = CustomerEntersPrice;
                        productVariant.MinimumCustomerEnteredPrice = MinimumCustomerEnteredPrice;
                        productVariant.MaximumCustomerEnteredPrice = MaximumCustomerEnteredPrice;
                        productVariant.Weight = Weight;
                        productVariant.Length = Length;
                        productVariant.Width = Width;
                        productVariant.Height = Height;
                        productVariant.Published = Published;
                        productVariant.CreatedOn = CreatedOn;
                        productVariant.UpdatedOn = DateTime.UtcNow;

                        IoCFactory.Resolve<IProductService>().UpdateProductVariant(productVariant);
                    }
                    else
                    {
                        var product = new Product()
                        {
                            Name = Name,
                            ShortDescription = ShortDescription,
                            FullDescription = FullDescription,
                            TemplateId = TemplateId,
                            ShowOnHomePage = ShowOnHomePage,
                            MetaKeywords = MetaKeywords,
                            MetaDescription = MetaDescription,
                            MetaTitle = MetaTitle,
                            AllowCustomerReviews = AllowCustomerReviews,
                            AllowCustomerRatings = AllowCustomerRatings,
                            Published = Published,
                            CreatedOn = CreatedOn,
                            UpdatedOn = DateTime.UtcNow
                        };
                        IoCFactory.Resolve<IProductService>().InsertProduct(product);

                        productVariant = new ProductVariant()
                        {
                            ProductId = product.ProductId,
                            SKU = SKU,
                            ManufacturerPartNumber = ManufacturerPartNumber,
                            IsGiftCard = IsGiftCard,
                            GiftCardType = GiftCardType,
                            IsDownload = IsDownload,
                            DownloadId = DownloadId,
                            UnlimitedDownloads = UnlimitedDownloads,
                            MaxNumberOfDownloads = MaxNumberOfDownloads,
                            DownloadActivationType = DownloadActivationType,
                            HasSampleDownload = HasSampleDownload,
                            SampleDownloadId = SampleDownloadId,
                            HasUserAgreement = HasUserAgreement,
                            UserAgreementText = UserAgreementText,
                            IsRecurring = IsRecurring,
                            CycleLength = CycleLength,
                            CyclePeriod = CyclePeriod,
                            TotalCycles = TotalCycles,
                            IsShipEnabled = IsShipEnabled,
                            IsFreeShipping = IsFreeShipping,
                            AdditionalShippingCharge = AdditionalShippingCharge,
                            IsTaxExempt = IsTaxExempt,
                            TaxCategoryId = TaxCategoryId,
                            ManageInventory = ManageInventory,
                            StockQuantity = StockQuantity,
                            DisplayStockAvailability = DisplayStockAvailability,
                            DisplayStockQuantity = DisplayStockQuantity,
                            MinStockQuantity = MinStockQuantity,
                            LowStockActivityId = LowStockActivityId,
                            NotifyAdminForQuantityBelow = NotifyAdminForQuantityBelow,
                            Backorders = Backorders,
                            OrderMinimumQuantity = OrderMinimumQuantity,
                            OrderMaximumQuantity = OrderMaximumQuantity,
                            DisableBuyButton = DisableBuyButton,
                            CallForPrice = CallForPrice,
                            Price = Price,
                            OldPrice = OldPrice,
                            ProductCost = ProductCost,
                            CustomerEntersPrice = CustomerEntersPrice,
                            MinimumCustomerEnteredPrice = MinimumCustomerEnteredPrice,
                            MaximumCustomerEnteredPrice = MaximumCustomerEnteredPrice,
                            Weight = Weight,
                            Length = Length,
                            Width = Width,
                            Height = Height,
                            Published = Published,
                            CreatedOn = CreatedOn,
                            UpdatedOn = DateTime.UtcNow
                        };

                        IoCFactory.Resolve<IProductService>().InsertProductVariant(productVariant);
                    }
                }
            }
        }
        #endregion
    }
}
