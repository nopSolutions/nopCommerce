using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 产品赠品表（赠送在售产品）
    /// </summary>
    public partial class ProductGiftProductMapping : BaseEntity
    {
        /// <summary>
        /// 名称（为空时，调用绑定礼品产品的名称）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 产品ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 【ProductAttributeValue.Id】产品不同规格价格调整ID
        /// </summary>
        public int? ProductAttributeValueId { get; set; }
        /// <summary>
        /// 赠送礼品的产品ID
        /// </summary>
        public int GiftProductId { get; set; }
        /// <summary>
        /// 【ProductAttributeValue.Id】产品不同规格价格调整ID
        /// </summary>
        public int? GiftProductAttributeValueId { get; set; }
        /// <summary>
        /// 赠送用户角色限制
        /// </summary>
        public int? CustomerRoleId { get; set; }
        /// <summary>
        /// Store限制
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 最小购买数量，没有达到不参与赠送，最小值1
        /// </summary>
        public int MinBuyQuantity { get; set; }
        /// <summary>
        /// 最大赠送数量，超出部分不参与赠送，0为不限制上限
        /// </summary>
        public int MaxGiveQuantity { get; set; }
        /// <summary>
        /// 赠送物品的数量
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 步进计数值，（买1送1，步进为1，买5送1，步进为5）
        /// </summary>
        public int StepCount { get; set; }
        /// <summary>
        /// 是否固定数量，（购买订单商品数变化，赠送数量不变）
        /// </summary>
        public bool FixedAmount { get; set; }
        /// <summary>
        /// 是否分开邮寄
        /// </summary>
        public bool SeparateDelivery { get; set; }
        /// <summary>
        /// 是否必须有库存，库存为0取消赠送，否则可以持续赠送
        /// </summary>
        public bool MustInStock { get; set; }
        /// <summary>
        /// 是否新用户才赠送
        /// </summary>
        public bool NewUserGift { get; set; }
        /// <summary>
        /// 发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDateTimeUtc { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

    }
}
