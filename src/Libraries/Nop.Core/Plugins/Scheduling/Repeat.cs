using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Plugins.Scheduling
{
    /// <summary>
    /// Specifies wether an action should be executed more than once.
    /// </summary>
    public enum Repeat
    {
        /// <summary>Execute the action once and then remove it.</summary>
        Once = 1,
        /// <summary>Execute the action after the specified duration as long as the application is running.</summary>
        Indefinitely = 2
    }
}
