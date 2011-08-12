//Contributor:  Nicholas Mayne

using System;
using System.Collections.Generic;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;

namespace Nop.Services.Authentication.External
{
    public partial class ExternalAuthorizer : IExternalAuthorizer
    {
        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly CustomerSettings _customerSettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        
        #endregion

        #region Ctor

        public ExternalAuthorizer(IAuthenticationService authenticationService,
            IOpenAuthenticationService openAuthenticationService,
            ICustomerService customerService,
            IWorkContext workContext,
            CustomerSettings customerSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings)
        {
            this._authenticationService = authenticationService;
            this._openAuthenticationService = openAuthenticationService;
            this._customerService = customerService;
            this._workContext = workContext;
            this._customerSettings = customerSettings;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
        }
        
        #endregion

        #region Utilities

        private bool RegistrationIsEnabled()
        {
            return _customerSettings.UserRegistrationType != UserRegistrationType.Disabled && !_externalAuthenticationSettings.AutoRegisterEnabled;
        }

        private bool AutoRegistrationIsEnabled()
        {
            return _customerSettings.UserRegistrationType != UserRegistrationType.Disabled && _externalAuthenticationSettings.AutoRegisterEnabled;
        }

        private bool AccountDoesNotExistAndUserIsNotLoggedOn(Customer userFound, Customer userLoggedIn)
        {
            return userFound == null && userLoggedIn == null;
        }

        private bool AccountIsAssignedToLoggedOnAccount(Customer userFound, Customer userLoggedIn)
        {
            return userFound.Id.Equals(userLoggedIn.Id);
        }

        private bool AccountAlreadyExists(Customer userFound, Customer userLoggedIn)
        {
            return userFound != null && userLoggedIn != null;
        }

        private void StoreParametersForRoundTrip(OpenAuthenticationParameters parameters)
        {
            HttpContext.Current.Session["nop.externalauth.parameters"] = parameters;
        }

        #endregion

        #region Methods

        public virtual AuthorizationResult Authorize(OpenAuthenticationParameters parameters)
        {
            var userFound = _openAuthenticationService.GetUser(parameters);

            var userLoggedIn = _workContext.CurrentCustomer.IsRegistered() ? _workContext.CurrentCustomer : null;

            if (AccountAlreadyExists(userFound, userLoggedIn))
            {
                if (AccountIsAssignedToLoggedOnAccount(userFound, userLoggedIn))
                {
                    // The person is trying to log in as himself.. bit weird
                    return new AuthorizationResult(OpenAuthenticationStatus.Authenticated);
                }

                var result = new AuthorizationResult(OpenAuthenticationStatus.Error);
                result.AddError("Account is already assigned");
                return result;
            }
            if (AccountDoesNotExistAndUserIsNotLoggedOn(userFound, userLoggedIn))
            {
                StoreParametersForRoundTrip(parameters);

                if (AutoRegistrationIsEnabled())
                {
                    //register user
                    var details = new RegistrationDetails(parameters);
                    var randomPassword = CommonHelper.GenerateRandomDigitCode(20);

                    var currentCustomer = _workContext.CurrentCustomer;

                    bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                    var registrationRequest = new CustomerRegistrationRequest(currentCustomer, details.EmailAddress,
                        _customerSettings.UsernamesEnabled ? details.UserName : details.EmailAddress, randomPassword, PasswordFormat.Clear, isApproved);
                    var registrationResult = _customerService.RegisterCustomer(registrationRequest);
                    if (registrationResult.Success)
                    {
                        userFound = currentCustomer;
                        _openAuthenticationService.AssociateExternalAccountWithUser(currentCustomer, parameters);
                        ExternalAuthorizerHelper.RemoveParameters();
                    }
                    else
                    {
                        ExternalAuthorizerHelper.RemoveParameters();

                        var result = new AuthorizationResult(OpenAuthenticationStatus.Error);
                        foreach (var error in registrationResult.Errors)
                            result.AddError(string.Format(error));
                        return result;
                    }
                }
                else if (RegistrationIsEnabled())
                {
                    return new AuthorizationResult(OpenAuthenticationStatus.AssociateOnLogon);
                }
                else
                {
                    ExternalAuthorizerHelper.RemoveParameters();

                    var result = new AuthorizationResult(OpenAuthenticationStatus.Error);
                    result.AddError("User does not exist on system");
                    return result;
                }
            }
            if (userFound == null)
            {
                _openAuthenticationService.AssociateExternalAccountWithUser(userLoggedIn, parameters);
            }

            _authenticationService.SignIn(userFound ?? userLoggedIn, false);

            return new AuthorizationResult(OpenAuthenticationStatus.Authenticated);
        }

        #endregion
    }
}