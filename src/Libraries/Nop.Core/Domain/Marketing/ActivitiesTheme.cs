using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 活动主题/专区表
    /// </summary>
    public partial class ActivitiesTheme : BaseEntity
    {
        /// <summary>
        /// 活动标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 活动描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 活动内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 用户角色限制
        /// </summary>
        public int? CustomerRoleId { get; set; }
        /// <summary>
        /// store限制
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 活动图片/封面图
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 活动横幅广告图地址
        /// </summary>
        public string BannerImageUrl { get; set; }
        /// <summary>
        /// 分享图片URL
        /// </summary>
        public string ShareImageUrl { get; set; }
        /// <summary>
        /// 活动链接
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 活动二维码图片
        /// </summary>
        public string QrcodeUrl { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTimeUtc { get; set; }
        /// <summary>
        /// 是否系统活动
        /// </summary>
        public bool SysActivities { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

    }
}
