using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 商品营销标签分类
    /// </summary>
    public partial class OfficialCustomer : BaseEntity
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 客服名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 客服昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImageUrl { get; set; }
        /// <summary>
        /// 客服二维码
        /// </summary>
        public string QrCodeUrl { get; set; }
        /// <summary>
        /// 类别Ids，以逗号分开
        /// </summary>
        public string CategoryIds { get; set; }
        /// <summary>
        /// 联系电话，多个电话以逗号分开
        /// </summary>
        public string ContactNumber { get; set; }
        /// <summary>
        /// 工作时间
        /// </summary>
        public string WorkTime { get; set; }
        /// <summary>
        /// 服务内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }

    }
}
