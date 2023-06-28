namespace Nop.Core.Domain.Media
{
    /// <summary>
    /// Helper class for making pictures hashes from DB
    /// </summary>
    public partial class PictureHashItem : IComparable, IComparable<PictureHashItem>
    {
        /// <summary>
        /// Gets or sets the picture ID
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the picture hash
        /// </summary>
        public byte[] Hash { get; set; }

        /// <summary>
        /// Compares this instance to a specified and returns an indication
        /// </summary>
        /// <param name="obj">An object to compare with this instance</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as PictureHashItem);
        }

        /// <summary>
        /// Compares this instance to a specified and returns an indication
        /// </summary>
        /// <param name="other">An object to compare with this instance</param>
        /// <returns></returns>
        public int CompareTo(PictureHashItem other)
        {
            return other == null ? -1 : PictureId.CompareTo(other.PictureId);
        }
    }
}