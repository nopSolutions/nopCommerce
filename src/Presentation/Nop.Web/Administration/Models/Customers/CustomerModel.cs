using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Customers;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Customers
{
    [Validator(typeof(CustomerValidator))]
    public class CustomerModel : BaseNopEntityModel
    {
        public CustomerModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
        }

        public bool AllowUsersToChangeUsernames { get; set; }
        public bool UsernamesEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Gender")]
        public string Gender { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.LastName")]
        [AllowHtml]
        public string LastName { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.FullName")]
        public string FullName { get; set; }
        
        public bool DateOfBirthEnabled { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        public bool CompanyEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Company")]
        [AllowHtml]
        public string Company { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }
        
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.IsTaxExempt")]
        public bool IsTaxExempt { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Affiliate")]
        public int? AffiliateId { get; set; }




        //time zone
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.TimeZoneId")]
        [AllowHtml]
        public string TimeZoneId { get; set; }

        public bool AllowCustomersToSetTimeZone { get; set; }

        public IList<SelectListItem> AvailableTimeZones { get; set; }





        //EU VAT
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.VatNumber")]
        [AllowHtml]
        public string VatNumber { get; set; }

        public string VatNumberStatusNote { get; set; }

        public bool DisplayVatNumber { get; set; }





        //registration date
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }




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
        [AllowHtml]
        public string AddRewardPointsMessage { get; set; }



        
        //properties used for filtering (customer list page)
        public string SearchCustomerRoleIds { get; set; }
        public string SearchCustomerEmail { get; set; }
        public string SearchCustomerUsername { get; set; }
        

        
        #region Nested classes
        public class RewardPointsHistoryModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Points")]
            public int Points { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.PointsBalance")]
            public int PointsBalance { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Message")]
            [AllowHtml]
            public string Message { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Date")]
            public DateTime CreatedOn { get; set; }
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
            public DateTime CreatedOn { get; set; }

        }

        public class OrderModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.Orders.Order")]
            public override int Id { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.Orders.OrderStatus")]
            public string OrderStatus { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.Orders.PaymentStatus")]
            public string PaymentStatus { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.Orders.ShippingStatus")]
            public string ShippingStatus { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.Orders.OrderTotal")]
            public string OrderTotal { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.Orders.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }
        #endregion
    }
}