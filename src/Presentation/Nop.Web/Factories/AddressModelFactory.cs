using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Common;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the address model factory
    /// </summary>
    public partial class AddressModelFactory : IAddressModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ICountryService _countryService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AddressModelFactory(AddressSettings addressSettings,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            ICountryService countryService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            IWorkContext workContext)
        {
            _addressSettings = addressSettings;
            _addressAttributeFormatter = addressAttributeFormatter;
            _addressAttributeParser = addressAttributeParser;
            _addressAttributeService = addressAttributeService;
            _countryService = countryService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _stateProvinceService = stateProvinceService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare address attributes
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address entity</param>
        /// <param name="overrideAttributesXml">Overridden address attributes in XML format; pass null to use CustomAttributes of address entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareCustomAddressAttributesAsync(AddressModel model,
            Address address, string overrideAttributesXml = "")
        {
            var attributes = await _addressAttributeService.GetAllAddressAttributesAsync();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddressAttributeModel
                {
                    Id = attribute.Id,
                    ControlId = string.Format(NopCommonDefaults.AddressAttributeControlName, attribute.Id),
                    Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _addressAttributeService.GetAddressAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new AddressAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                var selectedAddressAttributes = !string.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    address?.CustomAttributes;
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = await _addressAttributeParser.ParseAddressAttributeValuesAsync(selectedAddressAttributes);
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
                            if (!string.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                var enteredText = _addressAttributeParser.ParseValues(selectedAddressAttributes, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                model.CustomAddressAttributes.Add(attributeModel);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address entity</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="addressSettings">Address settings</param>
        /// <param name="loadCountries">Countries loading function; pass null if countries do not need to load</param>
        /// <param name="prePopulateWithCustomerFields">Whether to populate model properties with the customer fields (used with the customer entity)</param>
        /// <param name="customer">Customer entity; required if prePopulateWithCustomerFields is true</param>
        /// <param name="overrideAttributesXml">Overridden address attributes in XML format; pass null to use CustomAttributes of the address entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareAddressModelAsync(AddressModel model,
            Address address, bool excludeProperties,
            AddressSettings addressSettings,
            Func<Task<IList<Country>>> loadCountries = null,
            bool prePopulateWithCustomerFields = false,
            Customer customer = null,
            string overrideAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (addressSettings == null)
                throw new ArgumentNullException(nameof(addressSettings));

            if (!excludeProperties && address != null)
            {
                model.Id = address.Id;
                model.FirstName = address.FirstName;
                model.LastName = address.LastName;
                model.Email = address.Email;
                model.Company = address.Company;
                model.CountryId = address.CountryId;
                model.CountryName = await _countryService.GetCountryByAddressAsync(address) is Country country ? await _localizationService.GetLocalizedAsync(country, x => x.Name) : null;
                model.StateProvinceId = address.StateProvinceId;
                model.StateProvinceName = await _stateProvinceService.GetStateProvinceByAddressAsync(address) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name) : null;
                model.County = address.County;
                model.City = address.City;
                model.Address1 = address.Address1;
                model.Address2 = address.Address2;
                model.ZipPostalCode = address.ZipPostalCode;
                model.PhoneNumber = address.PhoneNumber;
                model.FaxNumber = address.FaxNumber;
            }

            if (address == null && prePopulateWithCustomerFields)
            {
                if (customer == null)
                    throw new Exception("Customer cannot be null when prepopulating an address");
                model.Email = customer.Email;
                model.FirstName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute);
                model.LastName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastNameAttribute);
                model.Company = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CompanyAttribute);
                model.Address1 = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.StreetAddressAttribute);
                model.Address2 = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.StreetAddress2Attribute);
                model.ZipPostalCode = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute);
                model.City = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CityAttribute);
                model.County = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CountyAttribute);
                model.PhoneNumber = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute);
                model.FaxNumber = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FaxAttribute);
            }

            //countries and states
            if (addressSettings.CountryEnabled && loadCountries != null)
            {
                var countries = await loadCountries();

                if (_addressSettings.PreselectCountryIfOnlyOne && countries.Count == 1)
                {
                    model.CountryId = countries[0].Id;
                }
                else
                {
                    model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
                }

                foreach (var c in countries)
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (addressSettings.StateProvinceEnabled)
                {
                    var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
                    var states = (await _stateProvinceService
                        .GetStateProvincesByCountryIdAsync(model.CountryId ?? 0, languageId))
                        .ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem
                            {
                                Text = await _localizationService.GetLocalizedAsync(s, x => x.Name),
                                Value = s.Id.ToString(),
                                Selected = (s.Id == model.StateProvinceId)
                            });
                        }
                    }
                    else
                    {
                        var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);
                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                            Value = "0"
                        });
                    }
                }
            }

            //form fields
            model.CompanyEnabled = addressSettings.CompanyEnabled;
            model.CompanyRequired = addressSettings.CompanyRequired;
            model.StreetAddressEnabled = addressSettings.StreetAddressEnabled;
            model.StreetAddressRequired = addressSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = addressSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = addressSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = addressSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = addressSettings.ZipPostalCodeRequired;
            model.CityEnabled = addressSettings.CityEnabled;
            model.CityRequired = addressSettings.CityRequired;
            model.CountyEnabled = addressSettings.CountyEnabled;
            model.CountyRequired = addressSettings.CountyRequired;
            model.CountryEnabled = addressSettings.CountryEnabled;
            model.StateProvinceEnabled = addressSettings.StateProvinceEnabled;
            model.PhoneEnabled = addressSettings.PhoneEnabled;
            model.PhoneRequired = addressSettings.PhoneRequired;
            model.FaxEnabled = addressSettings.FaxEnabled;
            model.FaxRequired = addressSettings.FaxRequired;

            //customer attribute services
            if (_addressAttributeService != null && _addressAttributeParser != null)
            {
                await PrepareCustomAddressAttributesAsync(model, address, overrideAttributesXml);
            }
            if (_addressAttributeFormatter != null && address != null)
            {
                model.FormattedCustomAddressAttributes = await _addressAttributeFormatter.FormatAttributesAsync(address.CustomAttributes);
            }
        }

        #endregion
    }
}