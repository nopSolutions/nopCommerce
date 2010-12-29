using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StructureMap;
using Nop.Web.Framework.IoC;
using Nop.Core.Tasks;

namespace Nop.Web.MVC.Infrastructure {
    public class DIBootstrapper : IBootstrapper {
        private static bool hasStarted;

        public void BootstrapStructureMap() {
            ObjectFactory.Initialize(x => {
                x.AddRegistry(new DIRegistry());
            });

            DependencyResolver.SetResolver(new StructureMapDependencyResolver(ObjectFactory.Container));

            // execute startup tasks
            ObjectFactory.GetAllInstances<IStartupTask>().ToList()
                .ForEach(t => {
                    t.Execute();
                });
        }

        public static void Restart() {
            if (hasStarted) {
                ObjectFactory.ResetDefaults();
            } else {
                Boot();
                hasStarted = true;
            }
        }

        public static void Boot() {
            new DIBootstrapper().BootstrapStructureMap();
        }
    }
}