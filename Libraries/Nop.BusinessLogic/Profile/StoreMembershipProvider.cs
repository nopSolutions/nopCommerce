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
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;


namespace NopSolutions.NopCommerce.BusinessLogic.Profile
{
    /// <summary>
    /// Manages storage of membership information for a nopCommerce application in a data source
    /// </summary>
    public partial class StoreMembershipProvider : MembershipProvider
    {
        #region Fields
        private string _appName;
        private bool _enablePasswordReset;
        private bool _enablePasswordRetrieval;
        private int _maxInvalidPasswordAttempts;
        private int _minRequiredNonalphanumericCharacters;
        private int _minRequiredPasswordLength;
        private int _passwordAttemptWindow;
        private string _passwordStrengthRegularExpression;
        private bool _requiresQuestionAndAnswer;
        private bool _requiresUniqueEmail;
        #endregion

        #region Methods
        /// <summary>
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <param name="username">The user to update the password for. </param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        /// <returns>true if the password was updated successfully; otherwise, false.</returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <param name="username">The user to change the password question and answer for.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
        /// <returns>true if the password question and answer are updated successfully; otherwise, false.</returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <param name="username">The user name for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="passwordQuestion">The password question for the new user.</param>
        /// <param name="passwordAnswer">The password answer for the new user.</param>
        /// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
        /// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
        /// <param name="status">A MembershipCreateStatus enumeration value indicating whether the user was created successfully.</param>
        /// <returns>A MembershipUser object populated with the information for the newly created user.</returns>
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            MembershipUser user = null;

            string _username = string.Empty;
            string _email = string.Empty;
            if (IoC.Resolve<ICustomerService>().UsernamesEnabled)
            {
                _username = username;
                _email = email;
            }
            else
            {
                //little hack here. username variable was used to store customer email
                _username = username;
                _email = username;
            }

            var customer = IoC.Resolve<ICustomerService>().AddCustomer(_email, _username, password, 
                false, false, true, out status);

            if (status == MembershipCreateStatus.Success)
            {
                var dt = DateTime.UtcNow;
                user = new MembershipUser(this.Name, _username, customer.CustomerGuid, _email, string.Empty, null, true, false, dt, dt, dt, dt, dt);
            }
            return user;
        }

