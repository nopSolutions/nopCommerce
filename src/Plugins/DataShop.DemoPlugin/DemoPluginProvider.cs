using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using DataShop.DemoPlugin.Domain;
using DataShop.DemoPlugin.Data;
using DataShop.DemoPlugin.Services;

namespace DataShop.DemoPlugin
{
    public class DemoPluginProvider : BasePlugin
    {
        private readonly DataContext Context;

        public DemoPluginProvider(DataContext context)
        {
            this.Context = context;
        }

        public override void Install()
        {
            this.Context.Install();
            base.Install();
        }

        public override void Uninstall()
        {
            this.Context.Uninstall();
            base.Uninstall();
        }

    }
}
