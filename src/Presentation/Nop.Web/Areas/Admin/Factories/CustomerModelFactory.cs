using System.Text;
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
using Nop.Services.Attributes;
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
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the customer model factory implementation
/// </summary>
public partial class CustomerModelFactory : ICustomerModelFactory
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly DateTimeSettings _dateTimeSettings;
    protected readonly GdprSettings _gdprSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAffiliateService _affiliateService;
    protected readonly IAttributeFormatter<AddressAttribute, AddressAttributeValue> _addressAttributeFormatter;
    protected readonly IAttributeParser<CustomerAttribute, CustomerAttributeValue> _customerAttributeParser;
    protected readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    protected readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IExternalAuthenticationService _externalAuthenticationService;
    protected readonly IGdprService _gdprService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IGeoLookupService _geoLookupService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly IOrderService _orderService;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeFormatter _productAttributeFormatter;
    protected readonly IProductService _productService;
    protected readonly IRewardPointService _rewardPointService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly ITaxService _taxService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly RewardPointsSettings _rewardPointsSettings;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public CustomerModelFactory(AddressSettings addressSettings,
        CustomerSettings customerSettings,
        DateTimeSettings dateTimeSettings,
        GdprSettings gdprSettings,
        ForumSettings forumSettings,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAffiliateService affiliateService,
        IAttributeFormatter<AddressAttribute, AddressAttributeValue> addressAttributeFormatter,
        IAttributeParser<CustomerAttribute, CustomerAttributeValue> customerAttributeParser,
        IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService,
        IAuthenticationPluginManager authenticationPluginManager,
        IBackInStockSubscriptionService backInStockSubscriptionService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICountryService countryService,
        ICustomerActivityService customerActivityService,
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
        IWorkContext workContext,
        MediaSettings mediaSettings,
        RewardPointsSettings rewardPointsSettings,
        TaxSettings taxSettings)
    {
        _addressSettings = addressSettings;
        _customerSettings = customerSettings;
        _dateTimeSettings = dateTimeSettings;
        _gdprSettings = gdprSettings;
        _forumSettings = forumSettings;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _affiliateService = affiliateService;
        _addressAttributeFormatter = addressAttributeFormatter;
        _customerAttributeParser = customerAttributeParser;
        _customerAttributeService = customerAttributeService;
        _authenticationPluginManager = authenticationPluginManager;
        _backInStockSubscriptionService = backInStockSubscriptionService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _countryService = countryService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _externalAuthenticationService = externalAuthenticationService;
        _gdprService = gdprService;
        _genericAttributeService = genericAttributeService;
        _geoLookupService = geoLookupService;
        _localizationService = localizationService;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _orderService = orderService;
        _pictureService = pictureService;
        _priceFormatter = priceFormatter;
        _productAttributeFormatter = productAttributeFormatter;
        _productService = productService;
        _rewardPointService = rewardPointService;
        _shoppingCartService = shoppingCartService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _storeService = storeService;
        _taxService = taxService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _rewardPointsSettings = rewardPointsSettings;
        _taxSettings = taxSettings;
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
        ArgumentNullException.ThrowIfNull(model);

        var store = await _storeContext.GetCurrentStoreAsync();

        model.Message = string.Empty;
        model.ActivatePointsImmediately = true;
        model.StoreId = store.Id;

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
    }

    /// <summary>
    /// Prepare customer associated external authorization models
    /// </summary>
    /// <param name="models">List of customer associated external authorization models</param>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareAssociatedExternalAuthModelsAsync(IList<CustomerAssociatedExternalAuthModel> models, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(models);

        ArgumentNullException.ThrowIfNull(customer);

        foreach (var record in await _externalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(customer))
        {
            var method = await _authenticationPluginManager.LoadPluginBySystemNameAsync(record.ProviderSystemName);
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
        ArgumentNullException.ThrowIfNull(models);

        //get available customer attributes
        var customerAttributes = await _customerAttributeService.GetAllAttributesAsync();
        foreach (var attribute in customerAttributes)
        {
            var attributeModel = new CustomerModel.CustomerAttributeModel
            {
                Id = attribute.Id,
                Name = attribute.Name,
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType
            };

            if (attribute.ShouldHaveValues)
            {
                //values
                var attributeValues = await _customerAttributeService.GetAttributeValuesAsync(attribute.Id);
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
                var selectedCustomerAttributes = customer.CustomCustomerAttributesXML;
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
                            var selectedValues = await _customerAttributeParser.ParseAttributeValuesAsync(selectedCustomerAttributes);
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
                            var enteredText = _customerAttributeParser.ParseValues(selectedCustomerAttributes, attribute.Id);
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
        ArgumentNullException.ThrowIfNull(model);

        var separator = "<br />";
        var addressHtmlSb = new StringBuilder("<div>");
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
        var (addressLine, _) = await _addressService.FormatAddressAsync(address, languageId, separator, true);
        addressHtmlSb.Append(addressLine);

        var customAttributesFormatted = await _addressAttributeFormatter.FormatAttributesAsync(address?.CustomAttributes);
        if (!string.IsNullOrEmpty(customAttributesFormatted))
        {
            //already encoded
            addressHtmlSb.AppendFormat($"{separator}{customAttributesFormatted}");
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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        searchModel.CustomerId = customer.Id;

        //prepare available shopping cart types (search shopping cart by default)
        searchModel.ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart;
        await _baseAdminModelFactory.PrepareShoppingCartTypesAsync(searchModel.AvailableShoppingCartTypes, false);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.UsernamesEnabled = _customerSettings.UsernamesEnabled;
        searchModel.AvatarEnabled = _customerSettings.AllowCustomersToUploadAvatars;
        searchModel.FirstNameEnabled = _customerSettings.FirstNameEnabled;
        searchModel.LastNameEnabled = _customerSettings.LastNameEnabled;
        searchModel.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
        searchModel.CompanyEnabled = _customerSettings.CompanyEnabled;
        searchModel.PhoneEnabled = _customerSettings.PhoneEnabled;
        searchModel.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;

        //search registered customers by default
        var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
        if (registeredRole != null)
            searchModel.SelectedCustomerRoleIds.Add(registeredRole.Id);

        searchModel.AvailableActiveValues = new List<SelectListItem> {
            new(await _localizationService.GetResourceAsync("Admin.Common.All"), string.Empty),
            new(await _localizationService.GetResourceAsync("Admin.Common.Yes"), true.ToString(), true),
            new(await _localizationService.GetResourceAsync("Admin.Common.No"), false.ToString())
        };

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
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter customers
        _ = int.TryParse(searchModel.SearchDayOfBirth, out var dayOfBirth);
        _ = int.TryParse(searchModel.SearchMonthOfBirth, out var monthOfBirth);
        var createdFromUtc = !searchModel.SearchRegistrationDateFrom.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchRegistrationDateFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var createdToUtc = !searchModel.SearchRegistrationDateTo.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchRegistrationDateTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var lastActivityFromUtc = !searchModel.SearchLastActivityFrom.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchLastActivityFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var lastActivityToUtc = !searchModel.SearchLastActivityTo.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchLastActivityTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //exclude guests from the result when filter "by registration date" is used
        if (createdFromUtc.HasValue || createdToUtc.HasValue)
        {
            if (!searchModel.SelectedCustomerRoleIds.Any())
            {
                var customerRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
                searchModel.SelectedCustomerRoleIds = customerRoles
                    .Where(cr => cr.SystemName != NopCustomerDefaults.GuestsRoleName).Select(cr => cr.Id).ToList();
            }
            else
            {
                var guestRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName);
                if (guestRole != null)
                    searchModel.SelectedCustomerRoleIds.Remove(guestRole.Id);
            }
        }

        //get customers
        var customers = await _customerService.GetAllCustomersAsync(customerRoleIds: searchModel.SelectedCustomerRoleIds.ToArray(),
            email: searchModel.SearchEmail,
            username: searchModel.SearchUsername,
            firstName: searchModel.SearchFirstName,
            lastName: searchModel.SearchLastName,
            dayOfBirth: dayOfBirth,
            monthOfBirth: monthOfBirth,
            company: searchModel.SearchCompany,
            createdFromUtc: createdFromUtc,
            createdToUtc: createdToUtc,
            lastActivityFromUtc: lastActivityFromUtc,
            lastActivityToUtc: lastActivityToUtc,
            phone: searchModel.SearchPhone,
            zipPostalCode: searchModel.SearchZipPostalCode,
            ipAddress: searchModel.SearchIpAddress,
            isActive: searchModel.SearchIsActive,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new CustomerListModel().PrepareToGridAsync(searchModel, customers, () =>
        {
            return customers.SelectAwait(async customer =>
            {
                //fill in model values from the entity
                var customerModel = customer.ToModel<CustomerModel>();

                //convert dates to the user time
                customerModel.Email = (await _customerService.IsRegisteredAsync(customer))
                    ? customer.Email
                    : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                customerModel.FullName = await _customerService.GetCustomerFullNameAsync(customer);
                customerModel.Company = customer.Company;
                customerModel.Phone = customer.Phone;
                customerModel.ZipPostalCode = customer.ZipPostalCode;

                customerModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc);
                customerModel.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                customerModel.CustomerRoleNames = string.Join(", ",
                    (await _customerService.GetCustomerRolesAsync(customer)).Select(role => role.Name));
                if (_customerSettings.AllowCustomersToUploadAvatars)
                {
                    var avatarPictureId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute);
                    customerModel.AvatarUrl = await _pictureService
                        .GetPictureUrlAsync(avatarPictureId, _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
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
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.AllowSendingOfPrivateMessage = await _customerService.IsRegisteredAsync(customer) &&
                                                 _forumSettings.AllowPrivateMessages;
            model.AllowSendingOfWelcomeMessage = await _customerService.IsRegisteredAsync(customer) &&
                                                 _customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval;
            model.AllowReSendingOfActivationMessage = await _customerService.IsRegisteredAsync(customer) && !customer.Active &&
                                                      _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
            model.GdprEnabled = _gdprSettings.GdprEnabled;

            model.MultiFactorAuthenticationProvider = await _genericAttributeService
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
                model.FirstName = customer.FirstName;
                model.LastName = customer.LastName;
                model.Gender = customer.Gender;
                model.DateOfBirth = customer.DateOfBirth;
                model.Company = customer.Company;
                model.StreetAddress = customer.StreetAddress;
                model.StreetAddress2 = customer.StreetAddress2;
                model.ZipPostalCode = customer.ZipPostalCode;
                model.City = customer.City;
                model.County = customer.County;
                model.CountryId = customer.CountryId;
                model.StateProvinceId = customer.StateProvinceId;
                model.Phone = customer.Phone;
                model.Fax = customer.Fax;
                model.TimeZoneId = customer.TimeZoneId;
                model.VatNumber = customer.VatNumber;
                model.VatNumberStatusNote = await _localizationService.GetLocalizedEnumAsync(customer.VatNumberStatus);
                model.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);
                model.LastIpAddress = customer.LastIpAddress;
                model.LastVisitedPage = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastVisitedPageAttribute);
                model.SelectedCustomerRoleIds = (await _customerService.GetCustomerRoleIdsAsync(customer)).ToList();
                model.RegisteredInStore = (await _storeService.GetAllStoresAsync())
                    .FirstOrDefault(store => store.Id == customer.RegisteredInStoreId)?.Name ?? string.Empty;
                model.DisplayRegisteredInStore = model.Id > 0 && !string.IsNullOrEmpty(model.RegisteredInStore) &&
                                                 (await _storeService.GetAllStoresAsync()).Select(x => x.Id).Count() > 1;
                model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc);

                //prepare model affiliate
                var affiliate = await _affiliateService.GetAffiliateByIdAsync(customer.AffiliateId);
                if (affiliate != null)
                {
                    model.AffiliateId = affiliate.Id;
                    model.AffiliateName = await _affiliateService.GetAffiliateFullNameAsync(affiliate);
                }

                //prepare model newsletter subscriptions
                if (!string.IsNullOrEmpty(customer.Email))
                {
                    model.SelectedNewsletterSubscriptionStoreIds = await (await _storeService.GetAllStoresAsync())
                        .WhereAwait(async store => await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id) != null)
                        .Select(store => store.Id).ToListAsync();
                }
            }
            //prepare reward points model
            model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
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
                var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
                if (registeredRole != null)
                    model.SelectedCustomerRoleIds.Add(registeredRole.Id);
            }
        }

        model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
        model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
        model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
        model.LastNameEnabled = _customerSettings.LastNameEnabled;
        model.GenderEnabled = _customerSettings.GenderEnabled;
        model.NeutralGenderEnabled = _customerSettings.NeutralGenderEnabled;
        model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
        model.CompanyEnabled = _customerSettings.CompanyEnabled;
        model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
        model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
        model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
        model.CityEnabled = _customerSettings.CityEnabled;
        model.CountyEnabled = _customerSettings.CountyEnabled;
        model.CountryEnabled = _customerSettings.CountryEnabled;
        model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
        model.PhoneEnabled = _customerSettings.PhoneEnabled;
        model.FaxEnabled = _customerSettings.FaxEnabled;

        //set default values for the new model
        if (customer == null)
        {
            model.Active = true;
            model.DisplayVatNumber = false;
        }

        //prepare available vendors
        await _baseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.Customers.Customers.Fields.Vendor.None"));

        //prepare model customer attributes
        await PrepareCustomerAttributeModelsAsync(model.CustomerAttributes, customer);

        //prepare model stores for newsletter subscriptions
        model.AvailableNewsletterSubscriptionStores = (await _storeService.GetAllStoresAsync()).Select(store => new SelectListItem
        {
            Value = store.Id.ToString(),
            Text = store.Name,
            Selected = model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id)
        }).ToList();

        //prepare available customer roles
        var availableRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
        model.AvailableCustomerRoles = availableRoles.Select(role => new SelectListItem
        {
            Text = role.Name,
            Value = role.Id.ToString(),
            Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
        }).ToList();

        //prepare available time zones
        await _baseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

        //prepare available countries and states
        if (_customerSettings.CountryEnabled)
        {
            await _baseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);
            if (_customerSettings.StateProvinceEnabled)
                await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId == 0 ? null : (int?)model.CountryId);
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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get reward points history
        var rewardPoints = await _rewardPointService.GetRewardPointsHistoryAsync(customer.Id,
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
                var activatingDate = await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);
                rewardPointsHistoryModel.CreatedOn = activatingDate;

                rewardPointsHistoryModel.PointsBalance = historyEntry.PointsBalance.HasValue
                    ? historyEntry.PointsBalance.ToString()
                    : string.Format((await _localizationService.GetResourceAsync("Admin.Customers.Customers.RewardPoints.ActivatedLater")), activatingDate);
                rewardPointsHistoryModel.EndDate = !historyEntry.EndDateUtc.HasValue
                    ? null
                    : (DateTime?)(await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.EndDateUtc.Value, DateTimeKind.Utc));

                //fill in additional values (not existing in the entity)
                rewardPointsHistoryModel.StoreName = (await _storeService.GetStoreByIdAsync(historyEntry.StoreId))?.Name ?? "Unknown";

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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get customer addresses
        var addresses = (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
            .OrderByDescending(address => address.CreatedOnUtc).ThenByDescending(address => address.Id).ToList()
            .ToPagedList(searchModel);

        //prepare list model
        var model = await new CustomerAddressListModel().PrepareToGridAsync(searchModel, addresses, () =>
        {
            return addresses.SelectAwait(async address =>
            {
                //fill in model values from the entity        
                var addressModel = address.ToModel<AddressModel>();

                addressModel.CountryName = (await _countryService.GetCountryByAddressAsync(address))?.Name;
                addressModel.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Name;

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
        ArgumentNullException.ThrowIfNull(customer);

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
        await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
        model.Address.FirstNameRequired = true;
        model.Address.LastNameRequired = true;
        model.Address.EmailRequired = true;
        model.Address.CompanyRequired = _addressSettings.CompanyRequired;
        model.Address.CityRequired = _addressSettings.CityRequired;
        model.Address.CountyRequired = _addressSettings.CountyRequired;
        model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
        model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
        model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
        model.Address.PhoneRequired = _addressSettings.PhoneRequired;
        model.Address.FaxRequired = _addressSettings.FaxRequired;

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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get customer orders
        var orders = await _orderService.SearchOrdersAsync(customerId: customer.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new CustomerOrderListModel().PrepareToGridAsync(searchModel, orders, () =>
        {
            return orders.SelectAwait(async order =>
            {
                //fill in model values from the entity
                var orderModel = order.ToModel<CustomerOrderModel>();

                //convert dates to the user time
                orderModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                orderModel.StoreName = (await _storeService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Unknown";
                orderModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                orderModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                orderModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
                orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get customer shopping cart
        var shoppingCart = (await _shoppingCartService.GetShoppingCartAsync(customer, (ShoppingCartType)searchModel.ShoppingCartTypeId))
            .ToPagedList(searchModel);

        //prepare list model
        var model = await new CustomerShoppingCartListModel().PrepareToGridAsync(searchModel, shoppingCart, () =>
        {
            return shoppingCart.SelectAwait(async item =>
            {
                //fill in model values from the entity
                var shoppingCartItemModel = item.ToModel<ShoppingCartItemModel>();

                var product = await _productService.GetProductByIdAsync(item.ProductId);

                //fill in additional values (not existing in the entity)
                shoppingCartItemModel.ProductName = product.Name;
                shoppingCartItemModel.Store = (await _storeService.GetStoreByIdAsync(item.StoreId))?.Name ?? "Unknown";
                shoppingCartItemModel.AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(product, item.AttributesXml);
                var (unitPrice, _, _) = await _shoppingCartService.GetUnitPriceAsync(item, true);
                shoppingCartItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync((await _taxService.GetProductPriceAsync(product, unitPrice)).price);
                shoppingCartItemModel.UnitPriceValue = (await _taxService.GetProductPriceAsync(product, unitPrice)).price;
                var (subTotal, _, _, _) = await _shoppingCartService.GetSubTotalAsync(item, true);
                shoppingCartItemModel.Total = await _priceFormatter.FormatPriceAsync((await _taxService.GetProductPriceAsync(product, subTotal)).price);
                shoppingCartItemModel.TotalValue = (await _taxService.GetProductPriceAsync(product, subTotal)).price;

                //convert dates to the user time
                shoppingCartItemModel.UpdatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(item.UpdatedOnUtc, DateTimeKind.Utc);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get customer activity log
        var activityLog = await _customerActivityService.GetAllActivitiesAsync(customerId: customer.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new CustomerActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
        {
            return activityLog.SelectAwait(async logItem =>
            {
                //fill in model values from the entity
                var customerActivityLogModel = logItem.ToModel<CustomerActivityLogModel>();

                //fill in additional values (not existing in the entity)
                customerActivityLogModel.ActivityLogTypeName = (await _customerActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;

                //convert dates to the user time
                customerActivityLogModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get customer back in stock subscriptions
        var subscriptions = await _backInStockSubscriptionService.GetAllSubscriptionsByCustomerIdAsync(customer.Id,
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
                    await _dateTimeHelper.ConvertToUserTimeAsync(subscription.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                subscriptionModel.StoreName = (await _storeService.GetStoreByIdAsync(subscription.StoreId))?.Name ?? "Unknown";
                subscriptionModel.ProductName = (await _productService.GetProductByIdAsync(subscription.ProductId))?.Name ?? "Unknown";

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
        ArgumentNullException.ThrowIfNull(searchModel);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter customers
        var lastActivityFrom = DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes);

        //get online customers
        var customers = await _customerService.GetOnlineCustomersAsync(customerRoleIds: null,
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
                customerModel.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                customerModel.CustomerInfo = (await _customerService.IsRegisteredAsync(customer))
                    ? customer.Email
                    : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                customerModel.LastIpAddress = _customerSettings.StoreIpAddresses
                    ? customer.LastIpAddress
                    : await _localizationService.GetResourceAsync("Admin.Customers.OnlineCustomers.Fields.IPAddress.Disabled");
                customerModel.Location = _geoLookupService.LookupCountryName(customer.LastIpAddress);
                customerModel.LastVisitedPage = _customerSettings.StoreLastVisitedPage
                    ? await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastVisitedPageAttribute)
                    : await _localizationService.GetResourceAsync("Admin.Customers.OnlineCustomers.Fields.LastVisitedPage.Disabled");

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
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare request types
        await _baseAdminModelFactory.PrepareGdprRequestTypesAsync(searchModel.AvailableRequestTypes);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        var customerId = 0;
        var customerInfo = "";
        if (!string.IsNullOrEmpty(searchModel.SearchEmail))
        {
            var customer = await _customerService.GetCustomerByEmailAsync(searchModel.SearchEmail);
            if (customer != null)
                customerId = customer.Id;
            else
            {
                customerInfo = searchModel.SearchEmail;
            }
        }
        //get requests
        var gdprLog = await _gdprService.GetAllLogAsync(
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
                var customer = await _customerService.GetCustomerByIdAsync(log.CustomerId);

                var requestModel = log.ToModel<GdprLogModel>();

                //fill in additional values (not existing in the entity)
                requestModel.CustomerInfo = customer != null && !customer.Deleted && !string.IsNullOrEmpty(customer.Email)
                    ? customer.Email
                    : log.CustomerInfo;
                requestModel.RequestType = await _localizationService.GetLocalizedEnumAsync(log.RequestType);

                requestModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc);

                return requestModel;
            });
        });

        return model;
    }

    #endregion
}