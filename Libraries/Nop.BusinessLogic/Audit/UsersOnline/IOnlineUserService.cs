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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Installation;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Audit.UsersOnline
{
    /// <summary>
    /// Online user service interface
    /// </summary>
    public interface IOnlineUserService
    {
        /// <summary>
        /// Tracks current user
        /// </summary>
        void TrackCurrentUser();

        /// <summary>
        /// Clears user list
        /// </summary>
        void ClearUserList();

        /// <summary>
        /// Purges expired users
        /// </summary>
        void PurgeUsers();

        /// <summary>
        /// Get online users (guest)
        /// </summary>
        /// <returns>Online user list</returns>
        List<OnlineUserInfo> GetGuestList();

        /// <summary>
        /// Get online users (registered)
        /// </summary>
        /// <returns>Online user list</returns>
        List<OnlineUserInfo> GetRegisteredUsersOnline();

        /// <summary>
        /// Get online users (guests and registered users)
        /// </summary>
        /// <returns>Online user list</returns>
        List<OnlineUserInfo> GetAllUserList();
        
        /// <summary>
        /// Gets a value indicating whether tracking online users is enabled
        /// </summary>
        bool Enabled { get; set; }
        
        /// <summary>
        /// Gets a maximum online customer number
        /// </summary>
        int MaximumOnlineCustomers { get; set; }
    }
}