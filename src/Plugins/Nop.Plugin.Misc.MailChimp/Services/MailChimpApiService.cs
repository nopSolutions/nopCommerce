using System;
using System.Collections.Specialized;
using Nop.Plugin.Misc.MailChimp.Data;
using PerceptiveMCAPI.Methods;
using PerceptiveMCAPI.Types;

namespace Nop.Plugin.Misc.MailChimp.Services {
    public class MailChimpApiService : IMailChimpApiService {
        private readonly MailChimpSettings _mailChimpSettings;

        public MailChimpApiService(MailChimpSettings mailChimpSettings) {
            _mailChimpSettings = mailChimpSettings;
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Batches the subscribe.
        /// </summary>
        public void BatchSubscribe() {
            throw new NotImplementedException();
        }

        #endregion
    }
}