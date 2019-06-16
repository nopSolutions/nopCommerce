// Copyright [2011] [PagSeguro Internet Ltda.]
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Uol.PagSeguro
{
    /// <summary>
    /// Represents a payment request
    /// </summary>
    public class PaymentRequest
    {
        private IList<Item> items;

        /// <summary>
        /// Initializes a new instance of the PaymentRequest class
        /// </summary>
        public PaymentRequest()
        {
            this.Currency = Uol.PagSeguro.Currency.Brl;
        }

        /// <summary>
        /// Party that will be sending the money
        /// </summary>
        public Sender Sender
        {
            get;
            set;
        }

        /// <summary>
        /// Payment currency. 
        /// </summary>
        /// <remarks>
        /// The expected currency values are defined in the <c cref="T:Uol.PagSeguro.Currency">Currencty</c> class.
        /// </remarks>
        public string Currency
        {
            get;
            set;
        }

        /// <summary>
        /// Products/items in this payment request
        /// </summary>
        public IList<Item> Items
        {
            get
            {
                if (this.items == null)
                {
                    this.items = new List<Item>();
                }
                return items;
            }
        }

        /// <summary>
        /// Uri to where the PagSeguro payment page should redirect the user after the payment information is processed.
        /// </summary>
        /// <remarks>
        /// Typically this is a confirmation page on your web site.
        /// </remarks>
        public Uri RedirectUri
        {
            get;
            set;
        }

        /// <summary>
        /// Extra amount to be added to the transaction total
        /// </summary>
        /// <remarks>
        /// This value can be used to add an extra charge to the transaction 
        /// or provide a discount in the case <c>ExtraAmount</c> is a negative value.
        /// </remarks>
        public decimal? ExtraAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Reference code
        /// </summary>
        /// <remarks>
        /// Optional. You can use the reference code to store an identifier so you can 
        /// associate the PagSeguro transaction to a transaction in your system.
        /// </remarks>
        public string Reference
        {
            get;
            set;
        }

        /// <summary>
        /// Shipping information associated with this payment request
        /// </summary>
        public Shipping Shipping
        {
            get;
            set;
        }

        /// <summary>
        /// How long this payment request will remain valid, in seconds.
        /// </summary>
        /// <remarks>
        /// Optional. After this payment request is submitted, the payment code returned
        /// will remain valid for the period specified here. 
        /// </remarks>
        public long? MaxAge
        {
            get;
            set;
        }

        /// <summary>
        /// How many times the payment redirect uri returned by the payment web service can be accessed.
        /// </summary>
        /// <remarks>
        /// Optional. After this payment request is submitted, the payment redirect uri returned by
        /// the payment web service will remain valid for the number of uses specified here. 
        /// 
        /// See also <seealso cref="Uol.PagSeguro.PaymentRequestResponse.PaymentRedirectUri"/>
        /// </remarks>
        public long? MaxUses
        {
            get;
            set;
        }

        /// <summary>
        /// Calls the PagSeguro web service and register this request for payment
        /// </summary>
        /// <param name="credentials">PagSeguro credentials</param>
        /// <returns>The Uri to where the user needs to be redirected to in order to complete the payment process</returns>
        public Uri Register(Credentials credentials)
        {
            return PaymentService.Register(credentials, this);
        }


        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.GetType().Name).Append("(");
            builder.Append("Reference=").Append(this.Reference).Append(", ");
            string email = this.Sender == null ? null : this.Sender.Email;
            builder.Append("Sender.Email=").Append(email).Append(")");
            return builder.ToString();
        }
    }
}
