//Contributor: http://aspnetadvimage.codeplex.com/
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Nop.Core;
using Telerik.Web.Mvc.UI;

namespace Nop.Web.Framework.UI.Editor
{
    public partial class NetAdvDirectoryService: INetAdvDirectoryService
    {
        private readonly IWebHelper _webHelper;

        public NetAdvDirectoryService(IWebHelper webHelper)
        {
            this._webHelper = webHelper;
        }
        /// <summary>
        /// Gets the directory structure starting at the upload path
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>An array of tree items</returns>
        public virtual IEnumerable<TreeViewItemModel> GetDirectoryTree(HttpContextBase ctx)
        {
            return new[] { GetDirectoryRecursive(new DirectoryInfo(ctx.Server.MapPath(NetAdvImageSettings.UploadPath)), null, ctx) };
        }

        /// <summary>
        /// Builds the directory tree. A value of null for 'parentItem' denotes the root node.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="parentItem"></param>
        /// <param name="ctx"></param>
        /// <returns>A TreeViewItem structure</returns>
        public virtual TreeViewItemModel GetDirectoryRecursive(DirectoryInfo directory, TreeViewItemModel parentItem, HttpContextBase ctx)
        {
            // If 'parentNode' is null, assume we're starting at the upload path
            string path = parentItem != null ?
                Path.Combine(parentItem.Value, directory.Name) :
                directory.FullName;

            // Get or initalize list in session
            if (ctx.Session[NetAdvImageSettings.TreeStateSessionKey] == null)
                ctx.Session[NetAdvImageSettings.TreeStateSessionKey] = new List<string>();
            var expandedNodes = ctx.Session[NetAdvImageSettings.TreeStateSessionKey] as List<string>;

            // Create a new TreeViewItem
            var item = new TreeViewItemModel()
            {
                Text = directory.Name,
                Value = path,
                ImageUrl = _webHelper.GetStoreLocation() +  "Content/editors/tinymce/plugins/netadvimage/img/folder-horizontal.gif",
                Enabled = true,
                Expanded = parentItem == null ?
                    true : // Expand the root node
                    expandedNodes.Contains(path) // Or... get expanded state from session
            };

            // Recurse through the current directory's sub-directories
            foreach (var child in directory.GetDirectories())
            {
                TreeViewItemModel childNode = GetDirectoryRecursive(child, item, ctx);
                item.Items.Add(childNode);
            }

            return item;
        }

        /// <summary>
        /// Creates the upload directory if it does not already exist
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void CreateUploadDirectory(HttpContextBase ctx)
        {
            // Ensure upload directory exists
            if (!Directory.Exists(ctx.Server.MapPath(NetAdvImageSettings.UploadPath)))
                Directory.CreateDirectory(ctx.Server.MapPath(NetAdvImageSettings.UploadPath));
        }

        /// <summary>
        /// Moves a directory from one location to another
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public virtual string MoveDirectory(string path, string destinationPath)
        {
            try
            {
                // Move it!
                Directory.Move(path, Path.Combine(destinationPath, Path.GetFileName(path)));

                // Cleanup
                if (Directory.Exists(path))
                    Directory.Delete(path, true);

                // null == success
                return null;
            }
            catch (Exception ex)
            {
                // Return the error message to be alerted.
                return ex.Message;
            }
        }

        /// <summary>
        /// Deletes directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public virtual string DeleteDirectory(string path, HttpContextBase ctx)
        {
            try
            {
                if (path == ctx.Server.MapPath(NetAdvImageSettings.UploadPath))
                    return "You cannot delete the root folder.";

                // Delete it!
                Directory.Delete(path, true);
                return null;
            }
            catch (Exception ex)
            {
                // Return the error message to be alerted.
                return ex.Message;
            }
        }

        /// <summary>
        /// Inserts a new directory into the given physical path
        /// A unique name is applied to conflicting directory names
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual string AddDirectory(string path)
        {
            try
            {
                // Ensure unique folder name
                int counter = 1;
                while (Directory.Exists(Path.Combine(path, "New Folder" + (counter > 1 ? String.Format(" ({0})", counter) : String.Empty))))
                    counter++;

                // Add directory
                Directory.CreateDirectory(
                    Path.Combine(path, "New Folder" + (counter > 1 ? String.Format(" ({0})", counter) : String.Empty))
                    );

                return null;
            }
            catch (Exception ex)
            {
                // Return the error message to be alerted.
                return ex.Message;
            }
        }

        /// <summary>
        /// Renames a directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public virtual  string RenameDirectory(string path, string name, HttpContextBase ctx)
        {
            try
            {
                // Prevent renaming of root
                if (path.Equals(ctx.Server.MapPath(NetAdvImageSettings.UploadPath)))
                    throw new Exception("Cannot rename the root directory");

                // Trim extraneous slashes
                if (path.EndsWith(@"\"))
                    path = path.Substring(0, path.LastIndexOf(@"\"));

                name = Path.Combine(path.Substring(0, path.LastIndexOf(@"\") + 1), name.Trim());

                Directory.Move(path, Path.Combine(path, name));
                return null;
            }
            catch (Exception ex)
            {
                // Return the error message to be alerted.
                return ex.Message;
            }
        }
    }
}