using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Core.Domain.Weixin;

namespace Nop.Web.Areas.Admin.Models.Weixin
{
    /// <summary>
    /// Represents a QrCodeLimitModel
    /// </summary>
    public partial class QrCodeLimitBindingSourceModel : BaseNopEntityModel
    {
        public QrCodeLimitBindingSourceModel()
        {
            AvailableSceneTypes = new List<SelectListItem>();
            AvailableMessageTypes = new List<SelectListItem>();
        }

        #region Properties

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.QrCodeLimitId")]
        public int QrCodeLimitId { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.SupplierId")]
        public int SupplierId { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.SupplierShopId")]
        public int SupplierShopId { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.ProductId")]
        public int ProductId { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.Address")]
        public string Address { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.MarketingAdvertWayId")]
        public int MarketingAdvertWayId { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.UseFixUrl")]
        public bool UseFixUrl { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.Url")]
        public string Url { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.MessageResponse")]
        public bool MessageResponse { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.WSceneTypeId")]
        public byte WSceneTypeId { get; set; }
        public IList<SelectListItem> AvailableSceneTypes { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.MessageTypeId")]
        public byte MessageTypeId { get; set; }
        public IList<SelectListItem> AvailableMessageTypes { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.Content")]
        public string Content { get; set; }

        [NopResourceDisplayName("Admin.Weixin.QrCodeLimitBindingSources.Fields.UseBindingMessage")]
        public bool UseBindingMessage { get; set; }

        #endregion
    }
}