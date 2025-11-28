using elFinder.NetCore;
using elFinder.NetCore.Drivers.FileSystem;
using Microsoft.AspNetCore.Http;
using Nop.Core;

namespace Nop.Services.Media.ElFinder;

/// <summary>
/// elFinder service implementation
/// </summary>
public partial class ElFinderService : IElFinderService
{
    #region Fields

    protected readonly IElFinderFileProvider _fileProvider;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public ElFinderService(
        IElFinderFileProvider fileProvider,
        IWebHelper webHelper)
    {
        _fileProvider = fileProvider;
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Configure elFinder connector
    /// </summary>
    /// <param name="request">Http request</param>
    /// <returns>Connector</returns>
    public virtual async Task<Connector> GetConnectorAsync(HttpRequest request)
    {
        // Initialize file provider with current path
        await _fileProvider.InitializeAsync();

        var rootPath = _fileProvider.GetRootPath();
        var urlBase = _fileProvider.GetUrlBase();
        var thumbUrl = _fileProvider.GetUrlThumb();

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
            //AccessControlAttributes = new HashSet<NamedAccessControlAttributeSet>()
            //{
            //    new NamedAccessControlAttributeSet(PathHelper.MapPath("~/images/uploaded/placeholder.txt", rootPath))
            //    {
            //        Write = false,
            //        Locked = true
            //    },
            //},
            // Upload file type constraints
            UploadAllow = new[] { "image" },
            UploadDeny = new[] { "text/csv" },
            UploadOrder = new[] { "allow", "deny" }
        };

        driver.AddRoot(root);

        return new Connector(driver)
        {
            // This allows support for the "onlyMimes" option on the client.
            MimeDetect = MimeDetectOption.Internal
        };
        
    }

    #endregion
}
