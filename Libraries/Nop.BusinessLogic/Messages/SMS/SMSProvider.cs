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

namespace NopSolutions.NopCommerce.BusinessLogic.Messages.SMS
{
    /// <summary>
    /// Represents a SMS provider
    /// </summary>
    public partial class SMSProvider : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the SMSProvider class
        /// </summary>
        public SMSProvider()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the SMS provider identifier
        /// </summary>
        public int SMSProviderId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the class name
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the system keyword
        /// </summary>
        public string SystemKeyword { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the SMS provider is active
        /// </summary>
        public bool IsActive { get; set; }
        #endregion

        #region Custom properties
        /// <summary>
        /// Gets instance of ClassName type as ISMSProvider
        /// </summary>
        public ISMSProvider Instance
        {
            get
            {
                return Activator.CreateInstance(Type.GetType(ClassName)) as ISMSProvider;
            }
        }
        #endregion
    }
}