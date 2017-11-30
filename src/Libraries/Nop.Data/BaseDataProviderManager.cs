using System;
using Nop.Core.Data;

namespace Nop.Data
{
    /// <summary>
    /// Base data provider manager
    /// </summary>
    public abstract class BaseDataProviderManager
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="settings">Data settings</param>
        protected BaseDataProviderManager(DataSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Gets or sets settings
        /// </summary>
        protected DataSettings Settings { get; }

        /// <summary>
        /// Load data provider
        /// </summary>
        /// <returns>Data provider</returns>
        public abstract IDataProvider LoadDataProvider();
    }
}
