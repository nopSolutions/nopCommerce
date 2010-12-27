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
using Nop.Core.Domain;

namespace Nop.Core
{
    /// <summary>
    /// Working context
    /// </summary>
    public interface IWorkingContext
    {
        /// <summary>
        /// Gets or sets a value indicating whether the context is running in admin-mode
        /// </summary>
        bool IsAdmin {get;set;}

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        Customer User { get; set; }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        Language WorkingLanguage { get; set; }

        /// <summary>
        /// Get or set current theme (e.g. darkOrange)
        /// </summary>
        string WorkingTheme { get; set; }
    }
}
