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
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web
{
    /// <summary>
    /// Forum helper
    /// </summary>
    public partial class ForumHelper
    {
        #region Methods

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static List<PrivateMessage> GetCurrentUserSentPrivateMessages(int StartIndex, int PageSize)
        {
            int totalRecord = 0;
            return GetCurrentUserSentPrivateMessages(StartIndex, PageSize, out totalRecord);
        }

        public static List<PrivateMessage> GetCurrentUserSentPrivateMessages(int StartIndex, int PageSize, out int totalRecords)
        {
            if (PageSize <= 0)
                PageSize = 10;
            if (PageSize == int.MaxValue)
                PageSize = int.MaxValue - 1;

            int PageIndex = StartIndex / PageSize;

            totalRecords = 0;

            if (NopContext.Current.User == null)
                return new List<PrivateMessage>();

            var result = ForumManager.GetAllPrivateMessages(NopContext.Current.User.CustomerId, 0, null, false, null,
                string.Empty, PageSize, PageIndex, out totalRecords);
            return result;
        }

        public static int GetCurrentUserInboxPrivateMessagesCount()
        {
            int totalRecords = 0;
            GetCurrentUserInboxPrivateMessages(0, 1, out totalRecords);
            return totalRecords;
        }



        [DataObjectMethod(DataObjectMethodType.Select)]
        public static List<PrivateMessage> GetCurrentUserInboxPrivateMessages(int StartIndex, int PageSize)
        {
            int totalRecord = 0;
            return GetCurrentUserInboxPrivateMessages(StartIndex, PageSize, out totalRecord);
        }

        public static List<PrivateMessage> GetCurrentUserInboxPrivateMessages(int StartIndex, int PageSize, out int totalRecords)
        {
            if (PageSize <= 0)
                PageSize = 10;
            if (PageSize == int.MaxValue)
                PageSize = int.MaxValue - 1;

            int PageIndex = StartIndex / PageSize;

            totalRecords = 0;

            if (NopContext.Current.User == null)
                return new List<PrivateMessage>();

            var result = ForumManager.GetAllPrivateMessages(0, NopContext.Current.User.CustomerId, null, null, false,
                string.Empty, PageSize, PageIndex, out totalRecords);
            return result;
        }

        public static int GetCurrentUserForumSubscriptionsCount()
        {
            int totalRecords = 0;
            GetCurrentUserForumSubscriptions(0, 1, out totalRecords);
            return totalRecords;
        }



        [DataObjectMethod(DataObjectMethodType.Select)]
        public static List<ForumSubscription> GetCurrentUserForumSubscriptions(int StartIndex, int PageSize)
        {
            int totalRecord = 0;
            return GetCurrentUserForumSubscriptions(StartIndex, PageSize, out totalRecord);
        }

        public static List<ForumSubscription> GetCurrentUserForumSubscriptions(int StartIndex, int PageSize, out int totalRecords)
        {
            if (PageSize <= 0)
                PageSize = 10;
            if (PageSize == int.MaxValue)
                PageSize = int.MaxValue - 1;

            int PageIndex = StartIndex / PageSize;

            totalRecords = 0;

            if (NopContext.Current.User == null)
                return new List<ForumSubscription>();

            var result = ForumManager.GetAllSubscriptions(NopContext.Current.User.CustomerId, 0, 0, PageSize, PageIndex, out totalRecords);
            return result;
        }
        
        public static int GetCurrentUserSentPrivateMessagesCount()
        {
            int totalRecords = 0;
            GetCurrentUserSentPrivateMessages(0, 1, out totalRecords);
            return totalRecords;
        }



        [DataObjectMethod(DataObjectMethodType.Select)]
        public static List<ForumPost> GetUserLatestPosts(int StartIndex, int PageSize)
        {
            int totalRecord = 0;
            return GetUserLatestPosts(StartIndex, PageSize, out totalRecord);
        }

        public static List<ForumPost> GetUserLatestPosts(int StartIndex, int PageSize, out int totalRecords)
        {
            if (PageSize <= 0)
                PageSize = 10;
            if (PageSize == int.MaxValue)
                PageSize = int.MaxValue - 1;

            int PageIndex = StartIndex / PageSize;

            totalRecords = 0;
            //it's used on profile.aspx page, so we're using UserId query string parameter
            int userId = CommonHelper.QueryStringInt("UserId");
            var user = CustomerManager.GetCustomerById(userId);
            if (user == null)
                return new List<ForumPost>();

            var result = ForumManager.GetAllPosts(0,
                    user.CustomerId, string.Empty, false, PageSize, PageIndex, out totalRecords);
            return result;
        }

        public static int GetUserLatestPostCount()
        {
            int totalRecords = 0;
            GetUserLatestPosts(0, 1, out totalRecords);
            return totalRecords;
        }

        #endregion
    }
}
