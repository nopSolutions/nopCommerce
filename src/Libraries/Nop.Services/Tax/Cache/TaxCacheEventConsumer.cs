using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Services.Tax.Cache
{
    /// <summary>
    /// Tax cache event consumer (used for caching of tax)
    /// </summary>
    public partial class TaxCacheEventConsumer :
        //address
        IConsumer<EntityUpdatedEvent<Address>>
    {
        #region Fields

        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public TaxCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        public void HandleEvent(EntityUpdatedEvent<Address> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(NopTaxDefaults.TaxAddressPrefixCacheKey, eventMessage.Entity.Id));
        }

        #endregion
    }
}