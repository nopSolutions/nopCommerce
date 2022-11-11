using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Factories
{
    /// <summary>
    /// Represents the localized model factory
    /// </summary>
    public partial interface ILocalizedModelFactory
    {
        /// <summary>
        /// Prepare localized model for localizable entities
        /// </summary>
        /// <typeparam name="T">Localized model type</typeparam>
        /// <param name="configure">Model configuration action</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of localized model
        /// </returns>
        Task<IList<T>> PrepareLocalizedModelsAsync<T>(Func<T, int, Task> configure = null) where T : ILocalizedLocaleModel;
    }
}