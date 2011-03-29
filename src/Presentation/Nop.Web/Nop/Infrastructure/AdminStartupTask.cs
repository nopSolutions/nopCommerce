using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core.Infrastructure;
using Nop.Core.Tasks;

namespace Nop.Admin.Infrastructure
{
    public class AdminStartupTask : IStartupTask
    {
        public void Execute()
        {
            //set localization service for telerik
            Telerik.Web.Mvc.Infrastructure.DI.Current.Register(
                () => EngineContext.Current.Resolve<Telerik.Web.Mvc.Infrastructure.ILocalizationServiceFactory>());
        }

        public int Order
        {
            get { return 100; }
        }
    }
}