using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Framework.Security;

namespace Nop.Admin.Controllers
{
    //Controller for Roxy fileman (http://www.roxyfileman.com/) for TinyMCE editor
    //the original file was \RoxyFileman-1.4.5-net\fileman\asp_net\main.ashx

    //do not validate request token (XSRF)
    [AdminAntiForgery(true)]
    public class RoxyFilemanController : BaseAdminController
    {
        #region Constants
        
        private const string DEFAULT_ROOT_DIRECTORY = "/images/uploaded";
        private const string LANGUAGE_DIRECTORY = "/lib/Roxy_Fileman/lang";
        private const string CONFIGURATION_FILE = "/lib/Roxy_Fileman/conf.json";

        #endregion

        #region Fields

        private Dictionary<string, string> _settings;
        private Dictionary<string, string> _languageResources;

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public RoxyFilemanController(IHostingEnvironment hostingEnvironment,
            IPermissionService permissionService)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._permissionService = permissionService;
        }

        #endregion

        #region Methods

        public virtual void ProcessRequest()
        {
            var action = "DIRLIST";
            try
            {
                if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                    throw new Exception("You don't have required permission");

                if (!StringValues.IsNullOrEmpty(this.HttpContext.Request.Query["a"]))
                    action = this.HttpContext.Request.Query["a"];
                
                switch (action.ToUpper())
                {
                    case "DIRLIST":
                        CreateDirectoryTree(this.HttpContext.Request.Query["type"]);
                        break;
                    case "FILESLIST":
                        CreateFileList(this.HttpContext.Request.Query["d"], this.HttpContext.Request.Query["type"]);
                        break;
                    case "COPYDIR":
                        CopyDirectory(this.HttpContext.Request.Query["d"], this.HttpContext.Request.Query["n"]);
                        break;
                    case "COPYFILE":
                        CopyFile(this.HttpContext.Request.Query["f"], this.HttpContext.Request.Query["n"]);
                        break;
                    case "CREATEDIR":
                        CreateDirectory(this.HttpContext.Request.Query["d"], this.HttpContext.Request.Query["n"]);
                        break;
                    case "DELETEDIR":
                        DeleteDirectory(this.HttpContext.Request.Query["d"]);
                        break;
                    case "DELETEFILE":
                        DeleteFile(this.HttpContext.Request.Query["f"]);
                        break;
                    case "DOWNLOAD":
                        DownloadFile(this.HttpContext.Request.Query["f"]);
                        break;
                    case "DOWNLOADDIR":
                        DownloadDirectory(this.HttpContext.Request.Query["d"]);
                        break;
                    case "MOVEDIR":
                        MoveDirectory(this.HttpContext.Request.Query["d"], this.HttpContext.Request.Query["n"]);
                        break;
                    case "MOVEFILE":
                        MoveFile(this.HttpContext.Request.Query["f"], this.HttpContext.Request.Query["n"]);
                        break;
                    case "RENAMEDIR":
                        RenameDirectory(this.HttpContext.Request.Query["d"], this.HttpContext.Request.Query["n"]);
                        break;
                    case "RENAMEFILE":
                        RenameFile(this.HttpContext.Request.Query["f"], this.HttpContext.Request.Query["n"]);
                        break;
                    case "GENERATETHUMB":
                        int w = 140, h = 0;
                        int.TryParse(this.HttpContext.Request.Query["width"].ToString().Replace("px", ""), out w);
                        int.TryParse(this.HttpContext.Request.Query["height"].ToString().Replace("px", ""), out h);
                        CreateThumbnail(this.HttpContext.Request.Query["f"], w, h);
                        break;
                    case "UPLOAD":
                        UploadFiles(this.HttpContext.Request.Form?["d"]);
                        break;
                    default:
                        this.HttpContext.Response.WriteAsync(GetErrorResponse("This action is not implemented."));
                        break;
                }

            }
            catch (Exception ex)
            {
                if (action == "UPLOAD" && !IsAjaxUpload())
                    this.HttpContext.Response.WriteAsync($"<script>parent.fileUploaded({GetErrorResponse(GetLanguageResource("E_UploadNoFiles"))});</script>");
                else
                    this.HttpContext.Response.WriteAsync(GetErrorResponse(ex.Message));
            }
        }

