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
using System.Linq;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.Common;
using System.Diagnostics;

namespace NopSolutions.NopCommerce.BusinessLogic.Audit.UsersOnline
{
    /// <summary>
    /// Represents an online user manager
    /// </summary>
    public partial class OnlineUserManager
    {
        #region Const

        private const string TRACKINGCOOKIENAME = "nop.onlineusertracking";

        #endregion

        #region Fields

        private static object s_lock;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        static OnlineUserManager()
        {
            s_lock = new object();
        }

        #endregion

        #region Utilities

        private static Dictionary<Guid, OnlineUserInfo> GetUserList()
        {
            string key = "Nop.OnlineUserList";
            Dictionary<Guid, OnlineUserInfo> obj2 = NopStaticCache.Get(key) as Dictionary<Guid, OnlineUserInfo>;
            if (obj2 != null)
            {
                return obj2;
            }
            else
            {
                obj2 = new Dictionary<Guid, OnlineUserInfo>();
                NopStaticCache.Max(key, obj2);
            }

            return obj2;
        }

        #endregion 

        #region Methods

        /// <summary>
        /// Tracks current user
        /// </summary>
        public static void TrackCurrentUser()
        {
            try
            {
                if (!OnlineUserManager.Enabled || HttpContext.Current == null)
                    return;
                lock (s_lock)
                {
                    //user list
                    Dictionary<Guid, OnlineUserInfo> userList = GetUserList();

                    //getting current user info (OnlineUserInfo)
                    OnlineUserInfo oui = null;

                    //find online user info for registered user
                    if (NopContext.Current.User != null && !NopContext.Current.User.IsGuest)
                    {
                        foreach (var kvp in userList)
                        {
                            if (kvp.Value.AssociatedCustomerId.HasValue &&
                                kvp.Value.AssociatedCustomerId.Value == NopContext.Current.User.CustomerId)
                            {
                                oui = kvp.Value;
                                break;
                            }
                        }
                    }

                    //check tracking cookie if existing one was not found
                    if (oui == null)
                    {
                        string cookieValue = string.Empty;
                        if ((HttpContext.Current.Request.Cookies[TRACKINGCOOKIENAME] != null) && (HttpContext.Current.Request.Cookies[TRACKINGCOOKIENAME].Value != null))
                        {
                            cookieValue = HttpContext.Current.Request.Cookies[TRACKINGCOOKIENAME].Value;
                        }
                        if (!string.IsNullOrEmpty(cookieValue))
                        {
                            Guid onlineUserGuid = Guid.Empty;
                            Guid.TryParse(cookieValue, out onlineUserGuid);
                            if (onlineUserGuid != Guid.Empty)
                            {
                                if (userList.ContainsKey(onlineUserGuid))
                                {
                                    oui = userList[onlineUserGuid];
                                }
                            }
                        }
                    }

                    //create new user if existing one was not found
                    if (oui == null)
                    {
                        oui = new OnlineUserInfo();
                        oui.OnlineUserGuid = Guid.NewGuid();
                        oui.CreatedOn = DateTime.UtcNow;
                        userList.Add(oui.OnlineUserGuid, oui);
                    }

                    //update LastVisit and AssociatedCustomerGuid properties
                    oui.LastVisit = DateTime.UtcNow;
                    if (NopContext.Current.User != null && !NopContext.Current.User.IsGuest)
                        oui.AssociatedCustomerId = NopContext.Current.User.CustomerId;
                    else
                        oui.AssociatedCustomerId = null;

                    //save new cookie
                    HttpCookie cookie = new HttpCookie(TRACKINGCOOKIENAME);
                    cookie.Value = oui.OnlineUserGuid.ToString();
                    cookie.Expires = DateTime.Now.AddHours(1);
                    HttpContext.Current.Response.Cookies.Remove(TRACKINGCOOKIENAME);
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
            catch (NopException exc)
            {
                Debug.WriteLine(exc.ToString());
            }
        }

        /// <summary>
        /// Clears user list
        /// </summary>
        public static void ClearUserList()
        {
            if (!OnlineUserManager.Enabled)
                return;

            lock (s_lock)
            {
                //user list
                Dictionary<Guid, OnlineUserInfo> userList = GetUserList();
                userList.Clear();
            }
        }

        /// <summary>
        /// Purges expired users
        /// </summary>
        public static void PurgeUsers()
        {
            if (!OnlineUserManager.Enabled)
                return;

            int expMinutes = 20;

            lock (s_lock)
            {
                //user list
                List<Guid> usersToRemove = new List<Guid>();

                Dictionary<Guid, OnlineUserInfo> userList = GetUserList();
                foreach (KeyValuePair<Guid, OnlineUserInfo> kvp in userList)
                {
                    if (kvp.Value.LastVisit.AddMinutes(expMinutes) < DateTime.UtcNow)
                        usersToRemove.Add(kvp.Key);
                }

                foreach (Guid guid in usersToRemove)
                {
                    userList.Remove(guid);
                }
            }
        }

        /// <summary>
        /// Get online users
        /// </summary>
        /// <returns>Online user list</returns>
        public static List<OnlineUserInfo> GetOnlineUsers()
        {
            lock (s_lock)
            {
                //user list
                List<OnlineUserInfo> users = new List<OnlineUserInfo>();

                Dictionary<Guid, OnlineUserInfo> userList = GetUserList();
                foreach (KeyValuePair<Guid, OnlineUserInfo> kvp in userList)
                {
                    users.Add(kvp.Value);
                }

                return users;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether tracking online users is enabled
        /// </summary>
        public static bool Enabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("OnlineUserManager.Enabled", false);
            }
            set
            {
                SettingManager.SetParam("OnlineUserManager.Enabled", value.ToString());
            }
        }

        #endregion
    }
}
