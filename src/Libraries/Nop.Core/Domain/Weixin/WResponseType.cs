namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a ResponseType
    /// </summary>
    public enum WResponseType : byte
    {
        //回复方式：被动方式 passive，客服方式 custom
        //被动方式回复消息
        passive = 1,
        //客服方式回复消息
        custom = 2,
    }
}
