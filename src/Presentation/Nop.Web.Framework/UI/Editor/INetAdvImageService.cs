//Contributor: http://aspnetadvimage.codeplex.com/
using System.Collections.Generic;
using System.Web;

namespace Nop.Web.Framework.UI.Editor
{
    public partial interface INetAdvImageService
    {
        /// <summary>
        /// Gets a list of top-level images within a given directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        IEnumerable<NetAdvImage> GetImages(string path, HttpContextBase ctx);

        /// <summary>
        /// Deletes a image
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        string DeleteImage(string path, string name);
    }
}