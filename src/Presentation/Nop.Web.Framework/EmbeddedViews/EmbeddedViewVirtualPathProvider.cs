using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Nop.Web.Framework.EmbeddedViews
{
    public class EmbeddedViewVirtualPathProvider : VirtualPathProvider
    {
        private readonly EmbeddedViewTable _embeddedViews;

        public EmbeddedViewVirtualPathProvider(EmbeddedViewTable embeddedViews)
        {
            if (embeddedViews == null)
                throw new ArgumentNullException("embeddedViews");

            this._embeddedViews = embeddedViews;
        }

        private bool IsEmbeddedView(string virtualPath)
        {
            /*old validation
            it can cause issue if we have several views with the same full path:
            for example: both Nop.Plugin.Plugin1 and Nop.Plugin.Plugin2 can have Views\Config\Configure.cshtml files
            
            That's why we specify FQN for views into plugin controllers  
             */
            //string virtualPathAppRelative = VirtualPathUtility.ToAppRelative(virtualPath);

            //return virtualPathAppRelative.StartsWith("~/Views/", StringComparison.InvariantCultureIgnoreCase)
            //       && _embeddedViews.ContainsEmbeddedView(virtualPathAppRelative);
            if (string.IsNullOrEmpty(virtualPath))
                return false;

            string virtualPathAppRelative = VirtualPathUtility.ToAppRelative(virtualPath);
            if (!virtualPathAppRelative.StartsWith("~/Views/", StringComparison.InvariantCultureIgnoreCase))
                return false;
            var fullyQualifiedViewName = virtualPathAppRelative.Substring(virtualPathAppRelative.LastIndexOf("/") + 1, virtualPathAppRelative.Length - 1 - virtualPathAppRelative.LastIndexOf("/"));

            bool isEmbedded = _embeddedViews.ContainsEmbeddedView(fullyQualifiedViewName);
            return isEmbedded;
        }

        public override bool FileExists(string virtualPath)
        {
            return (IsEmbeddedView(virtualPath) ||
                    Previous.FileExists(virtualPath));
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (IsEmbeddedView(virtualPath))
            {
                string virtualPathAppRelative = VirtualPathUtility.ToAppRelative(virtualPath);
                var fullyQualifiedViewName = virtualPathAppRelative.Substring(virtualPathAppRelative.LastIndexOf("/") + 1, virtualPathAppRelative.Length - 1 - virtualPathAppRelative.LastIndexOf("/"));

                var embeddedViewMetadata = _embeddedViews.FindEmbeddedView(fullyQualifiedViewName);
                return new EmbeddedResourceVirtualFile(embeddedViewMetadata, virtualPath);
            }

            return Previous.GetFile(virtualPath);
        }

        public override CacheDependency GetCacheDependency(
            string virtualPath,
            IEnumerable virtualPathDependencies,
            DateTime utcStart)
        {
            return IsEmbeddedView(virtualPath)
                ? null : Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }
    }
}