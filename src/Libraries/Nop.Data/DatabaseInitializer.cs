
using System;
using System.Collections.Generic;
using System.Data.Entity.Database;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml;
using Nop.Core.Infrastructure;


namespace Nop.Data
{
    /// <summary>
    /// Database initializer
    /// </summary>
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<NopObjectContext>
    {
        protected override void Seed(NopObjectContext context)
        {
            EventBroker.Instance.OnInstallingDatabase(this, new EventArgs());

            base.Seed(context);
        }
    }
}