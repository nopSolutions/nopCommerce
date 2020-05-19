namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an MessageBindMapping
    /// </summary>
    public partial class WMessageBindMapping : BaseEntity
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public int WMessageId { get; set; }

        /// <summary>
        /// 绑定的场景ID
        /// </summary>
        public int BindSceneId { get; set; }

        /// <summary>
        /// 场景帮助类型值
        /// </summary>
        public byte MessageBindSceneTypeId { get; set; }

        /// <summary>
        /// 绑定消息的场景类型
        /// </summary>
        public WMessageBindSceneType MessageBindSceneType
        {
            get => (WMessageBindSceneType)MessageBindSceneTypeId;
            set => MessageBindSceneTypeId = (byte)value;
        }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 是否发布/启用
        /// </summary>
        public bool Published { get; set; }
    }
}
