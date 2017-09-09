using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.Shipping.FixedOrByWeight.Infrastructure.Cache
{
    /// <summary>
    /// Event consumer of the "Fixed or by weight" shipping plugin (used for removing unused settings)
    /// </summary>
    public partial class FixedOrByWeightEventConsumer : IConsumer<EntityDeleted<ShippingMethod>>
    {
        #region Fields
        
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public FixedOrByWeightEventConsumer(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle shipping method deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(EntityDeleted<ShippingMethod> eventMessage)
        {
            var shippingMethod = eventMessage?.Entity;
            if (shippingMethod == null)
                return;

            //delete saved fixed rate if exists
            var setting = _settingService.GetSetting(string.Format(FixedOrByWeightDefaults.FixedRateSettingsKey, shippingMethod.Id));
            if (setting != null)
                _settingService.DeleteSetting(setting);
        }

        #endregion
    }
}