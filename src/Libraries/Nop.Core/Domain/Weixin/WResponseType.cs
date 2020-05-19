namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a ResponseType
    /// </summary>
    public enum WResponseType : byte
    {
        /// <summary>
        /// 被动方式回复，回复方式：被动方式 passive，客服方式 custom
        /// </summary>
        Passive = 1,
        /// <summary>
        /// 客服方式回复消息,客服方式 custom
        /// </summary>
        Custom = 2,
        /// <summary>
        /// 模板方式回复消息
        /// </summary>
        Template = 3,
        /// <summary>
        /// 订阅方式回复消息
        /// </summary>
        Subscribe = 4,
        /// <summary>
        /// 自动回复关键词消息
        /// </summary>
        AutoreplayKeyWords = 5,
        /// <summary>
        /// 自动回复收到消息消息
        /// </summary>
        AutoreplayResponse = 6,
        /// <summary>
        /// 自动回复被关在消息
        /// </summary>
        AutoreplaySubscribe = 7
    }
}
