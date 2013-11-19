//Contributor: http://aspnetadvimage.codeplex.com/

using System.Collections.Generic;
using System.Web;

namespace Nop.Web.Framework.UI.Editor
{
    /// <summary>
    /// NetAdv image service
    /// </summary>
    public partial interface INetAdvImageService
    {
        /// <summary>
        /// Gets a list of top-level images within a given directory
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="ctx">HTTP context</param>
        /// <returns>A lit of images</returns>
        IEnumerable<NetAdvImage> GetImages(string path, HttpContextBase ctx);

        /// <summary>
        /// Deletes a image
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="name">Name</param>
        /// <returns>Error (if happens)</returns>
        string DeleteImage(string path, string name);
    }
}