        #endregion

        #region Utitlies

        protected virtual string GetRootDirectory()
        {
            var filesRoot = GetSetting("FILES_ROOT");

            var sessionPathKey = GetSetting("SESSION_PATH_KEY");
            if (!string.IsNullOrEmpty(sessionPathKey))
                filesRoot = this.HttpContext.Session.GetString(sessionPathKey);

            if (string.IsNullOrEmpty(filesRoot))
                filesRoot = DEFAULT_ROOT_DIRECTORY;

            return filesRoot;
        }

        protected virtual string GetFullPath(string path)
        {
            if (!path.StartsWith("/"))
                path = "/" + path;
            path = path.TrimEnd('/');
            path = path.Replace('/', '\\');

            return _hostingEnvironment.WebRootPath + path;
        }

        protected virtual string GetVirtualPath(string path)
        {
            path = path ?? string.Empty;

            var rootDirectory = GetRootDirectory();
            if (!path.StartsWith(rootDirectory))
                path = rootDirectory + path;

            return path;
        }

        protected virtual string GetSetting(string key)
        {
            if (_settings == null)
                _settings = ParseJson(GetFullPath(CONFIGURATION_FILE));

            if (_settings.TryGetValue(key, out string value))
                return value;

            return null;
        }

        protected virtual string GetLanguageResource(string key)
        {
            if (_languageResources == null)
                _languageResources = ParseJson(GetLanguageFile());

            if (_languageResources.TryGetValue(key, out string value))
                return value;

            return key;
        }

        protected virtual string GetLanguageFile()
        {
            var languageCode = GetSetting("LANG");
            var languageFile = $"{LANGUAGE_DIRECTORY}/{languageCode}.json";

            if (!System.IO.File.Exists(GetFullPath(languageFile)))
                languageFile = $"{LANGUAGE_DIRECTORY}/en.json";

            return GetFullPath(languageFile);
        }

