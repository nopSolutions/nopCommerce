using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using DirectoryHelper = System.IO.Directory;

namespace Nop.Services.Media.ElFinder;

/// <summary>
/// elFinder file provider implementation
/// </summary>
public partial class ElFinderFileProvider : IElFinderFileProvider
{
    #region Fields

    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly INopFileProvider _nopFileProvider;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IWebHelper _webHelper;
    protected readonly MediaSettings _mediaSettings;
    protected string _rootPath = string.Empty;
    protected string _urlBase = string.Empty;

    #endregion

    #region Ctor

    public ElFinderFileProvider(IActionContextAccessor actionContextAccessor,
        INopFileProvider nopFileProvider,
        IUrlHelperFactory urlHelperFactory,
        IWebHelper webHelper,
        MediaSettings mediaSettings)
    {
        _actionContextAccessor = actionContextAccessor;
        _nopFileProvider = nopFileProvider;
        _urlHelperFactory = urlHelperFactory;
        _webHelper = webHelper;
        _mediaSettings = mediaSettings;        
        _rootPath = nopFileProvider.Combine(nopFileProvider.GetLocalImagesPath(mediaSettings), NopElFinderDefaults.DefaultRootDirectory);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initialize file provider
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InitializeAsync()
    {
        var pathBase = _webHelper.GetStoreLocation();
        _urlBase = $"{pathBase}images/{NopElFinderDefaults.DefaultRootDirectory}/";
        
        // Create root directory if it doesn't exist
        if (!DirectoryHelper.Exists(_rootPath))
        {
            DirectoryHelper.CreateDirectory(_rootPath);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Get file provider root path
    /// </summary>
    /// <returns>Root path</returns>
    public virtual string GetRootPath()
    {
        return _rootPath;
    }

    /// <summary>
    /// Get URL base for file access
    /// </summary>
    /// <returns>URL base</returns>
    public virtual string GetUrlBase()
    {
        return _urlBase;
    }

    /// <summary>
    /// Get URL thumb
    /// </summary>
    /// <returns>URL thumb</returns>
    public virtual string GetUrlThumb()
    {
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        var thumbUrl = urlHelper.Action("Thumb", "ElFinder", new { area = "Admin" });

        return $"{thumbUrl}/";
    }

    #endregion
}
