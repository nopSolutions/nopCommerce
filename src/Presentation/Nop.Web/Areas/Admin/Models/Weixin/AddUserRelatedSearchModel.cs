using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Weixin
{
    /// <summary>
    /// Represents a AddUserRelatedSearchModel
    /// </summary>
    public partial class AddUserRelatedSearchModel : BaseSearchModel
    {
        #region Ctor

        public AddUserRelatedSearchModel()
        {
            AvailableSubscribe = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Weixin.Users.List.SearchUserNickName")]
        public string SearchUserNickName { get; set; }

        [NopResourceDisplayName("Admin.Weixin.Users.List.SearchUserRemark")]
        public string SearchUserRemark { get; set; }

        public IList<SelectListItem> AvailableSubscribe { get; set; }

        #endregion
    }
}