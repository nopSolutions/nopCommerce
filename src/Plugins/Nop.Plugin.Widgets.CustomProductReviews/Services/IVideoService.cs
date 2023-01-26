using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Plugin.Widgets.CustomProductReviews.Domains;

namespace Nop.Plugin.Widgets.CustomProductReviews.Services
{
    /// <summary>
    /// Video service interface
    /// </summary>
    public partial interface IVideoService
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
        /// Gets the loaded video binary depending on video storage settings
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video binary
        /// </returns>
        Task<byte[]> LoadVideoBinaryAsync(Video video);

        /// <summary>
        /// Get video SEO friendly name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<string> GetVideoSeNameAsync(string name);

        /// <summary>
        /// Gets the default video URL
        /// </summary>
        /// <param name="targetSize">The target video size (longest side)</param>
        /// <param name="defaultVideoType">Default video type</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video URL
        /// </returns>
        Task<string> GetDefaultVideoUrlAsync(int targetSize = 0,
            PictureType defaultVideoType = PictureType.Entity,
            string storeLocation = null);

        /// <summary>
        /// Get a video URL
        /// </summary>
        /// <param name="videoId">Video identifier</param>
        /// <param name="targetSize">The target video size (longest side)</param>
        /// <param name="showDefaultVideo">A value indicating whether the default video is shown</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <param name="defaultVideoType">Default video type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video URL
        /// </returns>
        Task<string> GetVideoUrlAsync(int videoId,
            int targetSize = 0,
            bool showDefaultVideo = true,
            string storeLocation = null,
            PictureType defaultVideoType = PictureType.Entity);

        /// <summary>
        /// Get a video URL
        /// </summary>
        /// <param name="video">Reference instance of Video</param>
        /// <param name="targetSize">The target video size (longest side)</param>
        /// <param name="showDefaultVideo">A value indicating whether the default video is shown</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <param name="defaultVideoType">Default video type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video URL
        /// </returns>
        Task<(string Url, Video Video)> GetVideoUrlAsync(Video video,
            int targetSize = 0,
            bool showDefaultVideo = true,
            string storeLocation = null,
            PictureType defaultVideoType = PictureType.Entity);

        /// <summary>
        /// Get a video local path
        /// </summary>
        /// <param name="video">Video instance</param>
        /// <param name="targetSize">The target video size (longest side)</param>
        /// <param name="showDefaultVideo">A value indicating whether the default video is shown</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        Task<string> GetThumbLocalPathAsync(Video video, int targetSize = 0, bool showDefaultVideo = true);

        /// <summary>
        /// Gets a video
        /// </summary>
        /// <param name="videoId">Video identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        Task<Video> GetVideoByIdAsync(int videoId);

        /// <summary>
        /// Deletes a video
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteVideoAsync(Video video);

        /// <summary>
        /// Gets a collection of videos
        /// </summary>
        /// <param name="virtualPath">Virtual path</param>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Items on each page</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the paged list of videos
        /// </returns>
        Task<IPagedList<Video>> GetVideosAsync(string virtualPath = "", int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets videos by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="recordsToReturn">Number of records to return. 0 if you want to get all items</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the videos
        /// </returns>
        //Task<IList<Video>> GetVideosByProductIdAsync(int productId, int recordsToReturn = 0);

        /// <summary>
        /// Inserts a video
        /// </summary>
        /// <param name="videoBinary">The video binary</param>
        /// <param name="mimeType">The video MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="altAttribute">"alt" attribute for "img" HTML element</param>
        /// <param name="titleAttribute">"title" attribute for "img" HTML element</param>
        /// <param name="isNew">A value indicating whether the video is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided video binary</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        Task<Video> InsertVideoAsync(byte[] videoBinary, string mimeType, string seoFilename,
            string altAttribute = null, string titleAttribute = null,
            bool isNew = true, bool validateBinary = true);

        /// <summary>
        /// Inserts a video
        /// </summary>
        /// <param name="formFile">Form file</param>
        /// <param name="defaultFileName">File name which will be use if IFormFile.FileName not present</param>
        /// <param name="virtualPath">Virtual path</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        Task<Video> InsertVideoAsync(string formFile, string defaultFileName = "", string virtualPath = "");

        /// <summary>
        /// Updates the video
        /// </summary>
        /// <param name="videoId">The video identifier</param>
        /// <param name="videoBinary">The video binary</param>
        /// <param name="mimeType">The video MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="altAttribute">"alt" attribute for "img" HTML element</param>
        /// <param name="titleAttribute">"title" attribute for "img" HTML element</param>
        /// <param name="isNew">A value indicating whether the video is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided video binary</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        Task<Video> UpdateVideoAsync(int videoId, byte[] videoBinary, string mimeType,
            string seoFilename, string altAttribute = null, string titleAttribute = null,
            bool isNew = true, bool validateBinary = true);

        /// <summary>
        /// Updates the video
        /// </summary>
        /// <param name="video">The video to update</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        Task<Video> UpdateVideoAsync(Video video);

        /// <summary>
        /// Updates a SEO filename of a video
        /// </summary>
        /// <param name="videoId">The video identifier</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        Task<Video> SetSeoFilenameAsync(int videoId, string seoFilename);

        /// <summary>
        /// Validates input video dimensions
        /// </summary>
        /// <param name="videoBinary">Video binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video binary or throws an exception
        /// </returns>
        Task<byte[]> ValidateVideoAsync(byte[] videoBinary, string mimeType);

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
        /// Get product video (for shopping cart and order details pages)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes (in XML format)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        //Task<Video> GetProductVideoAsync(Product product, string attributesXml);

        /// <summary>
        /// Get product video binary by video identifier
        /// </summary>
        /// <param name="videoId">The video identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video binary
        /// </returns>
        Task<VideoBinary> GetVideoBinaryByVideoIdAsync(int videoId);
    }
}