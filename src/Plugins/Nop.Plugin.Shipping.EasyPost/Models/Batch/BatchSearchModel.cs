using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.EasyPost.Models.Batch
{
    /// <summary>
    /// Represents a batch search model
    /// </summary>
    public record BatchSearchModel : BaseSearchModel
    {
        #region Ctor

        public BatchSearchModel()
        {
            AvailableStatuses = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Batch.Search.Status")]
        public int StatusId { get; set; }
        public IList<SelectListItem> AvailableStatuses { get; set; }

        #endregion
    }
}