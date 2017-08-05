using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Catalog;

namespace Nop.Services.Media
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the download binary array
        /// </summary>
        /// <param name="file">File</param>
        /// <returns>Download binary array</returns>
        public static byte[] GetDownloadBits(this IFormFile file)
        {
            using (var fileStream = file.OpenReadStream())
            using (var ms = new MemoryStream())
            {
                fileStream.CopyTo(ms);
                var fileBytes = ms.ToArray();
                return fileBytes;
            }
        }

        /// <summary>
        /// Gets the picture binary array
        /// </summary>
        /// <param name="file">File</param>
        /// <returns>Picture binary array</returns>
        public static byte[] GetPictureBits(this IFormFile file)
        {
            return GetDownloadBits(file);
        }

        /// <summary>
        /// Get product picture (for shopping cart and order details pages)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Atributes (in XML format)</param>
        /// <param name="pictureService">Picture service</param>
        /// <param name="productAttributeParser">Product attribute service</param>
        /// <returns>Picture</returns>
        public static Picture GetProductPicture(this Product product, string attributesXml,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (pictureService == null)
                throw new ArgumentNullException(nameof(pictureService));
            if (productAttributeParser == null)
                throw new ArgumentNullException(nameof(productAttributeParser));

            Picture picture = null;

            //first, let's see whether we have some attribute values with custom pictures
            var attributeValues = productAttributeParser.ParseProductAttributeValues(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                var attributePicture = pictureService.GetPictureById(attributeValue.PictureId);
                if (attributePicture != null)
                {
                    picture = attributePicture;
                    break;
                }
            }

            //now let's load the default product picture
            if (picture == null)
            {
                picture = pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
            }

            //let's check whether this product has some parent "grouped" product
            if (picture == null && !product.VisibleIndividually && product.ParentGroupedProductId > 0)
            {
                picture = pictureService.GetPicturesByProductId(product.ParentGroupedProductId, 1).FirstOrDefault();
            }

            return picture;
        }
    }
}
