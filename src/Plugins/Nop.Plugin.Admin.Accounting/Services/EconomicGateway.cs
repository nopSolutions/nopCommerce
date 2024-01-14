using AO.Services.Domain;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Plugin.Admin.Accounting.Models.EconomicModels;
using Nop.Services.Logging;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.Admin.Accounting.Services
{
    public class EconomicGateway : IEconomicGateway
    {
        #region Private variables
        private readonly ILogger _logger;
        private readonly string _appSecretToken;
        private readonly string _agreementGrantToken;
        /// <summary>
        /// https://restapi.e-conomic.com/
        /// </summary>
        private Uri _baseAddressUri = new Uri("https://restapi.e-conomic.com/"); 
        #endregion

        public EconomicGateway(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _appSecretToken = configuration["EconomicApiSettings:AppSecretToken"];
            _agreementGrantToken = configuration["EconomicApiSettings:AgreementGrantToken"];
        }
       
        public async Task<bool> HasBeenBookedAsync(AOInvoice invoice)
        {
            try
            {
                var request = AddHeaders(Method.GET);

                var client = new RestClient($"{_baseAddressUri}invoices/drafts?filter=customer.customerNumber$eq:{invoice.CustomerId}$and:notes.textLine1$like:{invoice.Id}");
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    if (await AnyResultsAsync(response.Content))
                    {
                        return true;
                    }
                }
                else
                {
                    var jsonObject = JObject.Parse(response.Content);
                    string message = $"E-conomic: HasBeenBookedAsync, invoiceid: {invoice.Id}.{Environment.NewLine}Error: {jsonObject["message"]?.ToString()}";
                    await _logger.ErrorAsync(message);
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
                return false;
            }

            return false;
        }

        public async Task<bool> CustomerExistAsync(int customerId)
        {
            try
            {
                var request = AddHeaders(Method.GET);

                var client = new RestClient($"{_baseAddressUri}customers?filter=customerNumber$eq:{customerId}");
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    if (await AnyResultsAsync(response.Content))
                    {
                        return true;
                    }
                }
                else
                {
                    var jsonObject = JObject.Parse(response.Content);
                    string message = $"E-conomic: CustomerExistAsync, customerId: {customerId}.{Environment.NewLine}Error: {jsonObject["message"]?.ToString()}";
                    await _logger.ErrorAsync(message);
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);          
            }

            return false;
        }

        public async Task<int> BookInvoiceAsync(InvoiceBookModel invoiceBookModel)
        {
            try
            {
                var client = new RestClient($"{_baseAddressUri}invoices/drafts");               

                // Serialize the invoiceModel object to JSON
                string jsonInvoiceModel = JsonConvert.SerializeObject(invoiceBookModel);

                // Create a POST request
                var request = AddHeaders(Method.POST);

                // Add JSON to the request body
                request.AddParameter("application/json", jsonInvoiceModel, ParameterType.RequestBody);

                // Execute the request
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    return await GetDraftInvoiceNumberAsync(response.Content);
                }
                else
                {                   
                    string message = $"E-conomic: BookInvoiceAsync, customer number: {invoiceBookModel.Customer?.CustomerNumber}.{Environment.NewLine}Error: {response.Content}";
                    await _logger.ErrorAsync(message);
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
                return -1;
            }

            return 0;   
        }

        public async Task<bool> CreateCustomerAsync(CustomerInfo customerInfo)
        {
            try
            {
                var client = new RestClient($"{_baseAddressUri}customers");

                // Serialize the CustomerInfo object to JSON
                string jsonCustomerInfo = JsonConvert.SerializeObject(customerInfo);

                // Create a POST request
                var request = AddHeaders(Method.POST);

                // Add JSON to the request body
                request.AddParameter("application/json", jsonCustomerInfo, ParameterType.RequestBody);

                // Execute the request
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    return true;
                }
                else
                {                    
                    string message = $"E-conomic: CreateCustomerAsync, CustomerNumber: {customerInfo.CustomerNumber}.{Environment.NewLine}Error: {response.Content}";
                    await _logger.ErrorAsync(message);
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
            }

            return false;
        }

        #region Private methods
        private async Task<bool> AnyResultsAsync(string jsonContent)
        {
            try
            {
                var jObject = JObject.Parse(jsonContent);
                var resultsCount = jObject["pagination"]?["results"]?.Value<int>();
                return resultsCount > 0;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
            }

            return false;
        }

        private async Task<int> GetDraftInvoiceNumberAsync(string jsonContent)
        {
            int? draftInvoiceNumber = 0;
            try
            {
                var jObject = JObject.Parse(jsonContent);
                draftInvoiceNumber = jObject["draftInvoiceNumber"]?.Value<int>();
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
            }

            return draftInvoiceNumber.Value;
        }

        private RestRequest AddHeaders(Method method)
        {
            var request = new RestRequest();
            request.Method = method;

            request.AddHeader("content-type", "application/json");

            request.AddHeader("X-AppSecretToken", _appSecretToken);
            request.AddHeader("X-AgreementGrantToken", _agreementGrantToken);

            return request;
        } 
        #endregion
    }
}