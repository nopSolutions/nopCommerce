
namespace Nop.Core.Configuration
{
    public interface IConfigurationProvider<TSettings> where TSettings : ISettings, new() 
    {
        TSettings Settings { get; }
        void LoadSettings(int storeId);
        void SaveSettings(TSettings settings);
        void DeleteSettings();
    }
}
