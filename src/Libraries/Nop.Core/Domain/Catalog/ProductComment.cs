using System;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a Product Comment
    /// </summary>
    public partial class ProductComment : BaseEntity
    {
        /// <summary>
        /// Store
        /// </summary>
        public int StoreId { get; set; }
            /// <summary>
            /// 用户id
            /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        /// 产品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 服务星级
        /// </summary>
        public byte ServiceStars { get; set; }
        /// <summary>
        /// 产品星级
        /// </summary>
        public byte ProductStars { get; set; }
        /// <summary>
        /// 环境星级
        /// </summary>
        public byte EnvStars { get; set; }
        /// <summary>
        /// 邮递星级
        /// </summary>
        public byte ShippingStars { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string CommentText { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplyContent { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? RepliedOnUtc { get; set; }
        /// <summary>
        /// 是否展现
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 是否审核通过
        /// </summary>
        public bool IsApproved { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

    }
}
