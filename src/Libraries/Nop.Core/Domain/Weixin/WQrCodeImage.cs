namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WQrCodeImage
    /// </summary>
    public partial class WQrCodeImage : BaseEntity
    {
        /// <summary>
        /// 图片标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 图片描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 广告底图图片链接
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 广告底图图片源文件链接(用于打印)
        /// </summary>
        public string ImageUrlOriginal { get; set; }
        /// <summary>
        /// 二维码内容类型
        /// </summary>
        public byte QrCodeJumpTypeId { get; set; }
        /// <summary>
        /// 二维码内容类型
        /// </summary>
        public WQrCodeJumpType QrCodeJumpType
        {
            get => (WQrCodeJumpType)QrCodeJumpTypeId;
            set => QrCodeJumpTypeId = (byte)value;
        }
        /// <summary>
        /// 图片位置
        /// </summary>
        public byte ImagePositionTypeId { get; set; }
        /// <summary>
        /// 图片位置
        /// </summary>
        public WImagePositionType ImagePositionType
        {
            get => (WImagePositionType)ImagePositionTypeId;
            set => ImagePositionTypeId = (byte)value;
        }

        /// <summary>
        /// 二维码合成x坐标偏移量
        /// </summary>
        public int QrCodeX { get; set; }
        /// <summary>
        /// 二维码合成y坐标偏移量
        /// </summary>
        public int QrCodeY { get; set; }
        /// <summary>
        /// 二维码长宽尺寸
        /// </summary>
        public int QrCodeSize { get; set; }

        /// <summary>
        /// 用户被自动打上的标签ID列表
        /// </summary>
        public string TagIdList { get; set; }
        /// <summary>
        /// 是否发布中
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 删除标志
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreatTime { get; set; }
    }
}
