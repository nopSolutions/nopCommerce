using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Nop.Plugin.Misc.MailChimp.Data;
using PerceptiveMCAPI;
using PerceptiveMCAPI.Methods;
using PerceptiveMCAPI.Types;

namespace Nop.Plugin.Misc.MailChimp.Services {
    public class MailChimpApiService : IMailChimpApiService {
        private readonly MailChimpSettings _mailChimpSettings;
        private readonly ISubscriptionEventQueueingService _subscriptionEventQueueingService;

        public MailChimpApiService(MailChimpSettings mailChimpSettings, ISubscriptionEventQueueingService subscriptionEventQueueingService) {
            _mailChimpSettings = mailChimpSettings;
            _subscriptionEventQueueingService = subscriptionEventQueueingService;
        }

        #region Implementation of IMailChimpApiService

        /// <summary>
        /// Retrieves the lists.
        /// </summary>
        /// <returns></returns>
        public NameValueCollection RetrieveLists() {
            var output = new NameValueCollection();

            // input parameters
            var listInput = new listsInput(_mailChimpSettings.ApiKey);

            // execute the request
            var cmd = new lists(listInput);
            listsOutput listOutput = cmd.Execute();

            if (listOutput != null && listOutput.result != null && listOutput.result.total > 0) {
                foreach (listsResults.DataItem item in listOutput.result.data) {
                    output.Add(item.name, item.id);
                }
            }

            return output;
        }

        /// <summary>
        /// Batches the unsubscribe.
        /// </summary>
        public void BatchUnsubscribe() {
            if (String.IsNullOrEmpty(_mailChimpSettings.DefaultListId)) throw new ArgumentException("MailChimp list is not specified");

            // Get all the queued records for subscription
            IList<MailChimpEventQueueRecord> recordList = _subscriptionEventQueueingService.ReadList(false);

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
            HandleErrorReport(listSubscribeOutput.api_ErrorMessages);
        }

        /// <summary>
        /// Batches the subscribe.
        /// </summary>
        public void BatchSubscribe() {
            if (String.IsNullOrEmpty(_mailChimpSettings.DefaultListId)) throw new ArgumentException("MailChimp list is not specified");

            // Get all the queued records for subscription
            IList<MailChimpEventQueueRecord> recordList = _subscriptionEventQueueingService.ReadList(true);

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

            foreach (MailChimpEventQueueRecord sub in recordList) {
                var entry = new Dictionary<string, object>();
                entry.Add("EMAIL", sub.Email);
                batch.Add(entry);
            }

            input.parms.batch = batch;

            // execution
            var cmd = new listBatchSubscribe(input);

            listBatchSubscribeOutput listSubscribeOutput = cmd.Execute();
            HandleErrorReport(listSubscribeOutput.api_ErrorMessages);
        }

        /// <summary>
        /// Handles the error report.
        /// </summary>
        /// <param name="apiErrorMessages">The API error messages.</param>
        private void HandleErrorReport(IList<Api_Error> apiErrorMessages) {
            if (apiErrorMessages.Count > 0) {
                var sb = new StringBuilder();
                //output.api_Request, output.api_Response, // raw data
                //output.api_ErrorMessages, output.api_ValidatorMessages); // & errors
                for (int i = 0; i < apiErrorMessages.Count; i++) {
                    sb.Append(apiErrorMessages[i].error);
                    if (i != apiErrorMessages.Count - 1) sb.Append(". ");
                }
            }
        }

        #endregion
    }
}