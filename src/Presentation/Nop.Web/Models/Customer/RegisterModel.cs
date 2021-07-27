using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Customer
{
    public partial class RegisterModel : BaseNopModel
    {
        public RegisterModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            CustomerAttributes = new List<CustomerAttributeModel>();
            GdprConsents = new List<GdprConsentModel>();
        }

        e(DataType.EmailAddress)]
      
        urceDisplayName("Account.Fields.Email")]
        public string Email { get; set; }

    

        ringEmailTwice { get; set; }
        [DataTyp
e(DataType.EmailAddress)]
        [NopRe
        layName("Account.Fields.ConfirmEmail")]
        public string ConfirmEmail { get; set; }

        public bool UsernamesEnabled { get; set; }
        [NopReso
urceDisplayName("Account.Fields.Username")]
        public string Username { get; set; }

        public bool CheckUsernameAvailabilityEnabled { get; set; }

        [DataType(DataType.Password)]
        [NoTri
          [NopRe
        layName("Account.Fields.Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [NoTri
          [NopRe
        layName("Account.Fields.ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [NopReso
urceDisplayName("Account.Fields.Gender")]
        public string Gender { get; set; }

        public bool FirstNameEnabled { get; set; }
        [NopReso
urceDisplayName("Account.Fields.FirstName")]
        public string FirstName { get; set; }
        public b
ool FirstNameRequired { get; set; }
        public bool LastNameEnabled { get; set; }
        [NopReso
urceDisplayName("Account.Fields.LastName")]
        public string LastName { get; set; }
        public b
ool LastNameRequired { get; set; }

        public bool DateOfBirthEnabled { get; set; }
        [NopReso
urceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthDay { get; set; }
        [NopReso
urceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthMonth { get; set; }
        [NopReso
urceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthYear { get; set; }
        public b
ool DateOfBirthRequired { get; set; }
        public D
ateTime? ParseDateOfBirth()
        {
            if (!DateOfBirthYear.HasValue || !DateOfBirthMonth.HasValue || !DateOfBirthDay.HasValue)
                return null;

            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(DateOfBirthYear.Value, DateOfBirthMonth.Value, DateOfBirthDay.Value);
            }
            catch { }
            retu
rn dateOfBirth;
        }

        public bool CompanyEnabled { get; set; }
        public bool CompanyRequired { get; set; }
        [NopReso
urceDisplayName("Account.Fields.Company")]
        public string Company { get; set; }

        public bool StreetAddressEnabled { get; set; }
        public bool StreetAddressRequired { get; set; }
        [NopReso
urceDisplayName("Account.Fields.StreetAddress")]
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        public bool StreetAddress2Required { get; set; }
        [NopReso
urceDisplayName("Account.Fields.StreetAddress2")]
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        public bool ZipPostalCodeRequired { get; set; }
        [NopReso
urceDisplayName("Account.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; set; }

        public bool CityEnabled { get; set; }
        public bool CityRequired { get; set; }
        [NopReso
urceDisplayName("Account.Fields.City")]
        public string City { get; set; }

        public bool CountyEnabled { get; set; }
        public bool CountyRequired { get; set; }
        [NopReso
urceDisplayName("Account.Fields.County")]
        public string County { get; set; }

        public bool CountryEnabled { get; set; }
        public bool CountryRequired { get; set; }
        [NopReso
urceDisplayName("Account.Fields.Country")]
        public int CountryId { get; set; }
        public I
List<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }
        public bool StateProvinceRequired { get; set; }
        [NopReso
urceDisplayName("Account.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        public I
List<SelectListItem> AvailableStates { get; set; }

        public bool PhoneEnabled { get; set; }
        public bool PhoneRequired { get; set; }
        [DataTyp
e(DataType.PhoneNumber)]
        [NopRe
        layName("Account.Fields.Phone")]
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }
        public bool FaxRequired { get; set; }
        [DataTyp
e(DataType.PhoneNumber)]
        [NopRe
        layName("Account.Fields.Fax")]
        public string Fax { get; set; }

        publ

        abled { get; set; }
        [NopResourceDisp
layName("Account.Fields.Newsletter")]
        public bool Newsletter { get; set; }

        public bool 

        nabled { get; set; }
        public bool AcceptPrivacyPolicyPopup { get; set; }

        //time zone
        [NopResourceDisplayName("Account.Fields.TimeZone")]
        public string TimeZoneId { get; set; }
        public bool AllowCustome
rsToSetTimeZone { get; set; }
        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [NopResourceDisplayName("Account.Fields.VatNumber")]
        public string VatNumber { get; set; }
        public bool DisplayVatNu
mber { get; set; }

        public bool HoneypotEnabled { get; set; }
        public bool DisplayCaptcha { get; set; }

        public IList<CustomerAttributeModel> CustomerAttributes { get; set; }

        public IList<GdprConsentModel> GdprConsents { get; set; }
    }
}