using System;
using System.Xml;
using Nop.Core;
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
            var maxTriesAttribute = node.Attributes["maxTries"];
            if (maxTriesAttribute != null && !string.IsNullOrWhiteSpace(maxTriesAttribute.Value))
            {
                this._maxTries = int.Parse(maxTriesAttribute.Value);
            }

            var queuedEmails = _queuedEmailService.SearchEmails(null, null, null, null,
                true, _maxTries, 0, 10000);
            foreach (var queuedEmail in queuedEmails)
            {
                var bcc = String.IsNullOrWhiteSpace(queuedEmail.Bcc) 
                            ? null 
                            : queuedEmail.Bcc.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var cc = String.IsNullOrWhiteSpace(queuedEmail.CC) 
                            ? null 
                            : queuedEmail.CC.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    _emailSender.SendEmail(queuedEmail.EmailAccount, queuedEmail.Subject, queuedEmail.Body,
                       queuedEmail.From, queuedEmail.FromName, queuedEmail.To, queuedEmail.ToName, bcc, cc);

                    queuedEmail.SentOnUtc = DateTime.UtcNow;
                }
                catch (Exception exc)
                {
                    _logger.Error(string.Format("Error sending e-mail. {0}", exc.Message), exc);
                }
                finally
                {
                    queuedEmail.SentTries = queuedEmail.SentTries + 1;
                    _queuedEmailService.UpdateQueuedEmail(queuedEmail);
                }
            }
        }
    }
}
