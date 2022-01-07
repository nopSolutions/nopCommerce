using System;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Logging;

namespace Nop.Plugin.Widgets.AccessiBe.Services
{
    /// <summary>
    /// Represents the plugin service implementation
    /// </summary>
    public class AccessiBeService
    {
        #region Fields

        private readonly AccessiBeSettings _accessiBeSettings;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AccessiBeService(AccessiBeSettings accessiBeSettings,
            ILogger logger,
            IStoreContext storeContext,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext)
        {
            _accessiBeSettings = accessiBeSettings;
            _logger = logger;
            _storeContext = storeContext;
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Handle function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="function">Function to execute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        private async Task<TResult> HandleFunctionAsync<TResult>(Func<Task<TResult>> function)
        {
            try
            {
                //check whether the plugin is active
                if (!await PluginActiveAsync())
                    return default;

                //execute function
                return await function();
            }
            catch (Exception exception)
            {
                //get a short error message
                var detailedException = exception;
                do
                {
                    detailedException = detailedException.InnerException;
                } while (detailedException?.InnerException != null);


                //log errors
                var error = $"{AccessiBeDefaults.SystemName} error: {Environment.NewLine}{exception.Message}";
                await _logger.ErrorAsync(error, exception, await _workContext.GetCurrentCustomerAsync());

                return default;
            }
        }

        /// <summary>
        /// Check whether the plugin is active for the current customer and the current store
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        private async Task<bool> PluginActiveAsync()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            return await _widgetPluginManager.IsPluginActiveAsync(AccessiBeDefaults.SystemName, customer, store?.Id ?? 0);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare installation script
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the installation script
        /// </returns>
        public async Task<string> PrepareScriptAsync()
        {
            return await HandleFunctionAsync(() => Task.FromResult(_accessiBeSettings.Script));
        }

        #endregion
    }
}