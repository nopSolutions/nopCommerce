using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Services.Authentication;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer registration service
    /// </summary>
    public partial class CustomerRegistrationService : ICustomerRegistrationService
    {
        #region Fields

        protected CustomerSettings CustomerSettings { get; }
        protected IAuthenticationService AuthenticationService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected IEncryptionService EncryptionService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IMultiFactorAuthenticationPluginManager MultiFactorAuthenticationPluginManager { get; }
        protected INewsLetterSubscriptionService NewsLetterSubscriptionService { get; }
        protected INotificationService NotificationService { get; }
        protected IRewardPointService RewardPointService { get; }
        protected IShoppingCartService ShoppingCartService { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreService StoreService { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected RewardPointsSettings RewardPointsSettings { get; }

        #endregion

        #region Ctor

        public CustomerRegistrationService(CustomerSettings customerSettings,
            IAuthenticationService authenticationService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IEncryptionService encryptionService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            INotificationService notificationService,
            IRewardPointService rewardPointService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            RewardPointsSettings rewardPointsSettings)
        {
            CustomerSettings = customerSettings;
            AuthenticationService = authenticationService;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            EncryptionService = encryptionService;
            EventPublisher = eventPublisher;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            MultiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            NewsLetterSubscriptionService = newsLetterSubscriptionService;
            NotificationService = notificationService;
            RewardPointService = rewardPointService;
            ShoppingCartService = shoppingCartService;
            StoreContext = storeContext;
            StoreService = storeService;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
            RewardPointsSettings = rewardPointsSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether the entered password matches with a saved one
        /// </summary>
        /// <param name="customerPassword">Customer password</param>
        /// <param name="enteredPassword">The entered password</param>
        /// <returns>True if passwords match; otherwise false</returns>
        protected bool PasswordsMatch(CustomerPassword customerPassword, string enteredPassword)
        {
            if (customerPassword == null || string.IsNullOrEmpty(enteredPassword))
                return false;

            var savedPassword = string.Empty;
            switch (customerPassword.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    savedPassword = enteredPassword;
                    break;
                case PasswordFormat.Encrypted:
                    savedPassword = EncryptionService.EncryptText(enteredPassword);
                    break;
                case PasswordFormat.Hashed:
                    savedPassword = EncryptionService.CreatePasswordHash(enteredPassword, customerPassword.PasswordSalt, CustomerSettings.HashedPasswordFormat);
                    break;
            }

            if (customerPassword.Password == null)
                return false;

            return customerPassword.Password.Equals(savedPassword);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validate customer
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<CustomerLoginResults> ValidateCustomerAsync(string usernameOrEmail, string password)
        {
            var customer = CustomerSettings.UsernamesEnabled ?
                await CustomerService.GetCustomerByUsernameAsync(usernameOrEmail) :
                await CustomerService.GetCustomerByEmailAsync(usernameOrEmail);

            if (customer == null)
                return CustomerLoginResults.CustomerNotExist;
            if (customer.Deleted)
                return CustomerLoginResults.Deleted;
            if (!customer.Active)
                return CustomerLoginResults.NotActive;
            //only registered can login
            if (!await CustomerService.IsRegisteredAsync(customer))
                return CustomerLoginResults.NotRegistered;
            //check whether a customer is locked out
            if (customer.CannotLoginUntilDateUtc.HasValue && customer.CannotLoginUntilDateUtc.Value > DateTime.UtcNow)
                return CustomerLoginResults.LockedOut;

            if (!PasswordsMatch(await CustomerService.GetCurrentPasswordAsync(customer.Id), password))
            {
                //wrong password
                customer.FailedLoginAttempts++;
                if (CustomerSettings.FailedPasswordAllowedAttempts > 0 &&
                    customer.FailedLoginAttempts >= CustomerSettings.FailedPasswordAllowedAttempts)
                {
                    //lock out
                    customer.CannotLoginUntilDateUtc = DateTime.UtcNow.AddMinutes(CustomerSettings.FailedPasswordLockoutMinutes);
                    //reset the counter
                    customer.FailedLoginAttempts = 0;
                }

                await CustomerService.UpdateCustomerAsync(customer);

                return CustomerLoginResults.WrongPassword;
            }

            var selectedProvider = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
            var store = await StoreContext.GetCurrentStoreAsync();
            var methodIsActive = await MultiFactorAuthenticationPluginManager.IsPluginActiveAsync(selectedProvider, customer, store.Id);
            if (methodIsActive)
                return CustomerLoginResults.MultiFactorAuthenticationRequired;
            if (!string.IsNullOrEmpty(selectedProvider))
                NotificationService.WarningNotification(await LocalizationService.GetResourceAsync("MultiFactorAuthentication.Notification.SelectedMethodIsNotActive"));

            //update login details
            customer.FailedLoginAttempts = 0;
            customer.CannotLoginUntilDateUtc = null;
            customer.RequireReLogin = false;
            customer.LastLoginDateUtc = DateTime.UtcNow;
            await CustomerService.UpdateCustomerAsync(customer);

            return CustomerLoginResults.Successful;
        }

        /// <summary>
        /// Register customer
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<CustomerRegistrationResult> RegisterCustomerAsync(CustomerRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Customer == null)
                throw new ArgumentException("Can't load current customer");

            var result = new CustomerRegistrationResult();
            if (request.Customer.IsSearchEngineAccount())
            {
                result.AddError("Search engine can't be registered");
                return result;
            }

            if (request.Customer.IsBackgroundTaskAccount())
            {
                result.AddError("Background task account can't be registered");
                return result;
            }

            if (await CustomerService.IsRegisteredAsync(request.Customer))
            {
                result.AddError("Current customer is already registered");
                return result;
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                result.AddError(await LocalizationService.GetResourceAsync("Account.Register.Errors.EmailIsNotProvided"));
                return result;
            }

            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError(await LocalizationService.GetResourceAsync("Common.WrongEmail"));
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError(await LocalizationService.GetResourceAsync("Account.Register.Errors.PasswordIsNotProvided"));
                return result;
            }

            if (CustomerSettings.UsernamesEnabled && string.IsNullOrEmpty(request.Username))
            {
                result.AddError(await LocalizationService.GetResourceAsync("Account.Register.Errors.UsernameIsNotProvided"));
                return result;
            }

            //validate unique user
            if (await CustomerService.GetCustomerByEmailAsync(request.Email) != null)
            {
                result.AddError(await LocalizationService.GetResourceAsync("Account.Register.Errors.EmailAlreadyExists"));
                return result;
            }

            if (CustomerSettings.UsernamesEnabled && await CustomerService.GetCustomerByUsernameAsync(request.Username) != null)
            {
                result.AddError(await LocalizationService.GetResourceAsync("Account.Register.Errors.UsernameAlreadyExists"));
                return result;
            }

            //at this point request is valid
            request.Customer.Username = request.Username;
            request.Customer.Email = request.Email;

            var customerPassword = new CustomerPassword
            {
                CustomerId = request.Customer.Id,
                PasswordFormat = request.PasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };
            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    customerPassword.Password = request.Password;
                    break;
                case PasswordFormat.Encrypted:
                    customerPassword.Password = EncryptionService.EncryptText(request.Password);
                    break;
                case PasswordFormat.Hashed:
                    var saltKey = EncryptionService.CreateSaltKey(NopCustomerServicesDefaults.PasswordSaltKeySize);
                    customerPassword.PasswordSalt = saltKey;
                    customerPassword.Password = EncryptionService.CreatePasswordHash(request.Password, saltKey, CustomerSettings.HashedPasswordFormat);
                    break;
            }

            await CustomerService.InsertCustomerPasswordAsync(customerPassword);

            request.Customer.Active = request.IsApproved;

            //add to 'Registered' role
            var registeredRole = await CustomerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
            if (registeredRole == null)
                throw new NopException("'Registered' role could not be loaded");

            await CustomerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = request.Customer.Id, CustomerRoleId = registeredRole.Id });

            //remove from 'Guests' role            
            if (await CustomerService.IsGuestAsync(request.Customer))
            {
                var guestRole = await CustomerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName);
                await CustomerService.RemoveCustomerRoleMappingAsync(request.Customer, guestRole);
            }

            //add reward points for customer registration (if enabled)
            if (RewardPointsSettings.Enabled && RewardPointsSettings.PointsForRegistration > 0)
            {
                var endDate = RewardPointsSettings.RegistrationPointsValidity > 0
                    ? (DateTime?)DateTime.UtcNow.AddDays(RewardPointsSettings.RegistrationPointsValidity.Value) : null;
                await RewardPointService.AddRewardPointsHistoryEntryAsync(request.Customer, RewardPointsSettings.PointsForRegistration,
                    request.StoreId, await LocalizationService.GetResourceAsync("RewardPoints.Message.EarnedForRegistration"), endDate: endDate);
            }

            await CustomerService.UpdateCustomerAsync(request.Customer);

            return result;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var result = new ChangePasswordResult();
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError(await LocalizationService.GetResourceAsync("Account.ChangePassword.Errors.EmailIsNotProvided"));
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError(await LocalizationService.GetResourceAsync("Account.ChangePassword.Errors.PasswordIsNotProvided"));
                return result;
            }

            var customer = await CustomerService.GetCustomerByEmailAsync(request.Email);
            if (customer == null)
            {
                result.AddError(await LocalizationService.GetResourceAsync("Account.ChangePassword.Errors.EmailNotFound"));
                return result;
            }

            //request isn't valid
            if (request.ValidateRequest && !PasswordsMatch(await CustomerService.GetCurrentPasswordAsync(customer.Id), request.OldPassword))
            {
                result.AddError(await LocalizationService.GetResourceAsync("Account.ChangePassword.Errors.OldPasswordDoesntMatch"));
                return result;
            }

            //check for duplicates
            if (CustomerSettings.UnduplicatedPasswordsNumber > 0)
            {
                //get some of previous passwords
                var previousPasswords = await CustomerService.GetCustomerPasswordsAsync(customer.Id, passwordsToReturn: CustomerSettings.UnduplicatedPasswordsNumber);

                var newPasswordMatchesWithPrevious = previousPasswords.Any(password => PasswordsMatch(password, request.NewPassword));
                if (newPasswordMatchesWithPrevious)
                {
                    result.AddError(await LocalizationService.GetResourceAsync("Account.ChangePassword.Errors.PasswordMatchesWithPrevious"));
                    return result;
                }
            }

            //at this point request is valid
            var customerPassword = new CustomerPassword
            {
                CustomerId = customer.Id,
                PasswordFormat = request.NewPasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };
            switch (request.NewPasswordFormat)
            {
                case PasswordFormat.Clear:
                    customerPassword.Password = request.NewPassword;
                    break;
                case PasswordFormat.Encrypted:
                    customerPassword.Password = EncryptionService.EncryptText(request.NewPassword);
                    break;
                case PasswordFormat.Hashed:
                    var saltKey = EncryptionService.CreateSaltKey(NopCustomerServicesDefaults.PasswordSaltKeySize);
                    customerPassword.PasswordSalt = saltKey;
                    customerPassword.Password = EncryptionService.CreatePasswordHash(request.NewPassword, saltKey,
                        request.HashedPasswordFormat ?? CustomerSettings.HashedPasswordFormat);
                    break;
            }

            await CustomerService.InsertCustomerPasswordAsync(customerPassword);

            //publish event
            await EventPublisher.PublishAsync(new CustomerPasswordChangedEvent(customerPassword));

            return result;
        }

        /// <summary>
        /// Login passed user
        /// </summary>
        /// <param name="customer">User to login</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <param name="isPersist">Is remember me</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of an authentication
        /// </returns>
        public virtual async Task<IActionResult> SignInCustomerAsync(Customer customer, string returnUrl, bool isPersist = false)
        {
            var currentCustomer = await WorkContext.GetCurrentCustomerAsync();
            if (currentCustomer?.Id != customer.Id)
            {
                //migrate shopping cart
                await ShoppingCartService.MigrateShoppingCartAsync(currentCustomer, customer, true);

                await WorkContext.SetCurrentCustomerAsync(customer);
            }

            //sign in new customer
            await AuthenticationService.SignInAsync(customer, isPersist);

            //raise event       
            await EventPublisher.PublishAsync(new CustomerLoggedinEvent(customer));

            //activity log
            await CustomerActivityService.InsertActivityAsync(customer, "PublicStore.Login",
                await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.Login"), customer);

            //redirect to the return URL if it's specified
            if (!string.IsNullOrEmpty(returnUrl))
                return new RedirectResult(returnUrl);

            return new RedirectToRouteResult("Homepage", null);
        }

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newEmail">New email</param>
        /// <param name="requireValidation">Require validation of new email address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetEmailAsync(Customer customer, string newEmail, bool requireValidation)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (newEmail == null)
                throw new NopException("Email cannot be null");

            newEmail = newEmail.Trim();
            var oldEmail = customer.Email;

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new NopException(await LocalizationService.GetResourceAsync("Account.EmailUsernameErrors.NewEmailIsNotValid"));

            if (newEmail.Length > 100)
                throw new NopException(await LocalizationService.GetResourceAsync("Account.EmailUsernameErrors.EmailTooLong"));

            var customer2 = await CustomerService.GetCustomerByEmailAsync(newEmail);
            if (customer2 != null && customer.Id != customer2.Id)
                throw new NopException(await LocalizationService.GetResourceAsync("Account.EmailUsernameErrors.EmailAlreadyExists"));

            if (requireValidation)
            {
                //re-validate email
                customer.EmailToRevalidate = newEmail;
                await CustomerService.UpdateCustomerAsync(customer);

                //email re-validation message
                await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute, Guid.NewGuid().ToString());
                await WorkflowMessageService.SendCustomerEmailRevalidationMessageAsync(customer, (await WorkContext.GetWorkingLanguageAsync()).Id);
            }
            else
            {
                customer.Email = newEmail;
                await CustomerService.UpdateCustomerAsync(customer);

                if (string.IsNullOrEmpty(oldEmail) || oldEmail.Equals(newEmail, StringComparison.InvariantCultureIgnoreCase))
                    return;

                //update newsletter subscription (if required)
                foreach (var store in await StoreService.GetAllStoresAsync())
                {
                    var subscriptionOld = await NewsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(oldEmail, store.Id);

                    if (subscriptionOld == null)
                        continue;

                    subscriptionOld.Email = newEmail;
                    await NewsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscriptionOld);
                }
            }
        }

        /// <summary>
        /// Sets a customer username
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newUsername">New Username</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetUsernameAsync(Customer customer, string newUsername)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (!CustomerSettings.UsernamesEnabled)
                throw new NopException("Usernames are disabled");

            newUsername = newUsername.Trim();

            if (newUsername.Length > NopCustomerServicesDefaults.CustomerUsernameLength)
                throw new NopException(await LocalizationService.GetResourceAsync("Account.EmailUsernameErrors.UsernameTooLong"));

            var user2 = await CustomerService.GetCustomerByUsernameAsync(newUsername);
            if (user2 != null && customer.Id != user2.Id)
                throw new NopException(await LocalizationService.GetResourceAsync("Account.EmailUsernameErrors.UsernameAlreadyExists"));

            customer.Username = newUsername;
            await CustomerService.UpdateCustomerAsync(customer);
        }

        #endregion
    }
}