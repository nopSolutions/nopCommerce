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
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Tasks;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Audit.UsersOnline
{
    /// <summary>
    /// Purge onlie users schedueled task implementation
    /// </summary>
    public partial class PurgeOnlineUsersTask : ITask
    {
        /// <summary>
        /// Executes the clear cache task
        /// </summary>
        /// <param name="node">XML node that represents a task description</param>
        public void Execute(XmlNode node)
        {
            try
            {
                IoCFactory.Resolve<IOnlineUserManager>().PurgeUsers();
            }
            catch (Exception ex)
            {
                IoCFactory.Resolve<ILogManager>().InsertLog(LogTypeEnum.CustomerError, "Error purging online users.", ex);
            }
        }
    }
}
