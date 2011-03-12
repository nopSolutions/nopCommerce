using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Configuration;

namespace Nop.Core.Plugins
{
    public interface IConfigurablePlugin : IPlugin, ISettings
    {
    }
}
