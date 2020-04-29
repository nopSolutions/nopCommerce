namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WMessage
    /// </summary>
    public partial class WMessage : BaseEntity
    {
        /// <summary>
        /// 是否显示封面
        /// </summary>
        public bool ShowCover { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// MediaId是否永久素材ID
        /// </summary>
        public bool MaterialMsg { get; set; }
        /// <summary>
        /// 发送的图片/语音/视频/图文消息（点击跳转到图文消息页）的媒体ID
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 图文消息/视频消息/音乐消息/小程序卡片的标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息关键词
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// 回复方式：被动方式 passive，客服方式 custom
        /// </summary>
        public byte ResponseTypeId { get; set; }
        /// <summary>
        /// 回复方式：被动方式 passive，客服方式 custom
        /// </summary>
        public WResponseType ResponseType 
        {
            get => (WResponseType)ResponseTypeId;
            set => ResponseTypeId = (byte)value;
        }
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
        /// 客服ID
        /// </summary>
        public string KfAccount { get; set; }
        /// <summary>
        /// 缩略图/小程序卡片图片的媒体ID，小程序卡片图片建议大小为520*416
        /// </summary>
        public string ThumbMediaId { get; set; }
        /// <summary>
        /// 封面图URL
        /// </summary>
        public string CoverUrl { get; set; }
        /// <summary>
        /// 图文消息的图片链接，支持JPG、PNG格式，较好的效果为大图640*320,对应media_id
        /// </summary>
        public string PicUrl { get; set; }
        /// <summary>
        /// 专门存储小图80*80,对应thumb_media_id
        /// </summary>
        public string ThumbPicUrl { get; set; }
        /// <summary>
        /// 图文消息/视频消息/音乐消息的描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Digest { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 音乐链接
        /// </summary>
        public string MusicUrl { get; set; }
        /// <summary>
        /// 高品质音乐链接，wifi环境优先使用该链接播放音乐
        /// </summary>
        public string HqMusicUrl { get; set; }
        /// <summary>
        /// 图文消息被点击后跳转的链接
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 原文的URL，若置空则无查看原文入口
        /// </summary>
        public string SourceUrl { get; set; }
        /// <summary>
        /// 小程序的appid，要求小程序的appid需要与公众号有关联关系
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 小程序的页面路径，跟app.json对齐，支持参数，比如pages/index/index? foo = bar
        /// </summary>
        public string PagePath { get; set; }
        /// <summary>
        /// text时的回复内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 图片素材上传时间戳
        /// </summary>
        public int CreatTime { get; set; }

    }
}
