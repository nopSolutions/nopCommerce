using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.ExtendedAuth.Service
{
    public partial class ExternalAuthenticationService_Override : ExternalAuthenticationService
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customerSettings">Customer settings</param>
        /// <param name="externalAuthenticationSettings">External authentication settings</param>
        /// <param name="authenticationService">Authentication service</param>
        /// <param name="customerActivityService">Customer activity service</param>
        /// <param name="customerRegistrationService">Customer registration service</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="pluginService">Plugin finder</param>
        /// <param name="externalAuthenticationRecordRepository">External authentication record repository</param>
        /// <param name="shoppingCartService">Shopping cart service</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="workContext">Work context</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="localizationSettings">Localization settings</param>
        public ExternalAuthenticationService_Override(
            CustomerSettings customerSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IAuthenticationPluginManager authenticationPluginManager,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
            IStoreContext storeContext,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings
            ) : base(
            customerSettings,
            externalAuthenticationSettings,
            authenticationPluginManager,
            customerRegistrationService,
            customerService,
            eventPublisher,
            genericAttributeService,
            localizationService,
            externalAuthenticationRecordRepository,
            storeContext,
            workContext,
            workflowMessageService,
            localizationSettings)
        {
            this._customerService = customerService;
            this._workContext = workContext;
            this._authenticationPluginManager = authenticationPluginManager;
        }

        #endregion

        #region Method

        /// <summary>
        /// Authenticate user by passed parameters
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        public override async Task<IActionResult> AuthenticateAsync(ExternalAuthenticationParameters parameters, string returnUrl = null)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (!await _authenticationPluginManager.IsPluginActiveAsync(parameters.ProviderSystemName))
                return ErrorAuthentication(new[] { "External authentication method cannot be loaded" }, returnUrl);

            //get current logged-in user
            var currentLoggedInUser = await _customerService.IsRegisteredAsync(
                await _workContext.GetCurrentCustomerAsync()) ? 
                    await _workContext.GetCurrentCustomerAsync() : 
                    null;

            //authenticate associated user if already exists
            var associatedUser = await GetUserByExternalAuthenticationParametersAsync(parameters);
            if (associatedUser != null)
                return await AuthenticateExistingUserAsync(associatedUser, currentLoggedInUser, returnUrl);

            //user is already exists or not
            var customer = await _customerService.GetCustomerByEmailAsync(parameters.Email);
            if (customer != null)
                return await AuthenticateExistingUserAsync(customer, currentLoggedInUser, returnUrl);

            //or associate and authenticate new user
            if (returnUrl == "/")
                returnUrl += "customer/info";
            else if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "/customer/info";            
            return await AuthenticateNewUserAsync(currentLoggedInUser, parameters, returnUrl);
        }

        #endregion
    }
}
