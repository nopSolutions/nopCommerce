using System.Collections.Specialized;
using PerceptiveMCAPI.Types;

namespace Nop.Plugin.Misc.MailChimp.Services
{
    public interface IMailChimpApiService
    {
        /// <summary>
        /// Retrieves the lists.
        /// </summary>
        /// <returns></returns>
        NameValueCollection RetrieveLists();

        /// <summary>
        /// Batches the unsubscribe.
        /// </summary>
        listBatchUnsubscribeOutput BatchUnsubscribe();

        /// <summary>
        /// Batches the subscribe.
        /// </summary>
        listBatchSubscribeOutput BatchSubscribe();
    }
}