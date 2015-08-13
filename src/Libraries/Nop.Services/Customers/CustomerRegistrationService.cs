using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
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

        private readonly ICustomerService _customerService;
        private readonly IEncryptionService _encryptionService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreService _storeService;
        private readonly IRewardPointService _rewardPointService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly CustomerSettings _customerSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customerService">Customer service</param>
        /// <param name="encryptionService">Encryption service</param>
        /// <param name="newsLetterSubscriptionService">Newsletter subscription service</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="storeService">Store service</param>
        /// <param name="rewardPointService">Reward points service</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="customerSettings">Customer settings</param>
        public CustomerRegistrationService(ICustomerService customerService, 
            IEncryptionService encryptionService, 
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ILocalizationService localizationService,
            IStoreService storeService,
            IRewardPointService rewardPointService,
            RewardPointsSettings rewardPointsSettings,
            CustomerSettings customerSettings)
        {
            this._customerService = customerService;
            this._encryptionService = encryptionService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._localizationService = localizationService;
            this._storeService = storeService;
            this._rewardPointService = rewardPointService;
            this._rewardPointsSettings = rewardPointsSettings;
            this._customerSettings = customerSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validate customer
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        public virtual CustomerLoginResults ValidateCustomer(string usernameOrEmail, string password)
        {
            var customer = _customerSettings.UsernamesEnabled ? 
                _customerService.GetCustomerByUsername(usernameOrEmail) :
                _customerService.GetCustomerByEmail(usernameOrEmail);

            if (customer == null)
                return CustomerLoginResults.CustomerNotExist;
            if (customer.Deleted)
                return CustomerLoginResults.Deleted;
            if (!customer.Active)
                return CustomerLoginResults.NotActive;
            //only registered can login
            if (!customer.IsRegistered())
                return CustomerLoginResults.NotRegistered;

            string pwd;
            switch (customer.PasswordFormat)
            {
                case PasswordFormat.Encrypted:
                    pwd = _encryptionService.EncryptText(password);
                    break;
                case PasswordFormat.Hashed:
                    pwd = _encryptionService.CreatePasswordHash(password, customer.PasswordSalt, _customerSettings.HashedPasswordFormat);
                    break;
                default:
                    pwd = password;
                    break;
            }

            bool isValid = pwd == customer.Password;
            if (!isValid)
                return CustomerLoginResults.WrongPassword;

            //save last login date
            customer.LastLoginDateUtc = DateTime.UtcNow;
            _customerService.UpdateCustomer(customer);
            return CustomerLoginResults.Successful;
        }

        /// <summary>
        /// Register customer
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

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
            if (request.Customer.IsRegistered())
            {
                result.AddError("Current customer is already registered");
                return result;
            }
            if (String.IsNullOrEmpty(request.Email))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.EmailIsNotProvided"));
                return result;
            }
            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError(_localizationService.GetResource("Common.WrongEmail"));
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.PasswordIsNotProvided"));
                return result;
            }
            if (_customerSettings.UsernamesEnabled)
            {
                if (String.IsNullOrEmpty(request.Username))
                {
                    result.AddError(_localizationService.GetResource("Account.Register.Errors.UsernameIsNotProvided"));
                    return result;
                }
            }

            //validate unique user
            if (_customerService.GetCustomerByEmail(request.Email) != null)
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.EmailAlreadyExists"));
                return result;
            }
            if (_customerSettings.UsernamesEnabled)
            {
                if (_customerService.GetCustomerByUsername(request.Username) != null)
                {
                    result.AddError(_localizationService.GetResource("Account.Register.Errors.UsernameAlreadyExists"));
                    return result;
                }
            }

            //at this point request is valid
            request.Customer.Username = request.Username;
            request.Customer.Email = request.Email;
            request.Customer.PasswordFormat = request.PasswordFormat;

            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    {
                        request.Customer.Password = request.Password;
                    }
                    break;
                case PasswordFormat.Encrypted:
                    {
                        request.Customer.Password = _encryptionService.EncryptText(request.Password);
                    }
                    break;
                case PasswordFormat.Hashed:
                    {
                        string saltKey = _encryptionService.CreateSaltKey(5);
                        request.Customer.PasswordSalt = saltKey;
                        request.Customer.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _customerSettings.HashedPasswordFormat);
                    }
                    break;
                default:
                    break;
            }

            request.Customer.Active = request.IsApproved;
            
            //add to 'Registered' role
            var registeredRole = _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered);
            if (registeredRole == null)
                throw new NopException("'Registered' role could not be loaded");
            request.Customer.CustomerRoles.Add(registeredRole);
            //remove from 'Guests' role
            var guestRole = request.Customer.CustomerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Guests);
            if (guestRole != null)
                request.Customer.CustomerRoles.Remove(guestRole);
            
            //Add reward points for customer registration (if enabled)
            if (_rewardPointsSettings.Enabled &&
                _rewardPointsSettings.PointsForRegistration > 0)
            {
                _rewardPointService.AddRewardPointsHistoryEntry(request.Customer, 
                    _rewardPointsSettings.PointsForRegistration,
                    request.StoreId,
                    _localizationService.GetResource("RewardPoints.Message.EarnedForRegistration"));
            }

            _customerService.UpdateCustomer(request.Customer);
            return result;
        }
        
        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual ChangePasswordResult ChangePassword(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new ChangePasswordResult();
            if (String.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.EmailIsNotProvided"));
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.PasswordIsNotProvided"));
                return result;
            }

            var customer = _customerService.GetCustomerByEmail(request.Email);
            if (customer == null)
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.EmailNotFound"));
                return result;
            }


            var requestIsValid = false;
            if (request.ValidateRequest)
            {
                //password
                string oldPwd;
                switch (customer.PasswordFormat)
                {
                    case PasswordFormat.Encrypted:
                        oldPwd = _encryptionService.EncryptText(request.OldPassword);
                        break;
                    case PasswordFormat.Hashed:
                        oldPwd = _encryptionService.CreatePasswordHash(request.OldPassword, customer.PasswordSalt, _customerSettings.HashedPasswordFormat);
                        break;
                    default:
                        oldPwd = request.OldPassword;
                        break;
                }

                bool oldPasswordIsValid = oldPwd == customer.Password;
                if (!oldPasswordIsValid)
                    result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.OldPasswordDoesntMatch"));

                if (oldPasswordIsValid)
                    requestIsValid = true;
            }
            else
                requestIsValid = true;


            //at this point request is valid
            if (requestIsValid)
            {
                switch (request.NewPasswordFormat)
                {
                    case PasswordFormat.Clear:
                        {
                            customer.Password = request.NewPassword;
                        }
                        break;
                    case PasswordFormat.Encrypted:
                        {
                            customer.Password = _encryptionService.EncryptText(request.NewPassword);
                        }
                        break;
                    case PasswordFormat.Hashed:
                        {
                            string saltKey = _encryptionService.CreateSaltKey(5);
                            customer.PasswordSalt = saltKey;
                            customer.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey, _customerSettings.HashedPasswordFormat);
                        }
                        break;
                    default:
                        break;
                }
                customer.PasswordFormat = request.NewPasswordFormat;
                _customerService.UpdateCustomer(customer);
            }

            return result;
        }

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newEmail">New email</param>
        public virtual void SetEmail(Customer customer, string newEmail)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (newEmail == null)
                throw new NopException("Email cannot be null");

            newEmail = newEmail.Trim();
            string oldEmail = customer.Email;

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new NopException(_localizationService.GetResource("Account.EmailUsernameErrors.NewEmailIsNotValid"));

            if (newEmail.Length > 100)
                throw new NopException(_localizationService.GetResource("Account.EmailUsernameErrors.EmailTooLong"));

            var customer2 = _customerService.GetCustomerByEmail(newEmail);
            if (customer2 != null && customer.Id != customer2.Id)
                throw new NopException(_localizationService.GetResource("Account.EmailUsernameErrors.EmailAlreadyExists"));

            customer.Email = newEmail;
            _customerService.UpdateCustomer(customer);

            //update newsletter subscription (if required)
            if (!String.IsNullOrEmpty(oldEmail) && !oldEmail.Equals(newEmail, StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var store in _storeService.GetAllStores())
                {
                    var subscriptionOld = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(oldEmail, store.Id);
                    if (subscriptionOld != null)
                    {
                        subscriptionOld.Email = newEmail;
                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscriptionOld);
                    }
                }
            }
        }

        /// <summary>
        /// Sets a customer username
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newUsername">New Username</param>
        public virtual void SetUsername(Customer customer, string newUsername)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (!_customerSettings.UsernamesEnabled)
                throw new NopException("Usernames are disabled");

            if (!_customerSettings.AllowUsersToChangeUsernames)
                throw new NopException("Changing usernames is not allowed");

            newUsername = newUsername.Trim();

            if (newUsername.Length > 100)
                throw new NopException(_localizationService.GetResource("Account.EmailUsernameErrors.UsernameTooLong"));

            var user2 = _customerService.GetCustomerByUsername(newUsername);
            if (user2 != null && customer.Id != user2.Id)
                throw new NopException(_localizationService.GetResource("Account.EmailUsernameErrors.UsernameAlreadyExists"));

            customer.Username = newUsername;
            _customerService.UpdateCustomer(customer);
        }

        #endregion
    }
}