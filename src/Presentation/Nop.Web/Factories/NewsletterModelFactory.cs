using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Models.Newsletter;

namespace Nop.Web.Factories
{
    public partial class NewsletterModelFactory : INewsletterModelFactory
    {
        private readonly ILocalizationService _localizationService;
        private readonly CustomerSettings _customerSettings;

        public NewsletterModelFactory(ILocalizationService localizationService,
            CustomerSettings customerSettings)
        {
            this._localizationService = localizationService;
            this._customerSettings = customerSettings;
        }

        public virtual NewsletterBoxModel PrepareNewsletterBoxModel()
        {
            var model = new NewsletterBoxModel()
            {
                AllowToUnsubscribe = _customerSettings.NewsletterBlockAllowToUnsubscribe
            };
            return model;
        }

        public virtual SubscriptionActivationModel PrepareSubscriptionActivationModel(bool active)
        {
            var model = new SubscriptionActivationModel();
            model.Result = active
                ? _localizationService.GetResource("Newsletter.ResultActivated")
                : _localizationService.GetResource("Newsletter.ResultDeactivated");

            return model;
        }
    }
}
