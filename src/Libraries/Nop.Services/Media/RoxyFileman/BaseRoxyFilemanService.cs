using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;

namespace Nop.Services.Media.RoxyFileman
{
    public abstract partial class BaseRoxyFilemanService
    {
        #region Fields

        private Dictionary<string, string> _settings;
        private Dictionary<string, string> _languageResources;

        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly INopFileProvider _fileProvider;
        protected readonly IWebHelper _webHelper;
        protected readonly IWorkContext _workContext;
        protected readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        protected BaseRoxyFilemanService(IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            INopFileProvider fileProvider,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _fileProvider = fileProvider;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether there are any restrictions on handling the file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if the file can be handled; otherwise false
        /// </returns>
        protected virtual async Task<bool> CanHandleFileAsync(string path)
        {
            var result = false;

            var fileExtension = _fileProvider.GetFileExtension(path).Replace(".", string.Empty).ToLowerInvariant();

            var forbiddenUploads = (await GetSettingAsync("FORBIDDEN_UPLOADS")).Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(forbiddenUploads))
            {
                var forbiddenFileExtensions = new ArrayList(Regex.Split(forbiddenUploads, "\\s+"));
                result = !forbiddenFileExtensions.Contains(fileExtension);
            }

            var allowedUploads = (await GetSettingAsync("ALLOWED_UPLOADS")).Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(allowedUploads))
                return result;

            var allowedFileExtensions = new ArrayList(Regex.Split(allowedUploads, "\\s+"));
            result = allowedFileExtensions.Contains(fileExtension);

            return result;
        }

        /// <summary>
        /// Get directories in the passed parent directory
        /// </summary>
        /// <param name="parentDirectoryPath">Path to the parent directory</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the array of the paths to the directories
        /// </returns>
        protected virtual async Task<ArrayList> GetDirectoriesByParentDirectoryAsync(string parentDirectoryPath)
        {
            var directories = new ArrayList();

            var directoryNames = _fileProvider.GetDirectories(parentDirectoryPath);
            foreach (var directory in directoryNames)
            {
                directories.Add(directory);
                directories.AddRange(await GetDirectoriesByParentDirectoryAsync(directory));
            }

            return directories;
        }

        /// <summary>
        /// Get a file type by file extension
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>File type</returns>
        protected virtual string GetFileType(string fileExtension)
        {
            var fileType = "file";

            fileExtension = fileExtension.ToLowerInvariant();
            if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".webp")
                fileType = "image";

            if (fileExtension == ".swf" || fileExtension == ".flv")
                fileType = "flash";

            // Media file types supported by HTML5
            if (fileExtension == ".mp4" || fileExtension == ".webm" // video
                || fileExtension == ".ogg") // audio
                fileType = "media";

            // These media extensions are supported by tinyMCE
            if (fileExtension == ".mov" // video
                || fileExtension == ".m4a" || fileExtension == ".mp3" || fileExtension == ".wav") // audio
                fileType = "media";

            /* These media extensions are not supported by HTML5 or tinyMCE out of the box
             * but may possibly be supported if You find players for them.
             * if (fileExtension == ".3gp" || fileExtension == ".flv" 
             *     || fileExtension == ".rmvb" || fileExtension == ".wmv" || fileExtension == ".divx"
             *     || fileExtension == ".divx" || fileExtension == ".mpg" || fileExtension == ".rmvb"
             *     || fileExtension == ".vob" // video
             *     || fileExtension == ".aif" || fileExtension == ".aiff" || fileExtension == ".amr"
             *     || fileExtension == ".asf" || fileExtension == ".asx" || fileExtension == ".wma"
             *     || fileExtension == ".mid" || fileExtension == ".mp2") // audio
             *     fileType = "media"; */

