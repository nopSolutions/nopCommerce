using Nop.Core.Configuration;

namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order settings
/// </summary>
public partial class OrderSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether customer can make re-order
    /// </summary>
    public bool IsReOrderAllowed { get; set; }

    /// <summary>
    /// Gets or sets a minimum order subtotal amount
    /// </summary>
    public decimal MinOrderSubtotalAmount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 'Minimum order subtotal amount' option
    /// should be evaluated over 'X' value including tax or not
    /// </summary>
    public bool MinOrderSubtotalAmountIncludingTax { get; set; }

    /// <summary>
    /// Gets or sets a minimum order total amount
    /// </summary>
    public decimal MinOrderTotalAmount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether automatically update order totals on editing an order in admin area
    /// </summary>
    public bool AutoUpdateOrderTotalsOnEditingOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether anonymous checkout allowed
    /// </summary>
    public bool AnonymousCheckoutAllowed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether checkout is disabled
    /// </summary>
    public bool CheckoutDisabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 'Terms of service' enabled on the shopping cart page
    /// </summary>
    public bool TermsOfServiceOnShoppingCartPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 'Terms of service' enabled on the order confirmation page
    /// </summary>
    public bool TermsOfServiceOnOrderConfirmPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether 'One-page checkout' is enabled
    /// </summary>
    public bool OnePageCheckoutEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether order totals should be displayed on 'Payment info' tab of 'One-page checkout' page
    /// </summary>
    public bool OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether "Billing address" step should be skipped
    /// </summary>
    public bool DisableBillingAddressCheckoutStep { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether "Order completed" page should be skipped
    /// </summary>
    public bool DisableOrderCompletedPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether "Pickup in store" options should be displayed on the shipping method page
    /// </summary>
    public bool DisplayPickupInStoreOnShippingMethodPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we should attach PDF invoice to "Order placed" email
    /// </summary>
    public bool AttachPdfInvoiceToOrderPlacedEmail { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we should attach PDF invoice to "Order paid" email
    /// </summary>
    public bool AttachPdfInvoiceToOrderPaidEmail { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we should attach PDF invoice to "Order processing" email
    /// </summary>
    public bool AttachPdfInvoiceToOrderProcessingEmail { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we should attach PDF invoice to "Order completed" email
    /// </summary>
    public bool AttachPdfInvoiceToOrderCompletedEmail { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether PDF invoices should be generated in customer language. Otherwise, use the current one
    /// </summary>
    public bool GeneratePdfInvoiceInCustomerLanguage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether "Return requests" are allowed
    /// </summary>
    public bool ReturnRequestsEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customers are allowed to upload files
    /// </summary>
    public bool ReturnRequestsAllowFiles { get; set; }

    /// <summary>
    /// Gets or sets maximum file size for upload file (return request). Set 0 to allow any file size
    /// </summary>
    public int ReturnRequestsFileMaximumSize { get; set; }

    /// <summary>
    /// Gets or sets a value "Return requests" number mask
    /// </summary>
    public string ReturnRequestNumberMask { get; set; }

    /// <summary>
    /// Gets or sets a number of days that the Return Request Link will be available for customers after order placing.
    /// </summary>
    public int NumberOfDaysReturnRequestAvailable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to activate related gift cards after completing the order
    /// </summary>
    public bool ActivateGiftCardsAfterCompletingOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to deactivate related gift cards after cancelling the order
    /// </summary>
    public bool DeactivateGiftCardsAfterCancellingOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to deactivate related gift cards after deleting the order
    /// </summary>
    public bool DeactivateGiftCardsAfterDeletingOrder { get; set; }

    /// <summary>
    /// Gets or sets an order placement interval in seconds (prevent 2 orders being placed within an X seconds time frame).
    /// </summary>
    public int MinimumOrderPlacementInterval { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether an order status should be set to "Complete" only when its shipping status is "Delivered". Otherwise, "Shipped" status will be enough.
    /// </summary>
    public bool CompleteOrderWhenDelivered { get; set; }

    /// <summary>
    /// Gets or sets a custom order number mask
    /// </summary>
    public string CustomOrderNumberMask { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the orders need to be exported with their products
    /// </summary>
    public bool ExportWithProducts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether administrators (in impersonation mode) are allowed to buy products marked as "Call for price"
    /// </summary>
    public bool AllowAdminsToBuyCallForPriceProducts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show product thumbnail in order details page"
    /// </summary>
    public bool ShowProductThumbnailInOrderDetailsPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the gift card usage history have to delete when an order is cancelled
    /// </summary>
    public bool DeleteGiftCardUsageHistory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display order amounts in customer's currency on the order details page in the admin area
    /// </summary>
    public bool DisplayCustomerCurrencyOnOrders { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether "Summary" block should be displayed on the order list table
    /// </summary>
    public bool DisplayOrderSummary { get; set; }
}