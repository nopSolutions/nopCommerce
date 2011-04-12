using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models
{
    [Validator(typeof(CustomerValidator))]
    public class CustomerModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Gender")]
        public string Gender { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.FirstName")]
        public string FirstName { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.LastName")]
        public string LastName { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.FullName")]
        public string FullName { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.AdminComment")]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.IsTaxExempt")]
        public bool IsTaxExempt { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.VatNumber")]
        public string VatNumber { get; set; }

        public int VatNumberStatusId { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.TimeZoneId")]
        public string TimeZoneId { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.RegistrationDate")]
        public DateTime CreatedOnUtc { get; set; }
        
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CustomerRoles")]
        public string CustomerRoleNames { get; set; }

        public int? AssociatedUserId { get; set; }

        public string AssociatedUserEmail { get; set; }
    }
}