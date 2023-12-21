using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the vendor model factory implementation
/// </summary>
public partial class VendorModelFactory : IVendorModelFactory
{
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAttributeParser<VendorAttribute, VendorAttributeValue> _vendorAttributeParser;
    protected readonly IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public VendorModelFactory(CurrencySettings currencySettings,
        ICurrencyService currencyService,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAttributeParser<VendorAttribute, VendorAttributeValue> vendorAttributeParser,
        IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        VendorSettings vendorSettings)
    {
        _currencySettings = currencySettings;
        _currencyService = currencyService;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _vendorAttributeParser = vendorAttributeParser;
        _vendorAttributeService = vendorAttributeService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _localizedModelFactory = localizedModelFactory;
        _urlRecordService = urlRecordService;
        _vendorService = vendorService;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare vendor associated customer models
    /// </summary>
    /// <param name="models">List of vendor associated customer models</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareAssociatedCustomerModelsAsync(IList<VendorAssociatedCustomerModel> models, Vendor vendor)
    {
        ArgumentNullException.ThrowIfNull(models);

        ArgumentNullException.ThrowIfNull(vendor);

        var associatedCustomers = await _customerService.GetAllCustomersAsync(vendorId: vendor.Id);
        foreach (var customer in associatedCustomers)
        {
            models.Add(new VendorAssociatedCustomerModel
            {
                Id = customer.Id,
                Email = customer.Email
            });
        }
    }

    /// <summary>
    /// Prepare vendor attribute models
    /// </summary>
    /// <param name="models">List of vendor attribute models</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareVendorAttributeModelsAsync(IList<VendorModel.VendorAttributeModel> models, Vendor vendor)
    {
        ArgumentNullException.ThrowIfNull(models);

        //get available vendor attributes
        var vendorAttributes = await _vendorAttributeService.GetAllAttributesAsync();
        foreach (var attribute in vendorAttributes)
        {
            var attributeModel = new VendorModel.VendorAttributeModel
            {
                Id = attribute.Id,
                Name = attribute.Name,
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType
            };

            if (attribute.ShouldHaveValues)
            {
                //values
                var attributeValues = await _vendorAttributeService.GetAttributeValuesAsync(attribute.Id);
                foreach (var attributeValue in attributeValues)
                {
                    var attributeValueModel = new VendorModel.VendorAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = attributeValue.Name,
                        IsPreSelected = attributeValue.IsPreSelected
                    };
                    attributeModel.Values.Add(attributeValueModel);
                }
            }

            //set already selected attributes
            if (vendor != null)
            {
                var selectedVendorAttributes = await _genericAttributeService.GetAttributeAsync<string>(vendor, NopVendorDefaults.VendorAttributes);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    {
                        if (!string.IsNullOrEmpty(selectedVendorAttributes))
                        {
                            //clear default selection
                            foreach (var item in attributeModel.Values)
                                item.IsPreSelected = false;

                            //select new values
                            var selectedValues = await _vendorAttributeParser.ParseAttributeValuesAsync(selectedVendorAttributes);
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
                        if (!string.IsNullOrEmpty(selectedVendorAttributes))
                        {
                            var enteredText = _vendorAttributeParser.ParseValues(selectedVendorAttributes, attribute.Id);
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
    /// Prepare vendor note search model
    /// </summary>
    /// <param name="searchModel">Vendor note search model</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>Vendor note search model</returns>
    protected virtual VendorNoteSearchModel PrepareVendorNoteSearchModel(VendorNoteSearchModel searchModel, Vendor vendor)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(vendor);

        searchModel.VendorId = vendor.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare vendor search model
    /// </summary>
    /// <param name="searchModel">Vendor search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor search model
    /// </returns>
    public virtual Task<VendorSearchModel> PrepareVendorSearchModelAsync(VendorSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged vendor list model
    /// </summary>
    /// <param name="searchModel">Vendor search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor list model
    /// </returns>
    public virtual async Task<VendorListModel> PrepareVendorListModelAsync(VendorSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get vendors
        var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true,
            name: searchModel.SearchName,
            email: searchModel.SearchEmail,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new VendorListModel().PrepareToGridAsync(searchModel, vendors, () =>
        {
            //fill in model values from the entity
            return vendors.SelectAwait(async vendor =>
            {
                var vendorModel = vendor.ToModel<VendorModel>();

                vendorModel.SeName = await _urlRecordService.GetSeNameAsync(vendor, 0, true, false);

                return vendorModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare vendor model
    /// </summary>
    /// <param name="model">Vendor model</param>
    /// <param name="vendor">Vendor</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor model
    /// </returns>
    public virtual async Task<VendorModel> PrepareVendorModelAsync(VendorModel model, Vendor vendor, bool excludeProperties = false)
    {
        Func<VendorLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (vendor != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = vendor.ToModel<VendorModel>();
                model.SeName = await _urlRecordService.GetSeNameAsync(vendor, 0, true, false);
            }

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(vendor, entity => entity.Name, languageId, false, false);
                locale.Description = await _localizationService.GetLocalizedAsync(vendor, entity => entity.Description, languageId, false, false);
                locale.MetaKeywords = await _localizationService.GetLocalizedAsync(vendor, entity => entity.MetaKeywords, languageId, false, false);
                locale.MetaDescription = await _localizationService.GetLocalizedAsync(vendor, entity => entity.MetaDescription, languageId, false, false);
                locale.MetaTitle = await _localizationService.GetLocalizedAsync(vendor, entity => entity.MetaTitle, languageId, false, false);
                locale.SeName = await _urlRecordService.GetSeNameAsync(vendor, languageId, false, false);
            };

            //prepare associated customers
            await PrepareAssociatedCustomerModelsAsync(model.AssociatedCustomers, vendor);

            //prepare nested search models
            PrepareVendorNoteSearchModel(model.VendorNoteSearchModel, vendor);
        }

        //set default values for the new model
        if (vendor == null)
        {
            model.PageSize = 6;
            model.Active = true;
            model.AllowCustomersToSelectPageSize = true;
            model.PageSizeOptions = _vendorSettings.DefaultVendorPageSizeOptions;
            model.PriceRangeFiltering = true;
            model.ManuallyPriceRange = true;
            model.PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
            model.PriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
        }

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare model vendor attributes
        await PrepareVendorAttributeModelsAsync(model.VendorAttributes, vendor);

        //prepare address model
        var address = await _addressService.GetAddressByIdAsync(vendor?.AddressId ?? 0);
        if (!excludeProperties && address != null)
            model.Address = address.ToModel(model.Address);
        await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);

        return model;
    }

    /// <summary>
    /// Prepare paged vendor note list model
    /// </summary>
    /// <param name="searchModel">Vendor note search model</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor note list model
    /// </returns>
    public virtual async Task<VendorNoteListModel> PrepareVendorNoteListModelAsync(VendorNoteSearchModel searchModel, Vendor vendor)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(vendor);

        //get vendor notes
        var vendorNotes = await _vendorService.GetVendorNotesByVendorAsync(vendor.Id, searchModel.Page - 1, searchModel.PageSize);

        //prepare list model
        var model = await new VendorNoteListModel().PrepareToGridAsync(searchModel, vendorNotes, () =>
        {
            //fill in model values from the entity
            return vendorNotes.SelectAwait(async note =>
            {
                //fill in model values from the entity        
                var vendorNoteModel = note.ToModel<VendorNoteModel>();

                //convert dates to the user time
                vendorNoteModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(note.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                vendorNoteModel.Note = _vendorService.FormatVendorNoteText(note);

                return vendorNoteModel;
            });
        });

        return model;
    }

    #endregion
}