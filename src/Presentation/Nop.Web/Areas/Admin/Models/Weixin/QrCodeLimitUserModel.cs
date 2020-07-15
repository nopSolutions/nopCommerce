using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Core.Domain.Weixin;

namespace Nop.Web.Areas.Admin.Models.Weixin
{
    /// <summary>
    /// Represents a QrCodeLimitUserModel
    /// </summary>
    public partial class QrCodeLimitUserModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitUsers.Fields.UserId")]
        public int UserId { get; set; }
        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitUsers.Fields.QrCodeLimitId")]
        public int QrCodeLimitId { get; set; }
        /// <summary>
        /// 新：
        /// </summary>
        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitUsers.Fields.HeadImageUrl")]
        public string HeadImageUrl { get; set; }
        /// <summary>
        /// 如果为空，调用WUser表 NickName+Remark
        /// </summary>
        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitUsers.Fields.UserName")]
        public string UserName { get; set; }
        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitUsers.Fields.Description")]
        public string Description { get; set; }
        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitUsers.Fields.TelNumber")]
        public string TelNumber { get; set; }
        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitUsers.Fields.AddressInfo")]
        public string AddressInfo { get; set; }
        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitUsers.Fields.ExpireTime")]
        [UIHint("DateTime")]
        public DateTime ExpireTime { get; set; }
        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitUsers.Fields.Published")]
        public bool Published { get; set; }


        #endregion
    }
}