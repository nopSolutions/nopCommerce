namespace Nop.Plugin.Misc.AbcCore.Delivery
{
    public class CartSlideoutInfo
    {
        public string ProductInfoHtml { get; init; }
        public string SubtotalHtml { get; init; }
        public string DeliveryOptionsHtml { get; init; }
        public string WarrantyHtml { get; init; }
        public int ShoppingCartItemId { get; init; }
        public int ProductId { get; init; }
    }
}
