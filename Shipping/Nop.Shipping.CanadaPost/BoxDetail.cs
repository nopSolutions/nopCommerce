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

using System.Runtime.Serialization;

namespace NopSolutions.NopCommerce.Shipping.Methods.CanadaPost
{
    /// <summary>
    /// Contain the Box detail selected by the response
    /// </summary>
    [DataContract]
    public class BoxDetail
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>The weight.</value>
        [DataMember]
        public double Weight { get; set; }

        /// <summary>
        /// Gets or sets the exprediter weight.
        /// </summary>
        /// <value>The exprediter weight.</value>
        [DataMember]
        public double ExpediterWeight { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        [DataMember]
        public double Length { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [DataMember]
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [DataMember]
        public double Height { get; set; }
        
        /// <summary>
        /// Gets or sets the Quantity.
        /// </summary>
        /// <value>The quantity.</value>
        [DataMember]
        public int Quantity { get; set; }
        #endregion
    }
}
