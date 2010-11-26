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
using System.Runtime.Serialization;

namespace NopSolutions.NopCommerce.Shipping.Methods.CanadaPost
{
    /// <summary>
    /// Information on a possible delivery rate
    /// </summary>
    [DataContract]
    public class DeliveryRate
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the shipping date.
        /// </summary>
        /// <value>The shipping date.</value>
        [DataMember]
        public DateTime ShippingDate { get; set; }

        /// <summary>
        /// Gets or sets the delivery date.
        /// </summary>
        /// <value>The delivery date.</value>
        [DataMember]
        public string DeliveryDate { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>The sequence.</value>
        [DataMember]
        public int Sequence { get; set; }

        #endregion
    }
}
