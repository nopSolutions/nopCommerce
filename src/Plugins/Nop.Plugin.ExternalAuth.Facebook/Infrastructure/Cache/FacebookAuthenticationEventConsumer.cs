using System.Linq;
using System.Security.Claims;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Events;

namespace Nop.Plugin.ExternalAuth.Facebook.Infrastructure.Cache
{
    /// <summary>
    /// Facebook authentication event consumer (used for saving customer fields on registration)
    /// </summary>
    public partial class FacebookAuthenticationEventConsumer : IConsumer<CustomerAutoRegisteredByExternalMethodEvent>
    {
        #region Fields
        
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public FacebookAuthenticationEventConsumer(IGenericAttributeService genericAttributeService)
        {
            this._genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public void HandleEvent(CustomerAutoRegisteredByExternalMethodEvent eventMessage)
        {
            if (eventMessage?.Customer == null || eventMessage.AuthenticationParameters == null)
                return;

            //handle event only for this authentication method
            if (!eventMessage.AuthenticationParameters.ProviderSystemName.Equals(FacebookAuthenticationDefaults.ProviderSystemName))
                return;

            //store some of the customer fields
            var firstName = eventMessage.AuthenticationParameters.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.GivenName)?.Value;
            if (!string.IsNullOrEmpty(firstName))
                _genericAttributeService.SaveAttribute(eventMessage.Customer, NopCustomerDefaults.FirstNameAttribute, firstName);

            var lastName = eventMessage.AuthenticationParameters.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.Surname)?.Value;
            if (!string.IsNullOrEmpty(lastName))
                _genericAttributeService.SaveAttribute(eventMessage.Customer, NopCustomerDefaults.LastNameAttribute, lastName);
        }

        #endregion
    }
}