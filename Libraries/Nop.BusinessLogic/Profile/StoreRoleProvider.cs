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
using System.Web.Security;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Installation;

namespace NopSolutions.NopCommerce.BusinessLogic.Profile
{
    /// <summary>
    /// Manages storage of role membership information for a nopCommerce application in a data source.
    /// </summary>
    public partial class StoreRoleProvider : RoleProvider
    {
        #region Fields
        private string _appName;
        #endregion

        #region Methods
        /// <summary>
        /// Adds the specified user names to the specified roles for the configured applicationName
        /// </summary>
        /// <param name="usernames">A string array of user names to be added to the specified roles.</param>
        /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new role to the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to create.</param>
        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes a role from the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to delete.</param>
        /// <param name="throwOnPopulatedRole">If true, throw an exception if roleName has one or more members and do not delete roleName.</param>
        /// <returns>true if the role was successfully deleted; otherwise, false.</returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <returns>A string array containing the names of all the users where the user name matches usernameToMatch and the user is a member of the specified role.</returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>A string array containing the names of all the roles stored in the data source for the configured applicationName.</returns>
        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName.
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>A string array containing the names of all the roles that the specified user is in for the configured applicationName.</returns>
        public override string[] GetRolesForUser(string username)
        {
            var roles = new List<string>();

            if (InstallerHelper.ConnectionStringIsSet())
            {
                Customer customer = null;
                if (IoC.Resolve<ICustomerService>().UsernamesEnabled)
                {
                    customer = IoC.Resolve<ICustomerService>().GetCustomerByUsername(username);
                }
                else
                {
                    customer = IoC.Resolve<ICustomerService>().GetCustomerByEmail(username);
                }
                if (customer == null)
                {
                    return roles.ToArray();
                }
                else
                {
                    var customerRoles = IoC.Resolve<ICustomerService>().GetCustomerRolesByCustomerId(customer.CustomerId, false);
                    foreach (var cr in customerRoles)
                    {
                        if (cr.Active)
                        {
                            if (!roles.Contains(cr.Name))
                                roles.Add(cr.Name);
                        }
                    }
                }
            }
            return roles.ToArray();
        }

        /// <summary>
        /// Gets a list of users in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to get the list of users for.</param>
        /// <returns>A string array containing the names of all the users who are members of the specified role for the configured applicationName.</returns>
        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
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
                name = "StoreRoleProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Roles Provider");
            }
            base.Initialize(name, config);
            this._appName = config["applicationName"];
            if (string.IsNullOrEmpty(this._appName))
            {
                this._appName = "NopCommerce";
            }
            if (this._appName.Length > 0x100)
            {
                throw new ProviderException("Provider application name too long");
            }
            config.Remove("applicationName");

            string connectionStringName = config["connectionStringName"];
            if (string.IsNullOrEmpty(connectionStringName))
            {
                this._appName = "NopSqlConnection";
            }
            config.Remove("connectionStringName");

            if (config.Count > 0)
            {
                string text2 = config.GetKey(0);
                if (!string.IsNullOrEmpty(text2))
                {
                    throw new ProviderException(string.Format("Provider unrecognized attribute {0}", text2));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified user is in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="username">The user name to search for.</param>
        /// <param name="roleName">The role to search in.</param>
        /// <returns>true if the specified user is in the specified role for the configured applicationName; otherwise, false.</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            if (String.IsNullOrEmpty(roleName))
                return false;

            if (InstallerHelper.ConnectionStringIsSet())
            {
                Customer customer = null;
                if (IoC.Resolve<ICustomerService>().UsernamesEnabled)
                {
                    customer = IoC.Resolve<ICustomerService>().GetCustomerByUsername(username);
                }
                else
                {
                    customer = IoC.Resolve<ICustomerService>().GetCustomerByEmail(username);
                }

                if (customer == null)
                {
                    return false;
                }

                var customerRoles = IoC.Resolve<ICustomerService>().GetCustomerRolesByCustomerId(customer.CustomerId, false);
                foreach (var cr in customerRoles)
                {
                    if (cr.Active)
                    {
                        if (roleName.ToLowerInvariant() == cr.Name.ToLowerInvariant())
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the specified user names from the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
        /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in the role data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to search for in the data source.</param>
        /// <returns>true if the role name already exists in the data source for the configured applicationName; otherwise, false.</returns>
        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the application to store and retrieve role information for.
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
        #endregion
    }
}
