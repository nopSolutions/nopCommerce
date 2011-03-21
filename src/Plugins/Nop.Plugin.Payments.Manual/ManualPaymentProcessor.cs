using System;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.Manual
{
    /// <summary>
    /// Manual payment processor
    /// </summary>
    public class ManualPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields
        private readonly ISettingService _settingService;
        #endregion

        #region Ctor

        public ManualPaymentProcessor(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        #endregion

        #region Utilties

        /// <summary>
        /// Gets current transaction mode
        /// </summary>
        /// <returns>Current transaction mode</returns>
        private TransactMode GetCurrentTransactionMode()
        {
            TransactMode transactionModeEnum = TransactMode.Authorize;
            string transactionMode =  _settingService.GetSettingByKey<string>("PaymentMethod.Manual.TransactionMode");
            if (!String.IsNullOrEmpty(transactionMode))
                transactionModeEnum = (TransactMode)Enum.Parse(typeof(TransactMode), transactionMode);
            return transactionModeEnum;
        }
        #endregion
        
        #region Methods


        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();

            result.AllowStoringCreditCardNumber = true;
            TransactMode transactionMode = GetCurrentTransactionMode();
            switch (transactionMode)
            {
                case TransactMode.Pending:
                    result.NewPaymentStatus = PaymentStatus.Pending;
                    break;
                case TransactMode.Authorize:
                    result.NewPaymentStatus = PaymentStatus.Authorized;
                    break;
                case TransactMode.AuthorizeAndCapture:
                    result.NewPaymentStatus = PaymentStatus.Paid;
                    break;
                default:
                    {
                        result.AddError("Not supported transaction type");
                        return result;
                    }
            }

            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //nothing
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return _settingService.GetSettingByKey<decimal>("PaymentMethod.Manual.AdditionalFee");
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();

            result.AllowStoringCreditCardNumber = true;
            TransactMode transactionMode = GetCurrentTransactionMode();
            switch (transactionMode)
            {
                case TransactMode.Pending:
                    result.NewPaymentStatus = PaymentStatus.Pending;
                    break;
                case TransactMode.Authorize:
                    result.NewPaymentStatus = PaymentStatus.Authorized;
                    break;
                case TransactMode.AuthorizeAndCapture:
                    result.NewPaymentStatus = PaymentStatus.Paid;
                    break;
                default:
                    {
                        result.AddError("Not supported transaction type");
                        return result;
                    }
            }

            //restore credit cart info
            if (processPaymentRequest.IsRecurringPayment)
            {
                if (processPaymentRequest.InitialOrder != null)
                {
                    //TODO ensure that the following properties will be saved (temporary commented):
                    //CreditCardType, CreditCardName, CreditCardNumber, CreditCardCvv2,
                    //CreditCardExpireMonth, CreditCardExpireYear
                    //result.CreditCardType = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Decrypt(processPaymentRequest.InitialOrder.CardType) : string.Empty;
                    //result.CreditCardName = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Decrypt(processPaymentRequest.InitialOrder.CardName) : string.Empty;
                    //result.CreditCardNumber = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Decrypt(processPaymentRequest.InitialOrder.CardNumber) : string.Empty;
                    //result.CreditCardCvv2 = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Decrypt(processPaymentRequest.InitialOrder.CardCvv2) : string.Empty;
                    //try
                    //{
                    //    result.CreditCardExpireMonth = Convert.ToInt32(processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Decrypt(processPaymentRequest.InitialOrder.CardExpirationMonth) : "0");
                    //    result.CreditCardExpireYear = Convert.ToInt32(processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Decrypt(processPaymentRequest.InitialOrder.CardExpirationYear) : "0");
                    //}
                    //catch
                    //{
                    //}
                }
            }

            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Cancelling recurring orders not supported");
            return result;
        }
        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.Manual;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Standard;
            }
        }

        /// <summary>
        /// Gets the friendly name
        /// </summary>
        public override string FriendlyName
        {
            get { return "Manual Processing"; }
        }

        /// <summary>
        /// Gets the system name
        /// </summary>
        public override string SystemName
        {
            get { return "Payments.Manual"; }
        }

        #endregion
        
    }
}
