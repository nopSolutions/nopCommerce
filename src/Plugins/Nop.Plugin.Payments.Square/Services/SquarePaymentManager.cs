using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.Square.Domain;
using Nop.Services.Logging;
using Square.Connect.Api;
using Square.Connect.Client;
using Square.Connect.Model;

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
        private readonly SquarePaymentSettings _squarePaymentSettings;

        #endregion

        #region Ctor

        public SquarePaymentManager(ILogger logger,
            IWorkContext workContext,
            SquarePaymentSettings squarePaymentSettings)
        {
            this._logger = logger;
            this._workContext = workContext;
            this._squarePaymentSettings = squarePaymentSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get the OAuth Square service base URL
        /// </summary>
        /// <returns>URL</returns>
        private string GetOAuthServiceUrl()
        {
            return "https://connect.squareup.com/oauth2";
        }

        /// <summary>
        /// Create the API configuration
        /// </summary>
        /// <returns>The API Configuration</returns>
        private Configuration CreateApiConfiguration()
        {
            //validate access token
            if (_squarePaymentSettings.UseSandbox && 
                (string.IsNullOrEmpty(_squarePaymentSettings.AccessToken) || 
                    !_squarePaymentSettings.AccessToken.StartsWith(SquarePaymentDefaults.SandboxCredentialsPrefix, StringComparison.InvariantCultureIgnoreCase)))

            {
                throw new NopException($"Sandbox access token should start with '{SquarePaymentDefaults.SandboxCredentialsPrefix}'");
            }

            return new Configuration
            {
                AccessToken = _squarePaymentSettings.AccessToken,
                UserAgent = SquarePaymentDefaults.UserAgent
            };
        }

        #endregion

        #region Methods

        #region Common

        /// <summary>
        /// Get active business locations
        /// </summary>
        /// <returns>List of location</returns>
        public IList<Location> GetActiveLocations()
        {
            try
            {
                //create location API
                var configuration = CreateApiConfiguration();
                var locationsApi = new LocationsApi(configuration);

                //get list of all locations
                var listLocationsResponse = locationsApi.ListLocations();
                if (listLocationsResponse == null)
                    throw new NopException("No service response");

                //check whether there are errors in the service response
                if (listLocationsResponse.Errors?.Any() ?? false)
                {
                    var errorsMessage = string.Join(";", listLocationsResponse.Errors.Select(error => error.ToString()));
                    throw new NopException($"There are errors in the service response. {errorsMessage}");
                }

                //filter active locations and locations that can process credit cards
                var activeLocations = listLocationsResponse.Locations?.Where(location => location?.Status == Location.StatusEnum.ACTIVE
                    && (location.Capabilities?.Contains(Location.CapabilitiesEnum.PROCESSING) ?? false)).ToList();
                if (!activeLocations?.Any() ?? true)
                    throw new NopException("There are no active locations for the account");

                return activeLocations;
            }
            catch (Exception exception)
            {
                //log full error
                _logger.Error($"Square payment error: {exception.Message}.", exception, _workContext.CurrentCustomer);

                return new List<Location>();
            }
        }

        /// <summary>
        /// Get customer by identifier
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Customer</returns>
        public Customer GetCustomer(string customerId)
        {
            try
            {
                //whether passed customer identifier exists
                if (string.IsNullOrEmpty(customerId))
                    return null;

                //create customer API
                var configuration = CreateApiConfiguration();
                var customersApi = new CustomersApi(configuration);

                //get customer by identifier
                var retrieveCustomerResponse = customersApi.RetrieveCustomer(customerId);
                if (retrieveCustomerResponse == null)
                    throw new NopException("No service response");

                //check whether there are errors in the service response
                if (retrieveCustomerResponse.Errors?.Any() ?? false)
                {
                    var errorsMessage = string.Join(";", retrieveCustomerResponse.Errors.Select(error => error.ToString()));
                    throw new NopException($"There are errors in the service response. {errorsMessage}");
                }

                return retrieveCustomerResponse.Customer;
            }
            catch (Exception exception)
            {
                //log full error
                _logger.Error($"Square payment error: {exception.Message}.", exception, _workContext.CurrentCustomer);

                return null;
            }
        }

        /// <summary>
        /// Create the new customer
        /// </summary>
        /// <param name="customerRequest">Request parameters to create customer</param>
        /// <returns>Customer</returns>
        public Customer CreateCustomer(CreateCustomerRequest customerRequest)
        {
            try
            {
                //create customer API
                var configuration = CreateApiConfiguration();
                var customersApi = new CustomersApi(configuration);

                //create the new customer
                var createCustomerResponse = customersApi.CreateCustomer(customerRequest);
                if (createCustomerResponse == null)
                    throw new NopException("No service response");

                //check whether there are errors in the service response
                if (createCustomerResponse.Errors?.Any() ?? false)
                {
                    var errorsMessage = string.Join(";", createCustomerResponse.Errors.Select(error => error.ToString()));
                    throw new NopException($"There are errors in the service response. {errorsMessage}");
                }

                return createCustomerResponse.Customer;
            }
            catch (Exception exception)
            {
                //log full error
                _logger.Error($"Square payment error: {exception.Message}.", exception, _workContext.CurrentCustomer);

                return null;
            }
        }

        /// <summary>
        /// Create the new card of the customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="cardRequest">Request parameters to create card of the customer</param>
        /// <returns>Card</returns>
        public Card CreateCustomerCard(string customerId, CreateCustomerCardRequest cardRequest)
        {
            try
            {
                //create customer API
                var configuration = CreateApiConfiguration();
                var customersApi = new CustomersApi(configuration);

                //create the new card of the customer
                var createCustomerCardResponse = customersApi.CreateCustomerCard(customerId, cardRequest);
                if (createCustomerCardResponse == null)
                    throw new NopException("No service response");

                //check whether there are errors in the service response
                if (createCustomerCardResponse.Errors?.Any() ?? false)
                {
                    var errorsMessage = string.Join(";", createCustomerCardResponse.Errors.Select(error => error.ToString()));
                    throw new NopException($"There are errors in the service response. {errorsMessage}");
                }

                return createCustomerCardResponse.Card;
            }
            catch (Exception exception)
            {
                //log full error
                _logger.Error($"Square payment error: {exception.Message}.", exception, _workContext.CurrentCustomer);

                return null;
            }
        }

        #endregion

        #region Payment workflow

        /// <summary>
        /// Get transaction by transaction identifier
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Transaction and/or errors if exist</returns>
        public (Transaction, string) GetTransaction(string transactionId)
        {
            try
            {
                //try to get the selected location
                var selectedLocation = GetActiveLocations().FirstOrDefault(location => location.Id.Equals(_squarePaymentSettings.LocationId));
                if (selectedLocation == null)
                    throw new NopException("Location is a required parameter for payment requests");

                //create transaction API
                var configuration = CreateApiConfiguration();
                var transactionsApi = new TransactionsApi(configuration);

                //get transaction by identifier
                var retrieveTransactionResponse = transactionsApi.RetrieveTransaction(selectedLocation.Id, transactionId);
                if (retrieveTransactionResponse == null)
                    throw new NopException("No service response");

                //check whether there are errors in the service response
                if (retrieveTransactionResponse.Errors?.Any() ?? false)
                {
                    var errorsMessage = string.Join(";", retrieveTransactionResponse.Errors.Select(error => error.ToString()));
                    throw new NopException($"There are errors in the service response. {errorsMessage}");
                }

                return (retrieveTransactionResponse.Transaction, null);
            }
            catch (Exception exception)
            {
                //log full error
                var errorMessage = exception.Message;
                _logger.Error($"Square payment error: {errorMessage}.", exception, _workContext.CurrentCustomer);

                if (exception is ApiException apiException)
                {
                    //try to get error details
                    var response = JsonConvert.DeserializeObject<RetrieveTransactionResponse>(apiException.ErrorContent) as RetrieveTransactionResponse;
                    if (response?.Errors?.Any() ?? false)
                        errorMessage = string.Join(";", response.Errors.Select(error => error.Detail));
                }

                return (null, errorMessage);
            }
        }

        /// <summary>
        /// Charge transaction
        /// </summary>
        /// <param name="chargeRequest">Request parameters to charge transaction</param>
        /// <returns>Transaction and/or errors if exist</returns>
        public (Transaction, string) Charge(ExtendedChargeRequest chargeRequest)
        {
            try
            {
                //try to get the selected location
                var selectedLocation = GetActiveLocations().FirstOrDefault(location => location.Id.Equals(_squarePaymentSettings.LocationId));
                if (selectedLocation == null)
                    throw new NopException("Location is a required parameter for payment requests");

                //create transaction API
                var configuration = CreateApiConfiguration();
                var transactionsApi = new TransactionsApi(configuration);

                //create charge transaction
                var chargeResponse = transactionsApi.Charge(selectedLocation.Id, chargeRequest);
                if (chargeResponse == null)
                    throw new NopException("No service response");

                //check whether there are errors in the service response
                if (chargeResponse.Errors?.Any() ?? false)
                {
                    var errorsMessage = string.Join(";", chargeResponse.Errors.Select(error => error.ToString()));
                    throw new NopException($"There are errors in the service response. {errorsMessage}");
                }

                return (chargeResponse.Transaction, null);
            }
            catch (Exception exception)
            {
                //log full error
                var errorMessage = exception.Message;
                _logger.Error($"Square payment error: {errorMessage}.", exception, _workContext.CurrentCustomer);

                if (exception is ApiException apiException)
                {
                    //try to get error details
                    var response = JsonConvert.DeserializeObject<ChargeResponse>(apiException.ErrorContent) as ChargeResponse;
                    if (response?.Errors?.Any() ?? false)
                        errorMessage = string.Join(";", response.Errors.Select(error => error.Detail));
                }

                return (null, errorMessage);
            }
        }

        /// <summary>
        /// Capture authorized transaction
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>True if the transaction successfully captured; otherwise false. And/or errors if exist</returns>
        public (bool, string) CaptureTransaction(string transactionId)
        {
            try
            {
                //try to get the selected location
                var selectedLocation = GetActiveLocations().FirstOrDefault(location => location.Id.Equals(_squarePaymentSettings.LocationId));
                if (selectedLocation == null)
                    throw new NopException("Location is a required parameter for payment requests");

                //create transaction API
                var configuration = CreateApiConfiguration();
                var transactionsApi = new TransactionsApi(configuration);

                //capture transaction by identifier
                var captureTransactionResponse = transactionsApi.CaptureTransaction(selectedLocation.Id, transactionId);
                if (captureTransactionResponse == null)
                    throw new NopException("No service response");

                //check whether there are errors in the service response
                if (captureTransactionResponse.Errors?.Any() ?? false)
                {
                    var errorsMessage = string.Join(";", captureTransactionResponse.Errors.Select(error => error.ToString()));
                    throw new NopException($"There are errors in the service response. {errorsMessage}");
                }

                //if there are no errors in the response, transaction was successfully captured
                return (true, null);
            }
            catch (Exception exception)
            {
                //log full error
                var errorMessage = exception.Message;
                _logger.Error($"Square payment error: {errorMessage}.", exception, _workContext.CurrentCustomer);

                if (exception is ApiException apiException)
                {
                    //try to get error details
                    var response = JsonConvert.DeserializeObject<CaptureTransactionResponse>(apiException.ErrorContent) as CaptureTransactionResponse;
                    if (response?.Errors?.Any() ?? false)
                        errorMessage = string.Join(";", response.Errors.Select(error => error.Detail));
                }

                return (false, errorMessage);
            }
        }

        /// <summary>
        /// Void authorized transaction
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>True if the transaction successfully voided; otherwise false. And/or errors if exist</returns>
        public (bool, string) VoidTransaction(string transactionId)
        {
            try
            {
                //try to get the selected location
                var selectedLocation = GetActiveLocations().FirstOrDefault(location => location.Id.Equals(_squarePaymentSettings.LocationId));
                if (selectedLocation == null)
                    throw new NopException("Location is a required parameter for payment requests");

                //create transaction API
                var configuration = CreateApiConfiguration();
                var transactionsApi = new TransactionsApi(configuration);

                //void transaction by identifier
                var voidTransactionResponse = transactionsApi.VoidTransaction(selectedLocation.Id, transactionId);
                if (voidTransactionResponse == null)
                    throw new NopException("No service response");

                //check whether there are errors in the service response
                if (voidTransactionResponse.Errors?.Any() ?? false)
                {
                    var errorsMessage = string.Join(";", voidTransactionResponse.Errors.Select(error => error.ToString()));
                    throw new NopException($"There are errors in the service response. {errorsMessage}");
                }

                //if there are no errors in the response, transaction was successfully voided
                return (true, null);
            }
            catch (Exception exception)
            {
                //log full error
                var errorMessage = exception.Message;
                _logger.Error($"Square payment error: {errorMessage}.", exception, _workContext.CurrentCustomer);

                if (exception is ApiException apiException)
                {
                    //try to get error details
                    var response = JsonConvert.DeserializeObject<VoidTransactionResponse>(apiException.ErrorContent) as VoidTransactionResponse;
                    if (response?.Errors?.Any() ?? false)
                        errorMessage = string.Join(";", response.Errors.Select(error => error.Detail));
                }

                return (false, errorMessage);
            }
        }

        /// <summary>
        /// Create a refund of the transaction
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <param name="refundRequest">Request parameters to create refund</param>
        /// <returns>Refund and/or errors if exist</returns>
        public (Refund, string) CreateRefund(string transactionId, CreateRefundRequest refundRequest)
        {
            try
            {
                //try to get the selected location
                var selectedLocation = GetActiveLocations().FirstOrDefault(location => location.Id.Equals(_squarePaymentSettings.LocationId));
                if (selectedLocation == null)
                    throw new NopException("Location is a required parameter for payment requests");

                //create transaction API
                var configuration = CreateApiConfiguration();
                var transactionsApi = new TransactionsApi(configuration);

                //create refund
                var createRefundResponse = transactionsApi.CreateRefund(selectedLocation.Id, transactionId, refundRequest);
                if (createRefundResponse == null)
                    throw new NopException("No service response");

                //check whether there are errors in the service response
                if (createRefundResponse.Errors?.Any() ?? false)
                {
                    var errorsMessage = string.Join(";", createRefundResponse.Errors.Select(error => error.ToString()));
                    throw new NopException($"There are errors in the service response. {errorsMessage}");
                }

                return (createRefundResponse.Refund, null);
            }
            catch (Exception exception)
            {
                //log full error
                var errorMessage = exception.Message;
                _logger.Error($"Square payment error: {errorMessage}.", exception, _workContext.CurrentCustomer);

                if (exception is ApiException apiException)
                {
                    //try to get error details
                    var response = JsonConvert.DeserializeObject<CreateRefundResponse>(apiException.ErrorContent) as CreateRefundResponse;
                    if (response?.Errors?.Any() ?? false)
                        errorMessage = string.Join(";", response.Errors.Select(error => error.Detail));
                }

                return (null, errorMessage);
            }
        }

        #endregion

        #region OAuth2 authorization

        /// <summary>
        /// Generate URL for the authorization permissions page
        /// </summary>
        /// <param name="verificationString">String to help protect against cross-site request forgery</param>
        /// <returns>URL</returns>
        public string GenerateAuthorizeUrl(string verificationString)
        {
            var serviceUrl = $"{GetOAuthServiceUrl()}/authorize";

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

            //create query parameters for the request
            var queryParameters = new Dictionary<string, string>
            {
                //The application ID.
                ["client_id"] = _squarePaymentSettings.ApplicationId,

                //Indicates whether you want to receive an authorization code ("code") or an access token ("token").
                ["response_type"] = "code",

                //A space-separated list of the permissions your application is requesting. 
                ["scope"] = requestingPermissions,

                //The locale to present the Permission Request form in. Currently supported values are en-US, en-CA, es-US, fr-CA, and ja-JP.
                //["locale"] = string.Empty,

                //If "false", the Square merchant must log in to view the Permission Request form, even if they already have a valid user session.
                ["session"] = "false",

                //Include this parameter and verify its value to help protect against cross-site request forgery.
                ["state"] = verificationString,

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
        /// <param name="accessTokenRequest">Request parameters to obtain access token</param>
        /// <returns>Access token</returns>
        public string ObtainAccessToken(ObtainAccessTokenRequest accessTokenRequest)
        {
            //create post data
            var postData = Encoding.Default.GetBytes(JsonConvert.SerializeObject(accessTokenRequest));

            //create web request
            var serviceUrl = $"{GetOAuthServiceUrl()}/token";
            var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.ContentLength = postData.Length;
            request.UserAgent = SquarePaymentDefaults.UserAgent;

            //post request
            using (var stream = request.GetRequestStream())
            {
                stream.Write(postData, 0, postData.Length);
            }

            //get response
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //return received access token
                var response = JsonConvert.DeserializeObject<ObtainAccessTokenResponse>(streamReader.ReadToEnd());
                return response?.AccessToken;
            }
        }

        /// <summary>
        /// Renew the expired access token
        /// </summary>
        /// <param name="accessTokenRequest">Request parameters to renew access token</param>
        /// <returns>Access token</returns>
        public string RenewAccessToken(RenewAccessTokenRequest accessTokenRequest)
        {
            //create post data
            var postData = Encoding.Default.GetBytes(JsonConvert.SerializeObject(accessTokenRequest));

            //create web request
            var serviceUrl = $"{GetOAuthServiceUrl()}/clients/{accessTokenRequest.ApplicationId}/access-token/renew";
            var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.ContentLength = postData.Length;
            request.UserAgent = SquarePaymentDefaults.UserAgent;

            //add authorization header
            request.Headers.Add(HttpRequestHeader.Authorization, $"Client {accessTokenRequest.ApplicationSecret}");

            //post request
            using (var stream = request.GetRequestStream())
            {
                stream.Write(postData, 0, postData.Length);
            }

            //get response
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //return received access token
                var response = JsonConvert.DeserializeObject<RenewAccessTokenResponse>(streamReader.ReadToEnd());
                return response?.AccessToken;
            }
        }

        /// <summary>
        /// Revoke all access tokens
        /// </summary>
        /// <param name="revokeTokenRequest">Request parameters to revoke access token</param>
        /// <returns>True if tokens were successfully revoked; otherwise false</returns>
        public bool RevokeAccessTokens(RevokeAccessTokenRequest revokeTokenRequest)
        {
            //create post data
            var postData = Encoding.Default.GetBytes(JsonConvert.SerializeObject(revokeTokenRequest));

            //create web request
            var serviceUrl = $"{GetOAuthServiceUrl()}/revoke";
            var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.ContentLength = postData.Length;
            request.UserAgent = SquarePaymentDefaults.UserAgent;

            //add authorization header
            request.Headers.Add(HttpRequestHeader.Authorization, $"Client {revokeTokenRequest.ApplicationSecret}");

            //post request
            using (var stream = request.GetRequestStream())
            {
                stream.Write(postData, 0, postData.Length);
            }

            //get response
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //return received value
                var response = JsonConvert.DeserializeObject<RevokeAccessTokenResponse>(streamReader.ReadToEnd());
                return response?.SuccessfullyRevoked ?? false;
            }
        }

        #endregion

        #endregion
    }
}