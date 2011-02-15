using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Nop.Core.Configuration;
using Nop.Core.Web;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Provides information about types in the current web application. 
    /// Optionally this class can look at all assemblies in the bin folder.
    /// </summary>
    public class WebAppTypeFinder : AppDomainTypeFinder
    {
        private IWebContext webContext;
        private bool ensureBinFolderAssembliesLoaded = true;
        private bool binFolderAssembliesLoaded = false;

        public WebAppTypeFinder(Web.IWebContext webContext)
        {
            this.webContext = webContext;
        }

        public WebAppTypeFinder(Web.IWebContext webContext, EngineSection engineConfiguration)
        {
            this.webContext = webContext;
            this.ensureBinFolderAssembliesLoaded = engineConfiguration.DynamicDiscovery;
            foreach (var assembly in engineConfiguration.Assemblies.AllElements)
                AssemblyNames.Add(assembly.Assembly);
        }

        #region Properties
        /// <summary>Gets or sets wether assemblies in the bin folder of the web application should be specificly checked for beeing loaded on application load. This is need in situations where plugins need to be loaded in the AppDomain after the application been reloaded.</summary>
        public bool EnsureBinFolderAssembliesLoaded
        {
            get { return ensureBinFolderAssembliesLoaded; }
            set { ensureBinFolderAssembliesLoaded = value; }
        }

        #endregion

        #region Methods
        public override IList<Assembly> GetAssemblies()
        {
            if (EnsureBinFolderAssembliesLoaded && !binFolderAssembliesLoaded)
            {
                binFolderAssembliesLoaded = true;
                LoadMatchingAssemblies(webContext.MapPath("~/bin"));
            }

            return base.GetAssemblies();
        }
        #endregion
    }
}
