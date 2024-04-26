using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents an address settings model
/// </summary>
public partial record AddressSettingsModel : BaseNopModel, ISettingsModel
{
    #region Ctor

    public AddressSettingsModel()
    {
        AvailableCountries = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.CompanyEnabled")]
    public bool CompanyEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.CompanyRequired")]
    public bool CompanyRequired { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddressEnabled")]
    public bool StreetAddressEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddressRequired")]
    public bool StreetAddressRequired { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddress2Enabled")]
    public bool StreetAddress2Enabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.StreetAddress2Required")]
    public bool StreetAddress2Required { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.ZipPostalCodeEnabled")]
    public bool ZipPostalCodeEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.ZipPostalCodeRequired")]
    public bool ZipPostalCodeRequired { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.CityEnabled")]
    public bool CityEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.CityRequired")]
    public bool CityRequired { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyEnabled")]
    public bool CountyEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountyRequired")]
    public bool CountyRequired { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.CountryEnabled")]
    public bool CountryEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.DefaultCountry")]
    public int? DefaultCountryId { get; set; }
    public IList<SelectListItem> AvailableCountries { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.StateProvinceEnabled")]
    public bool StateProvinceEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.PhoneEnabled")]
    public bool PhoneEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.PhoneRequired")]
    public bool PhoneRequired { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.FaxEnabled")]
    public bool FaxEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AddressFormFields.FaxRequired")]
    public bool FaxRequired { get; set; }

    #endregion
}