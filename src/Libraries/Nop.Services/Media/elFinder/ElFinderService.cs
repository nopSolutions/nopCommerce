using elFinder.NetCore;
using elFinder.NetCore.Drivers.FileSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;

namespace Nop.Services.Media.ElFinder;

/// <summary>
/// elFinder service implementation
/// </summary>
public partial class ElFinderService : IElFinderService
{
    #region Fields

    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly INopFileProvider _nopFileProvider;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IWebHelper _webHelper;
    protected readonly MediaSettings _mediaSettings;

    protected readonly Dictionary<int, Connector> _connectors = new();

    #endregion

    #region Ctor

    public ElFinderService(IActionContextAccessor actionContextAccessor,
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
    }

    #endregion

    #region Methods

    /// <summary>
    /// Configure elFinder connector
    /// </summary>
    /// <param name="storeId">Store identifier</param>
    /// <returns>Connector</returns>
    public virtual Connector GetConnector(int storeId)
    {
        if (_connectors.TryGetValue(storeId, out var cachedConnector))
            return cachedConnector;

        var pathBase = _webHelper.GetStoreLocation();
        var urlBase = $"{pathBase}{_mediaSettings.PicturePath}/{NopElFinderDefaults.DefaultRootDirectory}/";
        var rootPath = _nopFileProvider.Combine(_nopFileProvider.GetLocalImagesPath(_mediaSettings), NopElFinderDefaults.DefaultRootDirectory);

        // Create root directory if it doesn't exist
        _nopFileProvider.CreateDirectory(rootPath);

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        var thumbUrl = $"{urlHelper.Action("Thumb", "ElFinder", new { area = "Admin" })}/";

        var driver = new FileSystemDriver();

        // Get root volume configuration
        var root = new RootVolume(
            rootPath,
            urlBase,
            thumbUrl)
        {
            IsReadOnly = false,
            // If locked, files and directories cannot be deleted, renamed or moved
            IsLocked = false,
            Alias = NopElFinderDefaults.DefaultRootDirectory,
            MaxUploadSizeInMb = NopElFinderDefaults.MaxUploadFileSize,
            // Upload file type constraints
            UploadAllow = new[] { "image" },
            UploadDeny = new[] { "text/csv" },
            UploadOrder = new[] { "allow", "deny" }
        };

        driver.AddRoot(root);

        var connector = new Connector(driver)
        {
            // This allows support for the "onlyMimes" option on the client.
            MimeDetect = MimeDetectOption.Internal
        };

        _connectors.Add(storeId, connector);

        return connector;
    }

    #endregion
}