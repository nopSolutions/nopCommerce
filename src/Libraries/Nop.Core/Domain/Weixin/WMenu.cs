namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WMenu
    /// </summary>
    public partial class WMenu : BaseEntity
    {
        /// <summary>
        /// 正确时微信系统返回的JSON数据包，如"menuid":"208379533"
        /// </summary>
        public long MenuId { get; set; }
        /// <summary>
        /// 个性化菜单标题描述，菜单在网站系统里面的名称
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// 详细描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 自定义菜单Json组合字符串
        /// </summary>
        public string MenuJsonCode { get; set; }
        /// <summary>
        /// 用户标签ID，可以通过用户标签管理接口获取
        /// </summary>
        public string TagId { get; set; }
        /// <summary>
        /// 性别：男=1，女=2，不填则不匹配
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 客户端版本，当前只具体到系统型号：IOS(1), Android(2),Others(3)，不填则不做匹配，默认0
        /// </summary>
        public string ClientPlatformType { get; set; }
        /// <summary>
        /// 国家信息，是用户在微信中设置的地区，具体请参考地区信息表
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 省份信息，是用户在微信中设置的地区，具体请参考地区信息表
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市信息，是用户在微信中设置的地区，具体请参考地区信息表
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 用户语言ID【WLanguageType】默认1
        /// </summary>
        public byte LanguageTypeId { get; set; }
        /// <summary>
        /// 用户语言ID【WLanguageType】默认1
        /// </summary>
        public WLanguageType LanguageType
        {
            get => (WLanguageType)LanguageTypeId;
            set => LanguageTypeId = (byte)value;
        }
        /// <summary>
        /// 状态，预留
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 菜单发布时间
        /// </summary>
        public int PublishTime { get; set; }
        /// <summary>
        /// 菜单取消发布时间
        /// </summary>
        public int UnPublishTime { get; set; }
        /// <summary>
        /// 菜单是否有效（系统返回的值）
        /// </summary>
        public bool IsMenuOpen { get; set; }
        /// <summary>
        /// 发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 是否启用个性化条件
        /// </summary>
        public bool Personal { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }

    }
}
