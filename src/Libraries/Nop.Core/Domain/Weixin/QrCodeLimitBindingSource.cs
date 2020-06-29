namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an 永久二维码绑定信息（一对一）
    /// </summary>
    public partial class QrCodeLimitBindingSource : BaseEntity
    {
        /// <summary>
        /// 永久二维码ID
        /// </summary>
        public int QrCodeLimitId { get; set; }
            /// <summary>
            /// 供应商ID（用于统计）
            /// </summary>
        public int SupplierId { get; set; }
        /// <summary>
        /// 店铺ID（用于统计）
        /// </summary>
        public int SupplierShopId { get; set; }
        /// <summary>
        /// 所属产品ID（为回复消息传递必要参数）
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 宣传地点（用于统计），统计里面采用查找【MarketingAdvertAddress】Name值获得ID，便于销售员在手机端完成对地点的设置
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 单独表：易拉宝，店铺贴广告图，线上分享，DM单,个人名片……
        /// </summary>
        public int MarketingAdvertWayId { get; set; }
        /// <summary>
        /// 是否使用指定链接
        /// </summary>
        public bool UseFixUrl { get; set; }
        /// <summary>
        /// 链接（使用产品链接回复的时候使用备用）
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 是否启用消息回复
        /// </summary>
        public bool MessageResponse { get; set; }
        /// <summary>
        /// 场景类型
        /// </summary>
        public byte WSceneTypeId { get; set; }
        /// <summary>
        /// 场景类型
        /// </summary>
        public WSceneType WSceneType
        {
            get => (WSceneType)WSceneTypeId;
            set => WSceneTypeId = (byte)value;
        }
        /// <summary>
        /// 【WMessageType】消息类型:文本为text，图片为image，语音为voice，视频消息为video，音乐消息为music，图文消息（点击跳转到外链）为news，图文消息（点击跳转到图文消息页面）为mpnews，卡券为wxcard，小程序为miniprogrampage
        /// </summary>
        public byte MessageTypeId { get; set; }
        /// <summary>
        /// 【WMessageType】
        /// </summary>
        public WMessageType MessageType
        {
            get => (WMessageType)MessageTypeId;
            set => MessageTypeId = (byte)value;
        }
        /// <summary>
        /// 文本消息内容（其他消息格式从产品参数中获取，或从绑定的消息ID中回复）
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否采用绑定的消息回复（启用时候，通过查询绑定的消息表回复消息）【WMessageBindMapping】表
        /// </summary>
        public bool UseBindingMessage { get; set; }
        /// <summary>
        /// 是否发布（未发布表示暂停上面的绑定）
        /// </summary>

        public bool Published { get; set; }


    }
}
