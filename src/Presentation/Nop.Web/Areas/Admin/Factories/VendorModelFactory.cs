using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Vendors;
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

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the vendor model factory implementation
    /// </summary>
    public partial class VendorModelFactory : IVendorModelFactory
    {
        #region Fields

        protected CurrencySettings CurrencySettings { get; }
        protected ICurrencyService CurrencyService { get; }
        protected IAddressModelFactory AddressModelFactory { get; }
        protected IAddressService AddressService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedModelFactory LocalizedModelFactory { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IVendorAttributeParser VendorAttributeParser { get; }
        protected IVendorAttributeService VendorAttributeService { get; }
        protected IVendorService VendorService { get; }
        protected VendorSettings VendorSettings { get; }

        #endregion

        #region Ctor

        public VendorModelFactory(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IUrlRecordService urlRecordService,
            IVendorAttributeParser vendorAttributeParser,
            IVendorAttributeService vendorAttributeService,
            IVendorService vendorService,
            VendorSettings vendorSettings)
        {
            CurrencySettings = currencySettings;
            CurrencyService = currencyService;
            AddressModelFactory = addressModelFactory;
            AddressService = addressService;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            LocalizedModelFactory = localizedModelFactory;
            UrlRecordService = urlRecordService;
            VendorAttributeParser = vendorAttributeParser;
            VendorAttributeService = vendorAttributeService;
            VendorService = vendorService;
            VendorSettings = vendorSettings;
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
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var associatedCustomers = await CustomerService.GetAllCustomersAsync(vendorId: vendor.Id);
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
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //get available vendor attributes
            var vendorAttributes = await VendorAttributeService.GetAllVendorAttributesAsync();
            foreach (var attribute in vendorAttributes)
            {
                var attributeModel = new VendorModel.VendorAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await VendorAttributeService.GetVendorAttributeValuesAsync(attribute.Id);
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
                    var selectedVendorAttributes = await GenericAttributeService.GetAttributeAsync<string>(vendor, NopVendorDefaults.VendorAttributes);
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
                                    var selectedValues = await VendorAttributeParser.ParseVendorAttributeValuesAsync(selectedVendorAttributes);
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
                                    var enteredText = VendorAttributeParser.ParseValues(selectedVendorAttributes, attribute.Id);
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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get vendors
            var vendors = await VendorService.GetAllVendorsAsync(showHidden: true,
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

                    vendorModel.SeName = await UrlRecordService.GetSeNameAsync(vendor, 0, true, false);

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
                    model.SeName = await UrlRecordService.GetSeNameAsync(vendor, 0, true, false);
                }

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await LocalizationService.GetLocalizedAsync(vendor, entity => entity.Name, languageId, false, false);
                    locale.Description = await LocalizationService.GetLocalizedAsync(vendor, entity => entity.Description, languageId, false, false);
                    locale.MetaKeywords = await LocalizationService.GetLocalizedAsync(vendor, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = await LocalizationService.GetLocalizedAsync(vendor, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = await LocalizationService.GetLocalizedAsync(vendor, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = await UrlRecordService.GetSeNameAsync(vendor, languageId, false, false);
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
                model.PageSizeOptions = VendorSettings.DefaultVendorPageSizeOptions;
                model.PriceRangeFiltering = true;
                model.ManuallyPriceRange = true;
                model.PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
                model.PriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
            }

            model.PrimaryStoreCurrencyCode = (await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await LocalizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare model vendor attributes
            await PrepareVendorAttributeModelsAsync(model.VendorAttributes, vendor);

            //prepare address model
            var address = await AddressService.GetAddressByIdAsync(vendor?.AddressId ?? 0);
            if (!excludeProperties && address != null)
                model.Address = address.ToModel(model.Address);
            await AddressModelFactory.PrepareAddressModelAsync(model.Address, address);

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            //get vendor notes
            var vendorNotes = await VendorService.GetVendorNotesByVendorAsync(vendor.Id, searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = await new VendorNoteListModel().PrepareToGridAsync(searchModel, vendorNotes, () =>
            {
                //fill in model values from the entity
                return vendorNotes.SelectAwait(async note =>
                {
                    //fill in model values from the entity        
                    var vendorNoteModel = note.ToModel<VendorNoteModel>();

                    //convert dates to the user time
                    vendorNoteModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(note.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    vendorNoteModel.Note = VendorService.FormatVendorNoteText(note);

                    return vendorNoteModel;
                });
            });

            return model;
        }

        #endregion
    }
}