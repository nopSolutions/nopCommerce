using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 分销人/合伙人申请信息表，没有申请的不参与佣金提成
    /// </summary>
    public partial class PartnerApplicationForm : BaseEntity
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
        /// 手机号码(修改任何信息都需要验证)
        /// </summary>
        public string TelephoneNumber { get; set; }
        /// <summary>
        /// 收款微信号（需要验证）
        /// </summary>
        public string WechatAccount { get; set; }
        /// <summary>
        /// 收款支付宝账号（需要验证）
        /// </summary>
        public string AlipayAccount { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 银行账号（姓名同Name值）
        /// </summary>
        public string BankAccount { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? EndDateTimeUtc { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime { get; set; }
        /// <summary>
        /// 状态（预留）
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 是否已经审核
        /// </summary>
        public bool Approved { get; set; }
        /// <summary>
        /// 锁定
        /// </summary>
        public bool Locked { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }

    }
}
