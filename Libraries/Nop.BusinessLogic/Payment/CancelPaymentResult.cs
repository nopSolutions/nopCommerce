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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Represents a CancelPaymentResult
    /// </summary>
    public partial class CancelPaymentResult
    {
        #region Fields
        private string _authorizationTransactionId = string.Empty;
        private string _captureTransactionId = string.Empty;
        private string _subscriptionTransactionId = string.Empty;
        private decimal _amount = decimal.Zero;
        private bool _isPartialRefund;
        private string _error = string.Empty;
        private string _fullError = string.Empty;
        private PaymentStatusEnum _paymentStatus = PaymentStatusEnum.Pending;
        #endregion

        #region Properties

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
        /// Gets or sets an amount
        /// </summary>
        public decimal Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether it's a partial refund; otherwize, full refund
        /// </summary>
        public bool IsPartialRefund
        {
            get
            {
                return _isPartialRefund;
            }
            set
            {
                _isPartialRefund = value;
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
        #endregion
    }
}
