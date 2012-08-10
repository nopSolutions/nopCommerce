using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Seo;

namespace Nop.Services.Media
{
    /// <summary>
    /// Picture service
    /// </summary>
    public partial class PictureService : IPictureService
    {
        #region Fields

        private static readonly object s_lock = new object();

        private readonly IRepository<Picture> _pictureRepository;
        private readonly IRepository<ProductPicture> _productPictureRepository;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IEventPublisher _eventPublisher;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pictureRepository">Picture repository</param>
        /// <param name="productPictureRepository">Product picture repository</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="mediaSettings">Media settings</param>
        public PictureService(IRepository<Picture> pictureRepository,
            IRepository<ProductPicture> productPictureRepository,
            ISettingService settingService, IWebHelper webHelper,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings)
        {
            this._pictureRepository = pictureRepository;
            this._productPictureRepository = productPictureRepository;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._eventPublisher = eventPublisher;
            this._mediaSettings = mediaSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Returns the file extension from mime type.
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>File extension</returns>
        protected virtual string GetFileExtensionFromMimeType(string mimeType)
        {
            if (mimeType == null)
                return null;

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
            return lastPart;
        }

        /// <summary>
        /// Returns the first ImageCodecInfo instance with the specified mime type.
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>ImageCodecInfo</returns>
        protected virtual  ImageCodecInfo GetImageCodecInfoFromMimeType(string mimeType)
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
        protected virtual  ImageCodecInfo GetImageCodecInfoFromExtension(string fileExt)
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
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        protected virtual  void SavePictureInFile(int pictureId, byte[] pictureBinary, string mimeType)
        {
            string lastPart = GetFileExtensionFromMimeType(mimeType);
            string localFilename = string.Format("{0}_0.{1}", pictureId.ToString("0000000"), lastPart);            
            File.WriteAllBytes(Path.Combine(LocalImagePath, localFilename), pictureBinary);
        }

        /// <summary>
        /// Delete a picture on file system
        /// </summary>
        /// <param name="picture">Picture</param>
        protected virtual  void DeletePictureOnFileSystem(Picture picture)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            string lastPart = GetFileExtensionFromMimeType(picture.MimeType);
            string localFilename = string.Format("{0}_0.{1}", picture.Id.ToString("0000000"), lastPart);
            string localFilepath = Path.Combine(LocalImagePath, localFilename);
            if (File.Exists(localFilepath))
            {
                File.Delete(localFilepath);
            }
        }

