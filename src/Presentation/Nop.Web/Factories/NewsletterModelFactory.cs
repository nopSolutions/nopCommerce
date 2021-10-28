using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Models.Newsletter;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the newsletter model factory
    /// </summary>
    public partial class NewsletterModelFactory : INewsletterModelFactory
    {
        #region Fields

        protected CustomerSettings CustomerSettings { get; }
        protected ILocalizationService LocalizationService { get; }

        #endregion

        #region Ctor

        public NewsletterModelFactory(CustomerSettings customerSettings,
            ILocalizationService localizationService)
        {
            CustomerSettings = customerSettings;
            LocalizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the newsletter box model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the newsletter box model
        /// </returns>
        public virtual Task<NewsletterBoxModel> PrepareNewsletterBoxModelAsync()
        {
            var model = new NewsletterBoxModel
            {
                AllowToUnsubscribe = CustomerSettings.NewsletterBlockAllowToUnsubscribe
            };

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the subscription activation model
        /// </summary>
        /// <param name="active">Whether the subscription has been activated</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the subscription activation model
        /// </returns>
        public virtual async Task<SubscriptionActivationModel> PrepareSubscriptionActivationModelAsync(bool active)
        {
            var model = new SubscriptionActivationModel
            {
                Result = active
                ? await LocalizationService.GetResourceAsync("Newsletter.ResultActivated")
                : await LocalizationService.GetResourceAsync("Newsletter.ResultDeactivated")
            };

            return model;
        }

        #endregion
    }
}
