using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Response result type enumeration. Indicates a result of a request.
    /// </summary>
    public enum ResponseResultType
    {
        /// <summary>
        /// Success
        /// </summary>
        [EnumMember(Value = "SUCCESS")]
        Success,

        /// <summary>
        /// Communication error
        /// </summary>
        [EnumMember(Value = "COMMUNICATION_ERROR")]
        CommunicationError,

        /// <summary>
        /// Authentication error
        /// </summary>
        [EnumMember(Value = "AUTHENTICATION_ERROR")]
        AuthenticationError,

        /// <summary>
        /// Decline
        /// </summary>
        [EnumMember(Value = "DECLINE")]
        Decline,

        /// <summary>
        /// Decline AVS
        /// </summary>
        [EnumMember(Value = "DECLINE_AVS")]
        DeclineAvs,

        /// <summary>
        /// Decline CVV
        /// </summary>
        [EnumMember(Value = "DECLINE_CVV")]
        DeclineCvv,

        /// <summary>
        /// Unsupported card
        /// </summary>
        [EnumMember(Value = "UNSUPPORTED_CARD")]
        UnsupportedCard,

        /// <summary>
        /// Invalid name
        /// </summary>
        [EnumMember(Value = "INVALID_NAME")]
        InvalidName,

        /// <summary>
        /// Invalid address
        /// </summary>
        [EnumMember(Value = "INVALID_ADDRESS")]
        InvalidAddress,

        /// <summary>
        /// Invalid card number
        /// </summary>
        [EnumMember(Value = "INVALID_CARD_NUMBER")]
        InvalidCardNumber,

        /// <summary>
        /// Invalid CVV
        /// </summary>
        [EnumMember(Value = "INVALID_CVV")]
        InvalidCvv,

        /// <summary>
        /// Invalid expiration
        /// </summary>
        [EnumMember(Value = "INVALID_EXPIRATION")]
        InvalidExpiration,

        /// <summary>
        /// Gateway error
        /// </summary>
        [EnumMember(Value = "GATEWAY_ERROR")]
        GatewayError,

        /// <summary>
        /// Bad request
        /// </summary>
        [EnumMember(Value = "BAD_REQUEST")]
        BadRequest,

        /// <summary>
        /// Invalid routing number
        /// </summary>
        [EnumMember(Value = "INVALID_ROUTING_NUMBER")]
        InvalidRoutingNumber,

        /// <summary>
        /// Invalid AVS
        /// </summary>
        [EnumMember(Value = "INVALID_AVS")]
        InvalidAvs,

        /// <summary>
        /// Approved
        /// </summary>
        [EnumMember(Value = "APPROVED")]
        Approved
    }
}