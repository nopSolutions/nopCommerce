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
    public partial class WorkContext : IWorkContext
    {
        private bool _isAdminMode;
        private Customer _currentUser;
        private CustomerSession _customerSession;
        private Language _workingLanguage;
        private Currency _workingCurrency;
        private string _workingTheme;
        private readonly HttpContextBase _contextBase;

        public WorkContext(HttpContextBase contextBase)
        {
            this._contextBase = contextBase;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the context is running in admin-mode
        /// </summary>
        public bool IsAdminMode
        {
            get
            {
                return _isAdminMode;
            }
            set
            {
                _isAdminMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public Customer CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
            }
        }

        /// <summary>
        /// Gets or sets the current customer session
        /// </summary>
        public CustomerSession CustomerSession
        {
            get
            {
                return _customerSession;
            }
            set
            {
                _customerSession = value;
            }
        }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public Language WorkingLanguage
        {
            get
            {
                return _workingLanguage;
            }
            set
            {
                _workingLanguage = value;
            }
        }

        /// <summary>
        /// Get or set current user working currency
        /// </summary>
        public Currency WorkingCurrency
        {
            get
            {
                return _workingCurrency;
            }
            set
            {
                _workingCurrency = value;
            }
        }

        /// <summary>
        /// Get or set current theme (e.g. darkOrange)
        /// </summary>
        public string WorkingTheme
        {
            get
            {
                return _workingTheme;
            }
            set
            {
                _workingTheme = value;
            }
        }
    }
}
