using Microsoft.AspNetCore.Http;

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
    /// <param name="form">Request form data</param>
    public AppSettingsSavingEvent(IList<IConfig> configurations, IFormCollection form)
    {
        _configurations = configurations;
        FormData = form;
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

    #region Properties

    /// <summary>
    /// Gets request form data if exists
    /// </summary>
    public IFormCollection FormData { get; protected set; }

    #endregion
}