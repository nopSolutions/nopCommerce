using Humanizer;
using Nop.Core.Domain.Weixin;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 产品广告图
    /// </summary>
    public partial class ProductAdvertImage : BaseEntity
    {
        /// <summary>
        /// 标题（不为空）
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 图片描述（可空）
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
        /// 【WImagePositionType】
        /// </summary>
        public byte ImagePositionTypeId { get; set; }
        /// <summary>
        /// 【WImagePositionType】
        /// </summary>
        public WImagePositionType ImagePositionType
        {
            get => (WImagePositionType)ImagePositionTypeId;
            set => ImagePositionTypeId = (byte)value;
        }
        /// <summary>
        /// 用户被自动打上的标签ID列表
        /// </summary>
        public string TagIdList { get; set; }
        /// <summary>
        /// 二维码合成x坐标偏移量
        /// </summary>
        public int CoordinateX { get; set; }
        /// <summary>
        /// 二维码合成y坐标偏移量
        /// </summary>
        public int CoordinateY { get; set; }
        /// <summary>
        /// 二维码长宽尺寸
        /// </summary>
        public int QrcodeSize { get; set; }
        /// <summary>
        /// 【Product.Id】绑定产品ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 商家提供的优惠券ID，是折扣广告图是调用该ID
        /// </summary>
        public int SupplierVoucherCouponId { get; set; }
        /// <summary>
        /// 是否产品折扣广告图类型：产品类，优惠券类
        /// </summary>
        public bool IsDiscountAdver { get; set; }
        /// <summary>
        /// 生成的二维码是否是关注类型临时二维码，否则生成推荐链接形式二维码
        /// </summary>
        public bool SubscribeQrcode { get; set; }
        /// <summary>
        /// 是否前台发布中
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 删除标志
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public bool DisplayOrder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreatTime { get; set; }
    }
}
