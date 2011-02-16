using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Plugins.Scheduling
{
    /// <summary>
    /// Interface of a timer wrapper that beats at a certain interval.
    /// </summary>
    public interface IHeart
    {
        event EventHandler Beat;
    }
}
