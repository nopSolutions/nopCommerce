//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.BusinessLogic.Media
{
    /// <summary>
    /// Picture service interface
    /// </summary>
    public partial interface IPictureService
    {
        /// <summary>
        /// Gets the default picture URL
        /// </summary>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <returns></returns>
        string GetDefaultPictureUrl(int targetSize);

        /// <summary>
        /// Gets the default picture URL
        /// </summary>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <returns></returns>
        string GetDefaultPictureUrl(PictureTypeEnum defaultPictureType,
            int targetSize);

        /// <summary>
        /// Loads a cpiture from file
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>Picture binary</returns>
        byte[] LoadPictureFromFile(int pictureId, string mimeType);

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="imageId">Picture identifier</param>
        /// <returns>Picture URL</returns>
        string GetPictureUrl(int imageId);

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <returns>Picture URL</returns>
        string GetPictureUrl(Picture picture);

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="imageId">Picture identifier</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <returns>Picture URL</returns>
        string GetPictureUrl(int imageId, int targetSize);

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <returns>Picture URL</returns>
        string GetPictureUrl(Picture picture, int targetSize);

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="imageId">Picture identifier</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns></returns>
        string GetPictureUrl(int imageId, int targetSize,
            bool showDefaultPicture);
        
        /// <summary>
        /// Gets all picture urls as a string array
        /// </summary>
        /// <param name="pictureId">Id of picture</param>
        /// <returns>Array containing urls for a picture in all sizes avaliable</returns>
        List<String> GetPictureUrls(int pictureId);

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns></returns>
        string GetPictureUrl(Picture picture, int targetSize,
            bool showDefaultPicture);

        /// <summary>
        /// Get a picture local path
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns></returns>
        string GetPictureLocalPath(Picture picture, int targetSize, bool showDefaultPicture);

        /// <summary>
        /// Gets a picture
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <returns>Picture</returns>
        Picture GetPictureById(int pictureId);

        /// <summary>
        /// Deletes a picture
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        void DeletePicture(int pictureId);

        /// <summary>
        /// Validates input picture dimensions
        /// </summary>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>Picture binary or throws an exception</returns>
        byte[] ValidatePicture(byte[] pictureBinary, string mimeType);
        
        /// <summary>
        /// Gets a collection of pictures
        /// </summary>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Items on each page</param>
        /// <returns>Paged list of pictures</returns>
        PagedList<Picture> GetPictures(int pageSize, int pageIndex);
        
        /// <summary>
        /// Gets pictures by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Pictures</returns>
        List<Picture> GetPicturesByProductId(int productId);

        /// <summary>
        /// Gets pictures by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="recordsToReturn">Number of records to return. 0 if you want to get all items</param>
        /// <returns>Pictures</returns>
        List<Picture> GetPicturesByProductId(int productId,
            int recordsToReturn);

        /// <summary>
        /// Inserts a picture
        /// </summary>
        /// <param name="pictureBinary">The picture binary</param>
        /// <param name="mimeType">The picture MIME type</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <returns>Picture</returns>
        Picture InsertPicture(byte[] pictureBinary, string mimeType, bool isNew);

        /// <summary>
        /// Updates the picture
        /// </summary>
        /// <param name="pictureId">The picture identifier</param>
        /// <param name="pictureBinary">The picture binary</param>
        /// <param name="mimeType">The picture MIME type</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <returns>Picture</returns>
        Picture UpdatePicture(int pictureId, byte[] pictureBinary,
            string mimeType, bool isNew);

        /// <summary>
        /// Gets an image quality
        /// </summary>
        long ImageQuality { get; }

        /// <summary>
        /// Gets a local thumb image path
        /// </summary>
        string LocalThumbImagePath { get;}

        /// <summary>
        /// Gets the local image path
        /// </summary>
        string LocalImagePath { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the images should be stored in data base.
        /// </summary>
        bool StoreInDB { get; set; }
    }
}
