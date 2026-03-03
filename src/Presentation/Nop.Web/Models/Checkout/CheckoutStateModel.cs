
namespace Nop.Web.Models.Checkout;

public class CheckoutStateModel
{
    public int? ShippingAddressId { get; set; }

    public string? ShippingOption { get; set; }

    public int? BillingAddressId { get; set; }

    public string? PaymentMethodSystemName { get; set; }

    public bool ShipToSameAddress { get; set; }

    // Pickup

    /**
     * How is pickup represented in the system?
     * It is represented as a ShippingOption (stored as a generic attribute under
     * NopCustomerDefaults.SelectedShippingOptionAttribute) like other options,
     * but whose property IsPickupInStore is true.
     * 
     * The pickup point on the other hand is stored under NopCustomerDefaults.SelectedPickupPointAttribute.
     */

    public bool PickupInStore { get; set; }

    public string? PickupPoint { get; set; }
}
