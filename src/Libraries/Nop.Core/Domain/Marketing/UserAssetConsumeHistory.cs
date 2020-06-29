using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 资产消费表
    /// </summary>
    public partial class UserAssetConsumeHistory : BaseEntity
    {
        /// <summary>
        /// 资产消费明细所属人ID
        /// </summary>
        public int OwnerUserId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 【UserAssetIncomeHistory.Id】收入资产ID
        /// </summary>
        public int? UserAssetIncomeHistoryId { get; set; }
        /// <summary>
        /// 使用的订单Id
        /// </summary>
        public int? UsedWithOrderId { get; set; }
        /// <summary>
        /// 验单人员ID
        /// </summary>
        public int? VerifyUserId { get; set; }
        /// <summary>
        /// 【AssetConsumType】积分/账户收支出方式EnumID
        /// </summary>
        public byte AssetConsumTypeId { get; set; }
        /// <summary>
        /// 积分/账户收支出方式EnumID
        /// </summary>
        public AssetConsumType AssetConsumType
        {
            get => (AssetConsumType)AssetConsumTypeId;
            set => AssetConsumTypeId = (byte)value;
        }
        /// <summary>
        /// 使用金额
        /// </summary>
        public decimal UsedValue { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool Completed { get; set; }
        /// <summary>
        /// 是否无效（购买订单退货或取消时，标注为无效，并备注退单）
        /// </summary>
        public bool IsInvalid { get; set; }
        /// <summary>
        /// 删除（不直接删除）
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

    }
}
