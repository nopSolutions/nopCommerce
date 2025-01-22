using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.PageNotFound
{
    public partial record PageNotFoundSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.System.PageNotFound.List.Slug")]
        public string Slug { get; set; }

        [NopResourceDisplayName("Admin.System.PageNotFound.List.CustomerEmail")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.System.PageNotFound.List.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [NopResourceDisplayName("Admin.System.PageNotFound.List.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [NopResourceDisplayName("Admin.System.PageNotFound.List.IpAddress")]
        public string IpAddress { get; set; }
    }
}