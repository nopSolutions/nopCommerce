using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Common;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Common
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

        [NopResourceDisplayName("Admin.Address.Fields.FirstName")]
        public string FirstName { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.LastName")]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Admin.Address.Fields.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.Company")]
        public string Company { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.Country")]
        public int? CountryId { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.Country")]
        public string CountryName { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.StateProvince")]
        public int? StateProvinceId { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.StateProvince")]
        public string StateProvinceName { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.City")]
        public string City { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.Address1")]
        public string Address1 { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.Address2")]
        public string Address2 { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        [NopResourceDisplayName("Admin.Address.Fields.PhoneNumber")]
        public string PhoneNumber { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.FaxNumber")]
        public string FaxNumber { get; set; }

        //address in HTML format (usually used in grids)
        [NopResourceDisplayName("Admin.Address")]
        public string AddressHtml { get; set; }

        //formatted custom address attributes
        public string FormattedCustomAddressAttributes { get; set; }
        public IList<AddressAttributeModel> CustomAddressAttributes { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public bool FirstNameEnabled { get; set; }
        public bool FirstNameRequired { get; set; }
        public bool LastNameEnabled { get; set; }
        public bool LastNameRequired { get; set; }
        public bool EmailEnabled { get; set; }
        public bool EmailRequired { get; set; }
        public bool CompanyEnabled { get; set; }
        public bool CompanyRequired { get; set; }
        public bool CountryEnabled { get; set; }
        public bool CountryRequired { get; set; }
        public bool StateProvinceEnabled { get; set; }
        public bool CityEnabled { get; set; }
        public bool CityRequired { get; set; }
        public bool StreetAddressEnabled { get; set; }
        public bool StreetAddressRequired { get; set; }
        public bool StreetAddress2Enabled { get; set; }
        public bool StreetAddress2Required { get; set; }
        public bool ZipPostalCodeEnabled { get; set; }
        public bool ZipPostalCodeRequired { get; set; }
        public bool PhoneEnabled { get; set; }
        public bool PhoneRequired { get; set; }
        public bool FaxEnabled { get; set; }
        public bool FaxRequired { get; set; }

        #region Nested classes

        public partial class AddressAttributeModel : BaseNopEntityModel
        {
            public AddressAttributeModel()
            {
                Values = new List<AddressAttributeValueModel>();
            }

            public string Name { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Selected value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<AddressAttributeValueModel> Values { get; set; }
        }

        public partial class AddressAttributeValueModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }
        }

        #endregion
    }
}