namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WQrCodeLimitMsgMapping
    /// </summary>
    public partial class WQrCodeLimitMsgMapping : BaseEntity
    {
        /// <summary>
        /// 永久二维码数字ID(当前使用自增ID，实现分组后，调用QrCodeId值)
        /// </summary>
        public int WQrCodeLimitId { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public byte MessageTypeId { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public WMessageType MessageType
        {
            get => (WMessageType)MessageTypeId;
            set => MessageTypeId = (byte)value;
        }
        /// <summary>
        /// 内容或对应的消息ID
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 消息ID列表，以逗号分开
        /// </summary>
        /// <returns></returns>
        public string MessageIds { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 状态标志，预留（是否启用等默认0）
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 是否使用消息ID，否则使用Content内容
        /// </summary>
        public bool UseMessageId { get; set; }
        /// <summary>
        /// 删除标志
        /// </summary>
        public bool Deleted { get; set; }

    }
}
