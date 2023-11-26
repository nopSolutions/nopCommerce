using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Common.Interfaces
{
    /// <summary>
    /// Interface for a transient service in dependency injection.
    /// Transient services are created anew each time they are requested.
    /// 
    /// If an interface inherits this interface, it will be automatically registered
    /// by a common dependency injection configuration class.
    /// </summary>
    public interface ITransientService
    {
    }
}
