using System;
using System.IO;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Media;

namespace Nop.Services.ExportImport.Help
{
    /// <summary>
    /// Basic class control objects through properties
    /// </summary>
    public class BaseGetProperties
    {
        protected readonly IPictureService _pictureService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pictureService">Picture service</param>
        public BaseGetProperties(IPictureService pictureService)
        {
            this._pictureService = pictureService;
        }

        /// <summary>
        /// Returns the path to the image file for the category
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Path to the image file</returns>
        protected string GetPictures(Category category)
        {
            return GetPictures(category.PictureId);
        }

        /// <summary>
        /// Returns the path to the image file for the manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>Path to the image file</returns>
        protected string GetPictures(Manufacturer manufacturer)
        {
            return GetPictures(manufacturer.PictureId);
        }

        /// <summary>
        /// Returns the path to the image file by ID
        /// </summary>
        /// <param name="pictureId">Picture ID</param>
        /// <returns>Path to the image file</returns>
        private string GetPictures(int pictureId)
        {
            var picture = _pictureService.GetPictureById(pictureId);
            return _pictureService.GetThumbLocalPath(picture);
        }

        /// <summary>
        /// Get mime type
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Mime type</returns>
        protected static string GetMimeTypeFromFilePath(string filePath)
        {
            var mimeType = MimeMapping.GetMimeMapping(filePath);

            //little hack here because MimeMapping does not contain all mappings (e.g. PNG)
            if (mimeType == "application/octet-stream")
                mimeType = "image/jpeg";

            return mimeType;
        }

        /// <summary>
        /// Creates or loads the image
        /// </summary>
        /// <param name="picturePath">The path to the image file</param>
        /// <param name="name">The name of the object</param>
        /// <param name="picId">Image identifier, may be null</param>
        /// <returns>The image or null if the image has not changed</returns>
        protected Picture LoadPicture(string picturePath, string name, int? picId=null)
        {
            if (String.IsNullOrEmpty(picturePath) || !File.Exists(picturePath))
                return null;
            
            var mimeType = GetMimeTypeFromFilePath(picturePath);
            var newPictureBinary = File.ReadAllBytes(picturePath);
            var pictureAlreadyExists = false;
            if (picId != null)
            {
                //compare with existing product pictures
                var existingPicture = _pictureService.GetPictureById(picId.Value);

                var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                //picture binary after validation (like in database)
                var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                    existingBinary.SequenceEqual(newPictureBinary))
                {
                    pictureAlreadyExists = true;
                }
            }

            if (pictureAlreadyExists) return null;

            var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType,
                _pictureService.GetPictureSeName(name));
            return newPicture;
        }
    }
}
