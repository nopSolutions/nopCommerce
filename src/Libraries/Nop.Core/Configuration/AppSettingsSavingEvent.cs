namespace Nop.Core.Configuration;

/// <summary>
/// Represents the event that is raised when App Settings are saving
/// </summary>
public partial class AppSettingsSavingEvent
{
    #region Fields

    protected readonly IList<IConfig> _configurations;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="configurations">List of configuration to save</param>
    public AppSettingsSavingEvent(IList<IConfig> configurations)
    {
        _configurations = configurations;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Add configuration to save
    /// </summary>
    /// <param name="config">Configuration to save</param>
    public void AddConfig<TConfig>(TConfig config) where TConfig : class, IConfig
    {
        if (_configurations.OfType<TConfig>().FirstOrDefault() is { } currentConfig)
            _configurations[_configurations.IndexOf(currentConfig)] = config;
        else
            _configurations.Add(config);
    }

    #endregion
}