        protected virtual Dictionary<string, string> ParseJson(string file)
        {
            var result = new Dictionary<string, string>();
            var json = string.Empty;
            try
            {
                json = System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8)?.Trim();
            }
            catch { }
            
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
                catch { }
            }

            return result;
        }

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

        protected virtual bool CanHandleFile(string filename)
        {
            var result = false;
            var fileExtension = new FileInfo(filename).Extension.Replace(".", "").ToLower();

            var forbiddenUploads = GetSetting("FORBIDDEN_UPLOADS").Trim().ToLower();
            if (!string.IsNullOrEmpty(forbiddenUploads))
            {
                var forbiddenFileExtensions = new ArrayList(Regex.Split(forbiddenUploads, "\\s+"));
                result = !forbiddenFileExtensions.Contains(fileExtension);
            }

            var allowedUploads = GetSetting("ALLOWED_UPLOADS").Trim().ToLower();
            if (!string.IsNullOrEmpty(allowedUploads))
            {
                var allowedFileExtensions = new ArrayList(Regex.Split(allowedUploads, "\\s+"));
                result = allowedFileExtensions.Contains(fileExtension);
            }

            return result;
        }

        protected virtual string GetResponse(string type, string message)
        {
            message = message ?? string.Empty;
            return $"{{\"res\":\"{type}\",\"msg\":\"{message.Replace("\"", "\\\"")}\"}}";
        }

        protected virtual string GetSuccessResponse(string message = null)
        {
            return GetResponse("ok", message);
        }
        
        protected virtual string GetErrorResponse(string message = null)
        {
            return GetResponse("error", message);
        }

        protected virtual void CreateDirectoryTree(string type)
        {
            var rootDirectoryPath = GetFullPath(GetVirtualPath(null));
            var rootDirectory = new DirectoryInfo(rootDirectoryPath);
            if (!rootDirectory.Exists)
                throw new Exception("Invalid files root directory. Check your configuration.");

            var allDirectories = GetDirectories(rootDirectory.FullName);
            allDirectories.Insert(0, rootDirectory.FullName);

            this.HttpContext.Response.WriteAsync("[");
            for (var i = 0; i < allDirectories.Count; i++)
            {
                var directoryPath = (string)allDirectories[i];
                this.HttpContext.Response.WriteAsync($"{{\"p\":\"/{directoryPath.Replace(rootDirectoryPath, string.Empty).Replace("\\", "/").TrimStart('/')}\",\"f\":\"{GetFiles(directoryPath, type).Count}\",\"d\":\"{Directory.GetDirectories(directoryPath).Length}\"}}");
                if (i < allDirectories.Count - 1)
                    this.HttpContext.Response.WriteAsync(",");
            }
            this.HttpContext.Response.WriteAsync("]");
        }

        protected virtual ArrayList GetDirectories(string path)
        {
            var directories = new ArrayList();

            var directoryNames = Directory.GetDirectories(path);
            foreach (var directory in directoryNames)
            {
                directories.Add(directory);
                directories.AddRange(GetDirectories(directory));
            }

            return directories;
        }

        protected virtual void CreateFileList(string path, string type)
        {
            path = GetVirtualPath(path);
            var files = GetFiles(GetFullPath(path), type);

            this.HttpContext.Response.WriteAsync("[");
            for (var i = 0; i < files.Count; i++)
            {
                var width = 0;
                var height = 0;
                var file = new FileInfo(files[i]);
                if (GetFileType(file.Extension) == "image")
                {
                    try
                    {
                        using (var stream = new FileStream(file.FullName, FileMode.Open))
                        {
                            using (var image = Image.FromStream(stream))
                            {
                                width = image.Width;
                                height = image.Height;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                this.HttpContext.Response.WriteAsync($"{{\"p\":\"{path.TrimEnd('/')}/{file.Name}\",\"t\":\"{Math.Ceiling(LinuxTimestamp(file.LastWriteTime))}\",\"s\":\"{file.Length}\",\"w\":\"{width}\",\"h\":\"{height}\"}}");

                if (i < files.Count - 1)
                    this.HttpContext.Response.WriteAsync(",");
            }
            this.HttpContext.Response.WriteAsync("]");
        }

        protected virtual List<string> GetFiles(string path, string type)
        {
            if (type == "#")
                type = string.Empty;

            var files = new List<string>();
            foreach (var fileName in Directory.GetFiles(path))
            {
                if (string.IsNullOrEmpty(type) || GetFileType(new FileInfo(fileName).Extension) == type)
                    files.Add(fileName);
            }

            return files;
        }

        protected virtual double LinuxTimestamp(DateTime date)
        {
            return (date.ToLocalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime()).TotalSeconds;
        }

        protected virtual void CopyDirectory(string path, string newPath)
        {
            var directoryPath = GetFullPath(GetVirtualPath(path));
            var directory = new DirectoryInfo(directoryPath);
            if (!directory.Exists)
                throw new Exception(GetLanguageResource("E_CopyDirInvalidPath"));

            var newDirectoryPath = GetFullPath(GetVirtualPath($"{newPath.TrimEnd('/')}/{directory.Name}"));
            var newDirectory = new DirectoryInfo(newDirectoryPath);
            if (newDirectory.Exists)
                throw new Exception(GetLanguageResource("E_DirAlreadyExists"));

            CopyDirectoryRecursively(directory.FullName, newDirectory.FullName);

            this.HttpContext.Response.WriteAsync(GetSuccessResponse());
        }

        protected virtual void CopyDirectoryRecursively(string sourcePath, string destinationPath)
        {
            var existingFiles = Directory.GetFiles(sourcePath);
            var existingDirectories = Directory.GetDirectories(sourcePath);

            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            foreach (var file in existingFiles)
            {
                var filePath = Path.Combine(destinationPath, new FileInfo(file).Name);
                if (!System.IO.File.Exists(filePath))
                    System.IO.File.Copy(file, filePath);
            }

            foreach (var directory in existingDirectories)
            {
                var directoryPath = Path.Combine(destinationPath, new DirectoryInfo(directory).Name);
                CopyDirectoryRecursively(directory, directoryPath);
            }
        }

        protected virtual void CopyFile(string path, string newPath)
        {
            var filePath = GetFullPath(GetVirtualPath(path));
            var file = new FileInfo(filePath);
            if (!file.Exists)
                throw new Exception(GetLanguageResource("E_CopyFileInvalisPath"));

            newPath = GetFullPath(GetVirtualPath(newPath));
            var newFileName = GetUniqueFileName(newPath, file.Name);
            try
            {
                System.IO.File.Copy(file.FullName, Path.Combine(newPath, newFileName));
                this.HttpContext.Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception(GetLanguageResource("E_CopyFile"));
            }
        }

        protected virtual string GetUniqueFileName(string directory, string fileName)
        {
            var uniqueFileName = fileName;

            int i = 0;
            while (System.IO.File.Exists(Path.Combine(directory, uniqueFileName)))
            {
                uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-Copy-{++i}{Path.GetExtension(fileName)}";
            }

            return uniqueFileName;
        }

        protected virtual void CreateDirectory(string path, string name)
        {
            path = GetFullPath(GetVirtualPath(path));
            if (!Directory.Exists(path))
                throw new Exception(GetLanguageResource("E_CreateDirInvalidPath"));

            try
            {
                path = Path.Combine(path, name);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                this.HttpContext.Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception(GetLanguageResource("E_CreateDirFailed"));
            }
        }

        protected virtual void DeleteDirectory(string path)
        {
            path = GetVirtualPath(path);
            if (path == GetRootDirectory())
                throw new Exception(GetLanguageResource("E_CannotDeleteRoot"));

            path = GetFullPath(path);
            if (!Directory.Exists(path))
                throw new Exception(GetLanguageResource("E_DeleteDirInvalidPath"));

            if (Directory.GetDirectories(path).Length > 0 || Directory.GetFiles(path).Length > 0)
                throw new Exception(GetLanguageResource("E_DeleteNonEmpty"));

            try
            {
                Directory.Delete(path);
                this.HttpContext.Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception(GetLanguageResource("E_CannotDeleteDir"));
            }
        }

        protected virtual void DeleteFile(string path)
        {
            path = GetFullPath(GetVirtualPath(path));
            if (!System.IO.File.Exists(path))
                throw new Exception(GetLanguageResource("E_DeleteFileInvalidPath"));

            try
            {
                System.IO.File.Delete(path);
                this.HttpContext.Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception(GetLanguageResource("E_DeletеFile"));
            }
        }

        public void DownloadDirectory(string path)
        {
            path = GetVirtualPath(path).TrimEnd('/');
            var fullPath = GetFullPath(path);
            if (!Directory.Exists(fullPath))
                throw new Exception(GetLanguageResource("E_CreateArchive"));

            var zipName = new FileInfo(fullPath).Name + ".zip";
            var zipPath = $"/{zipName}";
            if (path != GetRootDirectory())
                zipPath = GetVirtualPath(zipPath);
            zipPath = GetFullPath(zipPath);

            if (System.IO.File.Exists(zipPath))
                System.IO.File.Delete(zipPath);

            ZipFile.CreateFromDirectory(fullPath, zipPath, CompressionLevel.Fastest, true);

            this.HttpContext.Response.Clear();
            this.HttpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{zipName}\"");
            this.HttpContext.Response.ContentType = MimeTypes.ApplicationForceDownload;
            this.HttpContext.Response.SendFileAsync(zipPath).Wait();

            System.IO.File.Delete(zipPath);
        }

        protected virtual void DownloadFile(string path)
        {
            var filePath = GetFullPath(GetVirtualPath(path));
            var file = new FileInfo(filePath);
            if (file.Exists)
            {
                this.HttpContext.Response.Clear();
                this.HttpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{file.Name}\"");
                this.HttpContext.Response.ContentType = MimeTypes.ApplicationForceDownload;
                this.HttpContext.Response.SendFileAsync(file.FullName);
            }
        }

        protected virtual void MoveDirectory(string sourcePath, string destinationPath)
        {
            var fullSourcePath = GetFullPath(GetVirtualPath(sourcePath));
            var sourceDirectory = new DirectoryInfo(fullSourcePath);
            destinationPath = GetFullPath(GetVirtualPath(Path.Combine(destinationPath, sourceDirectory.Name)));
            var destinationDirectory = new DirectoryInfo(destinationPath);
            if (destinationDirectory.FullName.IndexOf(sourceDirectory.FullName) == 0)
                throw new Exception(GetLanguageResource("E_CannotMoveDirToChild"));

            if (!sourceDirectory.Exists)
                throw new Exception(GetLanguageResource("E_MoveDirInvalisPath"));

            if (destinationDirectory.Exists)
                throw new Exception(GetLanguageResource("E_DirAlreadyExists"));

            try
            {
                sourceDirectory.MoveTo(destinationDirectory.FullName);
                this.HttpContext.Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception($"{GetLanguageResource("E_MoveDir")} \"{sourcePath}\"");
            }
        }

        protected virtual void MoveFile(string sourcePath, string destinationPath)
        {
            var fullSourcePath = GetFullPath(GetVirtualPath(sourcePath));
            var sourceFile = new FileInfo(fullSourcePath);
            if (!sourceFile.Exists)
                throw new Exception(GetLanguageResource("E_MoveFileInvalisPath"));

            destinationPath = GetFullPath(GetVirtualPath(destinationPath));
            var destinationFile = new FileInfo(destinationPath);
            if (destinationFile.Exists)
                throw new Exception(GetLanguageResource("E_MoveFileAlreadyExists"));

            if (!CanHandleFile(destinationFile.Name))
                throw new Exception(GetLanguageResource("E_FileExtensionForbidden"));

            try
            {
                sourceFile.MoveTo(destinationFile.FullName);
                this.HttpContext.Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception($"{GetLanguageResource("E_MoveFile")} \"{sourcePath}\"");
            }
        }

        protected virtual void RenameDirectory(string sourcePath, string name)
        {
            var fullSourcePath = GetFullPath(GetVirtualPath(sourcePath));
            var sourceDirectory = new DirectoryInfo(fullSourcePath);
            var destinationDirectory = new DirectoryInfo(Path.Combine(sourceDirectory.Parent.FullName, name));
            if (GetVirtualPath(sourcePath) == GetRootDirectory())
                throw new Exception(GetLanguageResource("E_CannotRenameRoot"));

            if (!sourceDirectory.Exists)
                throw new Exception(GetLanguageResource("E_RenameDirInvalidPath"));

            if (destinationDirectory.Exists)
                throw new Exception(GetLanguageResource("E_DirAlreadyExists"));

            try
            {
                sourceDirectory.MoveTo(destinationDirectory.FullName);
                this.HttpContext.Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception($"{GetLanguageResource("E_RenameDir")} \"{sourcePath}\"");
            }
        }

        protected virtual void RenameFile(string sourcePath, string name)
        {
            var fullSourcePath = GetFullPath(GetVirtualPath(sourcePath));
            var sourceFile = new FileInfo(fullSourcePath);
            if (!sourceFile.Exists)
                throw new Exception(GetLanguageResource("E_RenameFileInvalidPath"));

            if (!CanHandleFile(name))
                throw new Exception(GetLanguageResource("E_FileExtensionForbidden"));

            try
            {
                var destinationPath = Path.Combine(sourceFile.Directory.FullName, name);
                var destinationFile = new FileInfo(destinationPath);
                sourceFile.MoveTo(destinationFile.FullName);
                this.HttpContext.Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception($"{GetLanguageResource("E_RenameFile")} \"{sourcePath}\"");
            }
        }

        protected virtual void CreateThumbnail(string path, int width, int height)
        {
            path = GetFullPath(GetVirtualPath(path));
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var image = new Bitmap(Bitmap.FromStream(stream)))
                {
                    var cropWidth = image.Width;
                    var cropHeight = image.Height;
                    var cropX = 0;
                    var cropY = 0;

                    var imgRatio = (double)image.Width / (double)image.Height;

                    if (height == 0)
                        height = Convert.ToInt32(Math.Floor((double)width / imgRatio));

                    if (width > image.Width)
                        width = image.Width;
                    if (height > image.Height)
                        height = image.Height;

                    var cropRatio = (double)width / (double)height;
                    cropWidth = Convert.ToInt32(Math.Floor((double)image.Height * cropRatio));
                    cropHeight = Convert.ToInt32(Math.Floor((double)cropWidth / cropRatio));

                    if (cropWidth > image.Width)
                    {
                        cropWidth = image.Width;
                        cropHeight = Convert.ToInt32(Math.Floor((double)cropWidth / cropRatio));
                    }

                    if (cropHeight > image.Height)
                    {
                        cropHeight = image.Height;
                        cropWidth = Convert.ToInt32(Math.Floor((double)cropHeight * cropRatio));
                    }

                    if (cropWidth < image.Width)
                        cropX = Convert.ToInt32(Math.Floor((double)(image.Width - cropWidth) / 2));
                    if (cropHeight < image.Height)
                        cropY = Convert.ToInt32(Math.Floor((double)(image.Height - cropHeight) / 2));

                    using (var cropImg = image.Clone(new Rectangle(cropX, cropY, cropWidth, cropHeight), PixelFormat.DontCare))
                    {
                        this.HttpContext.Response.Headers.Add("Content-Type", MimeTypes.ImagePng);
                        cropImg.GetThumbnailImage(width, height, () => { return false; }, IntPtr.Zero).Save(this.HttpContext.Response.Body, ImageFormat.Png);
                        this.HttpContext.Response.Body.Close();
                    }
                }
            }
        }

        protected virtual ImageFormat GetImageFormat(string filename)
        {
            var fileExtension = new FileInfo(filename).Extension.ToLower();
            switch (fileExtension)
            {
                case ".png":
                    return ImageFormat.Png;
                case ".gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }

        protected virtual void ImageResize(string path, string destinstionFile, int width, int height)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var image = Image.FromStream(stream))
                {
                    var ratio = (float)image.Width / (float)image.Height;
                    if ((image.Width <= width && image.Height <= height) || (width == 0 && height == 0))
                        return;

                    var newWidth = width;
                    int newHeight = Convert.ToInt16(Math.Floor((float)newWidth / ratio));
                    if ((height > 0 && newHeight > height) || (width == 0))
                    {
                        newHeight = height;
                        newWidth = Convert.ToInt16(Math.Floor((float)newHeight * ratio));
                    }

                    using (var newImage = new Bitmap(newWidth, newHeight))
                    {
                        using (var graphics = Graphics.FromImage(newImage))
                        {
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                            if (!string.IsNullOrEmpty(destinstionFile))
                                newImage.Save(destinstionFile, GetImageFormat(destinstionFile));
                        }
                    }
                }
            }
        }

        protected virtual bool IsAjaxUpload()
        {
            return this.HttpContext.Request.Form != null &&
                !StringValues.IsNullOrEmpty(this.HttpContext.Request.Form["method"]) &&
                this.HttpContext.Request.Form["method"] == "ajax";
        }

        protected virtual void UploadFiles(string path)
        {
            var result = GetSuccessResponse();
            var hasErrors = false;
            try
            {
                path = GetFullPath(GetVirtualPath(path));
                for (var i = 0; i < this.HttpContext.Request.Form.Files.Count; i++)
                {
                    var fileName = this.HttpContext.Request.Form.Files[i].FileName;
                    if (CanHandleFile(fileName))
                    {
                        var file = new FileInfo(fileName);
                        var uniqueFileName = GetUniqueFileName(path, file.Name);
                        var destinationFile = Path.Combine(path, uniqueFileName);
                        using (var stream = new FileStream(destinationFile, FileMode.OpenOrCreate))
                        {
                            this.HttpContext.Request.Form.Files[i].CopyTo(stream);
                        }
                        if (GetFileType(new FileInfo(uniqueFileName).Extension) == "image")
                        {
                            int.TryParse(GetSetting("MAX_IMAGE_WIDTH"), out int w);
                            int.TryParse(GetSetting("MAX_IMAGE_HEIGHT"), out int h);
                            ImageResize(destinationFile, destinationFile, w, h);
                        }
                    }
                    else
                    {
                        hasErrors = true;
                        result = GetErrorResponse(GetLanguageResource("E_UploadNotAll"));
                    }
                }
            }
            catch (Exception ex)
            {
                result = GetErrorResponse(ex.Message);
            }
            if (IsAjaxUpload())
            {
                if (hasErrors)
                    result = GetErrorResponse(GetLanguageResource("E_UploadNotAll"));

                this.HttpContext.Response.WriteAsync(result);
            }
            else
                this.HttpContext.Response.WriteAsync($"<script>parent.fileUploaded({result});</script>");
        }

        #endregion
    }
}