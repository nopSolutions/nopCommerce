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

namespace NopSolutions.NopCommerce.Payment.Methods.PayPal
{
    /// <summary>
    /// Represents paypal payer info
    /// </summary>
    public class PaypalPayer
    {
        /// <summary>
        /// Paypal payer 
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Paypal payer identifier
        /// </summary>
        public string PayerID { get; set; }
        /// <summary>
        /// Paypal payer country name
        /// </summary>
        public string PaypalCountryName { get; set; }
        /// <summary>
        /// Paypal payer email
        /// </summary>
        public string PayerEmail { get; set; }
        /// <summary>
        /// Paypal payer first name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Paypal payer last name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Paypal payer company name
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// Paypal payer address 1
        /// </summary>
        public string Address1 { get; set; }
        /// <summary>
        /// Paypal payer address 2 
        /// </summary>
        public string Address2 { get; set; }
        /// <summary>
        /// Paypal payer city
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Paypal payer state
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Paypal payer zip
        /// </summary>
        public string Zipcode { get; set; }
        /// <summary>
        /// Paypal payer phone
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Paypal payer country code
        /// </summary>
        public string CountryCode { get; set; }
    }
}
