using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Payments.AmazonPay.Models;

public record CheckoutConfirmModel : CheckoutShippingMethodModel
{
    public string MinOrderTotalWarning { get; set; }
    public string AmazonPayScript { get; set; }
    public bool RequiresShipping { get; set; }
    public bool CanAssociateAccount { get; set; }
    public bool CanCreateAccount { get; set; }
    public string BuyerId { get; set; }
    public string BuyerEmail { get; set; }
    public string BuyerName { get; set; }

    public void SetShippingMethodData(CheckoutShippingMethodModel shippingMethodData)
    {
        ShippingMethods = shippingMethodData.ShippingMethods;

        NotifyCustomerAboutShippingFromMultipleLocations =
            shippingMethodData.NotifyCustomerAboutShippingFromMultipleLocations;

        Warnings = shippingMethodData.Warnings;

        DisplayPickupInStore = shippingMethodData.DisplayPickupInStore;
        PickupPointsModel = shippingMethodData.PickupPointsModel;

        RequiresShipping = true;
    }
}