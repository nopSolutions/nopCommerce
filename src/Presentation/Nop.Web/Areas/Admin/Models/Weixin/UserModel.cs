using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Core.Domain.Weixin;

namespace Nop.Web.Areas.Admin.Models.Weixin
{
    /// <summary>
    /// Represents a user model
    /// </summary>
    public partial class UserModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Weixin.Users.Fields.OpenId")]
        public string OpenId { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.RefereeId")]
        public int RefereeId { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.WConfigId")]
        public int WConfigId { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.OpenIdHash")]
        public long OpenIdHash { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.UnionId")]
        public string UnionId { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.NickName")]
        public string NickName { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.Province")]
        public string Province { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.City")]
        public string City { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.Country")]
        public string Country { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.HeadImgUrl")]
        public string HeadImgUrl { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.Remark")]
        public string Remark { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.SysRemark")]
        public string SysRemark { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.GroupId")]
        public string GroupId { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.TagIdList")]
        public string TagIdList { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.Sex")]
        public byte Sex { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.CheckInTypeId")]
        public byte CheckInTypeId { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.LanguageTypeId")]
        public byte LanguageTypeId { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.SubscribeSceneTypeId")]
        public byte SubscribeSceneTypeId { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.RoleTypeId")]
        public byte RoleTypeId { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.SceneTypeId")]
        public byte SceneTypeId { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.Status")]
        public byte Status { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.SupplierShopId")]
        public int SupplierShopId { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.QrScene")]
        public int? QrScene { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.QrSceneStr")]
        public string QrSceneStr { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.Subscribe")]
        public bool Subscribe { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.AllowReferee")]
        public bool AllowReferee { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.AllowResponse")]
        public bool AllowResponse { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.AllowOrder")]
        public bool AllowOrder { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.AllowNotice")]
        public bool AllowNotice { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.AllowOrderNotice")]
        public bool AllowOrderNotice { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.InBlackList")]
        public bool InBlackList { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.Deleted")]
        public bool Deleted { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.SubscribeTime")]
        [UIHint("DateTimeNullable")]
        public DateTime? SubscribeTime { get; set; }

        [NopResourceDisplayName("Admin.Weixin.Users.Fields.UnSubscribeTime")]
        [UIHint("DateTimeNullable")]
        public DateTime? UnSubscribeTime { get; set; }

        [NopResourceDisplayName("Admin.Weixin.Users.Fields.UpdateTime")]
        [UIHint("DateTimeNullable")]
        public DateTime? UpdateTime { get; set; }
        [NopResourceDisplayName("Admin.Weixin.Users.Fields.CreatTime")]
        public DateTime CreatTime { get; set; }

        #endregion
    }
}