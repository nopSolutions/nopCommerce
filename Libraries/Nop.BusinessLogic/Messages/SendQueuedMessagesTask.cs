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
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Messages
{
    /// <summary>
    /// Represents a task for sending queued message 
    /// </summary>
    public partial class SendQueuedMessagesTask : ITask
    {
        private int _maxTries = 5;

        /// <summary>
        /// Executes a task
        /// </summary>
        /// <param name="node">Xml node that represents a task description</param>
        public void Execute(XmlNode node)
        {
            XmlAttribute attribute1 = node.Attributes["maxTries"];
            if (attribute1 != null && !String.IsNullOrEmpty(attribute1.Value))
            {
                this._maxTries = int.Parse(attribute1.Value);
            }

            var queuedEmails = IoC.Resolve<IMessageService>().GetAllQueuedEmails(10000, true, _maxTries);
            foreach (QueuedEmail queuedEmail in queuedEmails)
            {
                List<string> bcc = new List<string>();
                foreach (string str1 in queuedEmail.Bcc.Split(new char[] { ';' },  StringSplitOptions.RemoveEmptyEntries))
                {
                    bcc.Add(str1);
                }
                List<string> cc = new List<string>();
                foreach (string str1 in queuedEmail.CC.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    cc.Add(str1);
                }

                try
                {
                    IoC.Resolve<IMessageService>().SendEmail(queuedEmail.Subject, queuedEmail.Body,
                       new MailAddress(queuedEmail.From, queuedEmail.FromName),
                       new MailAddress(queuedEmail.To, queuedEmail.ToName), bcc, cc, queuedEmail.EmailAccount);

                    queuedEmail.SendTries = queuedEmail.SendTries + 1;
                    queuedEmail.SentOn = DateTime.UtcNow;
                    IoC.Resolve<IMessageService>().UpdateQueuedEmail(queuedEmail);
                }
                catch (Exception exc)
                {
                    queuedEmail.SendTries = queuedEmail.SendTries + 1;
                    IoC.Resolve<IMessageService>().UpdateQueuedEmail(queuedEmail);

                    IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.MailError, string.Format("Error sending e-mail. {0}", exc.Message), exc);
                }
            }
        }

    }
}
