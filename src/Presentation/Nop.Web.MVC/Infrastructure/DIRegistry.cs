using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StructureMap.Configuration.DSL;
using Nop.Core.Tasks;

namespace Nop.Web.MVC.Infrastructure
{
    public class DIRegistry : Registry
    {
        public DIRegistry()
        {
            //For<ISomething>().Use<Something>();

            // start up tasks

            Scan(scan => {
                scan.Assembly("Nop.Data");
                scan.Assembly("Nop.Services");
                scan.AddAllTypesOf<IStartupTask>();
            });
        }
    }
}