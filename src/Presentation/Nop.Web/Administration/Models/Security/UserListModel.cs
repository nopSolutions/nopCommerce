using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Security
{
    public class UserListModel : BaseNopModel
    {
        public GridModel<UserModel> Users { get; set; }

        [NopResourceDisplayName("Admin.Users.List.SearchEmail")]
        [AllowHtml]
        public string SearchEmail { get; set; }

        [NopResourceDisplayName("Admin.Users.List.SearchUsername")]
        [AllowHtml]
        public string SearchUsername { get; set; }
        
        public bool UsernamesEnabled { get; set; }
    }
}