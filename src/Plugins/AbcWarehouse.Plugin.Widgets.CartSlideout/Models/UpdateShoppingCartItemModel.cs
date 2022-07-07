namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Models
{
    public class UpdateShoppingCartItemModel
    {
        public int ShoppingCartItemId { get; set; }

        public int ProductAttributeMappingId { get; set; }

        public int ProductAttributeValueId { get; set; }

        public bool? IsChecked { get; set; }

        public bool IsValid()
        {
            return ShoppingCartItemId != 0 &&
                   ProductAttributeMappingId != 0 &&
                   IsChecked != null;
        }
    }
}
