using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.ShoppingCart;

public partial record MiniShoppingCartModel : BaseNopModel
{
    public MiniShoppingCartModel()
    {
        Items = new List<ShoppingCartItemModel>();
    }

    public IList<ShoppingCartItemModel> Items { get; set; }
    public int TotalProducts { get; set; }
    public string SubTotal { get; set; }
    public decimal SubTotalValue { get; set; }
    public bool DisplayShoppingCartButton { get; set; }
    public bool DisplayCheckoutButton { get; set; }
    public bool CurrentCustomerIsGuest { get; set; }
    public bool AnonymousCheckoutAllowed { get; set; }
    public bool ShowProductImages { get; set; }

    #region Nested Classes

    public partial record ShoppingCartItemModel : BaseNopEntityModel
    {
        public ShoppingCartItemModel()
        {
            Picture = new PictureModel();
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public int Quantity { get; set; }

        public string UnitPrice { get; set; }
        public decimal UnitPriceValue { get; set; }

        public string AttributeInfo { get; set; }

        public PictureModel Picture { get; set; }
    }

    #endregion
}