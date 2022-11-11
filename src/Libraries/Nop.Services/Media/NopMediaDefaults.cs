using Nop.Core.Caching;

namespace Nop.Services.Media
{
    /// <summary>
    /// Represents default values related to media services
    /// </summary>
    public static partial class NopMediaDefaults
    {
        /// <summary>
        /// Gets a multiple thumb directories length
        /// </summary>
        public static int MultipleThumbDirectoriesLength => 3;

        /// <summary>
        /// Gets a path to the image thumbs files
        /// </summary>
        public static string ImageThumbsPath => @"images\thumbs";

        /// <summary>
        /// Gets a default avatar file name
        /// </summary>
        public static string DefaultAvatarFileName => "default-avatar.jpg";

        /// <summary>
        /// Gets a default image file name
        /// </summary>
        public static string DefaultImageFileName => "default-image.png";

        #region Caching defaults

        /// <summary>
        /// Gets a key to cache whether thumb exists
        /// </summary>
        /// <remarks>
        /// {0} : thumb file name
        /// </remarks>
        public static CacheKey ThumbExistsCacheKey => new("Nop.azure.thumb.exists.{0}", ThumbsExistsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ThumbsExistsPrefix => "Nop.azure.thumb.exists.";

        #endregion
    }
}