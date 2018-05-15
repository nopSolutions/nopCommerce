using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Models;
using Nop.Plugin.Payments.Worldpay.Domain.Requests;
using Nop.Plugin.Payments.Worldpay.Domain.Responses;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.Worldpay.Services
{
    /// <summary>
    /// Represents the Worldpay payment manager
    /// </summary>
    public class WorldpayPaymentManager
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly WorldpayPaymentSettings _worldpayPaymentSettings;

        #endregion

        #region Ctor

        public WorldpayPaymentManager(ILogger logger,
            IWorkContext workContext,
            WorldpayPaymentSettings worldpayPaymentSettings)
        {
            this._logger = logger;
            this._workContext = workContext;
            this._worldpayPaymentSettings = worldpayPaymentSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get the Worldpay service base URL
        /// </summary>
        /// <returns>URL</returns>
        private string GetServiceUrl()
        {
            return _worldpayPaymentSettings.UseSandbox ? "https://gwapi.demo.securenet.com" : "https://gwapi.securenet.com";
        }

        /// <summary>
        /// Set developer credentials to POST, PUT, DELETE request
        /// </summary>
        /// <typeparam name="TRequest">Post request type</typeparam>
        /// <param name="request">Request</param>
        private void SetDeveloperCredentials<TRequest>(TRequest request) where TRequest : WorldpayPostRequest
        {
            if (int.TryParse(_worldpayPaymentSettings.UseSandbox ? WorldpayPaymentDefaults.SandboxDeveloperId : WorldpayPaymentDefaults.DeveloperId, out int developerId))
            {
                request.DeveloperApplication = new DeveloperApplication
                {
                    DeveloperId = developerId,
                    DeveloperVersion = _worldpayPaymentSettings.UseSandbox ? WorldpayPaymentDefaults.SandboxDeveloperVersion : WorldpayPaymentDefaults.DeveloperVersion
                };
            }
        }

        /// <summary>
        /// Process payment request
        /// </summary>
        /// <typeparam name="TRequest">Payment request type</typeparam>
        /// <typeparam name="TResponse">Payment response type</typeparam>
        /// <param name="request">Payment request</param>
        /// <returns>Transaction</returns>
        private Transaction ProcessPaymentRequest<TRequest, TResponse>(TRequest request)
            where TRequest : WorldpayPaymentRequest where TResponse : WorldpayPaymentResponse
        {
            //set developer credentials
            SetDeveloperCredentials(request);

            //process request
            var response = ProcessRequest<TRequest, TResponse>(request)
                ?? throw new NopException("An error occurred. Details in the log.");

            //whether request is approved
            if (response.Code != ResponseCode.Approved)
                throw new NopException($"The request was {response.Code} for a reason '{response.Result}'. {response.Message}");

            //return transaction
            return response.Transaction;
        }

        /// <summary>
        /// Process request
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="worldpayRequest">Request</param>
        /// <returns>Response</returns>
        private TResponse ProcessRequest<TRequest, TResponse>(TRequest worldpayRequest)
            where TRequest : WorldpayRequest where TResponse : WorldpayResponse
        {
            //create web request
            var serviceUrl = $"{GetServiceUrl()}/{worldpayRequest.GetRequestUrl()}";
            var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
            request.Method = worldpayRequest.GetRequestMethod();
            request.UserAgent = WorldpayPaymentDefaults.UserAgent;
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=utf-8";

            //add authorization header
            var login = string.Format("{0}:{1}", _worldpayPaymentSettings.SecureNetId, _worldpayPaymentSettings.SecureKey);
            var authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(login));
            request.Headers.Add(HttpRequestHeader.Authorization, string.Format("Basic {0}", authorization));

            try
            {
                //post request
                if (worldpayRequest.GetRequestMethod() != WebRequestMethods.Http.Get)
                {
                    //create post data
                    var postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(worldpayRequest,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    request.ContentLength = postData.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(postData, 0, postData.Length);
                    }
                }

                //get response
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return JsonConvert.DeserializeObject<TResponse>(streamReader.ReadToEnd());
                }
            }
            catch (Exception exception)
            {
                var errorMessage = $"Worldpay payment error: {exception.Message}.";
                try
                {
                    //try to get error response
                    if (exception is WebException webException)
                    {
                        var httpResponse = (HttpWebResponse)webException.Response;
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var errorResponse = streamReader.ReadToEnd();
                            errorMessage = $"{errorMessage} Details: {errorResponse}";
                            return JsonConvert.DeserializeObject<TResponse>(errorResponse);
                        }
                    }
                }
                finally
                {
                    //log errors
                    _logger.Error(errorMessage, exception, _workContext.CurrentCustomer);
                }

                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get a customer from the Vault
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Vault customer</returns>
        public VaultCustomer GetCustomer(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                return null;

            var request = new GetCustomerRequest { CustomerId = customerId };
            var response = ProcessRequest<GetCustomerRequest, GetCustomerResponse>(request);
            if (response != null && response.Code == ResponseCode.Approved)
            {
                //very strange behavior from the Worldpay API, why not just return a response with 'Error' code?
                if (!response.CustomerId.Equals("error", StringComparison.InvariantCultureIgnoreCase))
                    return response.Customer;

                //log this strange error
                _logger.Error($"Failed to retrieve customer. {response.Customer.Notes}", customer: _workContext.CurrentCustomer);
            }
            else
            {
                //log errors
                _logger.Error($"The request was {response?.Code} for a reason '{response?.Result}'. {response?.Message}", customer: _workContext.CurrentCustomer);
            }

            return null;
        }

        /// <summary>
        /// Create a customer in the Vault
        /// </summary>
        /// <param name="request">Request parameters to create a customer</param>
        /// <returns>Vault customer</returns>
        public VaultCustomer CreateCustomer(CreateCustomerRequest request)
        {
            //set developer credentials
            SetDeveloperCredentials(request);

            //try to create a customer
            var response = ProcessRequest<CreateCustomerRequest, CreateCustomerResponse>(request);
            if (response != null && response.Code == ResponseCode.Approved)
                return response.Customer;

            //log errors
            _logger.Error($"The request was {response?.Code} for a reason '{response?.Result}'. {response?.Message}", customer: _workContext.CurrentCustomer);

            return null;
        }

        /// <summary>
        /// Update a customer in the Vault
        /// </summary>
        /// <param name="request">Request parameters to update a customer</param>
        /// <returns>Vault customer</returns>
        public VaultCustomer UpdateCustomer(UpdateCustomerRequest request)
        {
            //set developer credentials
            SetDeveloperCredentials(request);

            //try to update a customer
            var response = ProcessRequest<UpdateCustomerRequest, UpdateCustomerResponse>(request);
            if (response != null && response.Code == ResponseCode.Approved)
                return response.Customer;

            //log errors
            _logger.Error($"The request was {response.Code} for a reason '{response.Result}'. {response.Message}", customer: _workContext.CurrentCustomer);

            return null;
        }

        /// <summary>
        /// Delete a customer credit card from the Vault
        /// </summary>
        /// <param name="request">Request parameters to delete a card</param>
        /// <returns>True if a card is successfully deleted; otherwise false</returns>
        public bool Deletecard(DeleteCardRequest request)
        {
            //set developer credentials
            SetDeveloperCredentials(request);

            //try to delete a card
            var response = ProcessRequest<DeleteCardRequest, DeleteCardResponse>(request);
            if (response != null && response.Code == ResponseCode.Approved)
                return true;

            //log errors
            _logger.Error($"The request was {response.Code} for a reason '{response.Result}'. {response.Message}", customer: _workContext.CurrentCustomer);

            return false;
        }

        /// <summary>
        /// Authorize a transaction
        /// </summary>
        /// <param name="authorizeRequest">Request parameters to authorize transaction</param>
        /// <returns>Transaction</returns>
        public Transaction Authorize(AuthorizeRequest authorizeRequest)
        {
            return ProcessPaymentRequest<AuthorizeRequest, AuthorizeResponse>(authorizeRequest);
        }

        /// <summary>
        /// Charge
        /// </summary>
        /// <param name="chargeRequest">Request parameters to charge</param>
        /// <returns>Transaction</returns>
        public Transaction Charge(ChargeRequest chargeRequest)
        {
            return ProcessPaymentRequest<ChargeRequest, ChargeResponse>(chargeRequest);
        }

        /// <summary>
        /// Capture an authorized transaction
        /// </summary>
        /// <param name="captureRequest">Request parameters to capture transaction</param>
        /// <returns>Transaction</returns>
        public Transaction CaptureTransaction(CaptureRequest captureRequest)
        {
            return ProcessPaymentRequest<CaptureRequest, CaptureResponse>(captureRequest);
        }

        /// <summary>
        /// Void an authorized transaction
        /// </summary>
        /// <param name="voidRequest">Request parameters to void transaction</param>
        /// <returns>Transaction</returns>
        public Transaction VoidTransaction(VoidRequest voidRequest)
        {
            return ProcessPaymentRequest<VoidRequest, VoidResponse>(voidRequest);
        }

        /// <summary>
        /// Refund a charged transaction
        /// </summary>
        /// <param name="refundRequest">Request parameters to refund transaction</param>
        /// <returns>Transaction</returns>
        public Transaction Refund(RefundRequest refundRequest)
        {
            return ProcessPaymentRequest<RefundRequest, RefundResponse>(refundRequest);
        }

        #endregion
    }
}