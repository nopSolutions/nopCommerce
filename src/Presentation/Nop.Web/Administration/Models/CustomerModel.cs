using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Models
{
    [Validator(typeof(CustomerValidator))]
    public class CustomerModel : BaseNopEntityModel
    {
        public CustomerModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
        }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Gender")]
        public string Gender { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.FirstName")]
        public string FirstName { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.LastName")]
        public string LastName { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.FullName")]
        public string FullName { get; set; }


        public bool DateOfBirthEnabled { get; set; }
        [UIHint("Date")]
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        public bool CompanyEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Company")]
        public string Company { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.AdminComment")]
        public string AdminComment { get; set; }
        
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.IsTaxExempt")]
        public bool IsTaxExempt { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Active")]
        public bool Active { get; set; }

        //time zone
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.TimeZoneId")]
        public string TimeZoneId { get; set; }
        public bool AllowCustomersToSetTimeZone { get; set; }
        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.VatNumber")]
        public string VatNumber { get; set; }
        public string VatNumberStatusNote { get; set; }
        public bool DisplayVatNumber { get; set; }

        //registration date
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CreatedOn")]
        public DateTime CreatedOnUtc { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CreatedOn")]
        public string CreatedOnStr { get; set; }
        
        //customer roles
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CustomerRoles")]
        public string CustomerRoleNames { get; set; }
        public List<CustomerRole> AvailableCustomerRoles { get; set; }
        public int[] SelectedCustomerRoleIds { get; set; }

        //reward points history
        public bool DisplayRewardPointsHistory { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsValue")]
        public int AddRewardPointsValue { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsMessage")]
        public string AddRewardPointsMessage { get; set; }

        //user accounts
        public string UserEmailStartsWith { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.UserAccount.AssociatedUser")]
        public int? AssociatedUserId { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.UserAccount.AssociatedUser")]
        public string AssociatedUserEmail { get; set; }


        //properties used for filtering (customer list page)
        public string SearchCustomerRoleIds { get; set; }

        #region Nested classes
        public class RewardPointsHistoryModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Points")]
            public int Points { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.PointsBalance")]
            public int PointsBalance { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Message")]
            public string Message { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Date")]
            public string CreatedOnStr { get; set; }
        }

        public class UserAccountModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.UserAccount.Fields.Email")]
            public string Email { get; set; }
            [NopResourceDisplayName("Admin.Customers.Customers.UserAccount.Fields.IsApproved")]
            public bool IsApproved { get; set; }
            [NopResourceDisplayName("Admin.Customers.Customers.UserAccount.Fields.IsLockedOut")]
            public bool IsLockedOut { get; set; }
            [NopResourceDisplayName("Admin.Customers.Customers.UserAccount.Fields.CreatedOn")]
            public string CreatedOnStr { get; set; }

        }
        #endregion
    }
}