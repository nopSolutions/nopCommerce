using System.IO;
using System.Linq;
using System.Threading;
using FluentMigrator;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Media;
using Nop.Services.Media.RoxyFileman;

namespace Nop.Web.Framework.Migrations.UpgradeTo460
{
    [NopMigration("2022-10-18 00:00:00", "Move uploaded images to disk", MigrationProcessType.Update)]
    public class UploadedImagesMigration : Migration
    {
        #region Fields

        private readonly IPictureService _pictureService;
        private readonly IRepository<Picture> _pictureRepository;
        private readonly IRoxyFilemanFileProvider _roxyFilemanFileProvider;

        #endregion

        public UploadedImagesMigration(IPictureService pictureService, IRepository<Picture> pictureRepository, IRoxyFilemanFileProvider roxyFilemanFileProvider)
        {
            _pictureService = pictureService;
            _pictureRepository = pictureRepository;
            _roxyFilemanFileProvider = roxyFilemanFileProvider;
        }

        public override void Up()
        {
            if (!_pictureService.IsStoreInDbAsync().Result)
                return;

            const int pageSize = 400;
            var pageIndex = 0;
            var uploadRoot = $"~{NopRoxyFilemanDefaults.DefaultRootDirectory}/";

            try
            {
                while (true)
                {
                    var pictures = _pictureService.GetPicturesAsync(uploadRoot,
                            pageIndex, pageSize).Result;
                    pageIndex++;

                    //all pictures flushed?
                    if (!pictures.Any())
                        break;

                    foreach (var picture in pictures)
                    {
                        if (string.IsNullOrEmpty(picture?.VirtualPath) || string.IsNullOrEmpty(picture.SeoFilename))
                            return;

                        var seoFileName = picture.SeoFilename;

                        var lastPart = _pictureService.GetFileExtensionFromMimeTypeAsync(picture.MimeType).Result;

                        var thumbFileName = $"{seoFileName}.{lastPart}";
                        var thumbDirectoryName = picture.VirtualPath[uploadRoot.Length..];
                        var thumbFilePath = Path.Combine(thumbDirectoryName, thumbFileName);

                        _roxyFilemanFileProvider.CreateDirectory("/", thumbDirectoryName);

                        if (picture.IsNew)
                        {
                            // delete old file if exist
                            _roxyFilemanFileProvider.DeleteFile(thumbFilePath);
                        }

                        if (_roxyFilemanFileProvider.GetFileInfo(thumbFilePath).Exists)
                        {
                            _pictureRepository.DeleteAsync(picture, false).Wait();
                            continue;
                        }
                        //the named mutex helps to avoid creating the same files in different threads,
                        //and does not decrease performance significantly, because the code is blocked only for the specific file.
                        //you should be very careful, mutexes cannot be used in with the await operation
                        //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
                        using var mutex = new Mutex(false, thumbFileName);

                        mutex.WaitOne();

                        try
                        {
                            //check, if the file was created, while we were waiting for the release of the mutex.
                            if (!_roxyFilemanFileProvider.GetFileInfo(thumbFilePath).Exists)
                            {
                                var pictureBinary = _pictureService.LoadPictureBinaryAsync(picture).Result;

                                if (pictureBinary == null || pictureBinary.Length == 0)
                                    continue;

                                using var stream = new MemoryStream(pictureBinary);
                                _roxyFilemanFileProvider.SaveFileAsync(thumbDirectoryName, thumbFileName, picture.MimeType, stream).Wait();
                            }
                        }
                        finally
                        {
                            mutex.ReleaseMutex();
                        }

                        _pictureRepository.DeleteAsync(picture, false).Wait();
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}