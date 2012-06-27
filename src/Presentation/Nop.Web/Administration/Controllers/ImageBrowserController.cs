using System.IO;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc.UI;

//Original code can be found here http://demos.telerik.com/aspnet-mvc/razor/editor/imagetool
namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class ImageBrowserController : EditorFileBrowserController
    {
        private const string UploadedImagesFolder = "~/content/images/uploaded/";

        /// <summary>
        /// Gets the base paths from which content will be served.
        /// </summary>
        public override string[] ContentPaths
        {
            get
            {
                var path = Server.MapPath(UploadedImagesFolder);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return new[] { UploadedImagesFolder };
            }
        }
    }
}
