using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Weixin
{
    /// <summary>
    /// Represents a user search model
    /// </summary>
    public partial class UserSearchModel : BaseSearchModel
    {
        #region Ctor

        public UserSearchModel()
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