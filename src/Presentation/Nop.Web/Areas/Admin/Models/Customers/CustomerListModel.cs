using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    public partial class CustomerListModel : BaseNopModel
    {
        public CustomerListModel()
        {
            SearchCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Customers.Customers.List.CustomerRoles")]
        public IList<int> SearchCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchEmail")]
        public string SearchEmail { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchUsername")]
        public string SearchUsername { get; set; }
        public bool UsernamesEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchFirstName")]
        public string SearchFirstName { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchLastName")]
        public string SearchLastName { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchDateOfBirth")]
        public string SearchDayOfBirth { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchDateOfBirth")]
        public string SearchMonthOfBirth { get; set; }
        public bool DateOfBirthEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchCompany")]
        public string SearchCompany { get; set; }
        public bool CompanyEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchPhone")]
        public string SearchPhone { get; set; }
        public bool PhoneEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchZipCode")]
        public string SearchZipPostalCode { get; set; }
        public bool ZipPostalCodeEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchIpAddress")]
        public string SearchIpAddress { get; set; }
    }
}