using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Common;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Common
{
    [Validator(typeof(AddressValidator))]
    public partial class AddressModel : BaseNopEntityModel
    {
        public AddressModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Address.Fields.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.LastName")]
        [AllowHtml]
        public string LastName { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.Company")]
        [AllowHtml]
        public string Company { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.Country")]
        public int? CountryId { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.Country")]
        [AllowHtml]
        public string CountryName { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.StateProvince")]
        public int? StateProvinceId { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.StateProvince")]
        [AllowHtml]
        public string StateProvinceName { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.City")]
        [AllowHtml]
        public string City { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.Address1")]
        [AllowHtml]
        public string Address1 { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.Address2")]
        [AllowHtml]
        public string Address2 { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.ZipPostalCode")]
        [AllowHtml]
        public string ZipPostalCode { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.PhoneNumber")]
        [AllowHtml]
        public string PhoneNumber { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.FaxNumber")]
        [AllowHtml]
        public string FaxNumber { get; set; }

        [NopResourceDisplayName("Admin.Address")]
        public string AddressHtml { get; set; }



        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }



        public bool FirstNameDisabled { get; set; }
        public bool LastNameDisabled { get; set; }
        public bool EmailDisabled { get; set; }
        public bool CompanyDisabled { get; set; }
        public bool CountryDisabled { get; set; }
        public bool StateProvinceDisabled { get; set; }
        public bool CityDisabled { get; set; }
        public bool Address1Disabled { get; set; }
        public bool Address2Disabled { get; set; }
        public bool ZipPostalCodeDisabled { get; set; }
        public bool PhoneNumberDisabled { get; set; }
        public bool FaxNumberDisabled { get; set; }
    }
}