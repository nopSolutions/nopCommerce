using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using SkiaSharp;

namespace Nop.Services.Media.RoxyFileman
{
    /// <summary>
    /// Looks up and manages uploaded files using the on-disk file system
    /// </summary>
    public class RoxyFilemanFileProvider : PhysicalFileProvider, IRoxyFilemanFileProvider
    {
        #region Fields

        protected INopFileProvider _nopFileProvider;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public RoxyFilemanFileProvider(INopFileProvider nopFileProvider) : base(nopFileProvider.Combine(nopFileProvider.WebRootPath, NopRoxyFilemanDefaults.DefaultRootDirectory))
        {
            _nopFileProvider = nopFileProvider;
        }

        public RoxyFilemanFileProvider(INopFileProvider defaultFileProvider, MediaSettings mediaSettings) : this(defaultFileProvider)
        {
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Utils

        /// <summary>
        /// Adjust image measures to target size
        /// </summary>
        /// <param name="image">Source image</param>
        /// <param name="maxWidth">Target width</param>
        /// <param name="maxHeight">Target height</param>
        /// <returns>Adjusted width and height</returns>
        protected virtual (int width, int height) ValidateImageMeasures(SKBitmap image, int maxWidth = 0, int maxHeight = 0)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            float width = Math.Min(image.Width, maxWidth);
            float height = Math.Min(image.Height, maxHeight);

            var targetSize = Math.Max(width, height);

            if (image.Height > image.Width)
            {
                // portrait
                width = image.Width * (targetSize / image.Height);
                height = targetSize;
            }
            else
            {
                // landscape or square
                width = targetSize;
                height = image.Height * (targetSize / image.Width);
            }

            return ((int)width, (int)height);
        }

        /// <summary>
        /// Get a file type by the specified path string
        /// </summary>
        /// <param name="subpath">The path string from which to get the file type</param>
        /// <returns>File type</returns>
        protected virtual string GetFileType(string subpath)
        {
            var fileExtension = Path.GetExtension(subpath)?.ToLowerInvariant();

            return fileExtension switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".svg" => "image",
                ".swf" or ".flv" => "flash",
                ".mp4" or ".webm" or ".ogg" or ".mov" or ".m4a" or ".mp3" or ".wav" => "media",
                _ => "file"
            };

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
        }

        /// <summary>
        /// Get the absolute path for the specified path string in the root directory for this instance
        /// </summary>
        /// <param name="path">The file or directory for which to obtain absolute path information</param>
        /// <returns>The fully qualified location of path, such as "C:\MyFile.txt"</returns>
        protected virtual string GetFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new RoxyFilemanException("NoFilesFound");

            path = path.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (Path.IsPathRooted(path))
                throw new RoxyFilemanException("NoFilesFound");

            var fullPath = Path.GetFullPath(Path.Combine(Root, path));

            if (!IsUnderneathRoot(fullPath))
                throw new RoxyFilemanException("NoFilesFound");