             return fileType;
        }

        /// <summary>
        /// Get the absolute path by virtual path
        /// </summary>
        /// <param name="virtualPath">Virtual path</param>
        /// <returns>Path</returns>
        protected virtual string GetFullPath(string virtualPath)
        {
            virtualPath ??= string.Empty;
            if (!virtualPath.StartsWith("/"))
                virtualPath = "/" + virtualPath;
            virtualPath = virtualPath.TrimEnd('/');

            return _fileProvider.Combine(_webHostEnvironment.WebRootPath, virtualPath);
        }

        /// <summary>
        /// Get the http context
        /// </summary>
        /// <returns>Http context</returns>
        protected virtual HttpContext GetHttpContext()
        {
            return _httpContextAccessor.HttpContext;
        }

        /// <summary>
        /// Get the absolute path to the language resources file
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the path
        /// </returns>
        protected virtual async Task<string> GetLanguageFileAsync()
        {
            var languageCode = await GetSettingAsync("LANG");
            var languageFile = $"{NopRoxyFilemanDefaults.LanguageDirectory}/{languageCode}.json";

            if (!_fileProvider.FileExists(GetFullPath(languageFile)))
                languageFile = $"{NopRoxyFilemanDefaults.LanguageDirectory}/en.json";

            return GetFullPath(languageFile);
        }

        /// <summary>
        /// Get the string to write to the response
        /// </summary>
        /// <param name="type">Type of the response</param>
        /// <param name="message">Additional message</param>
        /// <returns>String to write to the response</returns>
        protected virtual string GetResponse(string type, string message)
        {
            return $"{{\"res\":\"{type}\",\"msg\":\"{message?.Replace("\"", "\\\"")}\"}}";
        }

        /// <summary>
        /// Get the virtual path to root directory of uploaded files 
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the path
        /// </returns>
        protected virtual async Task<string> GetRootDirectoryAsync()
        {
            var filesRoot = await GetSettingAsync("FILES_ROOT");

            var sessionPathKey = await GetSettingAsync("SESSION_PATH_KEY");
            if (!string.IsNullOrEmpty(sessionPathKey))
                filesRoot = GetHttpContext().Session.GetString(sessionPathKey);

            if (string.IsNullOrEmpty(filesRoot))
                filesRoot = NopRoxyFilemanDefaults.DefaultRootDirectory;

            return filesRoot;
        }

        /// <summary>
        /// Get a value of the configuration setting
        /// </summary>
        /// <param name="key">Setting key</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting value
        /// </returns>
        protected virtual async Task<string> GetSettingAsync(string key)
        {
            if (_settings == null)
                _settings = await ParseJsonAsync(GetFullPath(NopRoxyFilemanDefaults.ConfigurationFile));

            if (_settings.TryGetValue(key, out var value))
                return value;

            return null;
        }

        /// <summary>
        /// Get the string to write a success response
        /// </summary>
        /// <param name="message">Additional message</param>
        /// <returns>String to write to the response</returns>
        protected virtual string GetSuccessResponse(string message = null)
        {
            return GetResponse("ok", message);
        }

        /// <summary>
        /// Get the unique name of the file (add -copy-(N) to the file name if there is already a file with that name in the directory)
        /// </summary>
        /// <param name="directoryPath">Path to the file directory</param>
        /// <param name="fileName">Original file name</param>
        /// <returns>Unique name of the file</returns>
        protected virtual string GetUniqueFileName(string directoryPath, string fileName)
        {
            var uniqueFileName = fileName;

            var i = 0;
            while (_fileProvider.FileExists(_fileProvider.Combine(directoryPath, uniqueFileName)))
            {
                uniqueFileName = $"{_fileProvider.GetFileNameWithoutExtension(fileName)}-Copy-{++i}{_fileProvider.GetFileExtension(fileName)}";
            }

            return uniqueFileName;
        }

        /// <summary>
        /// Get a virtual path with the root directory
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the path
        /// </returns>
        protected virtual async Task<string> GetVirtualPathAsync(string path)
        {
            path ??= string.Empty;

            var rootDirectory = await GetRootDirectoryAsync();
            if (!path.StartsWith(rootDirectory))
                path = rootDirectory + path;

            return path;
        }

        /// <summary>
        /// Parse the JSON file
        /// </summary>
        /// <param name="file">Path to the file</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of keys and values from the parsed file
        /// </returns>
        protected virtual async Task<Dictionary<string, string>> ParseJsonAsync(string file)
        {
            var result = new Dictionary<string, string>();
            var json = string.Empty;
            try
            {
                json = (await _fileProvider.ReadAllTextAsync(file, Encoding.UTF8))?.Trim();
            }
            catch
            {
                //ignore any exception
            }

            if (string.IsNullOrEmpty(json))
                return result;

            if (json.StartsWith("{"))
                json = json[1..^1];

            json = json.Trim();
            json = json[1..^1];

            var lines = Regex.Split(json, "\"\\s*,\\s*\"");
            foreach (var line in lines)
            {
                var tmp = Regex.Split(line, "\"\\s*:\\s*\"");
                try
                {
                    if (!string.IsNullOrEmpty(tmp[0]) && !result.ContainsKey(tmp[0]))
                        result.Add(tmp[0], tmp[1]);
                }
                catch
                {
                    //ignore any exception
                }
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration file path
        /// </summary>
        public virtual string GetConfigurationFilePath()
        {
            return GetFullPath(NopRoxyFilemanDefaults.ConfigurationFile);
        }

        /// <summary>
        /// Create configuration file for RoxyFileman
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task CreateConfigurationAsync()
        {
            var filePath = GetConfigurationFilePath();

            //create file if not exists
            _fileProvider.CreateFile(filePath);

            //try to read existing configuration
            var existingText = await _fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);
            var existingConfiguration = JsonConvert.DeserializeAnonymousType(existingText, new
            {
                FILES_ROOT = string.Empty,
                SESSION_PATH_KEY = string.Empty,
                THUMBS_VIEW_WIDTH = string.Empty,
                THUMBS_VIEW_HEIGHT = string.Empty,
                PREVIEW_THUMB_WIDTH = string.Empty,
                PREVIEW_THUMB_HEIGHT = string.Empty,
                MAX_IMAGE_WIDTH = string.Empty,
                MAX_IMAGE_HEIGHT = string.Empty,
                DEFAULTVIEW = string.Empty,
                FORBIDDEN_UPLOADS = string.Empty,
                ALLOWED_UPLOADS = string.Empty,
                FILEPERMISSIONS = string.Empty,
                DIRPERMISSIONS = string.Empty,
                LANG = string.Empty,
                DATEFORMAT = string.Empty,
                OPEN_LAST_DIR = string.Empty,
                INTEGRATION = string.Empty,
                RETURN_URL_PREFIX = string.Empty,
                DIRLIST = string.Empty,
                CREATEDIR = string.Empty,
                DELETEDIR = string.Empty,
                MOVEDIR = string.Empty,
                COPYDIR = string.Empty,
                RENAMEDIR = string.Empty,
                FILESLIST = string.Empty,
                UPLOAD = string.Empty,
                DOWNLOAD = string.Empty,
                DOWNLOADDIR = string.Empty,
                DELETEFILE = string.Empty,
                MOVEFILE = string.Empty,
                COPYFILE = string.Empty,
                RENAMEFILE = string.Empty,
                GENERATETHUMB = string.Empty
            });

            //check whether the path base has changed, otherwise there is no need to overwrite the configuration file
            var currentPathBase = _httpContextAccessor.HttpContext.Request.PathBase.ToString();
            if (existingConfiguration?.RETURN_URL_PREFIX?.Equals(currentPathBase) ?? false)
                return;

            //create configuration
            var configuration = new
            {
                FILES_ROOT = existingConfiguration?.FILES_ROOT ?? NopRoxyFilemanDefaults.DefaultRootDirectory,
                SESSION_PATH_KEY = existingConfiguration?.SESSION_PATH_KEY ?? string.Empty,
                THUMBS_VIEW_WIDTH = existingConfiguration?.THUMBS_VIEW_WIDTH ?? "140",
                THUMBS_VIEW_HEIGHT = existingConfiguration?.THUMBS_VIEW_HEIGHT ?? "120",
                PREVIEW_THUMB_WIDTH = existingConfiguration?.PREVIEW_THUMB_WIDTH ?? "300",
                PREVIEW_THUMB_HEIGHT = existingConfiguration?.PREVIEW_THUMB_HEIGHT ?? "200",
                MAX_IMAGE_WIDTH = existingConfiguration?.MAX_IMAGE_WIDTH ?? _mediaSettings.MaximumImageSize.ToString(),
                MAX_IMAGE_HEIGHT = existingConfiguration?.MAX_IMAGE_HEIGHT ?? _mediaSettings.MaximumImageSize.ToString(),
                DEFAULTVIEW = existingConfiguration?.DEFAULTVIEW ?? "list",
                FORBIDDEN_UPLOADS = existingConfiguration?.FORBIDDEN_UPLOADS ?? "zip js jsp jsb mhtml mht xhtml xht php phtml " +
                    "php3 php4 php5 phps shtml jhtml pl sh py cgi exe application gadget hta cpl msc jar vb jse ws wsf wsc wsh " +
                    "ps1 ps2 psc1 psc2 msh msh1 msh2 inf reg scf msp scr dll msi vbs bat com pif cmd vxd cpl htpasswd htaccess",
                ALLOWED_UPLOADS = existingConfiguration?.ALLOWED_UPLOADS ?? string.Empty,
                FILEPERMISSIONS = existingConfiguration?.FILEPERMISSIONS ?? "0644",
                DIRPERMISSIONS = existingConfiguration?.DIRPERMISSIONS ?? "0755",
                LANG = existingConfiguration?.LANG ?? (await _workContext.GetWorkingLanguageAsync()).UniqueSeoCode,
                DATEFORMAT = existingConfiguration?.DATEFORMAT ?? "dd/MM/yyyy HH:mm",
                OPEN_LAST_DIR = existingConfiguration?.OPEN_LAST_DIR ?? "yes",

                //no need user to configure
                INTEGRATION = "custom",
                RETURN_URL_PREFIX = currentPathBase,
                DIRLIST = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=DIRLIST",
                CREATEDIR = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=CREATEDIR",
                DELETEDIR = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=DELETEDIR",
                MOVEDIR = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=MOVEDIR",
                COPYDIR = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=COPYDIR",
                RENAMEDIR = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=RENAMEDIR",
                FILESLIST = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=FILESLIST",
                UPLOAD = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=UPLOAD",
                DOWNLOAD = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=DOWNLOAD",
                DOWNLOADDIR = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=DOWNLOADDIR",
                DELETEFILE = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=DELETEFILE",
                MOVEFILE = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=MOVEFILE",
                COPYFILE = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=COPYFILE",
                RENAMEFILE = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=RENAMEFILE",
                GENERATETHUMB = $"{currentPathBase}/Admin/RoxyFileman/ProcessRequest?a=GENERATETHUMB"
            };

            //save the file
            var text = JsonConvert.SerializeObject(configuration, Formatting.Indented);
            await _fileProvider.WriteAllTextAsync(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// Get the string to write an error response
        /// </summary>
        /// <param name="message">Additional message</param>
        /// <returns>String to write to the response</returns>
        public virtual string GetErrorResponse(string message = null)
        {
            return GetResponse("error", message);
        }

        /// <summary>
        /// Get the language resource value
        /// </summary>
        /// <param name="key">Language resource key</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the language resource value
        /// </returns>
        public virtual async Task<string> GetLanguageResourceAsync(string key)
        {
            if (_languageResources == null)
                _languageResources = await ParseJsonAsync(await GetLanguageFileAsync());

            if (_languageResources.TryGetValue(key, out var value))
                return value;

            return key;
        }

        /// <summary>
        /// Whether the request is made with ajax 
        /// </summary>
        /// <returns>True or false</returns>
        public virtual bool IsAjaxRequest()
        {
            return GetHttpContext().Request.Form != null &&
                   !StringValues.IsNullOrEmpty(GetHttpContext().Request.Form["method"]) &&
                   GetHttpContext().Request.Form["method"] == "ajax";
        }

        #endregion
    }
}
