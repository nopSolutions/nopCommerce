using System.Globalization;
using System.IO.Compression;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Themes;

namespace Nop.Services.Plugins;

/// <summary>
/// Represents the implementation of a service for uploading application extensions (plugins or themes) and favicon and app icons
/// </summary>
public partial class UploadService : IUploadService
{
    #region Fields

    protected readonly INopFileProvider _fileProvider;
    protected readonly IStoreContext _storeContext;
    protected readonly IThemeProvider _themeProvider;

    #endregion

    #region Ctor

    public UploadService(INopFileProvider fileProvider,
        IStoreContext storeContext,
        IThemeProvider themeProvider)
    {
        _fileProvider = fileProvider;
        _storeContext = storeContext;
        _themeProvider = themeProvider;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get information about the uploaded items in the archive
    /// </summary>
    /// <param name="archivePath">Path to the archive</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of uploaded items
    /// </returns>
    protected virtual async Task<IList<UploadedItem>> GetUploadedItemsAsync(string archivePath)
    {
        using var archive = ZipFile.OpenRead(archivePath);
        //try to get the entry containing information about the uploaded items 
        var uploadedItemsFileEntry = archive.Entries
            .FirstOrDefault(entry => entry.Name.Equals(NopPluginDefaults.UploadedItemsFileName, StringComparison.InvariantCultureIgnoreCase)
                                     && string.IsNullOrEmpty(_fileProvider.GetDirectoryName(entry.FullName)));
        if (uploadedItemsFileEntry == null)
            return null;

        //read the content of this entry if exists
        await using var unzippedEntryStream = uploadedItemsFileEntry.Open();
        using var reader = new StreamReader(unzippedEntryStream);
        var content = await reader.ReadToEndAsync();

        return JsonConvert.DeserializeObject<IList<UploadedItem>>(content);
    }

    /// <summary>
    /// Upload single item from the archive into the physical directory
    /// </summary>
    /// <param name="archivePath">Path to the archive</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the item descriptor
    /// </returns>
    protected virtual async Task<IDescriptor> UploadSingleItemAsync(string archivePath)
    {
        //get path to the plugins directory
        var pluginsDirectory = _fileProvider.MapPath(NopPluginDefaults.UploadedPath);

        //ensure plugins directory is created
        _fileProvider.CreateDirectory(pluginsDirectory);

        //get path to the themes directory
        var themesDirectory = string.Empty;
        if (!string.IsNullOrEmpty(NopPluginDefaults.ThemesPath))
            themesDirectory = _fileProvider.MapPath(NopPluginDefaults.ThemesPath);

        IDescriptor descriptor = null;
        string uploadedItemDirectoryName;
        using (var archive = ZipFile.OpenRead(archivePath))
        {
            //the archive should contain only one root directory (the plugin one or the theme one)
            var rootDirectories = archive.Entries.Select(p => p.FullName.Split('/')[0]).Distinct().ToList();

            if (rootDirectories.Count != 1)
            {
                throw new Exception("The archive should contain only one root plugin or theme directory. " +
                                    "For example, Payments.PayPalDirect or DefaultClean. " +
                                    $"To upload multiple items, the archive should have the '{NopPluginDefaults.UploadedItemsFileName}' file in the root");
            }

            //get directory name (remove the ending /)
            uploadedItemDirectoryName = rootDirectories.First();

            //try to get descriptor of the uploaded item
            foreach (var entry in archive.Entries)
            {
                //whether it's a plugin descriptor
                var isPluginDescriptor = entry.FullName
                    .Equals($"{uploadedItemDirectoryName}/{NopPluginDefaults.DescriptionFileName}", StringComparison.InvariantCultureIgnoreCase);

                //or whether it's a theme descriptor
                var isThemeDescriptor = entry.FullName
                    .Equals($"{uploadedItemDirectoryName}/{NopPluginDefaults.ThemeDescriptionFileName}", StringComparison.InvariantCultureIgnoreCase);

                if (!isPluginDescriptor && !isThemeDescriptor)
                    continue;

                await using var unzippedEntryStream = entry.Open();
                using var reader = new StreamReader(unzippedEntryStream);
                var content = await reader.ReadToEndAsync();

                //whether a plugin is upload 
                if (isPluginDescriptor)
                {
                    descriptor = PluginDescriptor.GetPluginDescriptorFromText(content);

                    //ensure that the plugin current version is supported
                    if (!((PluginDescriptor)descriptor).SupportedVersions.Contains(NopVersion.CURRENT_VERSION))
                        throw new Exception($"This plugin doesn't support the current version - {NopVersion.CURRENT_VERSION}");
                }

                //or whether a theme is upload 
                if (isThemeDescriptor)
                    descriptor = _themeProvider.GetThemeDescriptorFromText(content);

                break;
            }
        }

        if (descriptor == null)
            throw new Exception("No descriptor file is found. It should be in the root of the archive.");

        if (string.IsNullOrEmpty(uploadedItemDirectoryName))
            throw new Exception($"Cannot get the {(descriptor is PluginDescriptor ? "plugin" : "theme")} directory name");

        //get path to upload
        var directoryPath = descriptor is PluginDescriptor ? pluginsDirectory : themesDirectory;
        var pathToUpload = _fileProvider.Combine(directoryPath, uploadedItemDirectoryName);

        //ensure it's a new directory (e.g. some old files are not required when re-uploading a plugin)
        //furthermore, zip extract functionality cannot override existing files
        //but there could deletion issues (related to file locking, etc). In such cases the directory should be deleted manually
        if (_fileProvider.DirectoryExists(pathToUpload))
            _fileProvider.DeleteDirectory(pathToUpload);

        //unzip archive
        ZipFile.ExtractToDirectory(archivePath, directoryPath);

        return descriptor;
    }

    /// <summary>
    /// Upload multiple items from the archive into the physical directory
    /// </summary>
    /// <param name="archivePath">Path to the archive</param>
    /// <param name="uploadedItems">Uploaded items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of item descriptors
    /// </returns>
    protected virtual async Task<IList<IDescriptor>> UploadMultipleItemsAsync(string archivePath, IList<UploadedItem> uploadedItems)
    {
        //get path to the plugins directory
        var pluginsDirectory = _fileProvider.MapPath(NopPluginDefaults.UploadedPath);

        //ensure plugins directory is created
        _fileProvider.CreateDirectory(pluginsDirectory);

        //get path to the themes directory
        var themesDirectory = string.Empty;
        if (!string.IsNullOrEmpty(NopPluginDefaults.ThemesPath))
            themesDirectory = _fileProvider.MapPath(NopPluginDefaults.ThemesPath);

        //get descriptors of items contained in the archive
        var descriptors = new List<IDescriptor>();
        using (var archive = ZipFile.OpenRead(archivePath))
        {
            foreach (var item in uploadedItems)
            {
                if (!item.Type.HasValue)
                    continue;

                //ensure that the current version of nopCommerce is supported
                if (!item.SupportedVersions?.Contains(NopVersion.CURRENT_VERSION) ?? true)
                    continue;

                //the item path should end with a slash
                var itemPath = $"{item.DirectoryPath?.TrimEnd('/')}/";

                //get path to the descriptor entry in the archive
                var descriptorPath = string.Empty;
                if (item.Type == UploadedItemType.Plugin)
                    descriptorPath = $"{itemPath}{NopPluginDefaults.DescriptionFileName}";

                if (item.Type == UploadedItemType.Theme && !string.IsNullOrEmpty(NopPluginDefaults.ThemeDescriptionFileName))
                    descriptorPath = $"{itemPath}{NopPluginDefaults.ThemeDescriptionFileName}";

                //try to get the descriptor entry
                var descriptorEntry = archive.Entries.FirstOrDefault(entry => entry.FullName.Equals(descriptorPath, StringComparison.InvariantCultureIgnoreCase));
                if (descriptorEntry == null)
                    continue;

                //try to get descriptor of the uploaded item
                IDescriptor descriptor = null;
                await using var unzippedEntryStream = descriptorEntry.Open();
                using var reader = new StreamReader(unzippedEntryStream);
                var content = await reader.ReadToEndAsync();

                //whether a plugin is upload 
                if (item.Type == UploadedItemType.Plugin)
                    descriptor = PluginDescriptor.GetPluginDescriptorFromText(content);

                //or whether a theme is upload 
                if (item.Type == UploadedItemType.Theme)
                    descriptor = _themeProvider.GetThemeDescriptorFromText(content);

                if (descriptor == null)
                    continue;

                //ensure that the plugin current version is supported
                if (descriptor is PluginDescriptor pluginDescriptor && !pluginDescriptor.SupportedVersions.Contains(NopVersion.CURRENT_VERSION))
                    continue;

                //get path to upload
                var uploadedItemDirectoryName = _fileProvider.GetFileName(itemPath.TrimEnd('/'));
                var pathToUpload = _fileProvider.Combine(item.Type == UploadedItemType.Plugin ? pluginsDirectory : themesDirectory, uploadedItemDirectoryName);

                //ensure it's a new directory (e.g. some old files are not required when re-uploading a plugin or a theme)
                //furthermore, zip extract functionality cannot override existing files
                //but there could deletion issues (related to file locking, etc). In such cases the directory should be deleted manually
                if (_fileProvider.DirectoryExists(pathToUpload))
                    _fileProvider.DeleteDirectory(pathToUpload);

                //unzip entries into files
                var entries = archive.Entries.Where(entry => entry.FullName.StartsWith(itemPath, StringComparison.InvariantCultureIgnoreCase));
                foreach (var entry in entries)
                {
                    //get name of the file
                    var fileName = entry.FullName[itemPath.Length..];
                    if (string.IsNullOrEmpty(fileName))
                        continue;

                    var filePath = _fileProvider.Combine(pathToUpload, fileName);

                    //if it's a folder, we need to create it
                    if (string.IsNullOrEmpty(entry.Name) && !_fileProvider.DirectoryExists(filePath))
                    {
                        _fileProvider.CreateDirectory(filePath);
                        continue;
                    }

                    var directoryPath = _fileProvider.GetDirectoryName(filePath);

                    //whether the file directory is already exists, otherwise create the new one
                    if (!_fileProvider.DirectoryExists(directoryPath))
                        _fileProvider.CreateDirectory(directoryPath);

                    //unzip entry to the file (ignore directory entries)
                    if (!filePath.Equals($"{directoryPath}\\", StringComparison.InvariantCultureIgnoreCase))
                        entry.ExtractToFile(filePath);
                }

                //item is uploaded
                descriptors.Add(descriptor);
            }
        }

        return descriptors;
    }

    /// <summary>
    /// Creates the directory if not exist; otherwise deletes and creates directory 
    /// </summary>
    /// <param name="path"></param>
    protected virtual void CreateDirectory(string path)
    {
        //if the folder does not exist, create it
        //if the folder is already there - we delete it (since the pictures in the folder are in the unpacked version, there will be many files and it is easier for us to delete the folder than to delete all the files one by one) and create a new
        if (!_fileProvider.DirectoryExists(path))
        {
            _fileProvider.CreateDirectory(path);
        }
        else
        {
            _fileProvider.DeleteDirectory(path);
            _fileProvider.CreateDirectory(path);
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Upload plugins and/or themes
    /// </summary>
    /// <param name="archivefile">Archive file</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of uploaded items descriptor
    /// </returns>
    public virtual async Task<IList<IDescriptor>> UploadPluginsAndThemesAsync(IFormFile archivefile)
    {
        ArgumentNullException.ThrowIfNull(archivefile);

        var zipFilePath = string.Empty;
        var descriptors = new List<IDescriptor>();
        try
        {
            //only zip archives are supported
            if (!_fileProvider.GetFileExtension(archivefile.FileName)?.Equals(".zip", StringComparison.InvariantCultureIgnoreCase) ?? true)
                throw new Exception("Only zip archives are supported");

            //ensure that temp directory is created
            var tempDirectory = _fileProvider.MapPath(NopPluginDefaults.UploadsTempPath);
            _fileProvider.CreateDirectory(tempDirectory);

            //copy original archive to the temp directory
            zipFilePath = _fileProvider.Combine(tempDirectory, archivefile.FileName);
            await using (var fileStream = new FileStream(zipFilePath, FileMode.Create))
                await archivefile.CopyToAsync(fileStream);

            //try to get information about the uploaded items from the JSON file in the root of the archive
            //you can find a sample of such descriptive file in Libraries\Nop.Core\Plugins\Samples\
            var uploadedItems = await GetUploadedItemsAsync(zipFilePath);
            if (!uploadedItems?.Any() ?? true)
            {
                //JSON file doesn't exist, so there is a single plugin or theme in the archive, just unzip it
                descriptors.Add(await UploadSingleItemAsync(zipFilePath));
            }
            else
                descriptors.AddRange(await UploadMultipleItemsAsync(zipFilePath, uploadedItems));
        }
        finally
        {
            //delete temporary file
            if (!string.IsNullOrEmpty(zipFilePath))
                _fileProvider.DeleteFile(zipFilePath);
        }

        return descriptors;
    }

    /// <summary>
    /// Upload favicon and app icons
    /// </summary>
    /// <param name="archivefile">Archive file which contains a set of special icons for different OS and devices</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UploadIconsArchiveAsync(IFormFile archivefile)
    {
        ArgumentNullException.ThrowIfNull(archivefile);

        var zipFilePath = string.Empty;
        try
        {
            //only zip archives are supported
            if (!_fileProvider.GetFileExtension(archivefile.FileName)?.Equals(".zip", StringComparison.InvariantCultureIgnoreCase) ?? true)
                throw new Exception("Only zip archives are supported (*.zip)");

            //check if there is a folder for favicon and app icons for the current store (all store icons folders are in wwwroot/icons and are called icons_{storeId})
            var storeIconsPath = _fileProvider.GetAbsolutePath(string.Format(NopCommonDefaults.FaviconAndAppIconsPath, await _storeContext.GetActiveStoreScopeConfigurationAsync()));

            CreateDirectory(storeIconsPath);

            zipFilePath = _fileProvider.Combine(storeIconsPath, archivefile.FileName);
            await using (var fileStream = new FileStream(zipFilePath, FileMode.Create))
                await archivefile.CopyToAsync(fileStream);

            ZipFile.ExtractToDirectory(zipFilePath, storeIconsPath);
        }
        finally
        {
            //delete the zip file and leave only unpacked files in the folder
            if (!string.IsNullOrEmpty(zipFilePath))
                _fileProvider.DeleteFile(zipFilePath);
        }
    }

    /// <summary>
    /// Upload single favicon
    /// </summary>
    /// <param name="favicon">Favicon</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UploadFaviconAsync(IFormFile favicon)
    {
        ArgumentNullException.ThrowIfNull(favicon);

        //only icons are supported
        if (!_fileProvider.GetFileExtension(favicon.FileName)?.Equals(".ico", StringComparison.InvariantCultureIgnoreCase) ?? true)
            throw new Exception("Only icons are supported (*.ico)");

        //check if there is a folder for favicon (favicon folder is in wwwroot/icons and is called icons_{storeId})
        var storeFaviconPath = _fileProvider.GetAbsolutePath(string.Format(NopCommonDefaults.FaviconAndAppIconsPath, await _storeContext.GetActiveStoreScopeConfigurationAsync()));

        CreateDirectory(storeFaviconPath);

        var faviconPath = _fileProvider.Combine(storeFaviconPath, favicon.FileName);
        await using var fileStream = new FileStream(faviconPath, FileMode.Create);
        await favicon.CopyToAsync(fileStream);
    }

    /// <summary>
    /// Upload locale pattern for current culture
    /// </summary>
    /// <param name="cultureInfo">CultureInfo</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task UploadLocalePatternAsync(CultureInfo cultureInfo = null)
    {
        string getPath(string dirPath, string dirName)
        {
            return _fileProvider.GetAbsolutePath(string.Format(dirPath, dirName));
        }

        bool checkDirectoryExists(string dirPath, string dirName)
        {
            return _fileProvider.DirectoryExists(getPath(dirPath, dirName));
        }

        var tempFolder = "temp";
        try
        {
            //1. check if the archive with localization of templates is in its place
            var ziplocalePatternPath = getPath(NopCommonDefaults.LocalePatternPath, NopCommonDefaults.LocalePatternArchiveName);
            if (!_fileProvider.GetFileExtension(ziplocalePatternPath)?.Equals(".zip", StringComparison.InvariantCultureIgnoreCase) ?? true)
                throw new Exception($"Archive '{NopCommonDefaults.LocalePatternArchiveName}' to retrieve localization patterns not found.");

            var currentCulture = (cultureInfo is not null) ? cultureInfo : CultureInfo.CurrentCulture;

            //2. Check if there is already an unpacked folder with locales for the current culture in the lib directory, if not then
            if (!(checkDirectoryExists(NopCommonDefaults.LocalePatternPath, currentCulture.Name) ||
                  checkDirectoryExists(NopCommonDefaults.LocalePatternPath, currentCulture.TwoLetterISOLanguageName)))
            {
                var cultureToUse = string.Empty;

                //3. Unpack the archive into a temporary folder
                ZipFile.ExtractToDirectory(ziplocalePatternPath, getPath(NopCommonDefaults.LocalePatternPath, tempFolder));

                //4. Search in the temp unpacked archive a folder with locales by culture
                var sourcelocalePath = _fileProvider.Combine(getPath(NopCommonDefaults.LocalePatternPath, tempFolder), currentCulture.Name);
                if (_fileProvider.DirectoryExists(sourcelocalePath))
                {
                    cultureToUse = currentCulture.Name;
                }

                var sourcelocaleISOPath = _fileProvider.Combine(getPath(NopCommonDefaults.LocalePatternPath, tempFolder), currentCulture.TwoLetterISOLanguageName);
                if (_fileProvider.DirectoryExists(sourcelocaleISOPath))
                {
                    cultureToUse = currentCulture.TwoLetterISOLanguageName;
                    sourcelocalePath = sourcelocaleISOPath;
                }

                //5. Copy locales to destination folder
                if (!string.IsNullOrEmpty(cultureToUse))
                {
                    var destlocalePath = getPath(NopCommonDefaults.LocalePatternPath, cultureToUse);
                    if (_fileProvider.DirectoryExists(destlocalePath))
                    {
                        _fileProvider.DeleteDirectory(destlocalePath);
                    }

                    _fileProvider.DirectoryMove(sourcelocalePath, destlocalePath);
                }
            }
        }
        finally
        {
            //6. delete the zip file and leave only unpacked files in the folder
            if (checkDirectoryExists(NopCommonDefaults.LocalePatternPath, tempFolder))
                _fileProvider.DeleteDirectory(getPath(NopCommonDefaults.LocalePatternPath, tempFolder));
        }

        return Task.CompletedTask;
    }

    #endregion

    #region Nested classes

    /// <summary>
    /// Represents uploaded item (plugin or theme) details 
    /// </summary>
    public partial class UploadedItem
    {
        /// <summary>
        /// Gets or sets the type of an uploaded item
        /// </summary>
        [JsonProperty(PropertyName = "Type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public UploadedItemType? Type { get; set; }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        [JsonProperty(PropertyName = "SystemName")]
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets supported versions of nopCommerce
        /// </summary>
        [JsonProperty(PropertyName = "SupportedVersion")]
        public string SupportedVersions { get; set; }

        /// <summary>
        /// Gets or sets the path to binary files directory
        /// </summary>
        [JsonProperty(PropertyName = "DirectoryPath")]
        public string DirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the path to source files directory
        /// </summary>
        [JsonProperty(PropertyName = "SourceDirectoryPath")]
        public string SourceDirectoryPath { get; set; }
    }

    /// <summary>
    /// Uploaded item type enumeration
    /// </summary>
    public enum UploadedItemType
    {
        /// <summary>
        /// Plugin
        /// </summary>
        [EnumMember(Value = "Plugin")]
        Plugin,

        /// <summary>
        /// Theme
        /// </summary>
        [EnumMember(Value = "Theme")]
        Theme
    }

    #endregion
}