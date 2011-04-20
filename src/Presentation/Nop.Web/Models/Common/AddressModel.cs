using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Common;

namespace Nop.Web.Models.Common
{
    [Validator(typeof(AddressValidator))]
    public class AddressModel : BaseNopEntityModel
    {
        public AddressModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Address.Fields.FirstName")]
        public string FirstName { get; set; }
        [NopResourceDisplayName("Address.Fields.LastName")]
        public string LastName { get; set; }
        [NopResourceDisplayName("Address.Fields.Email")]
        public string Email { get; set; }
        [NopResourceDisplayName("Address.Fields.Company")]
        public string Company { get; set; }
        [NopResourceDisplayName("Address.Fields.Country")]
        public int? CountryId { get; set; }
        [NopResourceDisplayName("Address.Fields.Country")]
        public string CountryName { get; set; }
        [NopResourceDisplayName("Address.Fields.StateProvince")]
        public int? StateProvinceId { get; set; }
        [NopResourceDisplayName("Address.Fields.StateProvince")]
        public string StateProvinceName { get; set; }
        [NopResourceDisplayName("Address.Fields.City")]
        public string City { get; set; }
        [NopResourceDisplayName("Address.Fields.Address1")]
        public string Address1 { get; set; }
        [NopResourceDisplayName("Address.Fields.Address2")]
        public string Address2 { get; set; }
        [NopResourceDisplayName("Address.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; set; }
        [NopResourceDisplayName("Address.Fields.PhoneNumber")]
        public string PhoneNumber { get; set; }
        [NopResourceDisplayName("Address.Fields.FaxNumber")]
        public string FaxNumber { get; set; }

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