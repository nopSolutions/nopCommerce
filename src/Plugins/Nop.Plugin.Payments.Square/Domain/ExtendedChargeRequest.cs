using System.Collections.Generic;
using System.Runtime.Serialization;
using Square.Connect.Model;

namespace Nop.Plugin.Payments.Square.Domain
{
    /// <summary>
    /// Represents the parameters (with additional one) that can be included in the body of a request to the charge endpoint.
    /// </summary>
    public class ExtendedChargeRequest : ChargeRequest
    {
        #region Ctor

        public ExtendedChargeRequest(string integrationId = null,
            string idempotencyKey = null,
            Money amountMoney = null,
            string cardNonce = null,
            string customerCardId = null,
            bool? delayCapture = null,
            string referenceId = null,
            string note = null,
            string customerId = null,
            Address billingAddress = null,
            Address shippingAddress = null,
            string buyerEmailAddress = null,
            string orderId = null,
            List<AdditionalRecipient> additionalRecipients = null) : base(idempotencyKey,
            amountMoney,
            cardNonce,
            customerCardId,
            delayCapture,
            referenceId,
            note,
            customerId,
            billingAddress,
            shippingAddress,
            buyerEmailAddress,
            orderId,
            additionalRecipients)
        {
            IntegrationId = integrationId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// An extra parameter in the request in order to track revenue share transaction attribution from 3rd party merchant/developer hosted apps (where the application id will not be known up front).
        /// </summary>
        /// <value>An extra parameter in the request in order to track revenue share transaction attribution from 3rd party merchant/developer hosted apps (where the application id will not be known up front).</value>
        [DataMember(Name = "integration_id", EmitDefaultValue = false)]
        public string IntegrationId { get; set; }

        #endregion
    }
}