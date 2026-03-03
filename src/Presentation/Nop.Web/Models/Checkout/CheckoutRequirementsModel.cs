namespace Nop.Web.Models.Checkout;

// The checkout requirements are dynamic, and should be derived from the cart
// and the current state. Unlike state, they cannot be changed directly by the user.
public class CheckoutRequirementsModel
{
    // Can be false when shipping is not required (no physical products for instance)
    public bool ShippingRequired { get; set; }

    // Can be false when shipping is not required, or (int Nop's current implementation)
    // when pickup in store is elected.
    public bool ShippingMethodRequired { get; set; }

    // Can be false when payment is not required.
    public bool PaymentRequired { get; set; }

    // Can be false when payment is not required, or when the selected payment method
    // doesn't require payment info.
    public bool PaymentInfoRequired { get; set; }
}
