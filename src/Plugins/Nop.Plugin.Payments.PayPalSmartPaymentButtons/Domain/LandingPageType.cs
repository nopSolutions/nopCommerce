namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Domain
{
    /// <summary>
    /// Represents the type of landing page to show on the PayPal site for customer checkout
    /// </summary>
    public enum LandingPageType
    {
        /// <summary>
        /// When the customer clicks PayPal Checkout, the customer is redirected to a page to log in to PayPal and approve the payment.
        /// </summary>
        Login,

        /// <summary>
        /// When the customer clicks PayPal Checkout, the customer is redirected to a page to enter credit or debit card and other relevant billing information required to complete the purchase.
        /// </summary>
        Billing,

        /// <summary>
        /// When the customer clicks PayPal Checkout, the customer is redirected to either a page to log in to PayPal and approve the payment or to a page to enter credit or debit card and other relevant billing information required to complete the purchase, depending on their previous interaction with PayPal.
        /// </summary>
        No_preference
    }
}