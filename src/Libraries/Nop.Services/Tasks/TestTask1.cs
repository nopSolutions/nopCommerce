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
using System.Net.Mail;
using System.Xml;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Tasks;
using Nop.Services.Logging;

namespace Nop.Services.Tasks
{
    /// <summary>
    /// Test task. TODO delete this file after testing
    /// </summary>
    public partial class TestTask1 : ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        /// <param name="node">Xml node that represents a task description</param>
        public void Execute(XmlNode node)
        {
            var service1 = EngineContext.Current.Resolve<IWebHelper>();

            //TODO find a solution. We can't resolve IWorkContext because HttpContext parameter could not be resolved.
            //var service2 = EngineContext.Current.Resolve<IWorkContext>();
        }

    }
}
