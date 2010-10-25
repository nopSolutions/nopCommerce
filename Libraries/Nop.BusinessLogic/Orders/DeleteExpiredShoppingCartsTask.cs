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
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Tasks;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents a task for deleting expires shopping carts
    /// </summary>
    public partial class DeleteExpiredShoppingCartsTask : ITask
    {
        private int _deleteExpiredShoppingCartsOlderThanMinutes = 259200; //6 months

        /// <summary>
        /// Executes a task
        /// </summary>
        /// <param name="node">Xml node that represents a task description</param>
        public void Execute(XmlNode node)
        {
            XmlAttribute attribute1 = node.Attributes["deleteExpiredShoppingCartsOlderThanMinutes"];
            if (attribute1 != null && !String.IsNullOrEmpty(attribute1.Value))
            {
                this._deleteExpiredShoppingCartsOlderThanMinutes = int.Parse(attribute1.Value);
            }

            DateTime olderThan = DateTime.UtcNow;
            olderThan = olderThan.AddMinutes(-(double)this._deleteExpiredShoppingCartsOlderThanMinutes);
            IoCFactory.Resolve<IShoppingCartManager>().DeleteExpiredShoppingCartItems(olderThan);
        }
    }
}
