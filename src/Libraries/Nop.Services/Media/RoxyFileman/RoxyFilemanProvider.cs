using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;
using Nop.Core.Infrastructure;

namespace Nop.Services.Media.RoxyFileman
{
    /// <summary>
    /// Represent a file provider for roxyFileman images folder
    /// </summary>
    public class RoxyFilemanProvider : IFileProvider
    {
        #region Fields

        private readonly PhysicalFileProvider _physicalFileProvider;

        #endregion

        #region Ctor

        public RoxyFilemanProvider(string root)
        {
            _physicalFileProvider = new PhysicalFileProvider(root);
        }

        public RoxyFilemanProvider(string root, ExclusionFilters filters)
        {
            _physicalFileProvider = new PhysicalFileProvider(root, filters);
        }

        #endregion

        #region Mehods

        /// <summary>Enumerate a directory at the given path, if any.</summary>
        /// <param name="subpath">Relative path that identifies the directory.</param>
        /// <returns>Returns the contents of the directory.</returns>
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return _physicalFileProvider.GetDirectoryContents(subpath);
        }

        /// <summary>Locate a file at the given path.</summary>
        /// <param name="subpath">Relative path that identifies the file.</param>
        /// <returns>The file information. Caller must check Exists property.</returns>
        public IFileInfo GetFileInfo(string subpath)
        {
            var pictureService = EngineContext.Current.Resolve<IPictureService>();

            if (_physicalFileProvider.GetFileInfo(subpath).Exists || !pictureService.IsStoreInDbAsync().Result)
                return _physicalFileProvider.GetFileInfo(subpath);

            var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
            var roxyFilemanService = EngineContext.Current.Resolve<IRoxyFilemanService>();
            var virtualPath = fileProvider?.GetVirtualPath(fileProvider.GetDirectoryName(_physicalFileProvider.GetFileInfo(subpath).PhysicalPath));
            roxyFilemanService.FlushImagesOnDiskAsync(virtualPath).Wait();

            return _physicalFileProvider.GetFileInfo(subpath);
        }

        /// <summary>
        /// Creates a <see cref="T:Microsoft.Extensions.Primitives.IChangeToken" /> for the specified <paramref name="filter" />.
        /// </summary>
        /// <param name="filter">Filter string used to determine what files or folders to monitor. Example: **/*.cs, *.*, subFolder/**/*.cshtml.</param>
        /// <returns>An <see cref="T:Microsoft.Extensions.Primitives.IChangeToken" /> that is notified when a file matching <paramref name="filter" /> is added, modified or deleted.</returns>
        public IChangeToken Watch(string filter)
        {
            return _physicalFileProvider.Watch(filter);
        }

        #endregion
    }
}
