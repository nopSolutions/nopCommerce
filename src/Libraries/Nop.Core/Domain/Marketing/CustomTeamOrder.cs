using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 产品开团用户订单信息表
    /// </summary>
    public partial class CustomTeamOrder : BaseEntity
    {
        /// <summary>
        /// 【CustomTeam.Id】定制团ID
        /// </summary>
        public int CustomTeamId { get; set; }
        /// <summary>
        /// 购买订单ID
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 购买数量（一单购买多个）
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 购买用户ID
        /// </summary>
        public int WUserId { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 多个电话，用逗号或空格分开
        /// </summary>
        public string ContactNumber { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 状态：待支付=0，已支付=1，待发货=2，已发货=3，已收货=4，已完成=5
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 退单状态：无退单=0，退单中=1，已退单=2 
        /// </summary>
        public byte ReturnBack { get; set; }
        /// <summary>
        /// 是否锁定，锁定用户表示合同成立，不再退钱。
        /// </summary>
        public bool Locked { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 锁单时间
        /// </summary>
        public DateTime? LockeTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime { get; set; }

    }
}
