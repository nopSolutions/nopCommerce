
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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