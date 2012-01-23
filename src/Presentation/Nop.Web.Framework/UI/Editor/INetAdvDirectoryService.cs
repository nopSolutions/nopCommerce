//Contributor: http://aspnetadvimage.codeplex.com/
using System.Collections.Generic;
using System.IO;
using System.Web;
using Telerik.Web.Mvc.UI;

namespace Nop.Web.Framework.UI.Editor
{
    public partial interface INetAdvDirectoryService
    {
        /// <summary>
        /// Gets the directory structure starting at the upload path
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>An array of tree items</returns>
        IEnumerable<TreeViewItemModel> GetDirectoryTree(HttpContextBase ctx);

        /// <summary>
        /// Builds the directory tree. A value of null for 'parentItem' denotes the root node.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="parentItem"></param>
        /// <param name="ctx"></param>
        /// <returns>A TreeViewItem structure</returns>
        TreeViewItemModel GetDirectoryRecursive(DirectoryInfo directory, TreeViewItemModel parentItem, HttpContextBase ctx);

        /// <summary>
        /// Creates the upload directory if it does not already exist
        /// </summary>
        /// <param name="ctx"></param>
        void CreateUploadDirectory(HttpContextBase ctx);

        /// <summary>
        /// Moves a directory from one location to another
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        string MoveDirectory(string path, string destinationPath);

        /// <summary>
        /// Deletes directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        string DeleteDirectory(string path, HttpContextBase ctx);

        /// <summary>
        /// Inserts a new directory into the given physical path
        /// A unique name is applied to conflicting directory names
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string AddDirectory(string path);

        /// <summary>
        /// Renames a directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        string RenameDirectory(string path, string name, HttpContextBase ctx);
    }
}