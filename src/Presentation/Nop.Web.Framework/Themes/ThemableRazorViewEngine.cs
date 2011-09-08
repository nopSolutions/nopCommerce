using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Web.Framework.Themes
{
    public class ThemableRazorViewEngine : ThemeableBuildManagerViewEngine
    {
        public ThemableRazorViewEngine()
        {
            AreaViewLocationFormats = new[]
                                          {
                                              //themes
                                              "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.cshtml", 
                                              "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.vbhtml", 
                                              "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.cshtml", 
                                              "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.vbhtml",
                                              
                                              //default
                                              "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                              "~/Areas/{2}/Views/{1}/{0}.vbhtml", 
                                              "~/Areas/{2}/Views/Shared/{0}.cshtml", 
                                              "~/Areas/{2}/Views/Shared/{0}.vbhtml"
                                          };

            AreaMasterLocationFormats = new[]
                                            {
                                                //themes
                                                "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.cshtml", 
                                                "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.vbhtml", 
                                                "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.cshtml", 
                                                "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.vbhtml",


                                                //default
                                                "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                                "~/Areas/{2}/Views/{1}/{0}.vbhtml", 
                                                "~/Areas/{2}/Views/Shared/{0}.cshtml", 
                                                "~/Areas/{2}/Views/Shared/{0}.vbhtml"
                                            };

            AreaPartialViewLocationFormats = new[]
                                                 {
                                                     //themes
                                                    "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.cshtml", 
                                                    "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.vbhtml", 
                                                    "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.cshtml", 
                                                    "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.vbhtml",
                                                    
                                                    //default
                                                    "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                                    "~/Areas/{2}/Views/{1}/{0}.vbhtml", 
                                                    "~/Areas/{2}/Views/Shared/{0}.cshtml", 
                                                    "~/Areas/{2}/Views/Shared/{0}.vbhtml"
                                                 };

            ViewLocationFormats = new[]
                                      {
                                            //themes
                                            "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                            "~/Themes/{2}/Views/{1}/{0}.vbhtml", 
                                            "~/Themes/{2}/Views/Shared/{0}.cshtml",
                                            "~/Themes/{2}/Views/Shared/{0}.vbhtml",

                                            //default
                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/{1}/{0}.vbhtml", 
                                            "~/Views/Shared/{0}.cshtml",
                                            "~/Views/Shared/{0}.vbhtml",


                                            //Admin
                                            "~/Administration/Views/{1}/{0}.cshtml",
                                            "~/Administration/Views/{1}/{0}.vbhtml",
                                            "~/Administration/Views/Shared/{0}.cshtml",
                                            "~/Administration/Views/Shared/{0}.vbhtml",
                                      };

            MasterLocationFormats = new[]
                                        {
                                            //themes
                                            "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                            "~/Themes/{2}/Views/{1}/{0}.vbhtml", 
                                            "~/Themes/{2}/Views/Shared/{0}.cshtml", 
                                            "~/Themes/{2}/Views/Shared/{0}.vbhtml",

                                            //default
                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/{1}/{0}.vbhtml", 
                                            "~/Views/Shared/{0}.cshtml", 
                                            "~/Views/Shared/{0}.vbhtml"
                                        };

            PartialViewLocationFormats = new[]
                                             {
                                                 //themes
                                                "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                                "~/Themes/{2}/Views/{1}/{0}.vbhtml", 
                                                "~/Themes/{2}/Views/Shared/{0}.cshtml", 
                                                "~/Themes/{2}/Views/Shared/{0}.vbhtml",

                                                //default
                                                "~/Views/{1}/{0}.cshtml", 
                                                "~/Views/{1}/{0}.vbhtml", 
                                                "~/Views/Shared/{0}.cshtml", 
                                                "~/Views/Shared/{0}.vbhtml",

                                                //Admin
                                                "~/Administration/Views/{1}/{0}.cshtml",
                                                "~/Administration/Views/{1}/{0}.vbhtml",
                                                "~/Administration/Views/Shared/{0}.cshtml",
                                                "~/Administration/Views/Shared/{0}.vbhtml",
                                             };

            FileExtensions = new[] { "cshtml", "vbhtml" };
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            string layoutPath = null;
            var runViewStartPages = false;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions);
            //return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions, base.ViewPageActivator);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            string layoutPath = masterPath;
            var runViewStartPages = true;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, viewPath, layoutPath, runViewStartPages, fileExtensions);
        }
    }
}
