namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WMenuButton
    /// </summary>
    public partial class WMenuButton : BaseEntity
    {
        /// <summary>
        /// 【WMenu.Id】所属菜单ID
        /// </summary>
        public int WMenuId { get; set; }
        /// <summary>
        /// 菜单标题，不超过16个字节，子菜单不超过40个字节
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 【WMenuButtonType】
        /// </summary>
        public byte MenuButtonTypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public WMenuButtonType MenuButtonType
        {
            get => (WMenuButtonType)MenuButtonTypeId;
            set => MenuButtonTypeId = (byte)value;
        }
        /// <summary>
        /// 菜单按钮父级ID
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 不同按钮类型保存不同的值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 不同按钮类型保存不同的值
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 不同按钮类型保存不同的值
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// media_id类型和view_limited类型必须 调用新增永久素材接口返回的合法media_id
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 小程序的appid
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 小程序的页面路径
        /// </summary>
        public string PagePath { get; set; }
        /// <summary>
        /// 【WAutoreplyNewsInfo】回复类型News时候，对应News类型内容，id以逗号分开，多条图文时，首个Id为封面。
        /// </summary>
        public string WAutoreplyNewsInfoIds { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 是否根菜单（否则为子级菜单）
        /// </summary>
        public bool RootButton { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        public bool Published { get; set; }

    }
}
