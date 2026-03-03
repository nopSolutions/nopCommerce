namespace Nop.Web.Models.Checkout;

public partial record UpdateCheckoutStateRequestModel
{
    public int? BillingAddressId { get; set; }

    public int? ShippingAddressId { get; set; }

    public string? ShippingOption { get; set; }

    public string? PaymentMethodSystemName { get; set; }

    public bool ShipToSameAddress { get; set; }

    public bool PickupInStore { get; set; }

    public string PickupPoint { get; set; }
}
