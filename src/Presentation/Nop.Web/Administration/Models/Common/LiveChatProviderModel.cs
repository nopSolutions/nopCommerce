using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Common
{
    public class LiveChatProviderModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Configuration.LiveChats.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.LiveChats.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.LiveChats.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Configuration.LiveChats.Fields.IsActive")]
        public bool IsActive { get; set; }





        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}