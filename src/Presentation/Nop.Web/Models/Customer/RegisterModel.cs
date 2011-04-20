using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Customer;

namespace Nop.Web.Models.Customer
{
    [Validator(typeof(RegisterValidator))]
    public class RegisterModel : BaseNopModel
    {
        public RegisterModel()
        {
            this.AvailableTimeZones = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Account.Fields.Email")]
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.Username")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [NopResourceDisplayName("Account.Fields.Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [NopResourceDisplayName("Account.Fields.ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.Gender")]
        public string Gender { get; set; }

        [NopResourceDisplayName("Account.Fields.FirstName")]
        public string FirstName { get; set; }
        [NopResourceDisplayName("Account.Fields.LastName")]
        public string LastName { get; set; }

        public bool CompanyEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.Company")]
        public string Company { get; set; }
        
        //time zone
        [NopResourceDisplayName("Account.Fields.TimeZone")]
        public string TimeZoneId { get; set; }
        public bool AllowCustomersToSetTimeZone { get; set; }
        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [NopResourceDisplayName("Account.Fields.VatNumber")]
        public string VatNumber { get; set; }
        public string VatNumberStatusNote { get; set; }
        public bool DisplayVatNumber { get; set; }
    }
}