        /// <summary>
        /// Calculates picture dimensions whilst maintaining aspect
        /// </summary>
        /// <param name="originalSize">The original picture size</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <returns></returns>
        protected virtual  Size CalculateDimensions(Size originalSize, int targetSize)
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
        /// Delete picture thumbs
        /// </summary>
        /// <param name="picture">Picture</param>
        protected virtual  void DeletePictureThumbs(Picture picture)
        {
            string filter = string.Format("{0}*.*", picture.Id.ToString("0000000"));
            string[] currentFiles = System.IO.Directory.GetFiles(this.LocalThumbImagePath, filter);
            foreach (string currentFileName in currentFiles)
                File.Delete(Path.Combine(this.LocalThumbImagePath, currentFileName));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get picture SEO friendly name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        public virtual string GetPictureSeName(string name)
        {
            //if (String.IsNullOrEmpty(name))
            //    return name;

            //string okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            //name = name.Trim().ToLowerInvariant();

            //var sb = new StringBuilder();
            //foreach (char c in name.ToCharArray())
            //{
            //    string c2 = c.ToString();
            //    if (okChars.Contains(c2))
            //        sb.Append(c2);
            //}
            //string name2 = sb.ToString();
            //name2 = name2.Replace(" ", "_");
            //name2 = name2.Replace("-", "_");
            //while (name2.Contains("__"))
            //    name2 = name2.Replace("__", "_");
            //return name2.ToLowerInvariant();

            //use SeoExtensions implementation
            return SeoExtensions.GetSeName(name, true, false, false);
        }

        /// <summary>
        /// Gets the default picture URL
        /// </summary>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <param name="useSsl">Value indicating whether to get SSL protected picture URL; null to use the same value as the current page</param>
        /// <returns>Picture URL</returns>
        public virtual string GetDefaultPictureUrl(int targetSize = 0, PictureType defaultPictureType = PictureType.Entity, bool? useSsl = null)
        {
            string defaultImageName;
            switch (defaultPictureType)
            {
                case PictureType.Entity:
                    defaultImageName = _settingService.GetSettingByKey("Media.DefaultImageName", "noDefaultImage.gif");
                    break;
                case PictureType.Avatar:
                    defaultImageName = _settingService.GetSettingByKey("Media.Customer.DefaultAvatarImageName", "defaultAvatar.jpg");
                    break;
                default:
                    defaultImageName = _settingService.GetSettingByKey("Media.DefaultImageName", "noDefaultImage.gif");
                    break;
            }


            string relPath = (useSsl.HasValue
                                 ? _webHelper.GetStoreLocation(useSsl.Value)
                                 : _webHelper.GetStoreLocation())
                                 + "content/images/" + defaultImageName;
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
                    return (useSsl.HasValue
                               ? _webHelper.GetStoreLocation(useSsl.Value)
                               : _webHelper.GetStoreLocation())
                               + "content/images/thumbs/" + fname;
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
        public virtual byte[] LoadPictureFromFile(int pictureId, string mimeType)
        {
            string lastPart = GetFileExtensionFromMimeType(mimeType);
            string localFilename = string.Format("{0}_0.{1}", pictureId.ToString("0000000"), lastPart);
            if (!File.Exists(Path.Combine(LocalImagePath, localFilename)))
                return new byte[0];
            return File.ReadAllBytes(Path.Combine(LocalImagePath, localFilename));
        }

        /// <summary>
        /// Gets the loaded picture binary depending on picture storage settings
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <returns>Picture binary</returns>
        public virtual byte[] LoadPictureBinary(Picture picture)
        {
            return LoadPictureBinary(picture, this.StoreInDb);
        }

        /// <summary>
        /// Gets the loaded picture binary depending on picture storage settings
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <param name="fromDb">Load from database; otherwise, from file system</param>
        /// <returns>Picture binary</returns>
        public virtual byte[] LoadPictureBinary(Picture picture, bool fromDb)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            byte[] result = null;
            if (fromDb)
                result = picture.PictureBinary;
            else
                result = LoadPictureFromFile(picture.Id, picture.MimeType);
            return result;
        }

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <param name="useSsl">Value indicating whether to get SSL protected picture URL; null to use the same value as the current page</param>
        /// <returns>Picture URL</returns>
        public virtual string GetPictureUrl(int pictureId, int targetSize = 0, bool showDefaultPicture = true, bool? useSsl = null)
        {
            var picture = GetPictureById(pictureId);
            return GetPictureUrl(picture, targetSize, showDefaultPicture, useSsl);
        }
        
        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <param name="useSsl">Value indicating whether to get SSL protected picture URL; null to use the same value as the current page</param>
        /// <returns>Picture URL</returns>
        public virtual string GetPictureUrl(Picture picture, int targetSize = 0, bool showDefaultPicture = true, bool? useSsl = null)
        {
            string url = string.Empty;
            if (picture == null || LoadPictureBinary(picture).Length == 0)
            {
                if(showDefaultPicture)
                {
                    url = GetDefaultPictureUrl(targetSize, useSsl: useSsl);
                }
                return url;
            }

            string lastPart = GetFileExtensionFromMimeType(picture.MimeType);
            string localFilename;
            if (picture.IsNew)
            {
                DeletePictureThumbs(picture);

                picture = UpdatePicture(picture.Id, LoadPictureBinary(picture), picture.MimeType, picture.SeoFilename, false);
            }
            lock (s_lock)
            {
                string seoFileName = picture.SeoFilename; // = GetPictureSeName(picture.SeoFilename); //just for sure
                if (targetSize == 0)
                {
                    localFilename = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("{0}_{1}.{2}", picture.Id.ToString("0000000"), seoFileName, lastPart) :
                        string.Format("{0}.{1}", picture.Id.ToString("0000000"), lastPart);

                    if (!File.Exists(Path.Combine(this.LocalThumbImagePath, localFilename)))
                    {
                        if (!System.IO.Directory.Exists(this.LocalThumbImagePath))
                        {
                            System.IO.Directory.CreateDirectory(this.LocalThumbImagePath);
                        }
                        File.WriteAllBytes(Path.Combine(this.LocalThumbImagePath, localFilename), LoadPictureBinary(picture));
                    }
                }
                else
                {
                    localFilename = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("{0}_{1}_{2}.{3}", picture.Id.ToString("0000000"), seoFileName, targetSize, lastPart) :
                        string.Format("{0}_{1}.{2}", picture.Id.ToString("0000000"), targetSize, lastPart);
                    if (!File.Exists(Path.Combine(this.LocalThumbImagePath, localFilename)))
                    {
                        if (!System.IO.Directory.Exists(this.LocalThumbImagePath))
                        {
                            System.IO.Directory.CreateDirectory(this.LocalThumbImagePath);
                        }
                        using (var stream = new MemoryStream(LoadPictureBinary(picture)))
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
            url = (useSsl.HasValue
                ? _webHelper.GetStoreLocation(useSsl.Value)
                : _webHelper.GetStoreLocation())
                + "content/images/thumbs/" + localFilename;
            return url;
        }

        /// <summary>
        /// Get a picture local path
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns></returns>
        public virtual string GetPictureLocalPath(Picture picture, int targetSize = 0, bool showDefaultPicture = true)
        {
            string url = GetPictureUrl(picture, targetSize, showDefaultPicture);
            if(String.IsNullOrEmpty(url))
                return String.Empty;
            else
                return Path.Combine(this.LocalThumbImagePath, Path.GetFileName(url));
        }

        /// <summary>
        /// Gets a picture
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <returns>Picture</returns>
        public virtual Picture GetPictureById(int pictureId)
        {
            if (pictureId == 0)
                return null;

            var picture = _pictureRepository.GetById(pictureId);
            return picture;
        }

        /// <summary>
        /// Deletes a picture
        /// </summary>
        /// <param name="picture">Picture</param>
        public virtual void DeletePicture(Picture picture)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            //delete thumbs
            DeletePictureThumbs(picture);
            
            //delete from file system
            if (!this.StoreInDb)
                DeletePictureOnFileSystem(picture);

            //delete from database
            _pictureRepository.Delete(picture);

            //event notification
            _eventPublisher.EntityDeleted(picture);
        }

        /// <summary>
        /// Validates input picture dimensions
        /// </summary>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>Picture binary or throws an exception</returns>
        public virtual byte[] ValidatePicture(byte[] pictureBinary, string mimeType)
        {
            using (var stream = new MemoryStream(pictureBinary))
            {
                var b = new Bitmap(stream);
                int maxSize = _mediaSettings.MaximumImageSize;

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
        public virtual IPagedList<Picture> GetPictures(int pageIndex, int pageSize)
        {
            var query = from p in _pictureRepository.Table
                       orderby p.Id descending
                       select p;
            var pics = new PagedList<Picture>(query, pageIndex, pageSize);
            return pics;
        }
        
        /// <summary>
        /// Gets pictures by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Pictures</returns>
        public virtual IList<Picture> GetPicturesByProductId(int productId)
        {
            return GetPicturesByProductId(productId, 0);
        }

        /// <summary>
        /// Gets pictures by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="recordsToReturn">Number of records to return. 0 if you want to get all items</param>
        /// <returns>Pictures</returns>
        public virtual IList<Picture> GetPicturesByProductId(int productId, int recordsToReturn)
        {
            if (productId == 0)
                return new List<Picture>();

            
            var query = from p in _pictureRepository.Table
                        join pp in _productPictureRepository.Table on p.Id equals pp.PictureId
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
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <returns>Picture</returns>
        public virtual Picture InsertPicture(byte[] pictureBinary, string mimeType, string seoFilename, bool isNew)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

            pictureBinary = ValidatePicture(pictureBinary, mimeType);

            
            var picture = new Picture();
            picture.PictureBinary = (this.StoreInDb ? pictureBinary : new byte[0]);
            picture.MimeType = mimeType;
            picture.SeoFilename = seoFilename;
            picture.IsNew = isNew;

            _pictureRepository.Insert(picture);

            if(!this.StoreInDb)
                SavePictureInFile(picture.Id, pictureBinary, mimeType);
            
            //event notification
            _eventPublisher.EntityInserted(picture);

            return picture;
        }

        /// <summary>
        /// Updates the picture
        /// </summary>
        /// <param name="pictureId">The picture identifier</param>
        /// <param name="pictureBinary">The picture binary</param>
        /// <param name="mimeType">The picture MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <returns>Picture</returns>
        public virtual Picture UpdatePicture(int pictureId, byte[] pictureBinary, string mimeType, string seoFilename, bool isNew)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

            ValidatePicture(pictureBinary, mimeType);

            var picture = GetPictureById(pictureId);
            if (picture == null)
                return null;

            //delete old thumbs if a picture has been changed
            if (seoFilename != picture.SeoFilename)
                DeletePictureThumbs(picture);

            picture.PictureBinary = (this.StoreInDb ? pictureBinary : new byte[0]);
            picture.MimeType = mimeType;
            picture.SeoFilename = seoFilename;
            picture.IsNew = isNew;

            _pictureRepository.Update(picture);

            if(!this.StoreInDb)
                SavePictureInFile(picture.Id, pictureBinary, mimeType);

            //event notification
            _eventPublisher.EntityUpdated(picture);

            return picture;
        }

        /// <summary>
        /// Updates a SEO filename of a picture
        /// </summary>
        /// <param name="pictureId">The picture identifier</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <returns>Picture</returns>
        public virtual Picture SetSeoFilename(int pictureId, string seoFilename)
        {
            var picture = GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            //update if it has been changed
            if (seoFilename != picture.SeoFilename)
            {
                //update picture
                picture = UpdatePicture(picture.Id, LoadPictureBinary(picture), picture.MimeType, seoFilename, true);
            }
            return picture;
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
                return _mediaSettings.DefaultImageQuality;
            }
        }

        /// <summary>
        /// Gets a local thumb image path
        /// </summary>
        protected virtual string LocalThumbImagePath
        {
            get
            {
                string path = _webHelper.MapPath("~/content/images/thumbs");
                return path;
            }
        }

        /// <summary>
        /// Gets the local image path
        /// </summary>
        protected virtual string LocalImagePath
        {
            get
            {
                string path = _webHelper.MapPath("~/content/images");
                return path;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the images should be stored in data base.
        /// </summary>
        public virtual bool StoreInDb
        {
            get
            {
                return _settingService.GetSettingByKey<bool>("Media.Images.StoreInDB", true);
            }
            set
            {
                //check whether it's a new value
                if (this.StoreInDb != value)
                {
                    //save the nwe setting value
                    _settingService.SetSetting<bool>("Media.Images.StoreInDB", value);

                    //update all picture objects
                    var pictures = this.GetPictures(0, int.MaxValue);
                    for (int i = 0; i < pictures.Count; i++)
                    {
                        var picture = pictures[i];
                        var pictureBinary = LoadPictureBinary(picture, !value);
                        
                        //delete from file system
                        if (value)
                            DeletePictureOnFileSystem(picture);

                        //just update a picture (all required logic is in UpdatePicture method)
                        picture = UpdatePicture(picture.Id,
                            pictureBinary,
                            picture.MimeType,
                            picture.SeoFilename,
                            true);
                    }
                }
            }
        }
        #endregion
    }
}
