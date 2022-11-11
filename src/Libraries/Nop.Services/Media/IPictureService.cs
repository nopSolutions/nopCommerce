using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;

namespace Nop.Services.Media
{
    /// <summary>
    /// Picture service interface
    /// </summary>
    public partial interface IPictureService
    {
        /// <summary>
        /// Returns the file extension from mime type.
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the file extension
        /// </returns>
        Task<string> GetFileExtensionFromMimeTypeAsync(string mimeType);

        /// <summary>
        /// Gets the loaded picture binary depending on picture storage settings
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture binary
        /// </returns>
        Task<byte[]> LoadPictureBinaryAsync(Picture picture);

        /// <summary>
        /// Get picture SEO friendly name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<string> GetPictureSeNameAsync(string name);

        /// <summary>
        /// Gets the default picture URL
        /// </summary>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture URL
        /// </returns>
        Task<string> GetDefaultPictureUrlAsync(int targetSize = 0,
            PictureType defaultPictureType = PictureType.Entity,
            string storeLocation = null);

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture URL
        /// </returns>
        Task<string> GetPictureUrlAsync(int pictureId,
            int targetSize = 0,
            bool showDefaultPicture = true,
            string storeLocation = null,
            PictureType defaultPictureType = PictureType.Entity);

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="picture">Reference instance of Picture</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture URL
        /// </returns>
        Task<(string Url, Picture Picture)> GetPictureUrlAsync(Picture picture,
            int targetSize = 0,
            bool showDefaultPicture = true,
            string storeLocation = null,
            PictureType defaultPictureType = PictureType.Entity);

        /// <summary>
        /// Get a picture local path
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        Task<string> GetThumbLocalPathAsync(Picture picture, int targetSize = 0, bool showDefaultPicture = true);

        /// <summary>
        /// Gets a picture
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture
        /// </returns>
        Task<Picture> GetPictureByIdAsync(int pictureId);

        /// <summary>
        /// Deletes a picture
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeletePictureAsync(Picture picture);

        /// <summary>
        /// Gets a collection of pictures
        /// </summary>
        /// <param name="virtualPath">Virtual path</param>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Items on each page</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the paged list of pictures
        /// </returns>
        Task<IPagedList<Picture>> GetPicturesAsync(string virtualPath = "", int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets pictures by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="recordsToReturn">Number of records to return. 0 if you want to get all items</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the pictures
        /// </returns>
        Task<IList<Picture>> GetPicturesByProductIdAsync(int productId, int recordsToReturn = 0);

        /// <summary>
        /// Inserts a picture
        /// </summary>
        /// <param name="pictureBinary">The picture binary</param>
        /// <param name="mimeType">The picture MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="altAttribute">"alt" attribute for "img" HTML element</param>
        /// <param name="titleAttribute">"title" attribute for "img" HTML element</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture
        /// </returns>
        Task<Picture> InsertPictureAsync(byte[] pictureBinary, string mimeType, string seoFilename,
            string altAttribute = null, string titleAttribute = null,
            bool isNew = true, bool validateBinary = true);

        /// <summary>
        /// Inserts a picture
        /// </summary>
        /// <param name="formFile">Form file</param>
        /// <param name="defaultFileName">File name which will be use if IFormFile.FileName not present</param>
        /// <param name="virtualPath">Virtual path</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture
        /// </returns>
        Task<Picture> InsertPictureAsync(IFormFile formFile, string defaultFileName = "", string virtualPath = "");

        /// <summary>
        /// Updates the picture
        /// </summary>
        /// <param name="pictureId">The picture identifier</param>
        /// <param name="pictureBinary">The picture binary</param>
        /// <param name="mimeType">The picture MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="altAttribute">"alt" attribute for "img" HTML element</param>
        /// <param name="titleAttribute">"title" attribute for "img" HTML element</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture
        /// </returns>
        Task<Picture> UpdatePictureAsync(int pictureId, byte[] pictureBinary, string mimeType,
            string seoFilename, string altAttribute = null, string titleAttribute = null,
            bool isNew = true, bool validateBinary = true);

        /// <summary>
        /// Updates the picture
        /// </summary>
        /// <param name="picture">The picture to update</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture
        /// </returns>
        Task<Picture> UpdatePictureAsync(Picture picture);

        /// <summary>
        /// Updates a SEO filename of a picture
        /// </summary>
        /// <param name="pictureId">The picture identifier</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture
        /// </returns>
        Task<Picture> SetSeoFilenameAsync(int pictureId, string seoFilename);

        /// <summary>
        /// Validates input picture dimensions
        /// </summary>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <param name="fileName">Name of file</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture binary or throws an exception
        /// </returns>
        Task<byte[]> ValidatePictureAsync(byte[] pictureBinary, string mimeType, string fileName);

        /// <summary>
        /// Gets or sets a value indicating whether the images should be stored in data base.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<bool> IsStoreInDbAsync();

        /// <summary>
        /// Sets a value indicating whether the images should be stored in data base
        /// </summary>
        /// <param name="isStoreInDb">A value indicating whether the images should be stored in data base</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetIsStoreInDbAsync(bool isStoreInDb);

        /// <summary>
        /// Get product picture (for shopping cart and order details pages)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes (in XML format)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture
        /// </returns>
        Task<Picture> GetProductPictureAsync(Product product, string attributesXml);

        /// <summary>
        /// Get product picture binary by picture identifier
        /// </summary>
        /// <param name="pictureId">The picture identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture binary
        /// </returns>
        Task<PictureBinary> GetPictureBinaryByPictureIdAsync(int pictureId);

        /// <summary>
        /// Convert image from SVG format to PNG
        /// </summary>
        /// <param name="filePath">SVG file path</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains the byte array</returns>
        Task<byte[]> ConvertSvgToPngAsync(string filePath);

        /// <summary>
        /// Get content type for picture by file extension
        /// </summary>
        /// <param name="fileExtension">The file extension</param>
        /// <returns>Picture's content type</returns>
        string GetPictureContentTypeByFileExtension(string fileExtension);
    }
}