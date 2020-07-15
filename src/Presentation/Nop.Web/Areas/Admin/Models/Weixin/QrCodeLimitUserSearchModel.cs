using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Weixin
{
    /// <summary>
    /// Represents a QrCodeLimit User SearchModel
    /// </summary>
    public partial class QrCodeLimitUserSearchModel : BaseSearchModel
    {

        #region Properties

        public int UserId { get; set; }

        public int QrCodeLimitId { get; set; }

        #endregion
    }
}