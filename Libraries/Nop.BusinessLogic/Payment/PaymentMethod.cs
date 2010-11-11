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
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Represents a payment method
    /// </summary>
    public partial class PaymentMethod : BaseEntity
    {
        #region Fields
        /// <summary>
        /// Payment method type
        /// </summary>
        protected PaymentMethodTypeEnum paymentMethodType =  PaymentMethodTypeEnum.Unknown;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the PaymentMethod class
        /// </summary>
        public PaymentMethod()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the payment method identifier
        /// </summary>
        public int PaymentMethodId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the visible name
        /// </summary>
        public string VisibleName { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the configure template path
        /// </summary>
        public string ConfigureTemplatePath { get; set; }

        /// <summary>
        /// Gets or sets the user template path
        /// </summary>
        public string UserTemplatePath { get; set; }

        /// <summary>
        /// Gets or sets the class name
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the system keyword
        /// </summary>
        public string SystemKeyword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the payment method is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        /// <returns>A payment method type</returns>
        public PaymentMethodTypeEnum PaymentMethodType
        {
            get
            {
                if (this.paymentMethodType == PaymentMethodTypeEnum.Unknown)
                {
                    try
                    {
                        this.paymentMethodType = IoC.Resolve<IPaymentService>().GetPaymentMethodType(this.PaymentMethodId);
                    }
                    catch { }
                }
                return this.paymentMethodType;
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the restricted countries
        /// </summary>
        public virtual ICollection<Country> NpRestrictedCountries { get; set; }

        #endregion
    }
}