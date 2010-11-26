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

using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// Represents a shipping rate computation method
    /// </summary>
    public partial class ShippingRateComputationMethod : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the shipping rate computation method identifier
        /// </summary>
        public int ShippingRateComputationMethodId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the configure template path
        /// </summary>
        public string ConfigureTemplatePath { get; set; }

        /// <summary>
        /// Gets or sets the class name
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the method is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        /// <returns>A shipping rate computation method type</returns>
        public ShippingRateComputationMethodTypeEnum ShippingRateComputationMethodType
        {
            get
            {
                ShippingRateComputationMethodTypeEnum type = ShippingRateComputationMethodTypeEnum.Unknown;
                try
                {
                    type = IoC.Resolve<IShippingService>().GetShippingRateComputationMethodTypeEnum(this.ShippingRateComputationMethodId);
                }
                catch { }
                return type;
            }
        }

        #endregion
    }
}