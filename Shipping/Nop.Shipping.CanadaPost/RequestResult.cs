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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NopSolutions.NopCommerce.Shipping.Methods.CanadaPost
{/// <summary>
    /// Contain the result of a shipping request
    /// </summary>
    [DataContract]
    public class RequestResult
    {
        #region Fields
        private List<DeliveryRate> m_rates;
        private List<BoxDetail> m_boxes;
        #endregion

        #region Constructor

        public RequestResult()
        {
            this.m_rates = new List<DeliveryRate>();
            this.m_boxes = new List<BoxDetail>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>The status code.</value>
        [DataMember]
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        /// <value>The status message.</value>
        [DataMember]
        public string StatusMessage { get; set; }

        /// <summary>
        /// Gets or sets the available rates.
        /// </summary>
        /// <value>The available rates.</value>
        [DataMember]
        public List<DeliveryRate> AvailableRates
        {
            get
            {
                return m_rates;
            }
        }

        /// <summary>
        /// Gets the boxes.
        /// </summary>
        /// <value>The boxes.</value>
        [DataMember]
        public List<BoxDetail> Boxes
        {
            get
            {
                return m_boxes;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether it's error
        /// </summary>
        public bool IsError { get; set; }

        #endregion
    }
}
