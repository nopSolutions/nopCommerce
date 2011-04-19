using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public class CustomerInfoModel : BaseNopModel
    {
        public CustomerInfoModel()
        {
            this.AvailableTimeZones = new List<SelectListItem>();
        }
        //TODO localize attribute
        //TODO add validation rules

        [DisplayName("Email")]
        public string Email { get; set; }

        public bool AllowUsersToChangeUsernames { get; set; }
        public bool UsernamesEnabled { get; set; }
        [DisplayName("User name")]
        public string Username { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [DisplayName("Gender")]
        public string Gender { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }
        [DisplayName("Last name")]
        public string LastName { get; set; }

        public bool CompanyEnabled { get; set; }
        [DisplayName("Company name")]
        public string Company { get; set; }
        
        //time zone
        [DisplayName("Time zone")]
        public string TimeZoneId { get; set; }
        public bool AllowCustomersToSetTimeZone { get; set; }
        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [DisplayName("Vat number")]
        public string VatNumber { get; set; }
        public string VatNumberStatusNote { get; set; }
        public bool DisplayVatNumber { get; set; }

        public CustomerNavigationModel NavigationModel { get; set; }
    }
}