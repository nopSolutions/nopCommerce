using Nop.Plugin.Payments.Square.Domain;
using Square.Models;

namespace Nop.Plugin.Payments.Square.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="CreatePaymentRequest"/>.
    /// </summary>
    public static class CreatePaymentRequestExtensions
    {
        #region Methods

        /// <summary>
        /// Creates a extended <see cref="CreatePaymentRequest"/> with integration id.
        /// </summary>
        /// <param name="paymentRequest">Request to create payment</param>
        /// <param name="integrationId">An extra parameter in the request in order to track revenue share transaction attribution from 3rd party merchant/developer hosted apps (where the application id will not be known up front).</param>
        /// <returns>The extended request to create payment</returns>
        public static ExtendedCreatePaymentRequest ToExtendedRequest(this CreatePaymentRequest paymentRequest, string integrationId)
        {
            return new ExtendedCreatePaymentRequest
                (
                    sourceId: paymentRequest.SourceId,
                    idempotencyKey: paymentRequest.IdempotencyKey,
                    amountMoney: paymentRequest.AmountMoney,
                    tipMoney: paymentRequest.TipMoney,
                    appFeeMoney: paymentRequest.AppFeeMoney,
                    integrationId: integrationId,
                    delayDuration: paymentRequest.DelayDuration,
                    autocomplete: paymentRequest.Autocomplete,
                    orderId: paymentRequest.OrderId,
                    customerId: paymentRequest.CustomerId,
                    locationId: paymentRequest.LocationId,
                    referenceId: paymentRequest.ReferenceId,
                    verificationToken: paymentRequest.VerificationToken,
                    acceptPartialAuthorization: paymentRequest.AcceptPartialAuthorization,
                    buyerEmailAddress: paymentRequest.BuyerEmailAddress,
                    billingAddress: paymentRequest.BillingAddress,
                    shippingAddress: paymentRequest.ShippingAddress,
                    note: paymentRequest.Note,
                    statementDescriptionIdentifier: paymentRequest.StatementDescriptionIdentifier
                );
        }

        #endregion
    }
}
