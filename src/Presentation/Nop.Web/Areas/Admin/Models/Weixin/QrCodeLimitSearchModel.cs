using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Weixin
{
    /// <summary>
    /// Represents a user search model
    /// </summary>
    public partial class QrCodeLimitSearchModel : BaseSearchModel
    {
        #region Ctor

        public QrCodeLimitSearchModel()
        {
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimits.List.WConfigId")]
        public int WConfigId { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimits.List.WQrCodeCategoryId")]
        public int WQrCodeCategoryId { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimits.List.WQrCodeChannelId")]
        public int WQrCodeChannelId { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimits.List.SearchSysName")]
        public string SearchSysName { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimits.List.SearchFixedUse")]
        public bool? SearchFixedUse { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimits.List.SearchHasCreated")]
        public bool? SearchHasCreated { get; set; }

        #endregion
    }
}