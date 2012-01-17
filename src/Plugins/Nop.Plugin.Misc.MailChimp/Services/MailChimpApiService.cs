using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
        /// <param name="recordList">The records</param>
        public virtual listBatchUnsubscribeOutput BatchUnsubscribe(IEnumerable<MailChimpEventQueueRecord> recordList)
        {
            if (String.IsNullOrEmpty(_mailChimpSettings.DefaultListId)) 
                throw new ArgumentException("MailChimp list is not specified");

            var input = new listBatchUnsubscribeInput();
            //any directive overrides
            input.api_Validate = true;
            input.api_AccessType = EnumValues.AccessType.Serial;
            input.api_OutputType = EnumValues.OutputType.JSON;
            //method parameters
            input.parms.apikey = _mailChimpSettings.ApiKey;
            input.parms.id = _mailChimpSettings.DefaultListId;

            //batch the emails
            var batch = recordList.Select(sub => sub.Email).ToList();

            input.parms.emails = batch;

            //execution
            var cmd = new listBatchUnsubscribe(input);

            listBatchUnsubscribeOutput listSubscribeOutput = cmd.Execute();
            return listSubscribeOutput;
        }

        /// <summary>
        /// Batches the subscribe.
        /// </summary>
        /// <param name="recordList">The records</param>
        public virtual listBatchSubscribeOutput BatchSubscribe(IEnumerable<MailChimpEventQueueRecord> recordList)
        {
            if (String.IsNullOrEmpty(_mailChimpSettings.DefaultListId)) 
                throw new ArgumentException("MailChimp list is not specified");
            
            var input = new listBatchSubscribeInput();
            //any directive overrides
            input.api_Validate = true;
            input.api_AccessType = EnumValues.AccessType.Serial;
            input.api_OutputType = EnumValues.OutputType.JSON;
            //method parameters
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

            //execution
            var cmd = new listBatchSubscribe(input);

            listBatchSubscribeOutput listSubscribeOutput = cmd.Execute();
            return listSubscribeOutput;
        }
        
        public virtual SyncResult Synchronize()
        {
            var result = new SyncResult();

            // Get all the queued records for subscription/unsubscription
            var allRecords = _subscriptionEventQueueingService.GetAll();
            //get unique and latest records
            var allRecordsUnique = new List<MailChimpEventQueueRecord>();
            foreach (var item in allRecords
                .OrderByDescending(x => x.CreatedOnUtc))
            {
                var exists = allRecordsUnique
                    .Where(x => x.Email.Equals(item.Email, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault() != null;
                if (!exists)
                    allRecordsUnique.Add(item);
            }
            var subscribeRecords = allRecordsUnique.Where(x => x.IsSubscribe).ToList();
            var unsubscribeRecords = allRecordsUnique.Where(x => !x.IsSubscribe).ToList();
            
            //subscribe
            if (subscribeRecords.Count > 0)
            {
                var subscribeResult = BatchSubscribe(subscribeRecords);
                //result
                if (subscribeResult.api_ErrorMessages.Count > 0)
                {
                    foreach (var error in subscribeResult.api_ErrorMessages)
                        result.SubscribeErrors.Add(error.error);
                }
                else
                {
                    result.SubscribeResult = subscribeResult.ToString();
                }
            }
            else
            {
                result.SubscribeResult = "No records to add";
            }
            //unsubscribe
            if (unsubscribeRecords.Count > 0)
            {
                var unsubscribeResult = BatchUnsubscribe(unsubscribeRecords);
                //result
                if (unsubscribeResult.api_ErrorMessages.Count > 0)
                {
                    foreach (var error in unsubscribeResult.api_ErrorMessages)
                        result.UnsubscribeErrors.Add(error.error);
                }
                else
                {
                    result.UnsubscribeResult = unsubscribeResult.ToString();
                }
            }
            else
            {
                result.UnsubscribeResult = "No records to unsubscribe";
            }

            //delete the queued records
            foreach (var sub in allRecords)
                _subscriptionEventQueueingService.Delete(sub);

            //other useful properties of listBatchSubscribeOutput and listBatchUnsubscribeOutput
            //output.result.add_count
            //output.result.error_count
            //output.result.update_count
            //output.result.errors
            //output.api_Request, output.api_Response, // raw data
            //output.api_ErrorMessages, output.api_ValidatorMessages); // & errors
            return result;
        }
    }
}