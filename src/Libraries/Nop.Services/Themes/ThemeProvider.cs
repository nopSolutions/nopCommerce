using System.Text;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using Nop.Services.Plugins;

namespace Nop.Services.Themes;

/// <summary>
/// Represents a default theme provider implementation
/// </summary>
public partial class ThemeProvider : IThemeProvider
{
    #region Fields

    protected static readonly object _locker = new();

    protected readonly INopFileProvider _fileProvider;

    protected Dictionary<string, ThemeDescriptor> _themeDescriptors;

    #endregion

    #region Ctor

    public ThemeProvider(INopFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
        Initialize();
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Initializes theme provider
    /// </summary>
    protected virtual void Initialize()
    {
        if (_themeDescriptors != null)
            return;

        //prevent multi loading data
        lock (_locker)
        {
            //data can be loaded while we waited
            if (_themeDescriptors != null)
                return;

            //load all theme descriptors
            _themeDescriptors =
                new Dictionary<string, ThemeDescriptor>(StringComparer.InvariantCultureIgnoreCase);

            var themeDirectoryPath = _fileProvider.MapPath(NopPluginDefaults.ThemesPath);
            foreach (var descriptionFile in _fileProvider.GetFiles(themeDirectoryPath,
                         NopPluginDefaults.ThemeDescriptionFileName, false))
            {
                var text = _fileProvider.ReadAllText(descriptionFile, Encoding.UTF8);
                if (string.IsNullOrEmpty(text))
                    continue;

                //get theme descriptor
                var themeDescriptor = GetThemeDescriptorFromText(text);

                //some validation
                if (string.IsNullOrEmpty(themeDescriptor?.SystemName))
                    throw new Exception($"A theme descriptor '{descriptionFile}' has no system name");

                _themeDescriptors.TryAdd(themeDescriptor.SystemName, themeDescriptor);
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get theme descriptor from the description text
    /// </summary>
    /// <param name="text">Description text</param>
    /// <returns>Theme descriptor</returns>
    public ThemeDescriptor GetThemeDescriptorFromText(string text)
    {
        //get theme description from the JSON file
        var themeDescriptor = JsonConvert.DeserializeObject<ThemeDescriptor>(text);

        //some validation
        if (_themeDescriptors.ContainsKey(themeDescriptor.SystemName))
            throw new Exception($"A theme with '{themeDescriptor.SystemName}' system name is already defined");

        return themeDescriptor;
    }

    /// <summary>
    /// Get all themes
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the theme descriptor
    /// </returns>
    public Task<IList<ThemeDescriptor>> GetThemesAsync()
    {
        return Task.FromResult<IList<ThemeDescriptor>>(_themeDescriptors.Values.ToList());
    }

    /// <summary>
    /// Get a theme by the system name
    /// </summary>
    /// <param name="systemName">Theme system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the me descriptor
    /// </returns>
    public Task<ThemeDescriptor> GetThemeBySystemNameAsync(string systemName)
    {
        if (string.IsNullOrEmpty(systemName))
            return Task.FromResult<ThemeDescriptor>(null);

        _themeDescriptors.TryGetValue(systemName, out var descriptor);

        return Task.FromResult(descriptor);
    }

    /// <summary>
    /// Check whether the theme with specified system name exists
    /// </summary>
    /// <param name="systemName">Theme system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if the theme exists; otherwise false
    /// </returns>
    public Task<bool> ThemeExistsAsync(string systemName)
    {
        if (string.IsNullOrEmpty(systemName))
            return Task.FromResult(false);

        return Task.FromResult(_themeDescriptors.ContainsKey(systemName));
    }

    #endregion
}