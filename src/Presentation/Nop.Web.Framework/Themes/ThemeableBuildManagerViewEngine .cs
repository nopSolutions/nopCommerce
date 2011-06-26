using System.Web.Compilation;
using System.Web.Mvc;

namespace Nop.Web.Framework.Themes
{
    public abstract class ThemeableBuildManagerViewEngine : ThemeableVirtualPathProviderViewEngine
    {
		#region Methods 

		#region Protected Methods 

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            return BuildManager.GetObjectFactory(virtualPath, false) != null;
        }

		#endregion Protected Methods 

		#endregion Methods 
    }
}
