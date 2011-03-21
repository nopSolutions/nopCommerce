using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Themes
{
    public class ThemableRazorViewEngine : ThemeableBuildManagerViewEngine
    {
        public ThemableRazorViewEngine()
        {
            AreaViewLocationFormats = new[]
                                          {
                                              "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.cshtml", 
                                              "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.vbhtml", 
                                              "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.cshtml", 
                                              "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.vbhtml",

                                              "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                              "~/Areas/{2}/Views/{1}/{0}.vbhtml", 
                                              "~/Areas/{2}/Views/Shared/{0}.cshtml", 
                                              "~/Areas/{2}/Views/Shared/{0}.vbhtml"
                                          };
            AreaMasterLocationFormats = new[]
                                            {
                                                "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.cshtml", 
                                                "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.vbhtml", 
                                                "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.cshtml", 
                                                "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.vbhtml",

                                                "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                                "~/Areas/{2}/Views/{1}/{0}.vbhtml", 
                                                "~/Areas/{2}/Views/Shared/{0}.cshtml", 
                                                "~/Areas/{2}/Views/Shared/{0}.vbhtml"
                                            };
            AreaPartialViewLocationFormats = new[]
                                                 {
                                                    "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.cshtml", 
                                                    "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.vbhtml", 
                                                    "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.cshtml", 
                                                    "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.vbhtml",

                                                    "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                                    "~/Areas/{2}/Views/{1}/{0}.vbhtml", 
                                                    "~/Areas/{2}/Views/Shared/{0}.cshtml", 
                                                    "~/Areas/{2}/Views/Shared/{0}.vbhtml"
                                                 };
            ViewLocationFormats = new[]
                                      {
                                            "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                            "~/Themes/{2}/Views/{1}/{0}.vbhtml", 
                                            "~/Themes/{2}/Views/Shared/{0}.cshtml",
                                            "~/Themes/{2}/Views/Shared/{0}.vbhtml",

                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/{1}/{0}.vbhtml", 
                                            "~/Views/Shared/{0}.cshtml",
                                            "~/Views/Shared/{0}.vbhtml"
                                      };
            MasterLocationFormats = new[]
                                        {
                                            "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                            "~/Themes/{2}/Views/{1}/{0}.vbhtml", 
                                            "~/Themes/{2}/Views/Shared/{0}.cshtml", 
                                            "~/Themes/{2}/Views/Shared/{0}.vbhtml",

                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/{1}/{0}.vbhtml", 
                                            "~/Views/Shared/{0}.cshtml", 
                                            "~/Views/Shared/{0}.vbhtml"
                                        };
            PartialViewLocationFormats = new[]
                                             {
                                                "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                                "~/Themes/{2}/Views/{1}/{0}.vbhtml", 
                                                "~/Themes/{2}/Views/Shared/{0}.cshtml", 
                                                "~/Themes/{2}/Views/Shared/{0}.vbhtml",

                                                "~/Views/{1}/{0}.cshtml", 
                                                "~/Views/{1}/{0}.vbhtml", 
                                                "~/Views/Shared/{0}.cshtml", 
                                                "~/Views/Shared/{0}.vbhtml"
                                             };
            FileExtensions = new[] { "cshtml", "vbhtml" };
        }

        private string[] Append(string[] first, string[] second)
        {
            var list = new List<string>();
            list.AddRange(first);
            list.AddRange(second);
            return list.ToArray();
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            string layoutPath = null;
            bool runViewStartPages = false;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions);
            //return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions, base.ViewPageActivator);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            string layoutPath = masterPath;
            bool runViewStartPages = true;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, viewPath, layoutPath, runViewStartPages, fileExtensions);
        }
    }
}
