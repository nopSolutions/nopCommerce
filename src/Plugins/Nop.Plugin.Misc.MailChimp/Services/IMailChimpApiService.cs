using System.Collections.Specialized;

namespace Nop.Plugin.Misc.MailChimp.Services {
    public interface IMailChimpApiService {
        /// <summary>
        /// Retrieves the lists.
        /// </summary>
        /// <returns></returns>
        NameValueCollection RetrieveLists();

        /// <summary>
        /// Batches the unsubscribe.
        /// </summary>
        void BatchUnsubscribe();

        /// <summary>
        /// Batches the subscribe.
        /// </summary>
        void BatchSubscribe();
    }
}