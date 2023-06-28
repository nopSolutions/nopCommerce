using Nop.Core;

namespace Nop.Plugin.Payments.CyberSource
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class CyberSourceDefaults
    {
        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        public static string SystemName => "Payments.CyberSource";

        /// <summary>
        /// Gets the user agent used to request third-party services
        /// </summary>
        public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

        /// <summary>
        /// Gets the solution ID
        /// </summary>
        public static string SolutionId => "P8UZI75K";

        /// <summary>
        /// Gets the test api base url
        /// </summary>
        public static string TestApiBaseUrl => "apitest.cybersource.com";

        /// <summary>
        /// Gets the live api base url
        /// </summary>
        public static string LiveApiBaseUrl => "api.cybersource.com";

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Payments.CyberSource.Configure";

        /// <summary>
        /// Gets the payer redirect route name
        /// </summary>
        public static string PayerRedirectRouteName => "Plugin.Payments.CyberSource.PayerRedirect";

        /// <summary>
        /// Gets the one page checkout route name
        /// </summary>
        public static string OnePageCheckoutRouteName => "CheckoutOnePage";

        /// <summary>
        /// Gets the checkout payment info route name
        /// </summary>
        public static string CheckoutPaymentInfoRouteName => "CheckoutPaymentInfo";

        /// <summary>
        /// Gets the decision skip action name
        /// </summary>
        public static string DecisionSkipActionName => "DECISION_SKIP";

        /// <summary>
        /// Gets the token create action name
        /// </summary>
        public static string TokenCreateActionName => "TOKEN_CREATE";

        /// <summary>
        /// Gets the validate payer auth action name
        /// </summary>
        public static string ValidatePayerAuthActionName => "VALIDATE_CONSUMER_AUTHENTICATION";

        /// <summary>
        /// Gets the customer action token type name
        /// </summary>
        public static string CustomerActionTokenTypeName => "customer";

        /// <summary>
        /// Gets the payment instrument action token type name
        /// </summary>
        public static string PaymentInstrumentActionTokenTypeName => "paymentInstrument";

        /// <summary>
        /// Gets the session key to get or set order and payment statuses
        /// </summary>
        /// <remarks>0 - order GUID</remarks>
        public static string OrderStatusesSessionKey => "CyberSource.OrderStatuses-{0}";

        /// <summary>
        /// Gets the authorization date custom value
        /// </summary>
        public static string AuthorizationDateCustomValue => "Authorization Date";

        /// <summary>
        /// Gets the capture date custom value
        /// </summary>
        public static string CaptureDateCustomValue => "Capture Date";

        /// <summary>
        /// Gets the save card on file attribute name
        /// </summary>
        public static string SaveCardOnFileAttributeName => "CyberSource.SaveCardOnFile";

        /// <summary>
        /// Gets the payment with token attribute name
        /// </summary>
        public static string PaymentWithNewCardAttributeName => "CyberSource.PaymentWithNewCard";

        /// <summary>
        /// Gets the selected token id attribute name
        /// </summary>
        public static string SelectedTokenIdAttributeName => "CyberSource.SelectedTokenId";

        /// <summary>
        /// Gets the transient token attribute name
        /// </summary>
        public static string TransientTokenAttributeName => "CyberSource.TransientToken";

        /// <summary>
        /// Gets the authentication transaction id attribute name
        /// </summary>
        public static string AuthenticationTransactionIdAttributeName => "CyberSource.AuthenticationTransactionId";

        /// <summary>
        /// Gets the name of a generic attribute to store the order payment status
        /// </summary>
        public static string PaymentStatusAttributeName => "CyberSource.OrderPaymentStatus";

        /// <summary>
        /// Gets the name of a generic attribute to store the refund identifier
        /// </summary>
        public static string RefundIdAttributeName => "CyberSource.RefundId";

        /// <summary>
        /// Gets the CyberSource payment token list route name
        /// </summary>
        public static string CustomerTokensRouteName => "Plugin.Payments.CyberSource.CustomerTokens";

        /// <summary>
        /// Gets the CyberSource payment token add route name
        /// </summary>
        public static string CustomerTokenAddRouteName => "Plugin.Payments.CyberSource.CustomerTokenAdd";

        /// <summary>
        /// Gets the CyberSource payment token edit route name
        /// </summary>
        public static string CustomerTokenEditRouteName => "Plugin.Payments.CyberSource.CustomerTokenEdit";

        /// <summary>
        /// Gets the class name of the CyberSource payment token menu item
        /// </summary>
        public static string CustomerTokensMenuClassName => "cybersource-tokens";

        /// <summary>
        /// Gets the tab id of the CyberSource payment token menu item
        /// </summary>
        public static int CustomerTokensMenuTab => 500;

        /// <summary>
        /// Gets a name of the order status update schedule task
        /// </summary>
        public static string OrderStatusUpdateTaskName => "Order status update (CyberSource plugin)";

        /// <summary>
        /// Gets a type of the order status update schedule task
        /// </summary>
        public static string OrderStatusUpdateTask => "Nop.Plugin.Payments.CyberSource.Services.OrderStatusUpdateTask";

        /// <summary>
        /// Gets a payer auth tokenized card transaction type value
        /// </summary>
        public static string PayerAuthTokenizedCardTransactionTypeValue => "3";

        /// <summary>
        /// Gets a default period (in seconds) before the request times out
        /// </summary>
        public static int RequestTimeout => 30;

        /// <summary>
        /// Gets a number of minutes that conversion detail report download will be initiated frequently
        /// </summary>
        public static int ConversionDetailReportingFrequency => 15;

        /// <summary>
        /// Gets a number of days that refund can be initiated
        /// </summary>
        public static int NumberOfDaysRefundAvailable => 60;

        /// <summary>
        /// Gets a number of days that authorization can be captured
        /// </summary>
        public static int NumberOfDaysAuthorizationAvailable => 60;

        /// <summary>
        /// Gets the Flex Microform js script URL
        /// </summary>
        public static string FlexMicroformScriptUrl => "https://flex.cybersource.com/cybersource/assets/microform/0.11/flex-microform.min.js";

        #region Payer authentication

        public class PayerAuthenticationSetupStatus
        {
            /// <summary>
            /// Gets the success response status
            /// </summary>
            public static string Success => "COMPLETED";

            /// <summary>
            /// Gets the failed response status
            /// </summary>
            public static string Failed => "FAILED";
        }

        public class PayerAuthenticationStatus
        {
            /// <summary>
            /// Gets the success response status
            /// </summary>
            public static string Success => "AUTHENTICATION_SUCCESSFUL";

            /// <summary>
            /// Gets the pending response status
            /// </summary>
            public static string Pending => "PENDING_AUTHENTICATION";

            /// <summary>
            /// Gets the failed response status
            /// </summary>
            public static string Failed => "AUTHENTICATION_FAILED";
        }

        public class PayerAuthenticationErrorReason
        {
            /// <summary>
            /// Gets the invalid merchant configuration reason of the error status
            /// </summary>
            public static string Success => "INVALID_MERCHANT_CONFIGURATION";

            /// <summary>
            /// Gets the consumer authentication required reason of the error status
            /// </summary>
            public static string ConsumerAuthenticationRequired => "CONSUMER_AUTHENTICATION_REQUIRED";

            /// <summary>
            /// Gets the consumer authentication failed reason of the error status
            /// </summary>
            public static string ConsumerAuthenticationFailed => "CONSUMER_AUTHENTICATION_FAILED";

            /// <summary>
            /// Gets the authentication failed reason of the error status
            /// </summary>
            public static string AuthenticationFailed => "AUTHENTICATION_FAILED";
        }

        #endregion

        #region Response status

        public class ResponseStatus
        {
            /// <summary>
            /// Gets the AUTHORIZED response status
            /// </summary>
            public static string Authorized => "AUTHORIZED";

            /// <summary>
            /// Gets the AUTHORIZED_PENDING_REVIEW response status
            /// </summary>
            public static string AuthorizedPendingReview => "AUTHORIZED_PENDING_REVIEW";

            /// <summary>
            /// Gets the AUTHORIZED_RISK_DECLINED response status
            /// </summary>
            public static string AuthorizedRiskDeclined => "AUTHORIZED_RISK_DECLINED";

            /// <summary>
            /// Gets the DECLINED response status
            /// </summary>
            public static string Declined => "DECLINED";
        }

        #endregion

        #region Response error reason

        public class ResponseErrorReason
        {
            /// <summary>
            /// Gets the AVS_FAILED response error reason
            /// </summary>
            public static string AvsFailed => "AVS_FAILED";

            /// <summary>
            /// Gets the CV_FAILED response error reason
            /// </summary>
            public static string CvFailed => "CV_FAILED";

            /// <summary>
            /// Gets the DECISION_PROFILE_REVIEW response error reason
            /// </summary>
            public static string DecisionProfileReview => "DECISION_PROFILE_REVIEW";

            /// <summary>
            /// Gets the DECISION_PROFILE_REJECT response error reason
            /// </summary>
            public static string DecisionProfileReject => "DECISION_PROFILE_REJECT";
        }

        #endregion

        #region Decisions

        public class Decisions
        {
            /// <summary>
            /// Gets the accepted decision
            /// </summary>
            public static string Accepted => "ACCEPT";

            /// <summary>
            /// Gets the rejected decision
            /// </summary>
            public static string Rejected => "REJECT";
        }

        #endregion
    }
}