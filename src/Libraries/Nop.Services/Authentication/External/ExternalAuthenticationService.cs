using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Events;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// Represents external authentication service implementation
    /// </summary>
    public partial class ExternalAuthenticationService : IExternalAuthenticationService
    {
        #region Fields

        protected readonly CustomerSettings _customerSettings;
        protected readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        protected readonly IActionContextAccessor _actionContextAccessor;
        protected readonly IAuthenticationPluginManager _authenticationPluginManager;
        protected readonly ICustomerRegistrationService _customerRegistrationService;
        protected readonly ICustomerService _customerService;
        protected readonly IEventPublisher _eventPublisher;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILocalizationService _localizationService;
        protected readonly IRepository<ExternalAuthenticationRecord> _externalAuthenticationRecordRepository;
        protected readonly IStoreContext _storeContext;
        protected readonly IUrlHelperFactory _urlHelperFactory;
        protected readonly IWorkContext _workContext;
        protected readonly IWorkflowMessageService _workflowMessageService;
        protected readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public ExternalAuthenticationService(CustomerSettings customerSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IActionContextAccessor actionContextAccessor,
            IAuthenticationPluginManager authenticationPluginManager,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
            IStoreContext storeContext,
            IUrlHelperFactory urlHelperFactory,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings)
        {
            _customerSettings = customerSettings;
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _actionContextAccessor = actionContextAccessor;
            _authenticationPluginManager = authenticationPluginManager;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _externalAuthenticationRecordRepository = externalAuthenticationRecordRepository;
            _storeContext = storeContext;
            _urlHelperFactory = urlHelperFactory;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Authenticate user with existing associated external account
        /// </summary>
        /// <param name="associatedUser">Associated with passed external authentication parameters user</param>
        /// <param name="currentLoggedInUser">Current logged-in user</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        protected virtual async Task<IActionResult> AuthenticateExistingUserAsync(Customer associatedUser, Customer currentLoggedInUser, string returnUrl)
        {
            //log in guest user
            if (currentLoggedInUser == null)
                return await _customerRegistrationService.SignInCustomerAsync(associatedUser, returnUrl);

            //account is already assigned to another user
            if (currentLoggedInUser.Id != associatedUser.Id)
                return await ErrorAuthenticationAsync(new[]
                {
                    await _localizationService.GetResourceAsync("Account.AssociatedExternalAuth.AccountAlreadyAssigned")
                }, returnUrl);

            //or the user try to log in as himself. bit weird
            return SuccessfulAuthentication(returnUrl);
        }

        /// <summary>
        /// Authenticate current user and associate new external account with user
        /// </summary>
        /// <param name="currentLoggedInUser">Current logged-in user</param>
        /// <param name="parameters">Authentication parameters received from external authentication method</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        protected virtual async Task<IActionResult> AuthenticateNewUserAsync(Customer currentLoggedInUser, ExternalAuthenticationParameters parameters, string returnUrl)
        {
            //associate external account with logged-in user
            if (currentLoggedInUser != null)
            {
                await AssociateExternalAccountWithUserAsync(currentLoggedInUser, parameters);

                return SuccessfulAuthentication(returnUrl);
            }

            //or try to register new user
            if (_customerSettings.UserRegistrationType != UserRegistrationType.Disabled)
                return await RegisterNewUserAsync(parameters, returnUrl);

            //registration is disabled
            return await ErrorAuthenticationAsync(new[] { "Registration is disabled" }, returnUrl);
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="parameters">Authentication parameters received from external authentication method</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        protected virtual async Task<IActionResult> RegisterNewUserAsync(ExternalAuthenticationParameters parameters, string returnUrl)
        {
            //check whether the specified email has been already registered
            if (await _customerService.GetCustomerByEmailAsync(parameters.Email) != null)
            {
                var alreadyExistsError = string.Format(await _localizationService.GetResourceAsync("Account.AssociatedExternalAuth.EmailAlreadyExists"),
                    !string.IsNullOrEmpty(parameters.ExternalDisplayIdentifier) ? parameters.ExternalDisplayIdentifier : parameters.ExternalIdentifier);
                return await ErrorAuthenticationAsync(new[] { alreadyExistsError }, returnUrl);
            }

            //registration is approved if validation isn't required
            var registrationIsApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard ||
                (_customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation && !_externalAuthenticationSettings.RequireEmailValidation);

            //create registration request
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var registrationRequest = new CustomerRegistrationRequest(customer,
                parameters.Email, parameters.Email,
                CommonHelper.GenerateRandomDigitCode(20),
                PasswordFormat.Hashed,
                store.Id,
                registrationIsApproved);

            //whether registration request has been completed successfully
            var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
            if (!registrationResult.Success)
                return await ErrorAuthenticationAsync(registrationResult.Errors, returnUrl);

            //allow to save other customer values by consuming this event
            await _eventPublisher.PublishAsync(new CustomerAutoRegisteredByExternalMethodEvent(customer, parameters));

            //raise customer registered event
            await _eventPublisher.PublishAsync(new CustomerRegisteredEvent(customer));

            //store owner notifications
            if (_customerSettings.NotifyNewCustomerRegistration)
                await _workflowMessageService.SendCustomerRegisteredStoreOwnerNotificationMessageAsync(customer, _localizationSettings.DefaultAdminLanguageId);

            //associate external account with registered user
            await AssociateExternalAccountWithUserAsync(customer, parameters);

            //authenticate
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            if (registrationIsApproved)
            {
                await _workflowMessageService.SendCustomerWelcomeMessageAsync(customer, currentLanguage.Id);

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

                return await _customerRegistrationService.SignInCustomerAsync(customer, returnUrl, true);
            }

            //registration is succeeded but isn't activated
            if (_customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
            {
                //email validation message
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                await _workflowMessageService.SendCustomerEmailValidationMessageAsync(customer, currentLanguage.Id);

                return new RedirectToRouteResult("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation, returnUrl });
            }

            //registration is succeeded but isn't approved by admin
            if (_customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval)
                return new RedirectToRouteResult("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval, returnUrl });

            return await ErrorAuthenticationAsync(new[] { "Error on registration" }, returnUrl);
        }

        /// <summary>
        /// Add errors that occurred during authentication
        /// </summary>
        /// <param name="errors">Collection of errors</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        protected virtual async Task<IActionResult> ErrorAuthenticationAsync(IEnumerable<string> errors, string returnUrl)
        {
            var session = _httpContextAccessor.HttpContext?.Session;

            if (session != null)
            {
                var existsErrors = (await session.GetAsync<IList<string>>(NopAuthenticationDefaults.ExternalAuthenticationErrorsSessionKey))?.ToList() ?? new List<string>();

                existsErrors.AddRange(errors);

                await session.SetAsync(NopAuthenticationDefaults.ExternalAuthenticationErrorsSessionKey, existsErrors);
            }

            return new RedirectToActionResult("Login", "Customer", !string.IsNullOrEmpty(returnUrl) ? new { ReturnUrl = returnUrl } : null);
        }

        /// <summary>
        /// Redirect the user after successful authentication
        /// </summary>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        protected virtual IActionResult SuccessfulAuthentication(string returnUrl)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            //redirect to the return URL if it's specified
            if (!string.IsNullOrEmpty(returnUrl) && urlHelper.IsLocalUrl(returnUrl))
                return new RedirectResult(returnUrl);

            return new RedirectToRouteResult("Homepage", null);
        }

        #endregion

        #region Methods

        #region Authentication

        /// <summary>
        /// Authenticate user by passed parameters
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        public virtual async Task<IActionResult> AuthenticateAsync(ExternalAuthenticationParameters parameters, string returnUrl = null)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _authenticationPluginManager.IsPluginActiveAsync(parameters.ProviderSystemName, customer, store.Id))
                return await ErrorAuthenticationAsync(new[] { "External authentication method cannot be loaded" }, returnUrl);

            //get current logged-in user
            var currentLoggedInUser = await _customerService.IsRegisteredAsync(customer) ? customer : null;

            //authenticate associated user if already exists
            var associatedUser = await GetUserByExternalAuthenticationParametersAsync(parameters);
            if (associatedUser != null)
                return await AuthenticateExistingUserAsync(associatedUser, currentLoggedInUser, returnUrl);

            //or associate and authenticate new user
            return await AuthenticateNewUserAsync(currentLoggedInUser, parameters, returnUrl);
        }

        #endregion

        /// <summary>
        /// Get the external authentication records by identifier
        /// </summary>
        /// <param name="externalAuthenticationRecordId">External authentication record identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<ExternalAuthenticationRecord> GetExternalAuthenticationRecordByIdAsync(int externalAuthenticationRecordId)
        {
            return await _externalAuthenticationRecordRepository.GetByIdAsync(externalAuthenticationRecordId, cache => default);
        }

        /// <summary>
        /// Get list of the external authentication records by customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<ExternalAuthenticationRecord>> GetCustomerExternalAuthenticationRecordsAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var associationRecords = _externalAuthenticationRecordRepository.Table.Where(ear => ear.CustomerId == customer.Id);

            return await associationRecords.ToListAsync();
        }

        /// <summary>
        /// Delete the external authentication record
        /// </summary>
        /// <param name="externalAuthenticationRecord">External authentication record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteExternalAuthenticationRecordAsync(ExternalAuthenticationRecord externalAuthenticationRecord)
        {
            if (externalAuthenticationRecord == null)
                throw new ArgumentNullException(nameof(externalAuthenticationRecord));

            await _externalAuthenticationRecordRepository.DeleteAsync(externalAuthenticationRecord, false);
        }

        /// <summary>
        /// Get the external authentication record
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<ExternalAuthenticationRecord> GetExternalAuthenticationRecordByExternalAuthenticationParametersAsync(ExternalAuthenticationParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var associationRecord = await _externalAuthenticationRecordRepository.Table.FirstOrDefaultAsync(record =>
                record.ExternalIdentifier.Equals(parameters.ExternalIdentifier) && record.ProviderSystemName.Equals(parameters.ProviderSystemName));

            return associationRecord;
        }

        /// <summary>
        /// Associate external account with customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="parameters">External authentication parameters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AssociateExternalAccountWithUserAsync(Customer customer, ExternalAuthenticationParameters parameters)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var externalAuthenticationRecord = new ExternalAuthenticationRecord
            {
                CustomerId = customer.Id,
                Email = parameters.Email,
                ExternalIdentifier = parameters.ExternalIdentifier,
                ExternalDisplayIdentifier = parameters.ExternalDisplayIdentifier,
                OAuthAccessToken = parameters.AccessToken,
                ProviderSystemName = parameters.ProviderSystemName
            };

            await _externalAuthenticationRecordRepository.InsertAsync(externalAuthenticationRecord, false);
        }

        /// <summary>
        /// Get the particular user with specified parameters
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer
        /// </returns>
        public virtual async Task<Customer> GetUserByExternalAuthenticationParametersAsync(ExternalAuthenticationParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var associationRecord = _externalAuthenticationRecordRepository.Table.FirstOrDefault(record =>
                record.ExternalIdentifier.Equals(parameters.ExternalIdentifier) && record.ProviderSystemName.Equals(parameters.ProviderSystemName));
            if (associationRecord == null)
                return null;

            return await _customerService.GetCustomerByIdAsync(associationRecord.CustomerId);
        }

        /// <summary>
        /// Remove the association
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task RemoveAssociationAsync(ExternalAuthenticationParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var associationRecord = await _externalAuthenticationRecordRepository.Table.FirstOrDefaultAsync(record =>
                record.ExternalIdentifier.Equals(parameters.ExternalIdentifier) && record.ProviderSystemName.Equals(parameters.ProviderSystemName));

            if (associationRecord != null)
                await _externalAuthenticationRecordRepository.DeleteAsync(associationRecord, false);
        }


        #endregion
    }
}
