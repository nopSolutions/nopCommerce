using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Customers
{
    public partial class CustomerListModel : BaseNopModel
    {
        public GridModel<CustomerModel> Customers { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.CustomerRoles")]
        [AllowHtml]
        public List<CustomerRoleModel> AvailableCustomerRoles { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.CustomerRoles")]
        public int[] SearchCustomerRoleIds { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchEmail")]
        [AllowHtml]
        public string SearchEmail { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchUsername")]
        [AllowHtml]
        public string SearchUsername { get; set; }
        public bool UsernamesEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchFirstName")]
        [AllowHtml]
        public string SearchFirstName { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchLastName")]
        [AllowHtml]
        public string SearchLastName { get; set; }


        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchDateOfBirth")]
        [AllowHtml]
        public string SearchDayOfBirth { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchDateOfBirth")]
        [AllowHtml]
        public string SearchMonthOfBirth { get; set; }
        public bool DateOfBirthEnabled { get; set; }



        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchCompany")]
        [AllowHtml]
        public string SearchCompany { get; set; }
        public bool CompanyEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchPhone")]
        [AllowHtml]
        public string SearchPhone { get; set; }
        public bool PhoneEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchZipCode")]
        [AllowHtml]
        public string SearchZipPostalCode { get; set; }
        public bool ZipPostalCodeEnabled { get; set; }
    }
}