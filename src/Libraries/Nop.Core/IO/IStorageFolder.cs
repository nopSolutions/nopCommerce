
using System;

namespace Nop.Core.IO
{
    public interface IStorageFolder 
    {
        string GetPath();
        string GetName();
        long GetSize();
        DateTime GetLastUpdated();
        IStorageFolder GetParent();
    }
}