using System;
using System.IO;

namespace Nop.Core.IO
{
    public interface IStorageFile
    {
        string GetPath();
        string GetName();
        long GetSize();
        DateTime GetLastUpdated();
        string GetFileType();

        /// <summary>
        /// Creates a stream for reading from the file.
        /// </summary>
        Stream OpenRead();

        /// <summary>
        /// Creates a stream for writing to the file.
        /// </summary>
        Stream OpenWrite();
    }
}