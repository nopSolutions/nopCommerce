using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Web
{
    /// <summary>
    /// Represents a class that end and be disposed. Used to mark classes in 
    /// the request context that may be disposed when the request ends.
    /// </summary>
    public interface IClosable : IDisposable
    {
    }
}
