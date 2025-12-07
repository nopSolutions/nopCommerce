namespace Nop.Plugin.Theme.KungFu.Services;

public interface IThemeKungFuService
{
    Task<ThemeSyncResult> EnsureSyncedAsync(bool force = false);

    Task<ThemeSyncResult> GetStatusAsync();
}
