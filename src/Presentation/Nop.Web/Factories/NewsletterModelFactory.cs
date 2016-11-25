using System;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Models.Newsletter;

namespace Nop.Web.Factories
{
    public partial class NewsletterModelFactory : INewsletterModelFactory
    {
        private readonly CustomerSettings _customerSettings;

        public NewsletterModelFactory(CustomerSettings customerSettings)
        {
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
    }
}
