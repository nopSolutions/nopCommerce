using System.Threading.Tasks;
using Nop.Web.Models.Newsletter;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the newsletter model factory
    /// </summary>
    public partial interface INewsletterModelFactory
    {
        /// <summary>
        /// Prepare the newsletter box model
        /// </summary>
        /// <returns>Newsletter box model</returns>
        Task<NewsletterBoxModel> PrepareNewsletterBoxModelAsync();

        /// <summary>
        /// Prepare the subscription activation model
        /// </summary>
        /// <param name="active">Whether the subscription has been activated</param>
        /// <returns>Subscription activation model</returns>
        Task<SubscriptionActivationModel> PrepareSubscriptionActivationModelAsync(bool active);
    }
}
