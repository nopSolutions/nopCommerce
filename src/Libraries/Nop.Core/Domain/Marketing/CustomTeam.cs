using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 定制团/产品自主拼团
    /// </summary>
    public partial class CustomTeam : BaseEntity
    {
        /// <summary>
        /// 产品id
        /// </summary>
        public int ProductId { get; set; }
            /// <summary>
            /// 【ProductAttributeValue.Id】产品不同规格价格调整ID
            /// </summary>
        public int? ProductAttributeValueId { get; set; }
        /// <summary>
        /// 创建人WUserId
        /// </summary>
        public int? WUserId { get; set; }
        /// <summary>
        /// 团号（数字英文组合，大写，判断重复）
        /// </summary>
        public string GroupCode { get; set; }
        /// <summary>
        /// 最低成团人数
        /// </summary>
        public int MixGroupCount { get; set; }
        /// <summary>
        /// 最高成团人数，超出人数制动结束本团，或新建对外拼团
        /// </summary>
        public int MaxGroupCount { get; set; }
        /// <summary>
        /// 超出人数自动停止拼团，否则自动创建下一个拼团计划，超出人数进入下一个拼团计划
        /// </summary>
        public bool AutoStopGroup { get; set; }
        /// <summary>
        /// 最高拼团价格
        /// </summary>
        public decimal MaxGroupAmount { get; set; }
        /// <summary>
        /// 最低拼团价格
        /// </summary>
        public decimal MinGroupAmount { get; set; }
        /// <summary>
        /// 价格计算步进计算人数
        /// </summary>
        public int StepCount { get; set; }
        /// <summary>
        /// 组团口令，如不想外人入团，可设置组团口令
        /// </summary>
        public string GroupPassword { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 团队特色描述，对自组个性团使用
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 使用条件
        /// </summary>
        public string UseCondition { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 产品如需邮寄，邮寄地址是否限制为同一地址
        /// </summary>
        public bool SameAddress { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool Completed { get; set; }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool Locked { get; set; }
        /// <summary>
        /// 是否发布，如果描述内容不为空，需要审核后才能发布。（是否开团）
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTimeUtc { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime { get; set; }

    }
}
