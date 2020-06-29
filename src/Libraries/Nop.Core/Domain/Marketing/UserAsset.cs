using System;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// Represents an UserAsset
    /// </summary>
    public partial class UserAsset : BaseEntity
    {
        /// <summary>
        /// OwnerUserId【WUser.Id】
        /// </summary>
        public int OwnerUserId { get; set; }

        /// <summary>
        /// 合伙人备注信息
        /// </summary>
        public string PartnerRemark { get; set; }

        /// <summary>
        /// 合伙人等级
        /// </summary>
        public byte PartnerLevel { get; set; }
        /// <summary>
        /// 用户等级VIP等级
        /// </summary>
        public byte UserLevel { get; set; }
        /// <summary>
        /// 用户活跃度
        /// </summary>
        public byte ActiveLevel { get; set; }
        /// <summary>
        /// 年卡会员等级
        /// </summary>
        public byte AnnualCardLevel { get; set; }
        /// <summary>
        /// 年卡会员卡充值金额
        /// </summary>
        public decimal AnnualCardAmount { get; set; }
        /// <summary>
        /// 用户折扣百分比
        /// </summary>
        public decimal Discount { get; set; }
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
        /// 年卡会员过期时间
        /// </summary>
        public DateTime? AnnualCardExpDate { get; set; }
        /// <summary>
        /// 是否允许自己生成抵用券（要赠送的人有未使用的该券时，不能再赠送）
        /// </summary>
        public bool AllowCreateVoucher { get; set; }
        /// <summary>
        /// 是否允许自己生成折扣券（要赠送的人有未使用的该券时，不能再赠送）
        /// </summary>
        public bool AllowCreateCoupon { get; set; }
        /// <summary>
        /// 默认：False，是否允许为非自己的推荐人生成（要赠送的人有未使用的该券时，不能再赠送）
        /// </summary>
        public bool AllowCreateNotReference { get; set; }

        /// <summary>
        /// 是否允许获取推广佣金
        /// </summary>
        public bool CommissionAllowed { get; set; }

        /// <summary>
        /// 是否展示资产信息
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// 删除标志，不允许直接删除
        /// </summary>
        public bool Deleted { get; set; }

    }
}
