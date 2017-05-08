using System.Collections.Generic;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Validators.Common;

namespace Nop.Web.Models.Common
{
    [Validator(typeof(AddressValidator))]
    public partial class AddressModel : BaseNopEntityModel
    {
        public AddressModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            CustomAddressAttributes = new List<AddressAttributeModel>();
        }

        [NopResourceDisplayName("Address.Fields.FirstName")]

#if NET451
		[AllowHtml]
#endif
        public string FirstName { get; set; }
        [NopResourceDisplayName("Address.Fields.LastName")]

#if NET451
		[AllowHtml]
#endif
        public string LastName { get; set; }
        [NopResourceDisplayName("Address.Fields.Email")]

#if NET451
		[AllowHtml]
#endif
        public string Email { get; set; }


        public bool CompanyEnabled { get; set; }
        public bool CompanyRequired { get; set; }
        [NopResourceDisplayName("Address.Fields.Company")]

#if NET451
		[AllowHtml]
#endif
        public string Company { get; set; }

        public bool CountryEnabled { get; set; }
        [NopResourceDisplayName("Address.Fields.Country")]
        public int? CountryId { get; set; }
        [NopResourceDisplayName("Address.Fields.Country")]

#if NET451
		[AllowHtml]
#endif
        public string CountryName { get; set; }

        public bool StateProvinceEnabled { get; set; }
        [NopResourceDisplayName("Address.Fields.StateProvince")]
        public int? StateProvinceId { get; set; }
        [NopResourceDisplayName("Address.Fields.StateProvince")]

#if NET451
		[AllowHtml]
#endif
        public string StateProvinceName { get; set; }

        public bool CityEnabled { get; set; }
        public bool CityRequired { get; set; }
        [NopResourceDisplayName("Address.Fields.City")]

#if NET451
		[AllowHtml]
#endif
        public string City { get; set; }

        public bool StreetAddressEnabled { get; set; }
        public bool StreetAddressRequired { get; set; }
        [NopResourceDisplayName("Address.Fields.Address1")]

#if NET451
		[AllowHtml]
#endif
        public string Address1 { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        public bool StreetAddress2Required { get; set; }
        [NopResourceDisplayName("Address.Fields.Address2")]

#if NET451
		[AllowHtml]
#endif
        public string Address2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        public bool ZipPostalCodeRequired { get; set; }
        [NopResourceDisplayName("Address.Fields.ZipPostalCode")]

#if NET451
		[AllowHtml]
#endif
        public string ZipPostalCode { get; set; }

        public bool PhoneEnabled { get; set; }
        public bool PhoneRequired { get; set; }
        [NopResourceDisplayName("Address.Fields.PhoneNumber")]

#if NET451
		[AllowHtml]
#endif
        public string PhoneNumber { get; set; }

        public bool FaxEnabled { get; set; }
        public bool FaxRequired { get; set; }
        [NopResourceDisplayName("Address.Fields.FaxNumber")]

#if NET451
		[AllowHtml]
#endif
        public string FaxNumber { get; set; }
        
        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }


        public string FormattedCustomAddressAttributes { get; set; }
        public IList<AddressAttributeModel> CustomAddressAttributes { get; set; }
    }
}