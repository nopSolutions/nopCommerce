using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.Web.Mvc;

namespace Nop.Web.Framework.Themes
{
    public abstract class ThemeableBuildManagerViewEngine : ThemeableVirtualPathProviderViewEngine
    {
        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            return BuildManager.GetObjectFactory(virtualPath, false) != null;
        }
    }
}
