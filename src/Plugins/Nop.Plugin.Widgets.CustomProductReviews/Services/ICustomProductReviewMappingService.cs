using System.Collections.Generic;
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
    public partial interface ICustomProductReviewMappingService
    {

        /// <summary>
        /// Deletes a customProductReviewMapping
        /// </summary>
        /// <param name="customProductReviewMapping">Video</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteCustomProductReviewMappingAsync(CustomProductReviewMapping customProductReviewMapping);



        /// </returns>
        Task<CustomProductReviewMapping> InsertCustomProductReviewMappingAsync(int productReviewId, int? pictureId, int? videoIdId);


        /// <summary>
        /// Updates the customProductReviewMapping
        /// </summary>
        /// <param name="videoId">The customProductReviewMapping identifier</param>
        /// <param name="videoBinary">The customProductReviewMapping binary</param>
        /// <param name="mimeType">The customProductReviewMapping MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="altAttribute">"alt" attribute for "img" HTML element</param>
        /// <param name="titleAttribute">"title" attribute for "img" HTML element</param>
        /// <param name="isNew">A value indicating whether the customProductReviewMapping is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided customProductReviewMapping binary</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customProductReviewMapping
        /// </returns>
        Task<CustomProductReviewMapping> UpdateCustomProductReviewMappingAsync(int customProductReviewId, int productReviewId, int pictureId, int videoIdId);

        /// <summary>
        /// Updates the customProductReviewMapping
        /// </summary>
   
        Task<CustomProductReviewMapping> UpdateCustomProductReviewMappingAsync(CustomProductReviewMapping customProductReviewMapping);

        Task<CustomProductReviewMapping> GetCustomProductReviewMappingByIdAsync(int customProductReviewMappingId);







    }
}