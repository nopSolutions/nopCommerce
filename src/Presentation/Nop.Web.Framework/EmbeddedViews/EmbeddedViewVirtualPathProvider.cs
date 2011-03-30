
namespace Nop.Web.Framework.EmbeddedViews
{
    using System;
    using System.Collections;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Hosting;

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
            string checkPath = VirtualPathUtility.ToAppRelative(virtualPath);

            return checkPath.StartsWith("~/Views/", StringComparison.InvariantCultureIgnoreCase)
                   && _embeddedViews.ContainsEmbeddedView(checkPath);
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
                var embeddedViewMetadata = _embeddedViews.FindEmbeddedView(virtualPath);
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