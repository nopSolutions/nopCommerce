using System;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 店铺基本信息
    /// </summary>
    public partial class SupplierShop : BaseEntity
    {
        /// <summary>
        /// 【Supplier.Id】供应商ID
        /// </summary>
        public int SupplierId { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 店铺描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 店铺内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 店铺门图，横图/大图
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 店铺缩微图，方图/小图
        /// </summary>
        public string ThumbImageUrl { get; set; }
        /// <summary>
        /// 所在国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        public string District { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 店铺联系电话
        /// </summary>
        public string ContactNumber { get; set; }
        /// <summary>
        ///  店铺二维码
        /// </summary>
        public string QrCodeUrl { get; set; }
        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public decimal Latitude { get; set; }
        /// <summary>
        /// 地理位置经度
        /// </summary>
        public decimal Longitude { get; set; }
        /// <summary>
        /// 地理位置精度
        /// </summary>
        public decimal Precision { get; set; }
        /// <summary>
        /// 营业时间
        /// </summary>
        public string OpenTime { get; set; }
        /// <summary>
        /// 主要用于店铺临时对访客的消息提醒，如放假通知等。
        /// </summary>
        public string Notices { get; set; }
        /// <summary>
        /// 状态（保留字段，是否营业，是否审核等）
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }

    }
}
