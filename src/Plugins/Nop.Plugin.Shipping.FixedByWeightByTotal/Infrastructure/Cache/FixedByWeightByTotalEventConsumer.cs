using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Infrastructure.Cache
{
    /// <summary>
    /// Event consumer of the "Fixed or by weight" shipping plugin (used for removing unused settings)
    /// </summary>
    public partial class FixedByWeightByTotalEventConsumer : IConsumer<EntityDeletedEvent<ShippingMethod>>
    {
        #region Fields
        
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public FixedByWeightByTotalEventConsumer(ISettingService settingService)
        {
            _settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle shipping method deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ShippingMethod> eventMessage)
        {
            var shippingMethod = eventMessage?.Entity;
            if (shippingMethod == null)
                return;

            //delete saved fixed rate if exists
            var setting = await _settingService.GetSettingAsync(string.Format(FixedByWeightByTotalDefaults.FixedRateSettingsKey, shippingMethod.Id));
            if (setting != null)
                await _settingService.DeleteSettingAsync(setting);
        }

        #endregion
    }
}