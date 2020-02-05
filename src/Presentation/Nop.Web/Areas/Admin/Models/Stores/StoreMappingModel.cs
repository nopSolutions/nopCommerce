#region Extensions by QuanNH

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Stores
{
    public partial class StoreMappingModel : BaseNopEntityModel
    {
        public StoreMappingModel()
        {
            AvailableCustomers = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.System.StoreMapping.UserName")]
        public string UserName { get; set; }

        [NopResourceDisplayName("Admin.System.StoreMapping.StoreName")]
        public string StoreName { get; set; }

        [NopResourceDisplayName("Admin.System.StoreMapping.StoreUrl")]
        public string StoreUrl { get; set; }

        [NopResourceDisplayName("Admin.System.StoreMapping.EntityId")]
        public int EntityId { get; set; }

        [NopResourceDisplayName("Admin.System.StoreMapping.EntityName")]
        public string EntityName { get; set; }

        [NopResourceDisplayName("Admin.System.StoreMapping.StoreId")]
        public int StoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailableCustomers { get; set; } 
    }
}
#endregion