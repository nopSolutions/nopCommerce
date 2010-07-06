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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace NopSolutions.NopCommerce.Web
{
    /// <summary>
    /// Checkout step event arguments
    /// </summary>
    public partial class CheckoutStepEventArgs : EventArgs
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CheckoutStepEventArgs()
        {

        }

        /// <summary>
        /// A value indicating whether to shipping address is selected
        /// </summary>
        public bool ShippingAddressSelected { get; set; }

        /// <summary>
        /// A value indicating whether to billing address is selected
        /// </summary>
        public bool BillingAddressSelected { get; set; }

        /// <summary>
        /// A value indicating whether to shipping method is selected
        /// </summary>
        public bool ShippingMethodSelected { get; set; }

        /// <summary>
        /// A value indicating whether to payment method is selected
        /// </summary>
        public bool PaymentMethodSelected { get; set; }

        /// <summary>
        /// A value indicating whether to payment information is entered
        /// </summary>
        public bool PaymentInfoEntered { get; set; }

        /// <summary>
        /// A value indicating whether to order is confirmed
        /// </summary>
        public bool OrderConfirmed { get; set; }
    }
}