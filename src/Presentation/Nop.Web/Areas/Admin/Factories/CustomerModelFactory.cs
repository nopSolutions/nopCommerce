using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the customer model factory implementation
    /// </summary>
    public partial class CustomerModelFactory : ICustomerModelFactory
    {
        #region Fields

        protected AddressSettings AddressSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected DateTimeSettings DateTimeSettings { get; }
        protected GdprSettings GdprSettings { get; }
        protected ForumSettings ForumSettings { get; }
        protected IAclSupportedModelFactory AclSupportedModelFactory { get; }
        protected IAddressAttributeFormatter AddressAttributeFormatter { get; }
        protected IAddressModelFactory AddressModelFactory { get; }
        protected IAffiliateService AffiliateService { get; }
        protected IAuthenticationPluginManager AuthenticationPluginManager { get; }
        protected IBackInStockSubscriptionService BackInStockSubscriptionService { get; }
        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }
        protected ICountryService CountryService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerAttributeParser CustomerAttributeParser { get; }
        protected ICustomerAttributeService CustomerAttributeService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IExternalAuthenticationService ExternalAuthenticationService { get; }
        protected IGdprService GdprService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IGeoLookupService GeoLookupService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INewsLetterSubscriptionService NewsLetterSubscriptionService { get; }
        protected IOrderService OrderService { get; }
        protected IPictureService PictureService { get; }
        protected IPriceFormatter PriceFormatter { get; }
        protected IProductAttributeFormatter ProductAttributeFormatter { get; }
        protected IProductService ProductService { get; }
        protected IRewardPointService RewardPointService { get; }
        protected IShoppingCartService ShoppingCartService { get; }
        protected IStateProvinceService StateProvinceService { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreService StoreService { get; }
        protected ITaxService TaxService { get; }
        protected MediaSettings MediaSettings { get; }
        protected RewardPointsSettings RewardPointsSettings { get; }
        protected TaxSettings TaxSettings { get; }

        #endregion

        #region Ctor

        public CustomerModelFactory(AddressSettings addressSettings,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            GdprSettings gdprSettings,
            ForumSettings forumSettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressModelFactory addressModelFactory,
            IAffiliateService affiliateService,
            IAuthenticationPluginManager authenticationPluginManager,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            ICustomerActivityService customerActivityService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            IGeoLookupService geoLookupService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IRewardPointService rewardPointService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            ITaxService taxService,
            MediaSettings mediaSettings,
            RewardPointsSettings rewardPointsSettings,
            TaxSettings taxSettings)
        {
            AddressSettings = addressSettings;
            CustomerSettings = customerSettings;
            DateTimeSettings = dateTimeSettings;
            GdprSettings = gdprSettings;
            ForumSettings = forumSettings;
            AclSupportedModelFactory = aclSupportedModelFactory;
            AddressAttributeFormatter = addressAttributeFormatter;
            AddressModelFactory = addressModelFactory;
            AffiliateService = affiliateService;
            AuthenticationPluginManager = authenticationPluginManager;
            BackInStockSubscriptionService = backInStockSubscriptionService;
            BaseAdminModelFactory = baseAdminModelFactory;
            CountryService = countryService;
            CustomerActivityService = customerActivityService;
            CustomerAttributeParser = customerAttributeParser;
            CustomerAttributeService = customerAttributeService;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            ExternalAuthenticationService = externalAuthenticationService;
            GdprService = gdprService;
            GenericAttributeService = genericAttributeService;
            GeoLookupService = geoLookupService;
            LocalizationService = localizationService;
            NewsLetterSubscriptionService = newsLetterSubscriptionService;
            OrderService = orderService;
            PictureService = pictureService;
            PriceFormatter = priceFormatter;
            ProductAttributeFormatter = productAttributeFormatter;
            ProductService = productService;
            RewardPointService = rewardPointService;
            ShoppingCartService = shoppingCartService;
            StateProvinceService = stateProvinceService;
            StoreContext = storeContext;
            StoreService = storeService;
            TaxService = taxService;
            MediaSettings = mediaSettings;
            RewardPointsSettings = rewardPointsSettings;
            TaxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the reward points model to add to the customer
        /// </summary>
        /// <param name="model">Reward points model to add to the customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareAddRewardPointsToCustomerModelAsync(AddRewardPointsToCustomerModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var store = await StoreContext.GetCurrentStoreAsync();

            model.Message = string.Empty;
            model.ActivatePointsImmediately = true;
            model.StoreId = store.Id;

            //prepare available stores
            await BaseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
        }

        /// <summary>
        /// Prepare customer associated external authorization models
        /// </summary>
        /// <param name="models">List of customer associated external authorization models</param>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareAssociatedExternalAuthModelsAsync(IList<CustomerAssociatedExternalAuthModel> models, Customer customer)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            foreach (var record in await ExternalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(customer))
            {
                var method = await AuthenticationPluginManager.LoadPluginBySystemNameAsync(record.ProviderSystemName);
                if (method == null)
                    continue;

                models.Add(new CustomerAssociatedExternalAuthModel
                {
                    Id = record.Id,
                    Email = record.Email,
                    ExternalIdentifier = !string.IsNullOrEmpty(record.ExternalDisplayIdentifier)
                        ? record.ExternalDisplayIdentifier : record.ExternalIdentifier,
                    AuthMethodName = method.PluginDescriptor.FriendlyName
                });
            }
        }

        /// <summary>
        /// Prepare customer attribute models
        /// </summary>
        /// <param name="models">List of customer attribute models</param>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareCustomerAttributeModelsAsync(IList<CustomerModel.CustomerAttributeModel> models, Customer customer)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //get available customer attributes
            var customerAttributes = await CustomerAttributeService.GetAllCustomerAttributesAsync();
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerModel.CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await CustomerAttributeService.GetCustomerAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new CustomerModel.CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                if (customer != null)
                {
                    var selectedCustomerAttributes = await GenericAttributeService
                        .GetAttributeAsync<string>(customer, NopCustomerDefaults.CustomCustomerAttributes);
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                            {
                                if (!string.IsNullOrEmpty(selectedCustomerAttributes))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = await CustomerAttributeParser.ParseCustomerAttributeValuesAsync(selectedCustomerAttributes);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                                item.IsPreSelected = true;
                                }
                            }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                            {
                                //do nothing
                                //values are already pre-set
                            }
                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                            {
                                if (!string.IsNullOrEmpty(selectedCustomerAttributes))
                                {
                                    var enteredText = CustomerAttributeParser.ParseValues(selectedCustomerAttributes, attribute.Id);
                                    if (enteredText.Any())
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }
                            break;
                        case AttributeControlType.Datepicker:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                        case AttributeControlType.FileUpload:
                        default:
                            //not supported attribute control types
                            break;
                    }
                }

                models.Add(attributeModel);
            }
        }

        /// <summary>
        /// Prepare HTML string address
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareModelAddressHtmlAsync(AddressModel model, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var addressHtmlSb = new StringBuilder("<div>");

            if (AddressSettings.CompanyEnabled && !string.IsNullOrEmpty(model.Company))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Company));

            if (AddressSettings.StreetAddressEnabled && !string.IsNullOrEmpty(model.Address1))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Address1));

            if (AddressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(model.Address2))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Address2));

            if (AddressSettings.CityEnabled && !string.IsNullOrEmpty(model.City))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.City));

            if (AddressSettings.CountyEnabled && !string.IsNullOrEmpty(model.County))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.County));

            if (AddressSettings.StateProvinceEnabled && !string.IsNullOrEmpty(model.StateProvinceName))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.StateProvinceName));

            if (AddressSettings.ZipPostalCodeEnabled && !string.IsNullOrEmpty(model.ZipPostalCode))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.ZipPostalCode));

            if (AddressSettings.CountryEnabled && !string.IsNullOrEmpty(model.CountryName))
                addressHtmlSb.AppendFormat("{0}", WebUtility.HtmlEncode(model.CountryName));

            var customAttributesFormatted = await AddressAttributeFormatter.FormatAttributesAsync(address?.CustomAttributes);
            if (!string.IsNullOrEmpty(customAttributesFormatted))
            {
                //already encoded
                addressHtmlSb.AppendFormat("<br />{0}", customAttributesFormatted);
            }

            addressHtmlSb.Append("</div>");

            model.AddressHtml = addressHtmlSb.ToString();
        }

        /// <summary>
        /// Prepare reward points search model
        /// </summary>
        /// <param name="searchModel">Reward points search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Reward points search model</returns>
        protected virtual CustomerRewardPointsSearchModel PrepareRewardPointsSearchModel(CustomerRewardPointsSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer address search model
        /// </summary>
        /// <param name="searchModel">Customer address search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer address search model</returns>
        protected virtual CustomerAddressSearchModel PrepareCustomerAddressSearchModel(CustomerAddressSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer order search model
        /// </summary>
        /// <param name="searchModel">Customer order search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer order search model</returns>
        protected virtual CustomerOrderSearchModel PrepareCustomerOrderSearchModel(CustomerOrderSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer shopping cart search model
        /// </summary>
        /// <param name="searchModel">Customer shopping cart search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer shopping cart search model
        /// </returns>
        protected virtual async Task<CustomerShoppingCartSearchModel> PrepareCustomerShoppingCartSearchModelAsync(CustomerShoppingCartSearchModel searchModel,
            Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare available shopping cart types (search shopping cart by default)
            searchModel.ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart;
            await BaseAdminModelFactory.PrepareShoppingCartTypesAsync(searchModel.AvailableShoppingCartTypes, false);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer activity log search model
        /// </summary>
        /// <param name="searchModel">Customer activity log search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer activity log search model</returns>
        protected virtual CustomerActivityLogSearchModel PrepareCustomerActivityLogSearchModel(CustomerActivityLogSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer back in stock subscriptions search model
        /// </summary>
        /// <param name="searchModel">Customer back in stock subscriptions search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer back in stock subscriptions search model</returns>
        protected virtual CustomerBackInStockSubscriptionSearchModel PrepareCustomerBackInStockSubscriptionSearchModel(
            CustomerBackInStockSubscriptionSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer back in stock subscriptions search model
        /// </summary>
        /// <param name="searchModel">Customer back in stock subscriptions search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer back in stock subscriptions search model
        /// </returns>
        protected virtual async Task<CustomerAssociatedExternalAuthRecordsSearchModel> PrepareCustomerAssociatedExternalAuthRecordsSearchModelAsync(
            CustomerAssociatedExternalAuthRecordsSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();
            //prepare external authentication records
            await PrepareAssociatedExternalAuthModelsAsync(searchModel.AssociatedExternalAuthRecords, customer);

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare customer search model
        /// </summary>
        /// <param name="searchModel">Customer search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer search model
        /// </returns>
        public virtual async Task<CustomerSearchModel> PrepareCustomerSearchModelAsync(CustomerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.UsernamesEnabled = CustomerSettings.UsernamesEnabled;
            searchModel.AvatarEnabled = CustomerSettings.AllowCustomersToUploadAvatars;
            searchModel.FirstNameEnabled = CustomerSettings.FirstNameEnabled;
            searchModel.LastNameEnabled = CustomerSettings.LastNameEnabled;
            searchModel.DateOfBirthEnabled = CustomerSettings.DateOfBirthEnabled;
            searchModel.CompanyEnabled = CustomerSettings.CompanyEnabled;
            searchModel.PhoneEnabled = CustomerSettings.PhoneEnabled;
            searchModel.ZipPostalCodeEnabled = CustomerSettings.ZipPostalCodeEnabled;

            //search registered customers by default
            var registeredRole = await CustomerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
            if (registeredRole != null)
                searchModel.SelectedCustomerRoleIds.Add(registeredRole.Id);

            //prepare available customer roles
            await AclSupportedModelFactory.PrepareModelCustomerRolesAsync(searchModel);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged customer list model
        /// </summary>
        /// <param name="searchModel">Customer search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer list model
        /// </returns>
        public virtual async Task<CustomerListModel> PrepareCustomerListModelAsync(CustomerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter customers
            _ = int.TryParse(searchModel.SearchDayOfBirth, out var dayOfBirth);
            _ = int.TryParse(searchModel.SearchMonthOfBirth, out var monthOfBirth);

            //get customers
            var customers = await CustomerService.GetAllCustomersAsync(customerRoleIds: searchModel.SelectedCustomerRoleIds.ToArray(),
                email: searchModel.SearchEmail,
                username: searchModel.SearchUsername,
                firstName: searchModel.SearchFirstName,
                lastName: searchModel.SearchLastName,
                dayOfBirth: dayOfBirth,
                monthOfBirth: monthOfBirth,
                company: searchModel.SearchCompany,
                phone: searchModel.SearchPhone,
                zipPostalCode: searchModel.SearchZipPostalCode,
                ipAddress: searchModel.SearchIpAddress,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new CustomerListModel().PrepareToGridAsync(searchModel, customers, () =>
            {
                return customers.SelectAwait(async customer =>
                {
                    //fill in model values from the entity
                    var customerModel = customer.ToModel<CustomerModel>();

                    //convert dates to the user time
                    customerModel.Email = (await CustomerService.IsRegisteredAsync(customer))
                        ? customer.Email
                        : await LocalizationService.GetResourceAsync("Admin.Customers.Guest");
                    customerModel.FullName = await CustomerService.GetCustomerFullNameAsync(customer);
                    customerModel.Company = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CompanyAttribute);
                    customerModel.Phone = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute);
                    customerModel.ZipPostalCode = await GenericAttributeService
                        .GetAttributeAsync<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute);

                    customerModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc);
                    customerModel.LastActivityDate = await DateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    customerModel.CustomerRoleNames = string.Join(", ",
                        (await CustomerService.GetCustomerRolesAsync(customer)).Select(role => role.Name));
                    if (CustomerSettings.AllowCustomersToUploadAvatars)
                    {
                        var avatarPictureId = await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute);
                        customerModel.AvatarUrl = await PictureService
                            .GetPictureUrlAsync(avatarPictureId, MediaSettings.AvatarPictureSize, CustomerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                    }

                    return customerModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare customer model
        /// </summary>
        /// <param name="model">Customer model</param>
        /// <param name="customer">Customer</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer model
        /// </returns>
        public virtual async Task<CustomerModel> PrepareCustomerModelAsync(CustomerModel model, Customer customer, bool excludeProperties = false)
        {
            if (customer != null)
            {
                //fill in model values from the entity
                model ??= new CustomerModel();

                model.Id = customer.Id;
                model.DisplayVatNumber = TaxSettings.EuVatEnabled;
                model.AllowSendingOfPrivateMessage = await CustomerService.IsRegisteredAsync(customer) &&
                    ForumSettings.AllowPrivateMessages;
                model.AllowSendingOfWelcomeMessage = await CustomerService.IsRegisteredAsync(customer) &&
                    CustomerSettings.UserRegistrationType == UserRegistrationType.AdminApproval;
                model.AllowReSendingOfActivationMessage = await CustomerService.IsRegisteredAsync(customer) && !customer.Active &&
                    CustomerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                model.GdprEnabled = GdprSettings.GdprEnabled;

                model.MultiFactorAuthenticationProvider = await GenericAttributeService
                    .GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);

                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    model.Email = customer.Email;
                    model.Username = customer.Username;
                    model.VendorId = customer.VendorId;
                    model.AdminComment = customer.AdminComment;
                    model.IsTaxExempt = customer.IsTaxExempt;
                    model.Active = customer.Active;
                    model.FirstName = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute);
                    model.LastName = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastNameAttribute);
                    model.Gender = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.GenderAttribute);
                    model.DateOfBirth = await GenericAttributeService.GetAttributeAsync<DateTime?>(customer, NopCustomerDefaults.DateOfBirthAttribute);
                    model.Company = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CompanyAttribute);
                    model.StreetAddress = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.StreetAddressAttribute);
                    model.StreetAddress2 = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.StreetAddress2Attribute);
                    model.ZipPostalCode = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute);
                    model.City = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CityAttribute);
                    model.County = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CountyAttribute);
                    model.CountryId = await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.CountryIdAttribute);
                    model.StateProvinceId = await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute);
                    model.Phone = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute);
                    model.Fax = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FaxAttribute);
                    model.TimeZoneId = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.TimeZoneIdAttribute);
                    model.VatNumber = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.VatNumberAttribute);
                    model.VatNumberStatusNote = await LocalizationService.GetLocalizedEnumAsync((VatNumberStatus)await GenericAttributeService
                        .GetAttributeAsync<int>(customer, NopCustomerDefaults.VatNumberStatusIdAttribute));
                    model.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc);
                    model.LastActivityDate = await DateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);
                    model.LastIpAddress = customer.LastIpAddress;
                    model.LastVisitedPage = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastVisitedPageAttribute);
                    model.SelectedCustomerRoleIds = (await CustomerService.GetCustomerRoleIdsAsync(customer)).ToList();
                    model.RegisteredInStore = (await StoreService.GetAllStoresAsync())
                        .FirstOrDefault(store => store.Id == customer.RegisteredInStoreId)?.Name ?? string.Empty;
                    model.DisplayRegisteredInStore = model.Id > 0 && !string.IsNullOrEmpty(model.RegisteredInStore) &&
                        (await StoreService.GetAllStoresAsync()).Select(x => x.Id).Count() > 1;

                    //prepare model affiliate
                    var affiliate = await AffiliateService.GetAffiliateByIdAsync(customer.AffiliateId);
                    if (affiliate != null)
                    {
                        model.AffiliateId = affiliate.Id;
                        model.AffiliateName = await AffiliateService.GetAffiliateFullNameAsync(affiliate);
                    }

                    //prepare model newsletter subscriptions
                    if (!string.IsNullOrEmpty(customer.Email))
                    {
                        model.SelectedNewsletterSubscriptionStoreIds = await (await StoreService.GetAllStoresAsync())
                            .WhereAwait(async store => await NewsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id) != null)
                            .Select(store => store.Id).ToListAsync();
                    }
                }
                //prepare reward points model
                model.DisplayRewardPointsHistory = RewardPointsSettings.Enabled;
                if (model.DisplayRewardPointsHistory)
                    await PrepareAddRewardPointsToCustomerModelAsync(model.AddRewardPoints);

                //prepare nested search models
                PrepareRewardPointsSearchModel(model.CustomerRewardPointsSearchModel, customer);
                PrepareCustomerAddressSearchModel(model.CustomerAddressSearchModel, customer);
                PrepareCustomerOrderSearchModel(model.CustomerOrderSearchModel, customer);
                await PrepareCustomerShoppingCartSearchModelAsync(model.CustomerShoppingCartSearchModel, customer);
                PrepareCustomerActivityLogSearchModel(model.CustomerActivityLogSearchModel, customer);
                PrepareCustomerBackInStockSubscriptionSearchModel(model.CustomerBackInStockSubscriptionSearchModel, customer);
                await PrepareCustomerAssociatedExternalAuthRecordsSearchModelAsync(model.CustomerAssociatedExternalAuthRecordsSearchModel, customer);
            }
            else
            {
                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    //precheck Registered Role as a default role while creating a new customer through admin
                    var registeredRole = await CustomerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
                    if (registeredRole != null)
                        model.SelectedCustomerRoleIds.Add(registeredRole.Id);
                }
            }

            model.UsernamesEnabled = CustomerSettings.UsernamesEnabled;
            model.AllowCustomersToSetTimeZone = DateTimeSettings.AllowCustomersToSetTimeZone;
            model.FirstNameEnabled = CustomerSettings.FirstNameEnabled;
            model.LastNameEnabled = CustomerSettings.LastNameEnabled;
            model.GenderEnabled = CustomerSettings.GenderEnabled;
            model.DateOfBirthEnabled = CustomerSettings.DateOfBirthEnabled;
            model.CompanyEnabled = CustomerSettings.CompanyEnabled;
            model.StreetAddressEnabled = CustomerSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = CustomerSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = CustomerSettings.ZipPostalCodeEnabled;
            model.CityEnabled = CustomerSettings.CityEnabled;
            model.CountyEnabled = CustomerSettings.CountyEnabled;
            model.CountryEnabled = CustomerSettings.CountryEnabled;
            model.StateProvinceEnabled = CustomerSettings.StateProvinceEnabled;
            model.PhoneEnabled = CustomerSettings.PhoneEnabled;
            model.FaxEnabled = CustomerSettings.FaxEnabled;

            //set default values for the new model
            if (customer == null)
            {
                model.Active = true;
                model.DisplayVatNumber = false;
            }

            //prepare available vendors
            await BaseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors,
                defaultItemText: await LocalizationService.GetResourceAsync("Admin.Customers.Customers.Fields.Vendor.None"));

            //prepare model customer attributes
            await PrepareCustomerAttributeModelsAsync(model.CustomerAttributes, customer);

            //prepare model stores for newsletter subscriptions
            model.AvailableNewsletterSubscriptionStores = (await StoreService.GetAllStoresAsync()).Select(store => new SelectListItem
            {
                Value = store.Id.ToString(),
                Text = store.Name,
                Selected = model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id)
            }).ToList();

            //prepare model customer roles
            await AclSupportedModelFactory.PrepareModelCustomerRolesAsync(model);

            //prepare available time zones
            await BaseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

            //prepare available countries and states
            if (CustomerSettings.CountryEnabled)
            {
                await BaseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);
                if (CustomerSettings.StateProvinceEnabled)
                    await BaseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId == 0 ? null : (int?)model.CountryId);
            }

            return model;
        }

        /// <summary>
        /// Prepare paged reward points list model
        /// </summary>
        /// <param name="searchModel">Reward points search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the reward points list model
        /// </returns>
        public virtual async Task<CustomerRewardPointsListModel> PrepareRewardPointsListModelAsync(CustomerRewardPointsSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get reward points history
            var rewardPoints = await RewardPointService.GetRewardPointsHistoryAsync(customer.Id,
                showNotActivated: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new CustomerRewardPointsListModel().PrepareToGridAsync(searchModel, rewardPoints, () =>
            {
                return rewardPoints.SelectAwait(async historyEntry =>
                {
                    //fill in model values from the entity        
                    var rewardPointsHistoryModel = historyEntry.ToModel<CustomerRewardPointsModel>();

                    //convert dates to the user time
                    var activatingDate = await DateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);
                    rewardPointsHistoryModel.CreatedOn = activatingDate;

                    rewardPointsHistoryModel.PointsBalance = historyEntry.PointsBalance.HasValue
                        ? historyEntry.PointsBalance.ToString()
                        : string.Format((await LocalizationService.GetResourceAsync("Admin.Customers.Customers.RewardPoints.ActivatedLater")), activatingDate);
                    rewardPointsHistoryModel.EndDate = !historyEntry.EndDateUtc.HasValue
                        ? null
                        : (DateTime?)(await DateTimeHelper.ConvertToUserTimeAsync(historyEntry.EndDateUtc.Value, DateTimeKind.Utc));

                    //fill in additional values (not existing in the entity)
                    rewardPointsHistoryModel.StoreName = (await StoreService.GetStoreByIdAsync(historyEntry.StoreId))?.Name ?? "Unknown";

                    return rewardPointsHistoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged customer address list model
        /// </summary>
        /// <param name="searchModel">Customer address search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer address list model
        /// </returns>
        public virtual async Task<CustomerAddressListModel> PrepareCustomerAddressListModelAsync(CustomerAddressSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get customer addresses
            var addresses = (await CustomerService.GetAddressesByCustomerIdAsync(customer.Id))
                .OrderByDescending(address => address.CreatedOnUtc).ThenByDescending(address => address.Id).ToList()
                .ToPagedList(searchModel);

            //prepare list model
            var model = await new CustomerAddressListModel().PrepareToGridAsync(searchModel, addresses, () =>
            {
                return addresses.SelectAwait(async address =>
                {
                    //fill in model values from the entity        
                    var addressModel = address.ToModel<AddressModel>();

                    addressModel.CountryName = (await CountryService.GetCountryByAddressAsync(address))?.Name;
                    addressModel.StateProvinceName = (await StateProvinceService.GetStateProvinceByAddressAsync(address))?.Name;

                    //fill in additional values (not existing in the entity)
                    await PrepareModelAddressHtmlAsync(addressModel, address);

                    return addressModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare customer address model
        /// </summary>
        /// <param name="model">Customer address model</param>
        /// <param name="customer">Customer</param>
        /// <param name="address">Address</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer address model
        /// </returns>
        public virtual async Task<CustomerAddressModel> PrepareCustomerAddressModelAsync(CustomerAddressModel model,
            Customer customer, Address address, bool excludeProperties = false)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (address != null)
            {
                //fill in model values from the entity
                model ??= new CustomerAddressModel();

                //whether to fill in some of properties
                if (!excludeProperties)
                    model.Address = address.ToModel(model.Address);
            }

            model.CustomerId = customer.Id;

            //prepare address model
            await AddressModelFactory.PrepareAddressModelAsync(model.Address, address);
            model.Address.FirstNameRequired = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailRequired = true;
            model.Address.CompanyRequired = AddressSettings.CompanyRequired;
            model.Address.CityRequired = AddressSettings.CityRequired;
            model.Address.CountyRequired = AddressSettings.CountyRequired;
            model.Address.StreetAddressRequired = AddressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Required = AddressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeRequired = AddressSettings.ZipPostalCodeRequired;
            model.Address.PhoneRequired = AddressSettings.PhoneRequired;
            model.Address.FaxRequired = AddressSettings.FaxRequired;

            return model;
        }

        /// <summary>
        /// Prepare paged customer order list model
        /// </summary>
        /// <param name="searchModel">Customer order search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer order list model
        /// </returns>
        public virtual async Task<CustomerOrderListModel> PrepareCustomerOrderListModelAsync(CustomerOrderSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get customer orders
            var orders = await OrderService.SearchOrdersAsync(customerId: customer.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new CustomerOrderListModel().PrepareToGridAsync(searchModel, orders, () =>
            {
                return orders.SelectAwait(async order =>
                {
                    //fill in model values from the entity
                    var orderModel = order.ToModel<CustomerOrderModel>();

                    //convert dates to the user time
                    orderModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    orderModel.StoreName = (await StoreService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Unknown";
                    orderModel.OrderStatus = await LocalizationService.GetLocalizedEnumAsync(order.OrderStatus);
                    orderModel.PaymentStatus = await LocalizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                    orderModel.ShippingStatus = await LocalizationService.GetLocalizedEnumAsync(order.ShippingStatus);
                    orderModel.OrderTotal = await PriceFormatter.FormatPriceAsync(order.OrderTotal, true, false);

                    return orderModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged customer shopping cart list model
        /// </summary>
        /// <param name="searchModel">Customer shopping cart search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer shopping cart list model
        /// </returns>
        public virtual async Task<CustomerShoppingCartListModel> PrepareCustomerShoppingCartListModelAsync(CustomerShoppingCartSearchModel searchModel,
            Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get customer shopping cart
            var shoppingCart = (await ShoppingCartService.GetShoppingCartAsync(customer, (ShoppingCartType)searchModel.ShoppingCartTypeId))
                .ToPagedList(searchModel);

            //prepare list model
            var model = await new CustomerShoppingCartListModel().PrepareToGridAsync(searchModel, shoppingCart, () =>
            {
                return shoppingCart.SelectAwait(async item =>
                {
                    //fill in model values from the entity
                    var shoppingCartItemModel = item.ToModel<ShoppingCartItemModel>();

                    var product = await ProductService.GetProductByIdAsync(item.ProductId);

                    //fill in additional values (not existing in the entity)
                    shoppingCartItemModel.ProductName = product.Name;
                    shoppingCartItemModel.Store = (await StoreService.GetStoreByIdAsync(item.StoreId))?.Name ?? "Unknown";
                    shoppingCartItemModel.AttributeInfo = await ProductAttributeFormatter.FormatAttributesAsync(product, item.AttributesXml);
                    var (unitPrice, _, _) = await ShoppingCartService.GetUnitPriceAsync(item, true);
                    shoppingCartItemModel.UnitPrice = await PriceFormatter.FormatPriceAsync((await TaxService.GetProductPriceAsync(product, unitPrice)).price);
                    shoppingCartItemModel.UnitPriceValue = (await TaxService.GetProductPriceAsync(product, unitPrice)).price;
                    var (subTotal, _, _, _) = await ShoppingCartService.GetSubTotalAsync(item, true);
                    shoppingCartItemModel.Total = await PriceFormatter.FormatPriceAsync((await TaxService.GetProductPriceAsync(product, subTotal)).price);
                    shoppingCartItemModel.TotalValue = (await TaxService.GetProductPriceAsync(product, subTotal)).price;

                    //convert dates to the user time
                    shoppingCartItemModel.UpdatedOn = await DateTimeHelper.ConvertToUserTimeAsync(item.UpdatedOnUtc, DateTimeKind.Utc);

                    return shoppingCartItemModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged customer activity log list model
        /// </summary>
        /// <param name="searchModel">Customer activity log search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer activity log list model
        /// </returns>
        public virtual async Task<CustomerActivityLogListModel> PrepareCustomerActivityLogListModelAsync(CustomerActivityLogSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get customer activity log
            var activityLog = await CustomerActivityService.GetAllActivitiesAsync(customerId: customer.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new CustomerActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
            {
                return activityLog.SelectAwait(async logItem =>
                {
                    //fill in model values from the entity
                    var customerActivityLogModel = logItem.ToModel<CustomerActivityLogModel>();

                    //fill in additional values (not existing in the entity)
                    customerActivityLogModel.ActivityLogTypeName = (await CustomerActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;

                    //convert dates to the user time
                    customerActivityLogModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    return customerActivityLogModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged customer back in stock subscriptions list model
        /// </summary>
        /// <param name="searchModel">Customer back in stock subscriptions search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer back in stock subscriptions list model
        /// </returns>
        public virtual async Task<CustomerBackInStockSubscriptionListModel> PrepareCustomerBackInStockSubscriptionListModelAsync(
            CustomerBackInStockSubscriptionSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get customer back in stock subscriptions
            var subscriptions = await BackInStockSubscriptionService.GetAllSubscriptionsByCustomerIdAsync(customer.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new CustomerBackInStockSubscriptionListModel().PrepareToGridAsync(searchModel, subscriptions, () =>
            {
                return subscriptions.SelectAwait(async subscription =>
                {
                    //fill in model values from the entity
                    var subscriptionModel = subscription.ToModel<CustomerBackInStockSubscriptionModel>();

                    //convert dates to the user time
                    subscriptionModel.CreatedOn =
                        await DateTimeHelper.ConvertToUserTimeAsync(subscription.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    subscriptionModel.StoreName = (await StoreService.GetStoreByIdAsync(subscription.StoreId))?.Name ?? "Unknown";
                    subscriptionModel.ProductName = (await ProductService.GetProductByIdAsync(subscription.ProductId))?.Name ?? "Unknown";

                    return subscriptionModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare online customer search model
        /// </summary>
        /// <param name="searchModel">Online customer search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the online customer search model
        /// </returns>
        public virtual Task<OnlineCustomerSearchModel> PrepareOnlineCustomerSearchModelAsync(OnlineCustomerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged online customer list model
        /// </summary>
        /// <param name="searchModel">Online customer search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the online customer list model
        /// </returns>
        public virtual async Task<OnlineCustomerListModel> PrepareOnlineCustomerListModelAsync(OnlineCustomerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter customers
            var lastActivityFrom = DateTime.UtcNow.AddMinutes(-CustomerSettings.OnlineCustomerMinutes);

            //get online customers
            var customers = await CustomerService.GetOnlineCustomersAsync(customerRoleIds: null,
                 lastActivityFromUtc: lastActivityFrom,
                 pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new OnlineCustomerListModel().PrepareToGridAsync(searchModel, customers, () =>
            {
                return customers.SelectAwait(async customer =>
                {
                    //fill in model values from the entity
                    var customerModel = customer.ToModel<OnlineCustomerModel>();

                    //convert dates to the user time
                    customerModel.LastActivityDate = await DateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    customerModel.CustomerInfo = (await CustomerService.IsRegisteredAsync(customer))
                        ? customer.Email
                        : await LocalizationService.GetResourceAsync("Admin.Customers.Guest");
                    customerModel.LastIpAddress = CustomerSettings.StoreIpAddresses
                        ? customer.LastIpAddress
                        : await LocalizationService.GetResourceAsync("Admin.Customers.OnlineCustomers.Fields.IPAddress.Disabled");
                    customerModel.Location = GeoLookupService.LookupCountryName(customer.LastIpAddress);
                    customerModel.LastVisitedPage = CustomerSettings.StoreLastVisitedPage
                        ? await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastVisitedPageAttribute)
                        : await LocalizationService.GetResourceAsync("Admin.Customers.OnlineCustomers.Fields.LastVisitedPage.Disabled");

                    return customerModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare GDPR request (log) search model
        /// </summary>
        /// <param name="searchModel">GDPR request search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR request search model
        /// </returns>
        public virtual async Task<GdprLogSearchModel> PrepareGdprLogSearchModelAsync(GdprLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare request types
            await BaseAdminModelFactory.PrepareGdprRequestTypesAsync(searchModel.AvailableRequestTypes);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged GDPR request list model
        /// </summary>
        /// <param name="searchModel">GDPR request search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR request list model
        /// </returns>
        public virtual async Task<GdprLogListModel> PrepareGdprLogListModelAsync(GdprLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customerId = 0;
            var customerInfo = "";
            if (!string.IsNullOrEmpty(searchModel.SearchEmail))
            {
                var customer = await CustomerService.GetCustomerByEmailAsync(searchModel.SearchEmail);
                if (customer != null)
                    customerId = customer.Id;
                else
                {
                    customerInfo = searchModel.SearchEmail;
                }
            }
            //get requests
            var gdprLog = await GdprService.GetAllLogAsync(
                customerId: customerId,
                customerInfo: customerInfo,
                requestType: searchModel.SearchRequestTypeId > 0 ? (GdprRequestType?)searchModel.SearchRequestTypeId : null,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new GdprLogListModel().PrepareToGridAsync(searchModel, gdprLog, () =>
            {
                return gdprLog.SelectAwait(async log =>
                {
                    //fill in model values from the entity
                    var customer = await CustomerService.GetCustomerByIdAsync(log.CustomerId);

                    var requestModel = log.ToModel<GdprLogModel>();

                    //fill in additional values (not existing in the entity)
                    requestModel.CustomerInfo = customer != null && !customer.Deleted && !string.IsNullOrEmpty(customer.Email)
                        ? customer.Email
                        : log.CustomerInfo;
                    requestModel.RequestType = await LocalizationService.GetLocalizedEnumAsync(log.RequestType);

                    requestModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc);

                    return requestModel;
                });
            });

            return model;
        }

        #endregion
    }
}