            return fullPath;
        }

        /// <summary>
        /// Get image format by mime type
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>SKEncodedImageFormat</returns>
        protected virtual SKEncodedImageFormat GetImageFormatByMimeType(string mimeType)
        {
            var format = SKEncodedImageFormat.Jpeg;
            if (string.IsNullOrEmpty(mimeType))
                return format;

            var parts = mimeType.ToLowerInvariant().Split('/');
            var lastPart = parts[^1];

            switch (lastPart)
            {
                case "webp":
                    format = SKEncodedImageFormat.Webp;
                    break;
                case "png":
                case "gif":
                case "bmp":
                case "x-icon":
                    format = SKEncodedImageFormat.Png;
                    break;
                default:
                    break;
            }

            return format;
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
            while (GetFileInfo(Path.Combine(directoryPath, uniqueFileName)) is IFileInfo fileInfo && fileInfo.Exists)
            {
                uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-Copy-{++i}{Path.GetExtension(fileName)}";
            }

            return uniqueFileName;
        }

        /// <summary>
        /// Check the specified path is in the root directory of this instance
        /// </summary>
        /// <param name="fullPath">The absolute path</param>
        /// <returns>True if passed path is in the root; otherwise false</returns>
        protected virtual bool IsUnderneathRoot(string fullPath)
        {
            return fullPath
                .StartsWith(Root, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Scale image to fit the destination sizes
        /// </summary>
        /// <param name="data">Image data</param>
        /// <param name="format">SkiaSharp image format</param>
        /// <param name="maxWidth">Target width</param>
        /// <param name="maxHeight">Target height</param>
        /// <returns>The byte array of resized image</returns>
        protected virtual byte[] ResizeImage(byte[] data, SKEncodedImageFormat format, int maxWidth, int maxHeight)
        {
            using var sourceStream = new SKMemoryStream(data);
            using var inputData = SKData.Create(sourceStream);
            using var image = SKBitmap.Decode(inputData);

            var (width, height) = ValidateImageMeasures(image, maxWidth, maxHeight);

            var toBitmap = new SKBitmap(width, height, image.ColorType, image.AlphaType);

            if (!image.ScalePixels(toBitmap, SKFilterQuality.None))
                throw new Exception("Image scaling");

            var newImage = SKImage.FromBitmap(toBitmap);
            var imageData = newImage.Encode(format, _mediaSettings.DefaultImageQuality);

            newImage.Dispose();
            return imageData.ToArray();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Moves a file or a directory and its contents to a new location
        /// </summary>
        /// <param name="sourceDirName">The path of the file or directory to move</param>
        /// <param name="destDirName">
        /// The path to the new location for sourceDirName. If sourceDirName is a file, then destDirName
        /// must also be a file name
        /// </param>
        public virtual void DirectoryMove(string sourceDirName, string destDirName)
        {
            if (destDirName.IndexOf(sourceDirName, StringComparison.InvariantCulture) == 0)
                throw new RoxyFilemanException("E_CannotMoveDirToChild");

            var sourceDirInfo = new DirectoryInfo(GetFullPath(sourceDirName));
            if (!sourceDirInfo.Exists)
                throw new RoxyFilemanException("E_MoveDirInvalisPath");

            if (string.Equals(sourceDirInfo.FullName, Root, StringComparison.InvariantCultureIgnoreCase))
                throw new RoxyFilemanException("E_MoveDir");

            var newFullPath = Path.Combine(GetFullPath(destDirName), sourceDirInfo.Name);
            var destinationDirInfo = new DirectoryInfo(newFullPath);
            if (destinationDirInfo.Exists)
                throw new RoxyFilemanException("E_DirAlreadyExists");

            try
            {
                sourceDirInfo.MoveTo(destinationDirInfo.FullName);
            }
            catch
            {
                throw new RoxyFilemanException("E_MoveDir");
            }
        }

        /// <summary>
        /// Locate a file at the given path by directly mapping path segments to physical directories.
        /// </summary>
        /// <param name="subpath">A path under the root directory</param>
        /// <returns>The file information. Caller must check Microsoft.Extensions.FileProviders.IFileInfo.Exists property.</returns>
        public new IFileInfo GetFileInfo(string subpath)
        {
            if (string.IsNullOrEmpty(subpath))
                return new NotFoundFileInfo(subpath);

            subpath = subpath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Absolute paths not permitted.
            if (Path.IsPathRooted(subpath))
                return new NotFoundFileInfo(subpath);

            return base.GetFileInfo(subpath);
        }

        /// <summary>
        /// Create configuration file for RoxyFileman
        /// </summary>
        /// <param name="pathBase">The base path for the store</param>
        /// <param name="lang">Two-letter language code</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<RoxyFilemanConfig> GetOrCreateConfigurationAsync(string pathBase, string lang)
        {
            //check whether the path base has changed, otherwise there is no need to overwrite the configuration file
            if (Singleton<RoxyFilemanConfig>.Instance?.RETURN_URL_PREFIX?.Equals(pathBase) ?? false)
            {
                return Singleton<RoxyFilemanConfig>.Instance;
            }

            var filePath = _nopFileProvider.GetAbsolutePath(NopRoxyFilemanDefaults.ConfigurationFile);

            //create file if not exists
            _nopFileProvider.CreateFile(filePath);

            //try to read existing configuration
            var existingText = await _nopFileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);
            var existingConfiguration = JsonConvert.DeserializeObject<RoxyFilemanConfig>(existingText);

            //create configuration
            var configuration = new RoxyFilemanConfig
            {
                FILES_ROOT = existingConfiguration?.FILES_ROOT ?? NopRoxyFilemanDefaults.DefaultRootDirectory,
                SESSION_PATH_KEY = existingConfiguration?.SESSION_PATH_KEY ?? string.Empty,
                THUMBS_VIEW_WIDTH = existingConfiguration?.THUMBS_VIEW_WIDTH ?? 140,
                THUMBS_VIEW_HEIGHT = existingConfiguration?.THUMBS_VIEW_HEIGHT ?? 120,
                PREVIEW_THUMB_WIDTH = existingConfiguration?.PREVIEW_THUMB_WIDTH ?? 300,
                PREVIEW_THUMB_HEIGHT = existingConfiguration?.PREVIEW_THUMB_HEIGHT ?? 200,
                MAX_IMAGE_WIDTH = existingConfiguration?.MAX_IMAGE_WIDTH ?? _mediaSettings.MaximumImageSize,
                MAX_IMAGE_HEIGHT = existingConfiguration?.MAX_IMAGE_HEIGHT ?? _mediaSettings.MaximumImageSize,
                DEFAULTVIEW = existingConfiguration?.DEFAULTVIEW ?? "list",
                FORBIDDEN_UPLOADS = existingConfiguration?.FORBIDDEN_UPLOADS ?? string.Join(" ", NopRoxyFilemanDefaults.ForbiddenUploadExtensions),
                ALLOWED_UPLOADS = existingConfiguration?.ALLOWED_UPLOADS ?? string.Empty,
                FILEPERMISSIONS = existingConfiguration?.FILEPERMISSIONS ?? "0644",
                DIRPERMISSIONS = existingConfiguration?.DIRPERMISSIONS ?? "0755",
                LANG = existingConfiguration?.LANG ?? lang,
                DATEFORMAT = existingConfiguration?.DATEFORMAT ?? "dd/MM/yyyy HH:mm",
                OPEN_LAST_DIR = existingConfiguration?.OPEN_LAST_DIR ?? "yes",

                //no need user to configure
                INTEGRATION = "custom",
                RETURN_URL_PREFIX = $"{pathBase}/images/uploaded/",
                DIRLIST = $"{pathBase}/Admin/RoxyFileman/DirectoriesList",
                CREATEDIR = $"{pathBase}/Admin/RoxyFileman/CreateDirectory",
                DELETEDIR = $"{pathBase}/Admin/RoxyFileman/DeleteDirectory",
                MOVEDIR = $"{pathBase}/Admin/RoxyFileman/MoveDirectory",
                COPYDIR = $"{pathBase}/Admin/RoxyFileman/CopyDirectory",
                RENAMEDIR = $"{pathBase}/Admin/RoxyFileman/RenameDirectory",
                FILESLIST = $"{pathBase}/Admin/RoxyFileman/FilesList",
                UPLOAD = $"{pathBase}/Admin/RoxyFileman/UploadFiles",
                DOWNLOAD = $"{pathBase}/Admin/RoxyFileman/DownloadFile",
                DOWNLOADDIR = $"{pathBase}/Admin/RoxyFileman/DownloadDirectory",
                DELETEFILE = $"{pathBase}/Admin/RoxyFileman/DeleteFile",
                MOVEFILE = $"{pathBase}/Admin/RoxyFileman/MoveFile",
                COPYFILE = $"{pathBase}/Admin/RoxyFileman/CopyFile",
                RENAMEFILE = $"{pathBase}/Admin/RoxyFileman/RenameFile",
                GENERATETHUMB = $"{pathBase}/Admin/RoxyFileman/CreateImageThumbnail"
            };

            //save the file
            var text = JsonConvert.SerializeObject(configuration, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, text, Encoding.UTF8);

            Singleton<RoxyFilemanConfig>.Instance = configuration;

            return configuration;
        }

        /// <summary>
        /// Get all available directories as a directory tree
        /// </summary>
        /// <param name="type">Type of the file</param>
        /// <param name="isRecursive">A value indicating whether to return a directory tree recursively</param>
        /// <param name="rootDirectoryPath">Path to start directory</param>
        public virtual IEnumerable<RoxyDirectoryInfo> GetDirectories(string type, bool isRecursive = true, string rootDirectoryPath = "")
        {
            foreach (var item in GetDirectoryContents(rootDirectoryPath))
            {
                if (item.IsDirectory)
                {
                    var dirInfo = new DirectoryInfo(item.PhysicalPath);

                    yield return new RoxyDirectoryInfo(
                        getRelativePath(item.Name),
                        dirInfo.GetFiles().Count(x => isMatchType(x.Name)),
                        dirInfo.GetDirectories().Length);
                }

                if (!isRecursive)
                    break;

                foreach (var subDir in GetDirectories(type, isRecursive, getRelativePath(item.Name)))
                    yield return subDir;
            }

            string getRelativePath(string name) => Path.Combine(rootDirectoryPath, name);
            bool isMatchType(string name) => string.IsNullOrEmpty(type) || GetFileType(name) == type;
        }

        /// <summary>
        /// Get files in the passed directory
        /// </summary>
        /// <param name="directoryPath">Path to the files directory</param>
        /// <param name="type">Type of the files</param>
        /// <returns>
        /// The list of <see cref="RoxyImageInfo"/>
        /// </returns>
        public virtual IEnumerable<RoxyImageInfo> GetFiles(string directoryPath = "", string type = "")
        {
            var files = GetDirectoryContents(directoryPath);

            return files
                .Where(f => !f.IsDirectory && isMatchType(f.Name))
                .Select(f =>
                {
                    using var skData = SKData.Create(f.PhysicalPath);
                    var image = SKBitmap.DecodeBounds(skData);

                    return new RoxyImageInfo(getRelativePath(f.Name), f.LastModified, f.Length, image.Width, image.Height);
                });

            bool isMatchType(string name) => string.IsNullOrEmpty(type) || GetFileType(name) == type;
            string getRelativePath(string name) => Path.Combine(directoryPath, name);
        }

        /// <summary>
        /// Moves a specified file to a new location, providing the option to specify a new file name
        /// </summary>
        /// <param name="sourcePath">The name of the file to move. Can include a relative or absolute path</param>
        /// <param name="destinationPath">The new path and name for the file</param>
        public void FileMove(string sourcePath, string destinationPath)
        {
            var sourceFile = GetFileInfo(sourcePath);

            if (!sourceFile.Exists)
                throw new RoxyFilemanException("E_MoveFileInvalisPath");

            var destinationFile = GetFileInfo(destinationPath);
            if (destinationFile.Exists)
                throw new RoxyFilemanException("E_MoveFileAlreadyExists");

            try
            {
                new FileInfo(sourceFile.PhysicalPath)
                    .MoveTo(GetFullPath(destinationPath));
            }
            catch
            {
                throw new RoxyFilemanException("E_MoveFile");
            }
        }

        /// <summary>
        /// Copy the directory with the embedded files and directories
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination directory</param>
        public virtual void CopyDirectory(string sourcePath, string destinationPath)
        {
            var sourceDirInfo = new DirectoryInfo(GetFullPath(sourcePath));
            if (!sourceDirInfo.Exists)
                throw new RoxyFilemanException("E_CopyDirInvalidPath");

            var newPath = Path.Combine(GetFullPath(destinationPath), sourceDirInfo.Name);
            var destinationDirInfo = new DirectoryInfo(newPath);

            if (destinationDirInfo.Exists)
                throw new RoxyFilemanException("E_DirAlreadyExists");

            try
            {
                destinationDirInfo.Create();

                foreach (var file in sourceDirInfo.GetFiles())
                {
                    var newFile = GetFileInfo(Path.Combine(destinationPath, file.Name));
                    if (!newFile.Exists)
                        file.CopyTo(Path.Combine(destinationDirInfo.FullName, file.Name));
                }

                foreach (var directory in sourceDirInfo.GetDirectories())
                {
                    var destinationSubPath = Path.Combine(destinationPath, sourceDirInfo.Name, directory.Name);
                    var sourceSubPath = Path.Combine(sourcePath, directory.Name);
                    CopyDirectory(sourceSubPath, destinationSubPath);
                }
            }
            catch
            {
                throw new RoxyFilemanException("E_CopyFile");
            }
        }

        /// <summary>
        /// Rename the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="newName">New name of the directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual void RenameDirectory(string sourcePath, string newName)
        {
            try
            {
                var destinationPath = Path.Combine(Path.GetDirectoryName(sourcePath), newName);
                DirectoryMove(sourcePath, destinationPath);
            }
            catch (Exception ex)
            {
                throw new RoxyFilemanException("E_RenameDir", ex);
            }
        }

        /// <summary>
        /// Rename the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="newName">New name of the file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual void RenameFile(string sourcePath, string newName)
        {
            try
            {
                var destinationPath = Path.Combine(Path.GetDirectoryName(sourcePath), newName);
                FileMove(sourcePath, destinationPath);
            }
            catch (Exception ex)
            {
                throw new RoxyFilemanException("E_RenameFile", ex);
            }
        }

        /// <summary>
        /// Delete the file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual void DeleteFile(string path)
        {
            var fileToDelete = GetFileInfo(path);

            if (!fileToDelete.Exists)
                throw new RoxyFilemanException("E_DeleteFileInvalidPath");

            try
            {
                File.Delete(GetFullPath(path));
            }
            catch
            {
                throw new RoxyFilemanException("E_DeleteFile");
            }
        }

        /// <summary>
        /// Copy the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="destinationPath">Path to the destination file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual void CopyFile(string sourcePath, string destinationPath)
        {
            var sourceFile = GetFileInfo(sourcePath);

            if (!sourceFile.Exists)
                throw new RoxyFilemanException("E_CopyFileInvalisPath");

            var newFilePath = Path.Combine(destinationPath, sourceFile.Name);
            var destinationFile = GetFileInfo(newFilePath);

            if (destinationFile.Exists)
                newFilePath = Path.Combine(destinationPath, GetUniqueFileName(destinationPath, sourceFile.Name));

            try
            {
                File.Copy(sourceFile.PhysicalPath, GetFullPath(newFilePath));
            }
            catch
            {
                throw new RoxyFilemanException("E_CopyFile");
            }
        }

        /// <summary>
        /// Create the new directory
        /// </summary>
        /// <param name="parentDirectoryPath">Path to the parent directory</param>
        /// <param name="name">Name of the new directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual void CreateDirectory(string parentDirectoryPath, string name)
        {
            //validate path and get absolute form
            var fullPath = GetFullPath(Path.Combine(parentDirectoryPath, name));

            var newDirectory = new DirectoryInfo(fullPath);
            if (!newDirectory.Exists)
                newDirectory.Create();
        }

        /// <summary>
        /// Delete the directory
        /// </summary>
        /// <param name="path">Path to the directory</param>
        public virtual void DeleteDirectory(string path)
        {
            var sourceDirInfo = new DirectoryInfo(GetFullPath(path));
            if (!sourceDirInfo.Exists)
                throw new RoxyFilemanException("E_DeleteDirInvalidPath");

            if (string.Equals(sourceDirInfo.FullName, Root, StringComparison.InvariantCultureIgnoreCase))
                throw new RoxyFilemanException("E_CannotDeleteRoot");

            if (sourceDirInfo.GetFiles().Length > 0 || sourceDirInfo.GetDirectories().Length > 0)
                throw new RoxyFilemanException("E_DeleteNonEmpty");

            try
            {
                sourceDirInfo.Delete();
            }
            catch
            {
                throw new RoxyFilemanException("E_CannotDeleteDir");
            }
        }

        /// <summary>
        /// Save file in the root directory for this instance
        /// </summary>
        /// <param name="directoryPath">Directory path in the root</param>
        /// <param name="fileName">The file name and extension</param>
        /// <param name="contentType">Mime type</param>
        /// <param name="fileStream">The stream to read</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveFileAsync(string directoryPath, string fileName, string contentType, Stream fileStream)
        {
            var uniqueFileName = GetUniqueFileName(directoryPath, Path.GetFileName(fileName));
            var destinationFile = Path.Combine(directoryPath, uniqueFileName);

            await using var stream = new FileStream(GetFullPath(destinationFile), FileMode.Create);

            if (GetFileType(Path.GetExtension(uniqueFileName)) == "image")
            {
                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);

                var roxyConfig = Singleton<RoxyFilemanConfig>.Instance;

                var imageData = ResizeImage(memoryStream.ToArray(),
                    GetImageFormatByMimeType(contentType),
                    roxyConfig?.MAX_IMAGE_WIDTH ?? _mediaSettings.MaximumImageSize,
                    roxyConfig?.MAX_IMAGE_HEIGHT ?? _mediaSettings.MaximumImageSize);

                await stream.WriteAsync(imageData);
            }
            else
            {
                await fileStream.CopyToAsync(stream);
            }

            await stream.FlushAsync();
        }

        /// <summary>
        /// Get the thumbnail of the image
        /// </summary>
        /// <param name="sourcePath">File path</param>
        /// <param name="contentType">Mime type</param>
        /// <returns>Byte array of the specified image</returns>
        public virtual byte[] CreateImageThumbnail(string sourcePath, string contentType)
        {
            var imageInfo = GetFileInfo(sourcePath);

            if (!imageInfo.Exists)
                throw new RoxyFilemanException("Image not found");

            var roxyConfig = Singleton<RoxyFilemanConfig>.Instance;

            using var imageStream = imageInfo.CreateReadStream();
            using var ms = new MemoryStream();

            imageStream.CopyTo(ms);

            return ResizeImage(
                ms.ToArray(),
                GetImageFormatByMimeType(contentType),
                roxyConfig.THUMBS_VIEW_WIDTH,
                roxyConfig.THUMBS_VIEW_HEIGHT);
        }

        /// <summary>
        /// Create a zip archive of the specified directory.
        /// </summary>
        /// <param name="directoryPath">The directory path with files to compress</param>
        /// <returns>The byte array</returns>
        public virtual byte[] CreateZipArchiveFromDirectory(string directoryPath)
        {
            var sourceDirInfo = new DirectoryInfo(GetFullPath(directoryPath));
            if (!sourceDirInfo.Exists)
                throw new RoxyFilemanException("E_CreateArchive");

            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in sourceDirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    var fileRelativePath = file.FullName.Replace(sourceDirInfo.FullName, string.Empty)
                        .TrimStart('\\');

                    using var fileStream = file.OpenRead();
                    using var fileStreamInZip = archive.CreateEntry(fileRelativePath).Open();
                    fileStream.CopyTo(fileStreamInZip);
                }
            }

            //ToArray() should be outside of the archive using
            return memoryStream.ToArray();
        }

        #endregion
    }
}