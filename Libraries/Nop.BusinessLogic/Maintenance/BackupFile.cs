using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NopSolutions.NopCommerce.BusinessLogic.Maintenance
{
    /// <summary>
    /// Represents a backup file
    /// </summary>
    public class BackupFile
    {
        /// <summary>
        /// Gets the file size in bytes;
        /// </summary>
        public long FileSize { get; internal set; }

        /// <summary>
        /// Gets the full file name;
        /// </summary>
        public string FullFileName { get; internal set; }

        /// <summary>
        /// Gets the file name and extension without path string.
        /// </summary>
        public string FileName { get; internal set; }
    }
}
