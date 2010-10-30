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
using NopSolutions.NopCommerce.BusinessLogic.IoC;
using System.Data.Objects;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.BusinessLogic.Media
{
    /// <summary>
    /// Picture manager
    /// </summary>
    public partial class PictureManager : IPictureManager
    {
        #region Fields

        private static object s_lock = new object();

        /// <summary>
        /// object context
        /// </summary>
        protected NopObjectContext _context;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public PictureManager(NopObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Returns the first ImageCodecInfo instance with the specified mime type.
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>ImageCodecInfo</returns>
        private ImageCodecInfo GetImageCodecInfoFromMimeType(string mimeType)
        {
            var info = ImageCodecInfo.GetImageEncoders();
            foreach (var ici in info)
                if (ici.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase)) 
                    return ici;
            return null;
        }

        /// <summary>
        /// Returns the first ImageCodecInfo instance with the specified extension.
        /// </summary>
        /// <param name="fileExt">File extension</param>
        /// <returns>ImageCodecInfo</returns>
        private ImageCodecInfo GetImageCodecInfoFromExtension(string fileExt)
        {
            fileExt = fileExt.TrimStart(".".ToCharArray()).ToLower().Trim();
            switch (fileExt)
            {
                case "jpg":
                case "jpeg":
                    return GetImageCodecInfoFromMimeType("image/jpeg");
                case "png":
                    return GetImageCodecInfoFromMimeType("image/png");
                case "gif":
                    //use png codec for gif to preserve transparency
                    //return GetImageCodecInfoFromMimeType("image/gif");
                    return GetImageCodecInfoFromMimeType("image/png");
                default:
                    return GetImageCodecInfoFromMimeType("image/jpeg");
            }
        }

        /// <summary>
        /// Save picture on file system
        /// </summary>
        /// <param name="PictureId">Picture identifier</param>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        private void SavePictureInFile(int PictureId, byte[] pictureBinary, string mimeType)
        {
            string[] parts = mimeType.Split('/');
            string lastPart = parts[parts.Length - 1];
            switch(lastPart)
            {
                case "pjpeg":
                    lastPart = "jpg";
                    break;
                case "x-png":
                    lastPart = "png";
                    break;
                case "x-icon":
                    lastPart = "ico";
                    break;
            }
            string localFilename = string.Format("{0}_0.{1}", PictureId.ToString("0000000"), lastPart);            
            File.WriteAllBytes(Path.Combine(LocalImagePath, localFilename), pictureBinary);
        }

        /// <summary>
        /// Delete a picture on file system
        /// </summary>
        /// <param name="picture">Picture</param>
        private void DeletePictureOnFileSystem(Picture picture)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            string[] parts = picture.MimeType.Split('/');
            string lastPart = parts[parts.Length - 1];
            switch (lastPart)
            {
                case "pjpeg":
                    lastPart = "jpg";
                    break;
                case "x-png":
                    lastPart = "png";
                    break;
                case "x-icon":
                    lastPart = "ico";
                    break;
            }
            string localFilename = string.Format("{0}_0.{1}", picture.PictureId.ToString("0000000"), lastPart);
            string localFilepath = Path.Combine(LocalImagePath, localFilename);
            if (File.Exists(localFilepath))
            {
                File.Delete(localFilepath);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the default picture URL
        /// </summary>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <returns></returns>
        public string GetDefaultPictureUrl(int targetSize)
        {
            return GetDefaultPictureUrl(PictureTypeEnum.Entity, targetSize);
        }

        /// <summary>
        /// Gets the default picture URL
        /// </summary>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <returns></returns>
        public string GetDefaultPictureUrl(PictureTypeEnum defaultPictureType,
            int targetSize)
        {
            string defaultImageName = string.Empty;
            switch (defaultPictureType)
            {
                case PictureTypeEnum.Entity:
                    defaultImageName = IoCFactory.Resolve<ISettingManager>().GetSettingValue("Media.DefaultImageName");
                    break;
                case PictureTypeEnum.Avatar:
                    defaultImageName = IoCFactory.Resolve<ISettingManager>().GetSettingValue("Media.Customer.DefaultAvatarImageName");
                    break;
                default:
                    defaultImageName = IoCFactory.Resolve<ISettingManager>().GetSettingValue("Media.DefaultImageName");
                    break;
            }


            string relPath = CommonHelper.GetStoreLocation() + "images/" + defaultImageName;
            if (targetSize == 0)
                return relPath;
            else
            {
                string filePath = Path.Combine(LocalImagePath, defaultImageName);
                if (File.Exists(filePath))
                {
                    string fileExtension = Path.GetExtension(filePath);
                    string fname = string.Format("{0}_{1}{2}",
                        Path.GetFileNameWithoutExtension(filePath),
                        targetSize,
                        fileExtension);
                    if (!File.Exists(Path.Combine(LocalThumbImagePath, fname)))
                    {
                        var b = new Bitmap(filePath);

                        var newSize = CalculateDimensions(b.Size, targetSize);

                        if (newSize.Width < 1)
                            newSize.Width = 1;
                        if (newSize.Height < 1)
                            newSize.Height = 1;

                        var newBitMap = new Bitmap(newSize.Width, newSize.Height);
                        var g = Graphics.FromImage(newBitMap);
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        g.DrawImage(b, 0, 0, newSize.Width, newSize.Height);
                        var ep = new EncoderParameters();
                        ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, this.ImageQuality);
                        ImageCodecInfo ici = GetImageCodecInfoFromExtension(fileExtension);
                        if (ici == null)
                            ici = GetImageCodecInfoFromMimeType("image/jpeg");
                        newBitMap.Save(Path.Combine(LocalThumbImagePath, fname), ici, ep);
                        newBitMap.Dispose();
                        b.Dispose();
                    }
                    return CommonHelper.GetStoreLocation() + "images/thumbs/" + fname;
                }
                return relPath;
            }
        }

        /// <summary>
        /// Loads a cpiture from file
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>Picture binary</returns>
        public byte[] LoadPictureFromFile(int pictureId, string mimeType)
        {
            string[] parts = mimeType.Split('/');
            string lastPart = parts[parts.Length - 1];
            switch (lastPart)
            {
                case "pjpeg":
                    lastPart = "jpg";
                    break;
                case "x-png":
                    lastPart = "png";
                    break;
                case "x-icon":
                    lastPart = "ico";
                    break;
            }
            string localFilename = string.Empty;
            localFilename = string.Format("{0}_0.{1}", pictureId.ToString("0000000"), lastPart);
            if (!File.Exists(Path.Combine(LocalImagePath, localFilename)))
            {
                return new byte[0];
            }
            return File.ReadAllBytes(Path.Combine(LocalImagePath, localFilename));
        }

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="imageId">Picture identifier</param>
        /// <returns>Picture URL</returns>
        public string GetPictureUrl(int imageId)
        {
            Picture picture = GetPictureById(imageId);
            return GetPictureUrl(picture);
        }

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <returns>Picture URL</returns>
        public string GetPictureUrl(Picture picture)
        {
            return GetPictureUrl(picture, 0);
        }

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="imageId">Picture identifier</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <returns>Picture URL</returns>
        public string GetPictureUrl(int imageId, int targetSize)
        {
            var picture = GetPictureById(imageId);
            return GetPictureUrl(picture, targetSize);
        }

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <returns>Picture URL</returns>
        public string GetPictureUrl(Picture picture, int targetSize)
        {
            return GetPictureUrl(picture, targetSize, true);
        }

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="imageId">Picture identifier</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns></returns>
        public string GetPictureUrl(int imageId, int targetSize, 
            bool showDefaultPicture)
        {
            var picture = GetPictureById(imageId);
            return GetPictureUrl(picture, targetSize, showDefaultPicture);
        }
        
        /// <summary>
        /// Gets all picture urls as a string array
        /// </summary>
        /// <param name="pictureId">Id of picture</param>
        /// <returns>Array containing urls for a picture in all sizes avaliable</returns>
        public List<String> GetPictureUrls(int pictureId)
        {
            string filter = string.Format("*{0}*.*", pictureId.ToString("0000000"));

            List<String> urls = new List<string>();

            string orginalURL = GetPictureUrl(pictureId);

            string[] currentFiles = System.IO.Directory.GetFiles(this.LocalThumbImagePath, filter);

            foreach (string currentFileName in currentFiles)
            {
                string url = CommonHelper.GetStoreLocation() + "images/thumbs/" + Path.GetFileName(currentFileName);

                if (url != orginalURL)
                    urls.Add(url);
            }
            
            //add original picture to array
            urls.Add(orginalURL);

            if (urls.Count > 0)
            {
                //reverse sort order (this way the biggest picture usally comes first..)
                urls.Reverse();
            }

            return urls;
        }

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns></returns>
        public string GetPictureUrl(Picture picture, int targetSize,
            bool showDefaultPicture)
        {
            string url = string.Empty;
            if (picture == null || picture.LoadPictureBinary().Length == 0)
            {
                if(showDefaultPicture)
                {
                    url = GetDefaultPictureUrl(targetSize);
                }
                return url;
            }

            string[] parts = picture.MimeType.Split('/');
            string lastPart = parts[parts.Length - 1];
            switch (lastPart)
            {
                case "pjpeg":
                    lastPart = "jpg";
                    break;
                case "x-png":
                    lastPart = "png";
                    break;
                case "x-icon":
                    lastPart = "ico";
                    break;
            }

            string localFilename = string.Empty;
            if (picture.IsNew)
            {
                string filter = string.Format("{0}*.*", picture.PictureId.ToString("0000000"));
                string[] currentFiles = System.IO.Directory.GetFiles(this.LocalThumbImagePath, filter);
                foreach (string currentFileName in currentFiles)
                    File.Delete(Path.Combine(this.LocalThumbImagePath, currentFileName));

                picture = UpdatePicture(picture.PictureId, picture.LoadPictureBinary(), picture.MimeType, false);
            }
            lock (s_lock)
            {
                if (targetSize == 0)
                {
                    localFilename = string.Format("{0}.{1}", picture.PictureId.ToString("0000000"), lastPart);
                    if (!File.Exists(Path.Combine(this.LocalThumbImagePath, localFilename)))
                    {
                        if (!System.IO.Directory.Exists(this.LocalThumbImagePath))
                        {
                            System.IO.Directory.CreateDirectory(this.LocalThumbImagePath);
                        }
                        File.WriteAllBytes(Path.Combine(this.LocalThumbImagePath, localFilename), picture.LoadPictureBinary());
                    }
                }
                else
                {
                    localFilename = string.Format("{0}_{1}.{2}", picture.PictureId.ToString("0000000"), targetSize, lastPart);
                    if (!File.Exists(Path.Combine(this.LocalThumbImagePath, localFilename)))
                    {
                        if (!System.IO.Directory.Exists(this.LocalThumbImagePath))
                        {
                            System.IO.Directory.CreateDirectory(this.LocalThumbImagePath);
                        }
                        using (MemoryStream stream = new MemoryStream(picture.LoadPictureBinary()))
                        {
                            var b = new Bitmap(stream);

                            var newSize = CalculateDimensions(b.Size, targetSize);

                            if (newSize.Width < 1)
                                newSize.Width = 1;
                            if (newSize.Height < 1)
                                newSize.Height = 1;

                            var newBitMap = new Bitmap(newSize.Width, newSize.Height);
                            var g = Graphics.FromImage(newBitMap);
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            g.DrawImage(b, 0, 0, newSize.Width, newSize.Height);
                            var ep = new EncoderParameters();
                            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, this.ImageQuality);
                            ImageCodecInfo ici = GetImageCodecInfoFromExtension(lastPart);
                            if (ici == null)
                                ici = GetImageCodecInfoFromMimeType("image/jpeg");
                            newBitMap.Save(Path.Combine(this.LocalThumbImagePath, localFilename), ici, ep);
                            newBitMap.Dispose();
                            b.Dispose();
                        }
                    }
                }
            }
            url = CommonHelper.GetStoreLocation() + "images/thumbs/" + localFilename;
            return url;
        }

        /// <summary>
        /// Get a picture local path
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns></returns>
        public string GetPictureLocalPath(Picture picture, int targetSize, bool showDefaultPicture)
        {
            string url = GetPictureUrl(picture, targetSize, showDefaultPicture);
            if(String.IsNullOrEmpty(url))
            {
                return String.Empty;
            }
            else
            {
                return Path.Combine(this.LocalThumbImagePath, Path.GetFileName(url));
            }
        }

        /// <summary>
        /// Calculates picture dimensions whilst maintaining aspect
        /// </summary>
        /// <param name="originalSize">The original picture size</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <returns></returns>
        public Size CalculateDimensions(Size originalSize, int targetSize)
        {
            var newSize = new Size();
            if (originalSize.Height > originalSize.Width) // portrait 
            {
                newSize.Width = (int)(originalSize.Width * (float)(targetSize / (float)originalSize.Height));
                newSize.Height = targetSize;
            }
            else // landscape or square
            {
                newSize.Height = (int)(originalSize.Height * (float)(targetSize / (float)originalSize.Width));
                newSize.Width = targetSize;
            }
            return newSize;
        }

        /// <summary>
        /// Gets a picture
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <returns>Picture</returns>
        public Picture GetPictureById(int pictureId)
        {
            if (pictureId == 0)
                return null;

            
            var query = from p in _context.Pictures
                        where p.PictureId == pictureId
                        select p;
            var picture = query.SingleOrDefault();

            return picture;
        }

        /// <summary>
        /// Deletes a picture
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        public void DeletePicture(int pictureId)
        {
            var picture = GetPictureById(pictureId);
            if (picture == null)
                return;

            //delete thumbs
            string filter = string.Format("{0}*.*", pictureId.ToString("0000000"));
            string[] currentFiles = System.IO.Directory.GetFiles(this.LocalThumbImagePath, filter);
            foreach (string currentFileName in currentFiles)
                File.Delete(Path.Combine(this.LocalThumbImagePath, currentFileName));
            
            //delete from file system
            if (!this.StoreInDB)
            {
                DeletePictureOnFileSystem(picture);
            }

            //delete from database
            
            if (!_context.IsAttached(picture))
                _context.Pictures.Attach(picture);
            _context.DeleteObject(picture);
            _context.SaveChanges();
        }

        /// <summary>
        /// Validates input picture dimensions
        /// </summary>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>Picture binary or throws an exception</returns>
        public byte[] ValidatePicture(byte[] pictureBinary, string mimeType)
        {
            using (MemoryStream stream = new MemoryStream(pictureBinary))
            {
                var b = new Bitmap(stream);
                int maxSize = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("Media.MaximumImageSize", 1280);

                if ((b.Height > maxSize) || (b.Width > maxSize))
                {
                    var newSize = CalculateDimensions(b.Size, maxSize);
                    var newBitMap = new Bitmap(newSize.Width, newSize.Height);
                    var g = Graphics.FromImage(newBitMap);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.DrawImage(b, 0, 0, newSize.Width, newSize.Height);

                    var m = new MemoryStream();
                    var ep = new EncoderParameters();
                    ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, this.ImageQuality);
                    ImageCodecInfo ici = GetImageCodecInfoFromMimeType(mimeType);
                    if (ici == null)
                        ici = GetImageCodecInfoFromMimeType("image/jpeg");
                    newBitMap.Save(m, ici, ep);
                    newBitMap.Dispose();
                    b.Dispose();

                    return m.GetBuffer();
                }
                else
                {
                    b.Dispose();
                    return pictureBinary;
                }
            }
        }
        
        /// <summary>
        /// Gets a collection of pictures
        /// </summary>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Items on each page</param>
        /// <returns>Paged list of pictures</returns>
        public PagedList<Picture> GetPictures(int pageSize, int pageIndex)
        {
            if (pageSize <= 0)
                pageSize = 10;
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            if (pageIndex < 0)
                pageIndex = 0;
            if (pageIndex == int.MaxValue)
                pageIndex = int.MaxValue - 1;


            
            var query = from p in _context.Pictures
                       orderby p.PictureId descending
                       select p;
            var pics = new PagedList<Picture>(query, pageIndex, pageSize);
            return pics;
        }
        
        /// <summary>
        /// Gets pictures by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Pictures</returns>
        public List<Picture> GetPicturesByProductId(int productId)
        {
            return GetPicturesByProductId(productId, 0);
        }

        /// <summary>
        /// Gets pictures by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="recordsToReturn">Number of records to return. 0 if you want to get all items</param>
        /// <returns>Pictures</returns>
        public List<Picture> GetPicturesByProductId(int productId,
            int recordsToReturn)
        {
            if (productId == 0)
                return new List<Picture>();

            
            var query = from p in _context.Pictures
                        join pp in _context.ProductPictures on p.PictureId equals pp.PictureId
                        orderby pp.DisplayOrder
                        where pp.ProductId == productId
                        select p;

            if (recordsToReturn > 0)
                query = query.Take(recordsToReturn);

            var pics = query.ToList();
            return pics;
        }

        /// <summary>
        /// Inserts a picture
        /// </summary>
        /// <param name="pictureBinary">The picture binary</param>
        /// <param name="mimeType">The picture MIME type</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <returns>Picture</returns>
        public Picture InsertPicture(byte[] pictureBinary, string mimeType, bool isNew)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            pictureBinary = ValidatePicture(pictureBinary, mimeType);

            
            var picture = _context.Pictures.CreateObject();
            picture.PictureBinary = (this.StoreInDB ? pictureBinary : new byte[0]);
            picture.MimeType = mimeType;
            picture.IsNew = isNew;

            _context.Pictures.AddObject(picture);
            _context.SaveChanges();

            if(!this.StoreInDB && picture != null)
            {
                SavePictureInFile(picture.PictureId, pictureBinary, mimeType);
            }
            return picture;
        }

        /// <summary>
        /// Updates the picture
        /// </summary>
        /// <param name="pictureId">The picture identifier</param>
        /// <param name="pictureBinary">The picture binary</param>
        /// <param name="mimeType">The picture MIME type</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <returns>Picture</returns>
        public Picture UpdatePicture(int pictureId, byte[] pictureBinary,
            string mimeType, bool isNew)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            ValidatePicture(pictureBinary, mimeType);

            var picture = GetPictureById(pictureId);
            if (picture == null)
                return null;

            
            if (!_context.IsAttached(picture))
                _context.Pictures.Attach(picture);

            picture.PictureBinary = (this.StoreInDB ? pictureBinary : new byte[0]);
            picture.MimeType = mimeType;
            picture.IsNew = isNew;
            _context.SaveChanges();

            if(!this.StoreInDB && picture != null)
            {
                SavePictureInFile(picture.PictureId, pictureBinary, mimeType);
            }
            return picture;
        }

        /// <summary>
        /// Gets the picture binary array
        /// </summary>
        /// <param name="fs">File stream</param>
        /// <param name="size">Picture size</param>
        /// <returns>Picture binary array</returns>
        public byte[] GetPictureBits(Stream fs, int size)
        {
            byte[] img = new byte[size];
            fs.Read(img, 0, size);
            return img;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets an image quality
        /// </summary>
        public long ImageQuality
        {
            get
            {
                return 100L;
            }
        }

        /// <summary>
        /// Gets a local thumb image path
        /// </summary>
        public string LocalThumbImagePath
        {
            get
            {
                string path = HttpContext.Current.Request.PhysicalApplicationPath + "images\\thumbs";
                return path;
            }
        }

        /// <summary>
        /// Gets the local image path
        /// </summary>
        public string LocalImagePath
        {
            get
            {
                string path = HttpContext.Current.Request.PhysicalApplicationPath + "images";
                return path;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the images should be stored in data base.
        /// </summary>
        public bool StoreInDB
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Media.Images.StoreInDB", true);
            }
            set
            {
                //check whether it's a new value
                if (this.StoreInDB != value)
                {
                    //save the nwe setting value
                    IoCFactory.Resolve<ISettingManager>().SetParam("Media.Images.StoreInDB", value.ToString());

                    //update all picture objects
                    int totalRecords = 0;
                    var pictures = this.GetPictures(int.MaxValue, 0);
                    for (int i = 0; i < pictures.Count; i++)
                    {
                        var picture = pictures[i];
                        var pictureBinary = picture.LoadPictureBinary(!value);
                        
                        //delete from file system
                        if (value)
                        {
                            DeletePictureOnFileSystem(picture);
                        }

                        //just update a picture (all required logic is in UpdatePicture method)
                        picture = UpdatePicture(picture.PictureId,
                            pictureBinary,
                            picture.MimeType,
                            true);
                    }
                }
            }
        }
        #endregion
    }
}
