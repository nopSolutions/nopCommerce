using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB.Common;
using Microsoft.AspNetCore.Http;
using Nop.Core;

using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Widgets.CustomCustomProductReviews;
using Nop.Plugin.Widgets.CustomProductReviews.Domains;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Services.Seo;


namespace Nop.Plugin.Widgets.CustomProductReviews.Services
{
    /// <summary>
    /// CustomProductReviewMapping service
    /// </summary>
    public partial class CustomProductReviewMappingService : ICustomProductReviewMappingService
    {
        #region Fields

        private readonly IRepository<CustomProductReviewMapping> _customProductReviewMappingRepository;
        //private readonly IRepository<ProductCustomProductReviewMapping> _productCustomProductReviewMappingRepository;

        #endregion

        #region Ctor

        public CustomProductReviewMappingService(IRepository<CustomProductReviewMapping> customProductReviewMappingRepository)
        {
            _customProductReviewMappingRepository = customProductReviewMappingRepository;
        }

        #endregion




        #region CRUD methods

        /// <summary>
        /// Inserts a customProductReviewMapping
        /// </summary>
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
        public virtual async Task<CustomProductReviewMapping> InsertCustomProductReviewMappingAsync(int productReviewId, int pictureId, int videoIdId)
        {


            var customProductReviewMapping = new CustomProductReviewMapping
            {
                ProductReviewId = productReviewId,
                PictureId = pictureId,
                VideoId = videoIdId
            };

            await _customProductReviewMappingRepository.InsertAsync(customProductReviewMapping);

            return customProductReviewMapping;
        }

        /// <summary>
        /// Updates the customProductReviewMapping
        /// </summary>
        /// <param name="customProductReviewMappingId">The customProductReviewMapping identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customProductReviewMapping
        /// </returns>
        public virtual async Task<CustomProductReviewMapping> UpdateCustomProductReviewMappingAsync(int customProductReviewMappingId, int productReviewId, int pictureId, int videoIdId)
        {

            var customProductReviewMapping = await GetCustomProductReviewMappingByIdAsync(customProductReviewMappingId);
            if (customProductReviewMapping == null)
                return null;


            customProductReviewMapping.PictureId = pictureId;
            customProductReviewMapping.VideoId = videoIdId;
            customProductReviewMapping.ProductReviewId = productReviewId;

            await _customProductReviewMappingRepository.UpdateAsync(customProductReviewMapping);

            return customProductReviewMapping;
        }

        /// <summary>
        /// Updates the customProductReviewMapping
        /// </summary>
        /// <param name="customProductReviewMapping">The customProductReviewMapping to update</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customProductReviewMapping
        /// </returns>
        public virtual async Task<CustomProductReviewMapping> UpdateCustomProductReviewMappingAsync(CustomProductReviewMapping customProductReviewMapping)
        {
            if (customProductReviewMapping == null)
                return null;

            await _customProductReviewMappingRepository.UpdateAsync(customProductReviewMapping);

            return customProductReviewMapping;
        }

        /// <summary>
        /// Deletes a customProductReviewMapping
        /// </summary>
        /// <param name="customProductReviewMapping">CustomProductReviewMapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCustomProductReviewMappingAsync(CustomProductReviewMapping customProductReviewMapping)
        {
            if (customProductReviewMapping == null)
                throw new ArgumentNullException(nameof(customProductReviewMapping));

            //delete from database
            await _customProductReviewMappingRepository.DeleteAsync(customProductReviewMapping);
        }


        /// <summary>
        /// Gets a customProductReviewMapping
        /// </summary>
        /// <param name="customProductReviewMappingId">CustomProductReviewMapping identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customProductReviewMapping
        /// </returns>
        public virtual async Task<CustomProductReviewMapping> GetCustomProductReviewMappingByIdAsync(int customProductReviewMappingId)
        {
            return await _customProductReviewMappingRepository.GetByIdAsync(customProductReviewMappingId, cache => default);
        }

       

 
        /// <summary>
        /// Gets pictures by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="recordsToReturn">Number of records to return. 0 if you want to get all items</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the pictures
        /// </returns>
        //public virtual async Task<IList<CustomProductReviewMapping>> GetCustomProductReviewMappingsByProductIdAsync(int productId, int recordsToReturn = 0)
        //{
        //    if (productId == 0)
        //        return new List<CustomProductReviewMapping>();

        //    var query = from p in _customProductReviewMappingRepository.Table
        //                join pp in _productCustomProductReviewMappingRepository.Table on p.Id equals pp.customProductReviewMappingId
        //                orderby pp.DisplayOrder, pp.Id
        //                where pp.ProductId == productId
        //                select p;

        //    if (recordsToReturn > 0)
        //        query = query.Take(recordsToReturn);

        //    var pics = await query.ToListAsync();

        //    return pics;
        //}


  




        /// <summary>
        /// Get product customProductReviewMapping (for shopping cart and order details pages)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes (in XML format)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customProductReviewMapping
        /// </returns>
        //public virtual async Task<CustomProductReviewMapping> GetProductCustomProductReviewMappingAsync(Product product, string attributesXml)
        //{
        //    if (product == null)
        //        throw new ArgumentNullException(nameof(product));

        //    //first, try to get product attribute combination customProductReviewMapping
        //    var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
        //    var combinationCustomProductReviewMapping = await GetCustomProductReviewMappingByIdAsync(combination?.customProductReviewMappingId ?? 0);
        //    if (combinationCustomProductReviewMapping != null)
        //        return combinationCustomProductReviewMapping;

        //    //then, let's see whether we have attribute values with pictures
        //    var attributeCustomProductReviewMapping = await (await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml))
        //        .SelectAwait(async attributeValue => await GetCustomProductReviewMappingByIdAsync(attributeValue?.customProductReviewMappingId ?? 0))
        //        .FirstOrDefaultAsync(customProductReviewMapping => customProductReviewMapping != null);
        //    if (attributeCustomProductReviewMapping != null)
        //        return attributeCustomProductReviewMapping;

        //    //now let's load the default product customProductReviewMapping
        //    var productCustomProductReviewMapping = (await GetCustomProductReviewMappingsByProductIdAsync(product.Id, 1)).FirstOrDefault();
        //    if (productCustomProductReviewMapping != null)
        //        return productCustomProductReviewMapping;

        //    //finally, let's check whether this product has some parent "grouped" product
        //    if (product.VisibleIndividually || product.ParentGroupedProductId <= 0)
        //        return null;

        //    var parentGroupedProductCustomProductReviewMapping = (await GetCustomProductReviewMappingsByProductIdAsync(product.ParentGroupedProductId, 1)).FirstOrDefault();
        //    return parentGroupedProductCustomProductReviewMapping;
        //}

        /// <summary>
        /// Gets a value indicating whether the images should be stored in data base.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>

        

        #endregion
    }
}
