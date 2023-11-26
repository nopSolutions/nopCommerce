using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Common.Interfaces
{
    /// <summary>
    /// Interface for a singleton service in dependency injection.
    /// Singleton services are created once and shared throughout the application.
    /// 
    /// If an interface inherits this interface, it will be automatically registered
    /// by a common dependency injection configuration class.
    /// </summary>
    public interface ISingletonService
    {
    }
}
