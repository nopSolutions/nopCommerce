using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Core.Domain.Weixin;

namespace Nop.Web.Areas.Admin.Models.Weixin
{
    /// <summary>
    /// Represents a AddUserRelatedUserModel
    /// </summary>
    public partial class AddUserRelatedUserModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Weixin.Users.Fields.OpenId")]
        public string OpenId { get; set; }

        [NopResourceDisplayName("Admin.Weixin.Users.Fields.NickName")]
        public string NickName { get; set; }

        [NopResourceDisplayName("Admin.Weixin.Users.Fields.HeadImgUrl")]
        public string HeadImgUrl { get; set; }

        [NopResourceDisplayName("Admin.Weixin.Users.Fields.Remark")]
        public string Remark { get; set; }

        [NopResourceDisplayName("Admin.Weixin.Users.Fields.SysRemark")]
        public string SysRemark { get; set; }

        [NopResourceDisplayName("Admin.Weixin.Users.Fields.SupplierShopId")]
        public int SupplierShopId { get; set; }

        [NopResourceDisplayName("Admin.Weixin.Users.Fields.Subscribe")]
        public bool Subscribe { get; set; }

        [NopResourceDisplayName("Admin.Weixin.Users.Fields.AllowReferee")]
        public bool AllowReferee { get; set; }
        #endregion
    }
}