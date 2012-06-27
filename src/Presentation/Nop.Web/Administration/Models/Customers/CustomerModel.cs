using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Customers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Customers
{
    [Validator(typeof(CustomerValidator))]
    public partial class CustomerModel : BaseNopEntityModel
    {
        public CustomerModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
            SendEmail = new SendEmailModel();
            SendPm = new SendPmModel();
            AvailableCustomerRoles = new List<CustomerRoleModel>();
            AssociatedExternalAuthRecords = new List<AssociatedExternalAuthModel>();
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
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

        public bool StreetAddressEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.StreetAddress")]
        [AllowHtml]
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.StreetAddress2")]
        [AllowHtml]
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.ZipPostalCode")]
        [AllowHtml]
        public string ZipPostalCode { get; set; }

        public bool CityEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.City")]
        [AllowHtml]
        public string City { get; set; }

        public bool CountryEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Country")]
        public int CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public bool PhoneEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Phone")]
        [AllowHtml]
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Fax")]
        [AllowHtml]
        public string Fax { get; set; }
        






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

        //IP adderss
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.IPAddress")]
        public string LastIpAddress { get; set; }


        [NopResourceDisplayName("Admin.Customers.Customers.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }


        //customer roles
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CustomerRoles")]
        public string CustomerRoleNames { get; set; }
        public List<CustomerRoleModel> AvailableCustomerRoles { get; set; }
        public int[] SelectedCustomerRoleIds { get; set; }
        public bool AllowManagingCustomerRoles { get; set; }






        //reward points history
        public bool DisplayRewardPointsHistory { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsValue")]
        public int AddRewardPointsValue { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsMessage")]
        [AllowHtml]
        public string AddRewardPointsMessage { get; set; }
        
        //send email model
        public SendEmailModel SendEmail { get; set; }
        //send PM model
        public SendPmModel SendPm { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.AssociatedExternalAuth")]
        public IList<AssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; }

        
        #region Nested classes

        public partial class AssociatedExternalAuthModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.AssociatedExternalAuth.Fields.Email")]
            public string Email { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.AssociatedExternalAuth.Fields.ExternalIdentifier")]
            public string ExternalIdentifier { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.AssociatedExternalAuth.Fields.AuthMethodName")]
            public string AuthMethodName { get; set; }
        }

        public partial class RewardPointsHistoryModel : BaseNopEntityModel
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

        public partial class SendEmailModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.SendEmail.Subject")]
            [AllowHtml]
            public string Subject { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.SendEmail.Body")]
            [AllowHtml]
            public string Body { get; set; }
        }

        public partial class SendPmModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.SendPM.Subject")]
            public string Subject { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.SendPM.Message")]
            public string Message { get; set; }
        }

        public partial class OrderModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.Orders.ID")]
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

        public partial class ActivityLogModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.ActivityLog.ActivityLogType")]
            public string ActivityLogTypeName { get; set; }
            [NopResourceDisplayName("Admin.Customers.Customers.ActivityLog.Comment")]
            public string Comment { get; set; }
            [NopResourceDisplayName("Admin.Customers.Customers.ActivityLog.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }
        #endregion
    }
}