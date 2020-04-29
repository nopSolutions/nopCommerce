using System;

namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WUserAsset
    /// </summary>
    public partial class WUserAsset : BaseEntity
    {
        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 用户等级
        /// </summary>
        public byte UserLevel { get; set; }
        /// <summary>
        /// 用户活跃度
        /// </summary>
        public byte ActiveLevel { get; set; }
        /// <summary>
        /// 用户折扣百分比
        /// </summary>
        public byte Discount { get; set; }
        /// <summary>
        /// 虚拟币
        /// </summary>
        public decimal VirtualCurrency { get; set; }
        /// <summary>
        /// 用户总金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 实物卡
        /// </summary>
        public decimal PhysicalCard { get; set; }
        /// <summary>
        /// 虚拟卡
        /// </summary>
        public decimal VirtualCard { get; set; }
        /// <summary>
        /// 用户总积分
        /// </summary>
        public int Point { get; set; }
        /// <summary>
        /// 总分享次数
        /// </summary>
        public int ShareCount { get; set; }
        /// <summary>
        /// 升级经验值
        /// </summary>
        public int Exp { get; set; }
        /// <summary>
        /// 结算日期，结算后，结算日期前的明细表可以做删除标记或删除
        /// </summary>
        public DateTime? BalanceDate { get; set; }
        /// <summary>
        /// 删除标志，不允许直接删除
        /// </summary>
        public bool Deleted { get; set; }

    }
}
