using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public class CustomerAvatarModel : BaseNopModel
    {
        public string AvatarUrl { get; set; }
        public CustomerNavigationModel NavigationModel { get; set; }
    }
}