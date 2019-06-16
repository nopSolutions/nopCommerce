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
    /// Represents the response from the web service after a payment request was registered
    /// </summary>
    internal class PaymentRequestResponse
    {
        private Uri paymentRedirectBaseUri;
        private string code = String.Empty;

        /// <summary>
        /// Initializes a new instance of the PaymentRequestResponse class
        /// </summary>
        /// <param name="paymentRedirectBaseUri"></param>
        internal PaymentRequestResponse(Uri paymentRedirectBaseUri)
        {
            if (paymentRedirectBaseUri == null)
                throw new ArgumentNullException("paymentRedirectBaseUri");

            this.paymentRedirectBaseUri = paymentRedirectBaseUri;
        }

        /// <summary>
        /// Payment request code
        /// </summary>
        public string Code
        {
            get
            {
                return this.code;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.code = value;
            }
        }

        /// <summary>
        /// Registration date
        /// </summary>
        public DateTime RegistrationDate
        {
            get;
            set;
        }

        /// <summary>
        /// Uri for the payment page in the PagSeguro web site for this payment request
        /// </summary>
        public Uri PaymentRedirectUri
        {
            get
            {
                QueryStringBuilder builder = new QueryStringBuilder();
                builder.Append("code", this.Code);
                UriBuilder uriBuilder = new UriBuilder(this.paymentRedirectBaseUri);
                uriBuilder.Query = builder.ToString();
                return uriBuilder.Uri;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.GetType().Name);
            builder.Append('(');
            builder.Append("Code=").Append(this.Code).Append(", ");
            builder.Append("RegistrationDate=").Append(this.RegistrationDate).Append(", ");
            builder.Append("PaymentRedirectUri=").Append(this.PaymentRedirectUri.ToString());
            builder.Append(')');
            return builder.ToString();
        }
    }
}
