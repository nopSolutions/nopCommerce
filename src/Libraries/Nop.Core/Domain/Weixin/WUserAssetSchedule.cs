using System;

namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WUserAssetSchedule
    /// </summary>
    public partial class WUserAssetSchedule : BaseEntity
    {
        /// <summary>
        /// Openid
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 订单号/流水号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 积分/消费值（消费为负数，收入为正数）
        /// </summary>
        public decimal Value { get; set; }
        /// <summary>
        /// 资产类型EnumID
        /// </summary>
        public byte AssetTypeId { get; set; }
        /// <summary>
        /// 资产类型EnumID
        /// </summary>
        public WAssetType AssetType
        {
            get => (WAssetType)AssetTypeId;
            set => AssetTypeId = (byte)value;
        }
        /// <summary>
        /// 积分/账户收支方式
        /// </summary>
        public byte AssetConsumTypeId { get; set; }
        /// <summary>
        /// 积分/账户收支方式
        /// </summary>
        public WAssetConsumType AssetConsumType
        {
            get => (WAssetConsumType)AssetConsumTypeId;
            set => AssetConsumTypeId = (byte)value;
        }
        /// <summary>
        /// 是否未确定
        /// </summary>
        public bool Pending { get; set; }
        /// <summary>
        /// 已使用
        /// </summary>
        public bool Used { get; set; }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool Locked { get; set; }
        /// <summary>
        /// 已过期
        /// </summary>
        public bool Expired { get; set; }
        /// <summary>
        /// 删除（不直接删除）
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 可以使用的开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpireTime { get; set; }
        /// <summary>
        /// 使用时间/消费时间
        /// </summary>
        public DateTime? UseTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime { get; set; }

    }
}
