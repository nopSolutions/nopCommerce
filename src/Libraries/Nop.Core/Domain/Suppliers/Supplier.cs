using System;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 商品营销标签分类
    /// </summary>
    public partial class Supplier : BaseEntity
    {
        public int StoreId { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  所在国家
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
        /// 联系电话
        /// </summary>
        public string ContactNumber { get; set; }
        /// <summary>
        /// 所属行业代码
        /// </summary>
        public string Industry { get; set; }
        /// <summary>
        /// Qr二维码地址
        /// </summary>
        public string QrCodeUrl { get; set; }
        /// <summary>
        /// 营业执照图
        /// </summary>
        public string BusinessLicenseUrl { get; set; }
        /// <summary>
        /// 营业执照号
        /// </summary>
        public string BusinessLicenseCode { get; set; }
        /// <summary>
        /// 税务登记证图
        /// </summary>
        public string TaxLicenseUrl { get; set; }
        /// <summary>
        /// 税务登记证号
        /// </summary>
        public string TaxLicenseCode { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTimeUtc { get; set; }
        /// <summary>
        /// 合作类型：年费，免费，季度等（预留）
        /// </summary>
        public byte CooperationTypeId { get; set; }
        /// <summary>
        /// 店铺最大数量
        /// </summary>
        public int MaxShopCount { get; set; }
        /// <summary>
        /// 分组管理最大数量
        /// </summary>
        public int MaxGroupCount { get; set; }
        /// <summary>
        /// 每个店铺环境图最大数量
        /// </summary>
        public int MaxImageCount { get; set; }
        /// <summary>
        /// 每个店铺员工信息最大数量
        /// </summary>
        public int MaxStaffCount { get; set; }
        /// <summary>
        /// 每个店铺产品最大数量
        /// </summary>
        public int MaxProductCount { get; set; }
        /// <summary>
        /// 状态（保留字段，审核状态，过期状态等）
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 是否个人店铺
        /// </summary>
        public bool IsPersonal { get; set; }
        /// <summary>
        /// 发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        ///  删除
        /// </summary>
        public bool Deleted { get; set; }
    }
}
