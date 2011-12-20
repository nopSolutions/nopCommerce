using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Nop.Plugin.Misc.MailChimp.Data;
using Nop.Services.Logging;
using PerceptiveMCAPI;
using PerceptiveMCAPI.Methods;
using PerceptiveMCAPI.Types;

namespace Nop.Plugin.Misc.MailChimp.Services
{
    public class MailChimpApiService : IMailChimpApiService
    {
        private readonly MailChimpSettings _mailChimpSettings;
        private readonly ISubscriptionEventQueueingService _subscriptionEventQueueingService;
        private readonly ILogger _log;

        public MailChimpApiService(MailChimpSettings mailChimpSettings, ISubscriptionEventQueueingService subscriptionEventQueueingService, ILogger log)
        {
            _mailChimpSettings = mailChimpSettings;
            _subscriptionEventQueueingService = subscriptionEventQueueingService;
            _log = log;
        }

        /// <summary>
        /// Retrieves the lists.
        /// </summary>
        /// <returns></returns>
        public virtual NameValueCollection RetrieveLists()
        {
            var output = new NameValueCollection();
            try
            {

                // input parameters
                var listInput = new listsInput(_mailChimpSettings.ApiKey);

                // execute the request
                var cmd = new lists(listInput);
                listsOutput listOutput = cmd.Execute();

                if (listOutput != null && listOutput.result != null && listOutput.result.total > 0)
                {
                    foreach (listsResults.DataItem item in listOutput.result.data)
                    {
                        output.Add(item.name, item.id);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Debug(e.Message, e);
            }
            return output;
        }

        /// <summary>
        /// Batches the unsubscribe.
        /// </summary>
        public virtual listBatchUnsubscribeOutput BatchUnsubscribe()
        {
            if (String.IsNullOrEmpty(_mailChimpSettings.DefaultListId)) 
                throw new ArgumentException("MailChimp list is not specified");

            // Get all the queued records for subscription
            var recordList = _subscriptionEventQueueingService.ReadList(false);

            var input = new listBatchUnsubscribeInput();
            // any directive overrides
            input.api_Validate = true;
            input.api_AccessType = EnumValues.AccessType.Serial;
            input.api_OutputType = EnumValues.OutputType.JSON;
            // method parameters
            input.parms.apikey = _mailChimpSettings.ApiKey;
            input.parms.id = _mailChimpSettings.DefaultListId;

            //batch the emails
            var batch = recordList.Select(sub => sub.Email).ToList();

            input.parms.emails = batch;

            // execution
            var cmd = new listBatchUnsubscribe(input);

            listBatchUnsubscribeOutput listSubscribeOutput = cmd.Execute();

            return listSubscribeOutput;
        }

        /// <summary>
        /// Batches the subscribe.
        /// </summary>
        public virtual listBatchSubscribeOutput BatchSubscribe()
        {
            if (String.IsNullOrEmpty(_mailChimpSettings.DefaultListId)) 
                throw new ArgumentException("MailChimp list is not specified");

            // Get all the queued records for subscription
            var recordList = _subscriptionEventQueueingService.ReadList(true);

            var input = new listBatchSubscribeInput();
            // any directive overrides
            input.api_Validate = true;
            input.api_AccessType = EnumValues.AccessType.Serial;
            input.api_OutputType = EnumValues.OutputType.JSON;
            // method parameters
            input.parms.apikey = _mailChimpSettings.ApiKey;
            input.parms.id = _mailChimpSettings.DefaultListId;
            input.parms.double_optin = false;
            input.parms.replace_interests = true;
            input.parms.update_existing = true;
            var batch = new List<Dictionary<string, object>>();

            foreach (var sub in recordList)
            {
                var entry = new Dictionary<string, object>();
                entry.Add("EMAIL", sub.Email);
                batch.Add(entry);
            }

            input.parms.batch = batch;

            // execution
            var cmd = new listBatchSubscribe(input);

            listBatchSubscribeOutput listSubscribeOutput = cmd.Execute();

            return listSubscribeOutput;
        }
    }
}