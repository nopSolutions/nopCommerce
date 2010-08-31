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

        private static Dictionary<Guid, OnlineUserInfo> GetAnonymousUserList()
        {
            string key = "Nop.OnlineUserList.Anonymous";
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

        private static Dictionary<Guid, OnlineUserInfo> GetRegisteredUserList()
        {
            string key = "Nop.OnlineUserList.Registered";
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


        /// <summary>
        /// Purges expired users
        /// </summary>
        /// <param name="userList">User list</param>
        protected static void PurgeUsers(Dictionary<Guid, OnlineUserInfo> userList)
        {
            if (!OnlineUserManager.Enabled)
                return;

            if (userList == null)
                throw new ArgumentNullException("userList");

            int expMinutes = 20;

            lock (s_lock)
            {
                //user list
                List<Guid> usersToRemove = new List<Guid>();

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
        #endregion

        #region Methods

        /// <summary>
        /// Tracks current user
        /// </summary>
        public static void TrackCurrentUser()
        {
            try
            {
                if (!OnlineUserManager.Enabled || 
                    !InstallerHelper.ConnectionStringIsSet() ||
                    HttpContext.Current == null)
                    return;

                lock (s_lock)
                {
                    //getting current user info (OnlineUserInfo)
                    OnlineUserInfo oui = null;

                    //user list
                    Dictionary<Guid, OnlineUserInfo> userList = null;
                    if (NopContext.Current.User != null && !NopContext.Current.User.IsGuest)
                    {
                        //registered user
                        userList = GetRegisteredUserList();

                        //find user info
                        if (userList.ContainsKey(NopContext.Current.User.CustomerGuid))
                        {
                            oui = userList[NopContext.Current.User.CustomerGuid];
                        }

                        //create new user if existing one was not found
                        if (oui == null)
                        {
                            oui = new OnlineUserInfo();
                            oui.OnlineUserGuid = NopContext.Current.User.CustomerGuid;
                            oui.CreatedOn = DateTime.UtcNow;
                            userList.Add(oui.OnlineUserGuid, oui);
                        }

                        //update other properties
                        oui.LastVisit = DateTime.UtcNow;
                        oui.LastPageVisited = CommonHelper.GetThisPageUrl(false);
                        oui.IPAddress = NopContext.Current.UserHostAddress;
                        oui.AssociatedCustomerId = NopContext.Current.User.CustomerId;
                        HttpContext.Current.Response.Cookies.Remove(TRACKINGCOOKIENAME);
                    }
                    else
                    {
                        //guest
                        userList = GetAnonymousUserList();

                        //find user info
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

                        //create new user if existing one was not found
                        if (oui == null)
                        {
                            oui = new OnlineUserInfo();
                            oui.OnlineUserGuid = Guid.NewGuid();
                            oui.CreatedOn = DateTime.UtcNow;
                            userList.Add(oui.OnlineUserGuid, oui);
                        }

                        //update other properties
                        oui.LastVisit = DateTime.UtcNow;
                        oui.LastPageVisited = CommonHelper.GetThisPageUrl(false);
                        oui.IPAddress = NopContext.Current.UserHostAddress;
                        oui.AssociatedCustomerId = null;

                        //save new cookie
                        HttpCookie cookie = new HttpCookie(TRACKINGCOOKIENAME);
                        cookie.Value = oui.OnlineUserGuid.ToString();
                        cookie.Expires = DateTime.Now.AddHours(1);
                        HttpContext.Current.Response.Cookies.Remove(TRACKINGCOOKIENAME);
                        HttpContext.Current.Response.Cookies.Add(cookie);
                    }

                    //maximum online customers
                    int currentVisitors = GetAnonymousUserList().Count() + GetRegisteredUserList().Count();
                    if (currentVisitors > OnlineUserManager.MaximumOnlineCustomers)
                    {
                        OnlineUserManager.MaximumOnlineCustomers = currentVisitors;
                    }
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
                Dictionary<Guid, OnlineUserInfo> userList1 = GetAnonymousUserList();
                Dictionary<Guid, OnlineUserInfo> userList2 = GetRegisteredUserList();
                userList1.Clear();
                userList2.Clear();
            }
        }

        /// <summary>
        /// Purges expired users
        /// </summary>
        public static void PurgeUsers()
        {
            if (!OnlineUserManager.Enabled)
                return;

            PurgeUsers(GetAnonymousUserList());
            PurgeUsers(GetRegisteredUserList());
        }

        /// <summary>
        /// Get online users (guest)
        /// </summary>
        /// <returns>Online user list</returns>
        public static List<OnlineUserInfo> GetGuestList()
        {
            lock (s_lock)
            {
                //user list
                List<OnlineUserInfo> users = new List<OnlineUserInfo>();

                Dictionary<Guid, OnlineUserInfo> userList = GetAnonymousUserList();
                foreach (KeyValuePair<Guid, OnlineUserInfo> kvp in userList)
                {
                    users.Add(kvp.Value);
                }

                return users.OrderByDescending(oui => oui.LastVisit).ToList();
            }
        }

        /// <summary>
        /// Get online users (registered)
        /// </summary>
        /// <returns>Online user list</returns>
        public static List<OnlineUserInfo> GetRegisteredUsersOnline()
        {
            lock (s_lock)
            {
                //user list
                List<OnlineUserInfo> users = new List<OnlineUserInfo>();

                Dictionary<Guid, OnlineUserInfo> userList = GetRegisteredUserList();
                foreach (KeyValuePair<Guid, OnlineUserInfo> kvp in userList)
                {
                    users.Add(kvp.Value);
                }

                return users.OrderByDescending(oui => oui.LastVisit).ToList();
            }
        }

        /// <summary>
        /// Get online users (guests and registered users)
        /// </summary>
        /// <returns>Online user list</returns>
        public static List<OnlineUserInfo> GetAllUserList()
        {
            lock (s_lock)
            {
                //user list
                var allUsers = new List<OnlineUserInfo>();
                allUsers.AddRange(GetRegisteredUsersOnline());
                allUsers.AddRange(GetGuestList());

                return allUsers.OrderByDescending(oui => oui.LastVisit).ToList();
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
        
        /// <summary>
        /// Gets a maximum online customer number
        /// </summary>
        public static int MaximumOnlineCustomers
        {
            get
            {
                return SettingManager.GetSettingValueInteger("OnlineUserManager.MaximumOnlineCustomers", 0);
            }
            set
            {
                SettingManager.SetParam("OnlineUserManager.MaximumOnlineCustomers", value.ToString());
            }
        }
        #endregion
    }
}