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

using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace NopSolutions.NopCommerce.Shipping.Methods.CanadaPost
{
    /// <summary>
    /// An item of the parcel that will be shipped
    /// </summary>
    [DataContract]
    public class Item
    {
        #region Utilities

        /// <summary>
        /// Build an XML string with the item informations.
        /// </summary>
        /// <param name="includeComments">if set to <c>true</c> [include comments].</param>
        /// <returns></returns>
        internal string ToXml(bool includeComments)
        {
            var xmlString = new StringBuilder();
            xmlString.AppendLine("<item>");
            // Quantity (mandatory)
            xmlString.AppendLine("<quantity> " + this.Quantity + "</quantity>");
            // Weight (mandatory)
            xmlString.AppendLine("<weight> " + this.Weight.ToString("0.00", CultureInfo.InvariantCulture) + "</weight>");
            // Length (mandatory)
            xmlString.AppendLine("<length> " + this.Length.ToString("0.00", CultureInfo.InvariantCulture) + "</length>");
            // Width (mandatory)
            xmlString.AppendLine("<width> " + this.Width.ToString("0.00", CultureInfo.InvariantCulture) + "</width>");
            // Height (mandatory)
            xmlString.AppendLine("<height> " + this.Height.ToString("0.00", CultureInfo.InvariantCulture) + "</height>");
            // Description (mandatory)
            xmlString.AppendLine("<description> " + this.Description + "</description>");
            if (this.ReadyToShip == true)
            {   // if we want to include the comments in the xml
                if (includeComments == true)
                {
                    xmlString.AppendLine("<!--**********************************************-->");
                    xmlString.AppendLine("<!-- By adding the 'readyToShip' tag, Sell Online -->");
                    xmlString.AppendLine("<!-- will not pack this item in the boxes         -->");
                    xmlString.AppendLine("<!-- defined in the merchant profile.             -->");
                    xmlString.AppendLine("<!-- Instead, this item will be shipped in its    -->");
                    xmlString.AppendLine("<!--  original box: 1.5 kg and 20x30x20 cm        -->");
                    xmlString.AppendLine("<!--**********************************************-->");
                }
                // Ready to ship (optional)
                xmlString.AppendLine("<readyToShip/>");
            }
            xmlString.AppendLine("</item>");

            return xmlString.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        [DataMember]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the weight (kg).
        /// </summary>
        /// <value>The weight.</value>
        [DataMember]
        public decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the length (cm).
        /// </summary>
        /// <value>The length.</value>
        [DataMember]
        public decimal Length { get; set; }

        /// <summary>
        /// Gets or sets the width (cm).
        /// </summary>
        /// <value>The width.</value>
        [DataMember]
        public decimal Width { get; set; }

        /// <summary>
        /// Gets or sets the height (cm).
        /// </summary>
        /// <value>The height.</value>
        [DataMember]
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [ready to ship].
        /// </summary>
        /// <value><c>true</c> if [ready to ship]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool ReadyToShip { get; set; }

        #endregion
    }
}
