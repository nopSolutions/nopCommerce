using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.Tax.FixedRate.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer
    /// </summary>
    public partial class ModelCacheEventConsumer: 
        IConsumer<EntityDeleted<TaxCategory>>
    {
        private readonly ISettingService _settingService;

        public ModelCacheEventConsumer(ISettingService settingService)
        {
            this._settingService = settingService;
        }
        
        public void HandleEvent(EntityDeleted<TaxCategory> eventMessage)
        {
            if (eventMessage.Entity == null)
                return;

            //delete an appropriate setting when tax category is deleted
            var settingKey = string.Format("Tax.TaxProvider.FixedRate.TaxCategoryId{0}", eventMessage.Entity.Id);
            var setting = _settingService.GetSetting(settingKey);
            if (setting != null)
                _settingService.DeleteSetting(setting);
        }
    }
}
