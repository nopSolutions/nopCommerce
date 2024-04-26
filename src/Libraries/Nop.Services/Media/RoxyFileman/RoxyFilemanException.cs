using System.Text;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;

namespace Nop.Services.Media.RoxyFileman;

/// <summary>
/// Represents errors that occur when working with uploaded files
/// </summary>
public partial class RoxyFilemanException : Exception
{
    #region Fields

    protected static Dictionary<string, string> _languageResources;

    #endregion

    #region Ctor

    private RoxyFilemanException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the RoxyFilemanException
    /// </summary>
    /// <param name="key">Roxy language resource key</param>
    public RoxyFilemanException(string key) : base(GetLocalizedMessage(key))
    {
    }

    /// <summary>
    /// Initializes a new instance of the RoxyFilemanException
    /// </summary>
    /// <param name="key">Roxy language resource key</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public RoxyFilemanException(string key, Exception innerException) : base(GetLocalizedMessage(key), innerException)
    {
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get the language resource value
    /// </summary>
    /// <param name="key">Language resource key</param>
    /// <returns>
    /// The language resource value
    /// </returns>
    protected static string GetLocalizedMessage(string key)
    {
        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

        var roxyConfig = Singleton<RoxyFilemanConfig>.Instance;
        var languageFile = fileProvider.GetAbsolutePath($"{NopRoxyFilemanDefaults.LanguageDirectory}/{roxyConfig.LANG}.json");

        if (!fileProvider.FileExists(languageFile))
            languageFile = fileProvider.GetAbsolutePath($"{NopRoxyFilemanDefaults.LanguageDirectory}/en.json");

        if (_languageResources is null)
        {
            var json = fileProvider.ReadAllTextAsync(languageFile, Encoding.UTF8).Result;
            _languageResources = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        if (_languageResources is null)
            return key;

        if (_languageResources.TryGetValue(key, out var value))
            return value;

        return key;
    }

    #endregion
}