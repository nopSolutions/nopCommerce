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
using Nop.Core;
using Nop.Core.Domain;
using System.Web;

namespace Nop.Services
{
    /// <summary>
    /// Working context
    /// </summary>
    public partial class WorkingContext : IWorkingContext
    {
        HttpContextBase _contextBase;

        public WorkingContext(HttpContextBase contextBase)
        {
            this._contextBase = contextBase;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the context is running in admin-mode
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public Customer User
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public Language WorkingLanguage
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get or set current theme (e.g. darkOrange)
        /// </summary>
        public string WorkingTheme
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
