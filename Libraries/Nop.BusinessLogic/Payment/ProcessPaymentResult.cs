//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Represents a ProcessPaymentResult
    /// </summary>
    public partial class ProcessPaymentResult
    {
        #region Fields
        private string _avsResult = string.Empty;
        private string _authorizationTransactionId = string.Empty;
        private string _authorizationTransactionCode = string.Empty;
        private string _authorizationTransactionResult = string.Empty;
        private string _captureTransactionId = string.Empty;
        private string _captureTransactionResult = string.Empty;
        private string _subscriptionTransactionId = string.Empty;        
        private string _error = string.Empty;
        private string _fullError = string.Empty;
        private PaymentStatusEnum _paymentStatus = PaymentStatusEnum.Pending;
        private bool _allowStoringCreditCardNumber = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets an AVS result
        /// </summary>
        public string AVSResult
        {
            get
            {
                return _avsResult;
            }
            set
            {
                _avsResult = value;
            }
        }

        /// <summary>
        /// Gets or sets the authorization transaction identifier
        /// </summary>
        public string AuthorizationTransactionId
        {
            get
            {
                return _authorizationTransactionId;
            }
            set
            {
                _authorizationTransactionId = value;
            }
        }

        /// <summary>
        /// Gets or sets the authorization transaction code
        /// </summary>
        public string AuthorizationTransactionCode
        {
            get
            {
                return _authorizationTransactionCode;
            }
            set
            {
                _authorizationTransactionCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the authorization transaction result
        /// </summary>
        public string AuthorizationTransactionResult
        {
            get
            {
                return _authorizationTransactionResult;
            }
            set
            {
                _authorizationTransactionResult = value;
            }
        }

        /// <summary>
        /// Gets or sets the capture transaction identifier
        /// </summary>
        public string CaptureTransactionId
        {
            get
            {
                return _captureTransactionId;
            }
            set
            {
                _captureTransactionId = value;
            }
        }

        /// <summary>
        /// Gets or sets the capture transaction result
        /// </summary>
        public string CaptureTransactionResult
        {
            get
            {
                return _captureTransactionResult;
            }
            set
            {
                _captureTransactionResult = value;
            }
        }

        /// <summary>
        /// Gets or sets the subscription transaction identifier
        /// </summary>
        public string SubscriptionTransactionId
        {
            get
            {
                return _subscriptionTransactionId;
            }
            set
            {
                _subscriptionTransactionId = value;
            }
        }

        /// <summary>
        /// Gets or sets an error message for customer, or String.Empty if no errors
        /// </summary>
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
            }
        }

        /// <summary>
        /// Gets or sets a full error message, or String.Empty if no errors
        /// </summary>
        public string FullError
        {
            get
            {
                return _fullError;
            }
            set
            {
                _fullError = value;
            }
        }

        /// <summary>
        /// Gets or sets a payment status after processing
        /// </summary>
        public PaymentStatusEnum PaymentStatus
        {
            get
            {
                return _paymentStatus;
            }
            set
            {
                _paymentStatus = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether storing of credit card number, CVV2 is allowed
        /// </summary>
        public bool AllowStoringCreditCardNumber
        {
            get
            {
                return _allowStoringCreditCardNumber;
            }
            set
            {
                _allowStoringCreditCardNumber = value;
            }
        }
        #endregion
    }
}
