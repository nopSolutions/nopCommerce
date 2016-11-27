using Nop.Web.Models.Newsletter;

namespace Nop.Web.Factories
{
    public partial interface INewsletterModelFactory
    {
        NewsletterBoxModel PrepareNewsletterBoxModel();

        SubscriptionActivationModel PrepareSubscriptionActivationModel(bool active);
    }
}
