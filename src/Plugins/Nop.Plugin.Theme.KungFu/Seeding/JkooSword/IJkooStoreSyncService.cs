

namespace Nop.Plugin.Theme.KungFu.Seeding.JkooSword;

public interface IJkooStoreSyncService
{
    Task<SiteMapModel[]> LoadSiteMapAsync();
    
    Task LoadRobotsTxtAsync();
    
    Task<string[]> LoadUrls();
    
    Task SyncProductsAsync();
    
}