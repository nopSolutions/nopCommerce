using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Square;
using Square.Exceptions;
using Square.Models;
using SquareSdk = Square;

namespace Nop.Plugin.Payments.Square.Services
{
    /// <summary>
    /// Represents the Square payment manager
    /// </summary>
    public class SquarePaymentManager
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly SquareAuthorizationHttpClient _squareAuthorizationHttpClient;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public SquarePaymentManager(ILogger logger,
            IWorkContext workContext,
            SquareAuthorizationHttpClient squareAuthorizationHttpClient,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _logger = logger;
            _workContext = workContext;
            _squareAuthorizationHttpClient = squareAuthorizationHttpClient;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Creates the Square Client
        /// </summary>
        /// <param name="storeId">Store identifier for which configuration should be loaded</param>
        /// <returns>The Square Client</returns>
        private ISquareClient CreateSquareClient(int storeId)
        {
            var settings = _settingService.LoadSetting<SquarePaymentSettings>(storeId);

            //validate access token
            if (settings.UseSandbox && string.IsNullOrEmpty(settings.AccessToken))
                throw new NopException("Sandbox access token should not be empty");

            var client = new SquareClient.Builder()
                .AccessToken(settings.AccessToken)
                .AddAdditionalHeader("user-agent", SquarePaymentDefaults.UserAgent);
            
            if (settings.UseSandbox)
                client.Environment(SquareSdk.Environment.Sandbox);
            else
                client.Environment(SquareSdk.Environment.Production);

            return client.Build();
        }

        private void ThrowErrorsIfExists(IList<Error> errors)
        {
            //check whether there are errors in the service response
            if (errors?.Any() ?? false)
            {
                var errorsMessage = string.Join(";", errors.Select(error => error.Detail));
                throw new NopException($"There are errors in the service response. {errorsMessage}");
            }
        }

        private string CatchException(Exception exception)
        {
            //log full error
            var errorMessage = exception.Message;
            _logger.Error($"Square payment error: {errorMessage}.", exception, _workContext.CurrentCustomer);

            // check Square exception
            if (exception is ApiException apiException)
            {
                //try to get error details
                if (apiException?.Errors?.Any() ?? false)
                    errorMessage = string.Join(";", apiException.Errors.Select(error => error.Detail));
            }

            return errorMessage;
        }

        #endregion

        #region Methods

        #region Common

        /// <summary>
        /// Get selected active business locations
        /// </summary>
        /// <param name="storeId">Store identifier for which locations should be loaded</param>
        /// <returns>Location</returns>
        public Location GetSelectedActiveLocation(int storeId)
        {
            var client = CreateSquareClient(storeId);
            var settings = _settingService.LoadSetting<SquarePaymentSettings>(storeId);

            if (string.IsNullOrEmpty(settings.LocationId))
                return null;

            try
            {
                var locationResponse = client.LocationsApi.RetrieveLocation(settings.LocationId);
                if (locationResponse == null)
                    throw new NopException("No service response");

                ThrowErrorsIfExists(locationResponse.Errors);

                var location = locationResponse.Location;
                if (location == null
                      || location.Status != SquarePaymentDefaults.LOCATION_STATUS_ACTIVE
                         || (!location.Capabilities?.Contains(SquarePaymentDefaults.LOCATION_CAPABILITIES_PROCESSING) ?? true))
                {
                    throw new NopException("There are no selected active location for the account");
                }

                return location;
            }
            catch (Exception exception)
            {
                CatchException(exception);

                return null;
            }
        }

        /// <summary>
        /// Gets active business locations
        /// </summary>
        /// <param name="storeId">Store identifier for which locations should be loaded</param>
        /// <returns>List of location</returns>
        public IList<Location> GetActiveLocations(int storeId)
        {
            var client = CreateSquareClient(storeId);

            try
            {
                var listLocationsResponse = client.LocationsApi.ListLocations();
                if (listLocationsResponse == null)
                    throw new NopException("No service response");

                ThrowErrorsIfExists(listLocationsResponse.Errors);

                //filter active locations and locations that can process credit cards
                var activeLocations = listLocationsResponse.Locations?.Where(location => location?.Status == SquarePaymentDefaults.LOCATION_STATUS_ACTIVE
                    && (location.Capabilities?.Contains(SquarePaymentDefaults.LOCATION_CAPABILITIES_PROCESSING) ?? false)).ToList();
                if (!activeLocations?.Any() ?? true)
                    throw new NopException("There are no active locations for the account");

                return activeLocations;
            }
            catch (Exception exception)
            {
                CatchException(exception);

                return new List<Location>();
            }
        }

        /// <summary>
        /// Get customer by identifier
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="storeId">Store identifier for which customer should be loaded</param>
        /// <returns>Customer</returns>
        public Customer GetCustomer(string customerId, int storeId)
        {
            //whether passed customer identifier exists
            if (string.IsNullOrEmpty(customerId))
                return null;

            var client = CreateSquareClient(storeId);

            try
            {
                //get customer by identifier
                var retrieveCustomerResponse = client.CustomersApi.RetrieveCustomer(customerId);
                if (retrieveCustomerResponse == null)
                    throw new NopException("No service response");

                ThrowErrorsIfExists(retrieveCustomerResponse.Errors);

                return retrieveCustomerResponse.Customer;
            }
            catch (Exception exception)
            {
                CatchException(exception);

                return null;
            }
        }

        /// <summary>
        /// Create the new customer
        /// </summary>
        /// <param name="customerRequest">Request parameters to create customer</param>
        /// <param name="storeId">Store identifier for which customer should be created</param>
        /// <returns>Customer</returns>
        public Customer CreateCustomer(CreateCustomerRequest customerRequest, int storeId)
        {
            if (customerRequest == null)
                throw new ArgumentNullException(nameof(customerRequest));

            var client = CreateSquareClient(storeId);

            try
            {
                //create the new customer
                var createCustomerResponse = client.CustomersApi.CreateCustomer(customerRequest);
                if (createCustomerResponse == null)
                    throw new NopException("No service response");

                ThrowErrorsIfExists(createCustomerResponse.Errors);

                return createCustomerResponse.Customer;
            }
            catch (Exception exception)
            {
                CatchException(exception);

                return null;
            }
        }

        /// <summary>
        /// Create the new card of the customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="cardRequest">Request parameters to create card of the customer</param>
        /// <param name="storeId">Store identifier for which customer card should be created</param>
        /// <returns>Card</returns>
        public Card CreateCustomerCard(string customerId, CreateCustomerCardRequest cardRequest, int storeId)
        {
            if (cardRequest == null)
                throw new ArgumentNullException(nameof(cardRequest));

            //whether passed customer identifier exists
            if (string.IsNullOrEmpty(customerId))
                return null;

            var client = CreateSquareClient(storeId);

            try
            {
                //create the new card of the customer
                var createCustomerCardResponse = client.CustomersApi.CreateCustomerCard(customerId, cardRequest);
                if (createCustomerCardResponse == null)
                    throw new NopException("No service response");

                ThrowErrorsIfExists(createCustomerCardResponse.Errors);

                return createCustomerCardResponse.Card;
            }
            catch (Exception exception)
            {
                CatchException(exception);

                return null;
            }
        }

        #endregion

        #region Payment workflow

        /// <summary>
        /// Creates a payment
        /// </summary>
        /// <param name="paymentRequest">Request parameters to create payment</param>
        /// <param name="storeId">Store identifier for which payment should be created</param>
        /// <returns>Payment and/or errors if exist</returns>
        public (Payment, string) CreatePayment(CreatePaymentRequest paymentRequest, int storeId)
        {
            if (paymentRequest == null)
                throw new ArgumentNullException(nameof(paymentRequest));

            var client = CreateSquareClient(storeId);

            try
            {
                var paymentResponse = client.PaymentsApi.CreatePayment(paymentRequest);
                if (paymentResponse == null)
                    throw new NopException("No service response");

                ThrowErrorsIfExists(paymentResponse.Errors);

                return (paymentResponse.Payment, null);
            }
            catch (Exception exception)
            {
                return (null, CatchException(exception));
            }
        }

        /// <summary>
        /// Completes a payment
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <param name="storeId">Store identifier for which payment should be completed</param>
        /// <returns>True if the payment successfully completed; otherwise false. And/or errors if exist</returns>
        public (bool, string) CompletePayment(string paymentId, int storeId)
        {
            if (string.IsNullOrEmpty(paymentId))
                return (false, null);

            var client = CreateSquareClient(storeId);

            try
            {
                var paymentResponse = client.PaymentsApi.CompletePayment(paymentId, null);
                if (paymentResponse == null)
                    throw new NopException("No service response");

                ThrowErrorsIfExists(paymentResponse.Errors);

                //if there are no errors in the response, payment was successfully completed
                return (true, null);
            }
            catch (Exception exception)
            {
                return (false, CatchException(exception));
            }
        }

        /// <summary>
        /// Cancels a payment
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <param name="storeId">Store identifier for which payment should be canceled</param>
        /// <returns>True if the payment successfully canceled; otherwise false. And/or errors if exist</returns>
        public (bool, string) CancelPayment(string paymentId, int storeId)
        {
            if (string.IsNullOrEmpty(paymentId))
                return (false, null);

            var client = CreateSquareClient(storeId);

            try
            {
                var paymentResponse = client.PaymentsApi.CancelPayment(paymentId);
                if (paymentResponse == null)
                    throw new NopException("No service response");

                ThrowErrorsIfExists(paymentResponse.Errors);

                //if there are no errors in the response, payment was successfully canceled
                return (true, null);
            }
            catch (Exception exception)
            {
                return (false, CatchException(exception));
            }
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request parameters to create refund</param>
        /// <param name="storeId">Store identifier for which payment should be refunded</param>
        /// <returns>Payment refund and/or errors if exist</returns>
        public (PaymentRefund, string) RefundPayment(RefundPaymentRequest refundPaymentRequest, int storeId)
        {
            if (refundPaymentRequest == null)
                throw new ArgumentNullException(nameof(refundPaymentRequest));

            var client = CreateSquareClient(storeId);

            try
            {
                var refundPaymentResponse = client.RefundsApi.RefundPayment(refundPaymentRequest);
                if (refundPaymentResponse == null)
                    throw new NopException("No service response");

                ThrowErrorsIfExists(refundPaymentResponse.Errors);

                return (refundPaymentResponse.Refund, null);
            }
            catch (Exception exception)
            {
                return (null, CatchException(exception));
            }
        }
        #endregion

        #region OAuth2 authorization

        /// <summary>
        /// Generate URL for the authorization permissions page
        /// </summary>
        /// <param name="storeId">Store identifier for which authorization url should be created</param>
        /// <returns>URL</returns>
        public string GenerateAuthorizeUrl(int storeId)
        {
            var serviceUrl = $"{_squareAuthorizationHttpClient.BaseAddress}authorize";

            //list of all available permission scopes
            var permissionScopes = new List<string>
            {
                //GET endpoints related to a merchant's business and location entities.
                "MERCHANT_PROFILE_READ",

                //GET endpoints related to transactions and refunds.
                "PAYMENTS_READ",

                //POST, PUT, and DELETE endpoints related to transactions and refunds
                "PAYMENTS_WRITE",

                //GET endpoints related to customer management.
                "CUSTOMERS_READ",

                //POST, PUT, and DELETE endpoints related to customer management.
                "CUSTOMERS_WRITE",

                //GET endpoints related to settlements (deposits).
                "SETTLEMENTS_READ",

                //GET endpoints related to a merchant's bank accounts.
                "BANK_ACCOUNTS_READ",

                //GET endpoints related to a merchant's item library.
                "ITEMS_READ",

                //POST, PUT, and DELETE endpoints related to a merchant's item library.
                "ITEMS_WRITE",

                //GET endpoints related to a merchant's orders.
                "ORDERS_READ",

                //POST, PUT, and DELETE endpoints related to a merchant's orders.
                "ORDERS_WRITE",

                //GET endpoints related to employee management.
                "EMPLOYEES_READ",

                //POST, PUT, and DELETE endpoints related to employee management.
                "EMPLOYEES_WRITE",

                //GET endpoints related to employee timecards.
                "TIMECARDS_READ",

                //POST, PUT, and DELETE endpoints related to employee timecards.
                "TIMECARDS_WRITE"
            };

            //request all of the permissions
            var requestingPermissions = string.Join(" ", permissionScopes);

            var settings = _settingService.LoadSetting<SquarePaymentSettings>(storeId);

            //create query parameters for the request
            var queryParameters = new Dictionary<string, string>
            {
                //The application ID.
                ["client_id"] = settings.ApplicationId,

                //Indicates whether you want to receive an authorization code ("code") or an access token ("token").
                ["response_type"] = "code",

                //A space-separated list of the permissions your application is requesting. 
                ["scope"] = requestingPermissions,

                //The locale to present the Permission Request form in. Currently supported values are en-US, en-CA, es-US, fr-CA, and ja-JP.
                //["locale"] = string.Empty,

                //If "false", the Square merchant must log in to view the Permission Request form, even if they already have a valid user session.
                ["session"] = "false",

                //Include this parameter and verify its value to help protect against cross-site request forgery.
                ["state"] = settings.AccessTokenVerificationString,

                //The ID of the subscription plan to direct the merchant to sign up for, if any.
                //You can provide this parameter with no value to give a merchant the option to cancel an active subscription.
                //["plan_id"] = string.Empty,
            };

            //return generated URL
            return QueryHelpers.AddQueryString(serviceUrl, queryParameters);
        }

        /// <summary>
        /// Exchange the authorization code for an access token
        /// </summary>
        /// <param name="authorizationCode">Authorization code</param>
        /// <param name="storeId">Store identifier for which access token should be obtained</param>
        /// <returns>Access and refresh tokens</returns>
        public (string AccessToken, string RefreshToken) ObtainAccessToken(string authorizationCode, int storeId)
        {
            return _squareAuthorizationHttpClient.ObtainAccessTokenAsync(authorizationCode, storeId).Result;
        }

        /// <summary>
        /// Renew the expired access token
        /// </summary>
        /// <param name="storeId">Store identifier for which access token should be updated</param>
        /// <returns>Access and refresh tokens</returns>
        public (string AccessToken, string RefreshToken) RenewAccessToken(int storeId)
        {
            return _squareAuthorizationHttpClient.RenewAccessTokenAsync(storeId).Result;
        }

        /// <summary>
        /// Revoke all access tokens
        /// </summary>
        /// <param name="storeId">Store identifier for which access token should be revoked</param>
        /// <returns>True if tokens were successfully revoked; otherwise false</returns>
        public bool RevokeAccessTokens(int storeId)
        {
            return _squareAuthorizationHttpClient.RevokeAccessTokensAsync(storeId).Result;
        }

        #endregion

        #endregion
    }
}