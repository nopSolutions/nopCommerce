using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core;
using PeachPaymentsCheckoutSdk.Core;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Nop.Services.Directory;
using Nop.Core.Domain.Directory;
using Nop.Web.Framework.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using MaxMind.GeoIP2.Exceptions;

namespace Nop.Plugin.Payments.PeachPayments.Services
{
    public class ServiceManager
    {
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;

    
        public ServiceManager(CurrencyService currencyService,CurrencySettings currencySettings,ILogger logger,IWorkContext workContext)
        {
            _currencyService = currencyService;
            _currencySettings = currencySettings;
            _logger = logger;
            _workContext = workContext;
        }
        public static bool IsConfigured(PeachPaymentsSettings settings)
        {
            //client id and secret are required to request services
            return !string.IsNullOrEmpty(settings?.ClientId) && !string.IsNullOrEmpty(settings?.SecretKey);
        }
        public async Task<(string Script, string Error)> GetScriptAsync(PeachPaymentsSettings settings, string widgetZone)
        {
            return await HandleFunctionAsync(async () =>
            {
                //ensure that plugin is configured
                if (!IsConfigured(settings))
                    throw new NopException("Plugin not configured");

                var components = new List<string>() { "buttons" };

                var parameters = new Dictionary<string, string>
                {
                    ["client-id"] = settings.ClientId,
                    ["currency"] = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode?.ToUpperInvariant(),
                    ["intent"] = settings.PaymentType.ToString().ToLowerInvariant(),
                    ["commit"] = (settings.PaymentType == Domains.PaymentType.Capture).ToString().ToLowerInvariant(),
                    ["vault"] = false.ToString().ToLowerInvariant(),
                    ["debug"] = false.ToString().ToLowerInvariant(),
                    ["components"] = "",
                };
                if (!string.IsNullOrEmpty(settings.DisabledFunding))
                    parameters["disable-funding"] = settings.DisabledFunding;
                if (!string.IsNullOrEmpty(settings.EnabledFunding))
                    parameters["enable-funding"] = settings.EnabledFunding;
                if (widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore) || widgetZone.Equals(PublicWidgetZones.ProductDetailsTop))
                    components.Add("funding-eligibility");
                if (settings.DisplayPayLaterMessages)
                    components.Add("messages");
                parameters["components"] = string.Join(",", components);
                
                var pageType = widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore)
                    ? "cart"
                    : (widgetZone.Equals(PublicWidgetZones.ProductDetailsTop)
                    ? "product-details"
                    : "checkout");

                return $@"";
            });
        }
        private async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function)
        {
            try
            {
                //invoke function
                return (await function(), default);
            }
            catch (Exception exception)
            {
                //get a short error message
                var message = exception.Message;
                if (exception is HttpException httpException)
                {
                    //get error details if exist
                    var details = JsonConvert.DeserializeObject<ExceptionDetails>(httpException.Message);
                    message = details.Message?.Trim('.') ?? details.Name ?? message;
                    if (details?.Details?.Any() ?? false)
                    {
                        message += details.Details.Aggregate(":", (text, issue) => $"{text} " +
                            $"{(issue.Description ?? issue.Issue).Trim('.')}{(!string.IsNullOrEmpty(issue.Field) ? $"({issue.Field})" : null)},").Trim(',');
                    }
                }

                //log errors
                var logMessage = $"{PeachPaymentsDefaults.SystemName} error: {System.Environment.NewLine}{message}";
                await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());

                return (default, message);
            }
        }

    }
}
