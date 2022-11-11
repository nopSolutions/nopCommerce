using Nop.Core.Caching;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Represents default values related to messages services
    /// </summary>
    public static partial class NopMessageDefaults
    {
        /// <summary>
        /// Gets a key for notifications list from TempDataDictionary
        /// </summary>
        public static string NotificationListKey => "NotificationList";

        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : is active?
        /// </remarks>
        public static CacheKey MessageTemplatesAllCacheKey => new("Nop.messagetemplate.all.{0}-{1}", NopEntityCacheDefaults<MessageTemplate>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// {1} : store ID
        /// </remarks>
        public static CacheKey MessageTemplatesByNameCacheKey => new("Nop.messagetemplate.byname.{0}-{1}", MessageTemplatesByNamePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// </remarks>
        public static string MessageTemplatesByNamePrefix => "Nop.messagetemplate.byname.{0}";

        #endregion
    }
}