using Nop.Core.Caching;

namespace Nop.Web.Framework.Models.Cms;

public static partial class WidgetModelDefaults
{
    /// <summary>
    /// Key for widget info
    /// </summary>
    /// <remarks>
    /// {0} : current customer role IDs hash
    /// {1} : current store ID
    /// {2} : widget zone
    /// {3} : current theme name
    /// </remarks>
    public static CacheKey WidgetModelKey => new("Nop.pres.widget-{0}-{1}-{2}-{3}", WidgetPrefixCacheKey);
    public static string WidgetPrefixCacheKey => "Nop.pres.widget";
}