using System;
using System.Xml;

using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using Nop.Core.Tasks;
using Nop.Services.Logging;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Represents a task for sending queued message 
    /// </summary>
    public partial class QueuedMessagesSendTask : ITask
    {
        private int _maxTries = 5;

        private readonly IQueuedEmailService _queuedEmailService = EngineContext.Current.Resolve<IQueuedEmailService>();
        private readonly IEmailSender _emailSender = EngineContext.Current.Resolve<IEmailSender>();
        private readonly ILogger _logger = EngineContext.Current.Resolve<ILogger>();

        /// <summary>
        /// Executes a task
        /// </summary>
        /// <param name="node">Xml node that represents a task description</param>
        public void Execute(XmlNode node)
        {
            var maxTriesSttribute = node.Attributes["maxTries"];
            if (maxTriesSttribute != null && !string.IsNullOrWhiteSpace(maxTriesSttribute.Value))
            {
                this._maxTries = int.Parse(maxTriesSttribute.Value);
            }

            var queuedEmails = _queuedEmailService.GetAllQueuedEmails(10000, true, _maxTries);
            foreach (var queuedEmail in queuedEmails)
            {
                var bcc = queuedEmail.Bcc.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var cc = queuedEmail.CC.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    _emailSender.SendEmail(queuedEmail.EmailAccount, queuedEmail.Subject, queuedEmail.Body,
                       queuedEmail.From, queuedEmail.FromName, queuedEmail.To, queuedEmail.ToName, bcc, cc);

                    queuedEmail.SentOn = DateTime.UtcNow;
                }
                catch (Exception exc)
                {
                    _logger.InsertLog(LogLevel.Error, string.Format("Error sending e-mail. {0}", exc.Message), exc);
                }
                finally
                {
                    queuedEmail.SendTries = queuedEmail.SendTries + 1;
                    _queuedEmailService.UpdateQueuedEmail(queuedEmail);
                }
            }
        }
    }
}
