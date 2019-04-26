using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Infrastructure;

namespace Nop.Services.Media.RoxyFileman
{
    public abstract partial class BaseRoxyFilemanService
    {
        #region Fields

        private Dictionary<string, string> _settings;
        private Dictionary<string, string> _languageResources;

        protected readonly IHostingEnvironment _hostingEnvironment;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly INopFileProvider _fileProvider;

        #endregion

        #region Ctor

        protected BaseRoxyFilemanService(IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor,
            INopFileProvider fileProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _fileProvider = fileProvider;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether there are any restrictions on handling the file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>True if the file can be handled; otherwise false</returns>
        protected virtual bool CanHandleFile(string path)
        {
            var result = false;

            var fileExtension = _fileProvider.GetFileExtension(path).Replace(".", string.Empty).ToLower();

            var forbiddenUploads = GetSetting("FORBIDDEN_UPLOADS").Trim().ToLower();
            if (!string.IsNullOrEmpty(forbiddenUploads))
            {
                var forbiddenFileExtensions = new ArrayList(Regex.Split(forbiddenUploads, "\\s+"));
                result = !forbiddenFileExtensions.Contains(fileExtension);
            }

            var allowedUploads = GetSetting("ALLOWED_UPLOADS").Trim().ToLower();
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
        /// <returns>Array of the paths to the directories</returns>
        protected virtual ArrayList GetDirectories(string parentDirectoryPath)
        {
            var directories = new ArrayList();

            var directoryNames = _fileProvider.GetDirectories(parentDirectoryPath);
            foreach (var directory in directoryNames)
            {
                directories.Add(directory);
                directories.AddRange(GetDirectories(directory));
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

            fileExtension = fileExtension.ToLower();
            if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                fileType = "image";

            if (fileExtension == ".swf" || fileExtension == ".flv")
                fileType = "flash";

            return fileType;
        }

        /// <summary>
        /// Get the absolute path by virtual path
        /// </summary>
        /// <param name="virtualPath">Virtual path</param>
        /// <returns>Path</returns>
        protected virtual string GetFullPath(string virtualPath)
        {
            virtualPath = virtualPath ?? string.Empty;
            if (!virtualPath.StartsWith("/"))
                virtualPath = "/" + virtualPath;
            virtualPath = virtualPath.TrimEnd('/');
            virtualPath = virtualPath.Replace('/', '\\');

            return _hostingEnvironment.WebRootPath + virtualPath;
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
        /// <returns>Path</returns>
        protected virtual string GetLanguageFile()
        {
            var languageCode = GetSetting("LANG");
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
        /// <returns>Path</returns>
        protected virtual string GetRootDirectory()
        {
            var filesRoot = GetSetting("FILES_ROOT");

            var sessionPathKey = GetSetting("SESSION_PATH_KEY");
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
        /// <returns>Setting value</returns>
        protected virtual string GetSetting(string key)
        {
            if (_settings == null)
                _settings = ParseJson(GetFullPath(NopRoxyFilemanDefaults.ConfigurationFile));

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
        /// <returns>Path</returns>
        protected virtual string GetVirtualPath(string path)
        {
            path = path ?? string.Empty;

            var rootDirectory = GetRootDirectory();
            if (!path.StartsWith(rootDirectory))
                path = rootDirectory + path;

            return path;
        }

        /// <summary>
        /// Parse the JSON file
        /// </summary>
        /// <param name="file">Path to the file</param>
        /// <returns>Collection of keys and values from the parsed file</returns>
        protected virtual Dictionary<string, string> ParseJson(string file)
        {
            var result = new Dictionary<string, string>();
            var json = string.Empty;
            try
            {
                json = _fileProvider.ReadAllText(file, Encoding.UTF8)?.Trim();
            }
            catch
            {
                //ignore any exception
            }

            if (string.IsNullOrEmpty(json))
                return result;

            if (json.StartsWith("{"))
                json = json.Substring(1, json.Length - 2);

            json = json.Trim();
            json = json.Substring(1, json.Length - 2);

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
        /// <returns>Language resource value</returns>
        public virtual string GetLanguageResource(string key)
        {
            if (_languageResources == null)
                _languageResources = ParseJson(GetLanguageFile());

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
