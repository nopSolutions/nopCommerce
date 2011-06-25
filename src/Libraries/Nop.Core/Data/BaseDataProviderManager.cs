using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Data
{
    public abstract class BaseDataProviderManager
    {
        protected BaseDataProviderManager(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            this.Settings = settings;
        }
        protected Settings Settings { get; private set; }
        public abstract IDataProvider LoadDataProvider();
    }
}
