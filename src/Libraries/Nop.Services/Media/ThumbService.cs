using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;

namespace Nop.Services.Media;

/// <summary>
/// Picture thumb service
/// </summary>
public partial class ThumbService : IThumbService
{
    #region Fields

    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IWebHelper _webHelper;
    protected readonly MediaSettings _mediaSettings;

    #endregion

    #region Ctor

    public ThumbService(IHttpContextAccessor httpContextAccessor,
        INopFileProvider fileProvider,
        IWebHelper webHelper,
        MediaSettings mediaSettings)
    {
        _httpContextAccessor = httpContextAccessor;
        _fileProvider = fileProvider;
        _webHelper = webHelper;
        _mediaSettings = mediaSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Delete picture thumbs
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePictureThumbsAsync(Picture picture)
    {
        var filter = $"{picture.Id:0000000}*.*";
        var currentFiles = _fileProvider.GetFiles(_fileProvider.Combine(_fileProvider.GetLocalImagesPath(_mediaSettings), NopMediaDefaults.ImageThumbsPath), filter, false);
        foreach (var currentFileName in currentFiles)
        {
            var thumbFilePath = await GetThumbLocalPathByFileNameAsync(currentFileName);
            _fileProvider.DeleteFile(thumbFilePath);
        }
    }

    /// <summary>
    /// Get picture (thumb) local path
    /// </summary>
    /// <param name="thumbFileName">Filename</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture thumb path
    /// </returns>
    public virtual Task<string> GetThumbLocalPathByFileNameAsync(string thumbFileName)
    {
        var thumbsDirectoryPath = _fileProvider.Combine(_fileProvider.GetLocalImagesPath(_mediaSettings), NopMediaDefaults.ImageThumbsPath);

        if (_mediaSettings.MultipleThumbDirectories)
        {
            //get the first two letters of the file name
            var fileNameWithoutExtension = _fileProvider.GetFileNameWithoutExtension(thumbFileName);
            if (fileNameWithoutExtension != null && fileNameWithoutExtension.Length > NopMediaDefaults.MultipleThumbDirectoriesLength)
            {
                var subDirectoryName = fileNameWithoutExtension[..NopMediaDefaults.MultipleThumbDirectoriesLength];
                thumbsDirectoryPath = _fileProvider.Combine(_fileProvider.GetLocalImagesPath(_mediaSettings), NopMediaDefaults.ImageThumbsPath, subDirectoryName);
                _fileProvider.CreateDirectory(thumbsDirectoryPath);
            }
        }

        var thumbFilePath = _fileProvider.Combine(thumbsDirectoryPath, thumbFileName);
        return Task.FromResult(thumbFilePath);
    }

    /// <summary>
    /// Get a value indicating whether some file (thumb) already exists
    /// </summary>
    /// <param name="thumbFilePath">Thumb file path</param>
    /// <param name="thumbFileName">Thumb file name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check result
    /// </returns>
    public virtual Task<bool> GeneratedThumbExistsAsync(string thumbFilePath, string thumbFileName)
    {
        return Task.FromResult(_fileProvider.FileExists(thumbFilePath));
    }

    /// <summary>
    /// Get a picture thumb local path
    /// </summary>
    /// <param name="pictureUrl">Picture URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture thumb path
    /// </returns>
    public virtual async Task<string> GetThumbLocalPathAsync(string pictureUrl)
    {
        if (string.IsNullOrEmpty(pictureUrl))
            return string.Empty;

        return await GetThumbLocalPathByFileNameAsync(_fileProvider.GetFileName(pictureUrl));
    }

    /// <summary>
    /// Save a picture thumb
    /// </summary>
    /// <param name="thumbFilePath">Thumb file path</param>
    /// <param name="thumbFileName">Thumb file name</param>
    /// <param name="mimeType">MIME type</param>
    /// <param name="binary">Picture binary</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveThumbAsync(string thumbFilePath, string thumbFileName, string mimeType, byte[] binary)
    {
        //ensure \thumb directory exists
        var thumbsDirectoryPath = _fileProvider.Combine(_fileProvider.GetLocalImagesPath(_mediaSettings), NopMediaDefaults.ImageThumbsPath);
        _fileProvider.CreateDirectory(thumbsDirectoryPath);

        //save
        await _fileProvider.WriteAllBytesAsync(thumbFilePath, binary);
    }

    /// <summary>
    /// Get picture (thumb) URL 
    /// </summary>
    /// <param name="thumbFileName">Filename</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture thumb path
    /// </returns>
    public virtual Task<string> GetThumbUrlAsync(string thumbFileName, string storeLocation = null)
    {
        var pathBase = _httpContextAccessor.HttpContext?.Request.PathBase.Value ?? string.Empty;
        var imagesPathUrl = _mediaSettings.UseAbsoluteImagePath ? storeLocation : $"{pathBase}/";
        imagesPathUrl = string.IsNullOrEmpty(imagesPathUrl) ? _webHelper.GetStoreLocation() : imagesPathUrl;
        imagesPathUrl += "images/thumbs/";

        if (_mediaSettings.MultipleThumbDirectories)
        {
            //get the first two letters of the file name
            var fileNameWithoutExtension = _fileProvider.GetFileNameWithoutExtension(thumbFileName);
            if (fileNameWithoutExtension != null && fileNameWithoutExtension.Length > NopMediaDefaults.MultipleThumbDirectoriesLength)
            {
                var subDirectoryName = fileNameWithoutExtension[..NopMediaDefaults.MultipleThumbDirectoriesLength];
                imagesPathUrl = imagesPathUrl + subDirectoryName + "/";
            }
        }

        imagesPathUrl += thumbFileName;
        return Task.FromResult(imagesPathUrl);
    }

    /// <summary>
    /// Gets the information about thumbs
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the thumbs file count and size
    /// </returns>
    public virtual async Task<(int filesCount, long filesSize)> GetThumbsInfoAsync()
    {
        var filesCount = 0;
        var filesSize = 0L;
        var currentFiles = _fileProvider.GetFiles(_fileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath), topDirectoryOnly: false);
        
        foreach (var currentFileName in currentFiles)
        {
            if (currentFileName.EndsWith("placeholder.txt"))
                continue;

            filesCount++;

            var thumbFilePath = await GetThumbLocalPathByFileNameAsync(currentFileName);
            filesSize += _fileProvider.FileLength(thumbFilePath);
        }

        return (filesCount, filesSize);
    }

    /// <summary>
    /// Delete all thumbs
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAllThumbsAsync()
    {
        var currentFiles = _fileProvider.GetFiles(_fileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath), topDirectoryOnly: false);

        foreach (var currentFileName in currentFiles)
        {
            if (currentFileName.EndsWith("placeholder.txt"))
                continue;

            var thumbFilePath = await GetThumbLocalPathByFileNameAsync(currentFileName);
            _fileProvider.DeleteFile(thumbFilePath);
        }

        foreach (var directory in _fileProvider.GetDirectories(_fileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath)))
            _fileProvider.DeleteDirectory(directory);
    }

    #endregion
}