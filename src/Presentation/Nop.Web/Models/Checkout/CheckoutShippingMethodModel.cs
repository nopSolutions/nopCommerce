using Nop.Core.Domain.Shipping;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Checkout;

public partial record CheckoutShippingMethodModel : BaseNopModel
{
    public CheckoutShippingMethodModel()
    {
        ShippingMethods = new List<ShippingMethodModel>();
        Warnings = new List<string>();
    }

    public IList<ShippingMethodModel> ShippingMethods { get; set; }

    public bool NotifyCustomerAboutShippingFromMultipleLocations { get; set; }

    public IList<string> Warnings { get; set; }

    public bool DisplayPickupInStore { get; set; }
    public CheckoutPickupPointsModel PickupPointsModel { get; set; }

    #region Nested classes

    public partial record ShippingMethodModel : BaseNopModel
    {
        public string ShippingRateComputationMethodSystemName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Fee { get; set; }
        public decimal Rate { get; set; }
        public int DisplayOrder { get; set; }
        public bool Selected { get; set; }
        public ShippingOption ShippingOption { get; set; }
    }

    #endregion
}