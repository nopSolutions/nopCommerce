namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the product capability
/// </summary>
public enum ProductCapabilityType
{
    /// <summary>
    /// Enables Apple Pay capability. Supported only when PAYMENT_METHODS or PPCP, PAYMENT_METHODS is requested.
    /// </summary>
    APPLE_PAY,

    /// <summary>
    /// Enables users to request money by sending an invoice to another customer.
    /// </summary>
    SEND_INVOICE,

    /// <summary>
    /// A tailored donations solution for individuals, businesses, and non-profit customers to accept one-time and recurring donations via a Donate button, shareable link, or QR code.
    /// </summary>
    ACCEPT_DONATIONS,

    /// <summary>
    /// iDEAL is a Netherlands-based payment method that allows customers to complete transactions online using their bank credentials.
    /// </summary>
    ACCEPT_PYMTS_VIA_IDEAL,

    /// <summary>
    /// Receive unbranded card payments - Debit or Credit through Google Pay.
    /// </summary>
    GOOGLE_PAY,

    /// <summary>
    /// Enable merchant to receive direct credit card payment via phone/fax/mail. Merchant initiates transaction enters card into virtual terminal
    /// </summary>
    CARD_PROCESSING_VIRTUAL_TERMINAL,

    /// <summary>
    /// Enable merchant to accept AMEX. They can receive marketing communication from American express.
    /// </summary>
    AMEX_OPTBLUE,

    /// <summary>
    /// Receive direct credit card payment (card-not-present transactions)
    /// </summary>
    CUSTOM_CARD_PROCESSING,

    /// <summary>
    /// Access Fraud Protection tool (powered by Simility)
    /// </summary>
    FRAUD_TOOL_ACCESS,

    /// <summary>
    /// Receive debit card payment
    /// </summary>
    DEBIT_CARD_SWITCH,

    /// <summary>
    /// Merchant is considered as a commercial entity and hence needs to comply with the requirements and use the benefits, such as CE agreement and treatment on the credit card descriptor
    /// </summary>
    COMMERCIAL_ENTITY,

    /// <summary>
    /// Merchant checkout feature that provides businesses the ability to accept payments from consumers without a PayPal account using a credit or debit card through PayPal.
    /// </summary>
    GUEST_CHECKOUT,

    /// <summary>
    /// Receive credit card payment via inline /guest checkout
    /// </summary>
    PAYPAL_CHECKOUT,

    /// <summary>
    /// Allow merchants to create stable, predictable income by offering subscription plans.
    /// </summary>
    SUBSCRIPTIONS,

    /// <summary>
    /// Receive Venmo pay
    /// </summary>
    VENMO_PAY_PROCESSING,

    /// <summary>
    /// Receive local payment methods specific to a buyer region (eg Giropay, Ideal, Sofort)
    /// </summary>
    PAYPAL_CHECKOUT_ALTERNATIVE_PAYMENT_METHODS,

    /// <summary>
    /// Provide the Integrated QRC solution that is intended to be integrated with Partners who already provide payment acceptance via APMs directly to merchants.
    /// </summary>
    QR_CODE,

    /// <summary>
    /// Merchants can offer equal payment installment financing options as part of the revolving account.
    /// </summary>
    INSTALLMENTS,

    /// <summary>
    /// PayPal Checkout - Pay with PayPal Credit.
    /// </summary>
    PAYPAL_CHECKOUT_PAY_WITH_PAYPAL_CREDIT,

    /// <summary>
    /// Enables capability to save payment methods. Supported only when ADVANCED_VAULTING is requested and EXPRESS_CHECKOUT or PPCP is also requested.
    /// </summary>
    PAYPAL_WALLET_VAULTING_ADVANCED,

    /// <summary>
    /// Withdraw money from their PayPal balance to a linked domestic bank account.
    /// </summary>
    WITHDRAW_FUNDS_TO_DOMESTIC_BANK,

    /// <summary>
    /// Withdraw money from their PayPal balance.
    /// </summary>
    WITHDRAW_MONEY,

    /// <summary>
    /// Send money from their PayPal balance.
    /// </summary>
    SEND_MONEY
}