
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// 账单信息明细
    /// Represents a checkout attribute value
    /// </summary>
    public partial class CheckoutAttributeValue : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// 检测属性编号
        /// Gets or sets the checkout attribute mapping identifier
        /// </summary>
        public int CheckoutAttributeId { get; set; }

        /// <summary>
        /// 名称
        /// Gets or sets the checkout attribute name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 颜色组成
        /// Gets or sets the color RGB value (used with "Color squares" attribute type)
        /// </summary>
        public string ColorSquaresRgb { get; set; }

        /// <summary>
        /// 价格调整
        /// Gets or sets the price adjustment
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// 重量调整
        /// Gets or sets the weight adjustment
        /// </summary>
        public decimal WeightAdjustment { get; set; }

        /// <summary>
        /// 是否预售
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// 排序次序
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the checkout attribute
        /// </summary>
        public virtual CheckoutAttribute CheckoutAttribute { get; set; }
    }

}
