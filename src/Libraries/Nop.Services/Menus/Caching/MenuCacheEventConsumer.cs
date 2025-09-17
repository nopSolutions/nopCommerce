using Nop.Core.Domain.Menus;
using Nop.Services.Caching;

namespace Nop.Services.Menus.Caching;

/// <summary>
/// Represents a menu item cache event consumer
/// </summary>
public partial class MenuCacheEventConsumer : CacheEventConsumer<Menu>;