        /// <summary>
        /// Removes a user from the membership data source.
        /// </summary>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        /// <returns>true if the user was successfully deleted; otherwise, false.</returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        /// </summary>
        /// <param name="EmailToMatch">The e-mail address to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>A MembershipUserCollection collection that contains a page of pageSizeMembershipUser objects beginning at the page specified by pageIndex.</returns>
        public override MembershipUserCollection FindUsersByEmail(string EmailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>A MembershipUserCollection collection that contains a page of pageSizeMembershipUser objects beginning at the page specified by pageIndex.</returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>A MembershipUserCollection collection that contains a page of pageSizeMembershipUser objects beginning at the page specified by pageIndex.</returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the number of users currently accessing the application.
        /// </summary>
        /// <returns>The number of users currently accessing the application.</returns>
        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the password for the specified user name from the data source.
        /// </summary>
        /// <param name="username">The user to retrieve the password for.</param>
        /// <param name="answer">The password answer for the user.</param>
        /// <returns>The password for the specified user name.</returns>
        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>A MembershipUser object populated with the specified user's information from the data source.</returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="username">The name of the user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>A MembershipUser object populated with the specified user's information from the data source.</returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            Customer customer = null;

            string _username = string.Empty;
            string _email = string.Empty;
            if (IoC.Resolve<ICustomerService>().UsernamesEnabled)
            {
                customer = IoC.Resolve<ICustomerService>().GetCustomerByUsername(username);
                if (customer != null)
                {
                    _username = customer.Username;
                    _email = customer.Email;
                }
            }
            else
            {
                //little hack here. username variable was used to store customer email
                customer = IoC.Resolve<ICustomerService>().GetCustomerByEmail(username);
                if (customer != null)
                {
                    _username = customer.Email;
                    _email = customer.Email;
                }
            }

            if (customer == null)
                return null;
            var dt = DateTime.UtcNow;

            return new MembershipUser(this.Name, _username, customer.CustomerGuid, _email, string.Empty, null, true, false, dt, dt, dt, dt, dt);
        }

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address to search for.</param>
        /// <returns>The user name associated with the specified e-mail address. If no match is found, return null.</returns>
        public override string GetUserNameByEmail(string email)
        {
            var customer = IoC.Resolve<ICustomerService>().GetCustomerByEmail(email);
            if (customer == null)
                return null;

            if (IoC.Resolve<ICustomerService>().UsernamesEnabled)
            {
                return customer.Username;
            }
            else
            {
                //little hack here. username variable was used to store customer email
                return customer.Email;
            }
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "StoreMembershipProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Membership Provider for NopCommerce");
            }
            base.Initialize(name, config);
            this._enablePasswordReset = CommonHelper.ConfigGetBooleanValue(config, "enablePasswordReset", true);
            this._enablePasswordRetrieval = CommonHelper.ConfigGetBooleanValue(config, "enablePasswordRetrieval", true);
            this._requiresQuestionAndAnswer = CommonHelper.ConfigGetBooleanValue(config, "requiresQuestionAndAnswer", true);
            this._requiresUniqueEmail = CommonHelper.ConfigGetBooleanValue(config, "requiresUniqueEmail", true);
            this._maxInvalidPasswordAttempts = CommonHelper.ConfigGetIntValue(config, "maxInvalidPasswordAttempts", 5, false, 0);
            this._passwordAttemptWindow = CommonHelper.ConfigGetIntValue(config, "passwordAttemptWindow", 10, false, 0);
            this._minRequiredPasswordLength = CommonHelper.ConfigGetIntValue(config, "minRequiredPasswordLength", 7, false, 0x80);
            this._minRequiredNonalphanumericCharacters = CommonHelper.ConfigGetIntValue(config, "minRequiredNonalphanumericCharacters", 1, true, 0x80);
            this._passwordStrengthRegularExpression = config["passwordStrengthRegularExpression"];
            if (this._passwordStrengthRegularExpression != null)
            {
                this._passwordStrengthRegularExpression = this._passwordStrengthRegularExpression.Trim();
                if (this._passwordStrengthRegularExpression.Length != 0)
                {
                    try
                    {
                        new Regex(this._passwordStrengthRegularExpression);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new ProviderException(ex.Message, ex);
                    }
                }
            }
            this._passwordStrengthRegularExpression = string.Empty;
            if (this._minRequiredNonalphanumericCharacters > this._minRequiredPasswordLength)
            {
                throw new HttpException("MinRequiredNonalphanumericCharacters can not be more than MinRequiredPasswordLength");
            }
            this._appName = config["applicationName"];
            if (string.IsNullOrEmpty(this._appName))
            {
                this._appName = "NopCommerce";
            }
            if (this._appName.Length > 0x100)
            {
                throw new ProviderException("Provider application name too long");
            }

            string connectionStringName = config["connectionStringName"];
            if (string.IsNullOrEmpty(connectionStringName))
            {
                this._appName = "NopSqlConnection";
            }

            config.Remove("enablePasswordReset");
            config.Remove("enablePasswordRetrieval");
            config.Remove("requiresQuestionAndAnswer");
            config.Remove("applicationName");
            config.Remove("requiresUniqueEmail");
            config.Remove("maxInvalidPasswordAttempts");
            config.Remove("passwordAttemptWindow");
            config.Remove("commandTimeout");
            config.Remove("name");
            config.Remove("minRequiredPasswordLength");
            config.Remove("minRequiredNonalphanumericCharacters");
            config.Remove("passwordStrengthRegularExpression");
            config.Remove("connectionStringName");
            if (config.Count > 0)
            {
                string key = config.GetKey(0);
                if (!string.IsNullOrEmpty(key))
                {
                    throw new ProviderException(string.Format("Provider unrecognized attribute {0}", key));
                }
            }
        }

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <param name="username">The user to reset the password for.</param>
        /// <param name="answer">The password answer for the specified user.</param>
        /// <returns>The new password for the specified user.</returns>
        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears a lock so that the membership user can be validated.
        /// </summary>
        /// <param name="userName">The membership user whose lock status you want to clear.</param>
        /// <returns>true if the membership user was successfully unlocked; otherwise, false.</returns>
        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates information about a user in the data source.
        /// </summary>
        /// <param name="user">A MembershipUser object that represents the user to update and the updated information for the user.</param>
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <returns>true if the specified username and password are valid; otherwise, false.</returns>
        public override bool ValidateUser(string username, string password)
        {
            Customer customer = null;

            string _email = string.Empty;
            if (IoC.Resolve<ICustomerService>().UsernamesEnabled)
            {
                customer = IoC.Resolve<ICustomerService>().GetCustomerByUsername(username);
                if (customer != null)
                {
                    _email = customer.Email;
                }
            }
            else
            {
                //little hack here. username variable was used to store customer email
                customer = IoC.Resolve<ICustomerService>().GetCustomerByEmail(username);
                if (customer != null)
                {
                    _email = customer.Email;
                }
            }

            return IoC.Resolve<ICustomerService>().Login(_email, password);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The name of the application using the custom membership provider.
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return this._appName;
            }
            set
            {
                this._appName = value;
            }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        public override bool EnablePasswordReset
        {
            get
            {
                return this._enablePasswordReset;
            }
        }

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return this._maxInvalidPasswordAttempts;
            }
        }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return this._minRequiredNonalphanumericCharacters;
            }
        }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        public override int MinRequiredPasswordLength
        {
            get
            {
                return this._minRequiredPasswordLength;
            }
        }

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        public override int PasswordAttemptWindow
        {
            get
            {
                return this._passwordAttemptWindow;
            }
        }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        public override string PasswordStrengthRegularExpression
        {
            get
            {
                return this._passwordStrengthRegularExpression;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return this._requiresQuestionAndAnswer;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        public override bool RequiresUniqueEmail
        {
            get
            {
                return this._requiresUniqueEmail;
            }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to retrieve their passwords.
        /// </summary>
        public override bool EnablePasswordRetrieval
        {
            get
            {
                return this._enablePasswordRetrieval;
            }
        }

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                return MembershipPasswordFormat.Hashed;
            }
        }
        #endregion
    }
}