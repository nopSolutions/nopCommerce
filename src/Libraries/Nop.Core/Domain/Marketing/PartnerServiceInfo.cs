using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 合伙人自主服务信息（合伙人自己作为售后客服等信息）
    /// </summary>
    public partial class PartnerServiceInfo : BaseEntity
    {
        /// <summary>
        /// 微信用户ID
        /// </summary>
        public int WUserId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string SelfNumber { get; set; }
        /// <summary>
        /// 服务区域范围
        /// </summary>
        public string ServiceArea { get; set; }
        /// <summary>
        /// 服务内容范围
        /// </summary>
        public string ServiceContent { get; set; }
        /// <summary>
        /// 职务
        /// </summary>
        public string JobTitle { get; set; }
        /// <summary>
        /// 服务星级
        /// </summary>
        public byte Stars { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImageUrl { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>
        public string WeChat { get; set; }
        /// <summary>
        /// 个人二维码
        /// </summary>
        public string QrCodeUrl { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string TelNumber { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }
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
        /// 发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }


    }
}
