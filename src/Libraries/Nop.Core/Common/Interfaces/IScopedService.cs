using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Common.Interfaces
{
    /// <summary>
    /// Interface for a scoped service in dependency injection.
    /// Scoped services are created once per request.
    /// 
    /// If an interface inherits this interface, it will be automatically registered
    /// by a common dependency injection configuration class.
    /// </summary>
    public interface IScopedService
    {
    }
}
