using System.Collections.Generic;

namespace Nop.Web.Framework.Localization
{
    /// <summary>
    /// Localized model
    /// </summary>
    public interface ILocalizedModel
    {

    }
    /// <summary>
    /// Localized model
    /// </summary>
    /// <typeparam name="TLocalizedModel">Type</typeparam>
    public interface ILocalizedModel<TLocalizedModel> : ILocalizedModel
    {
        /// <summary>
        /// Locales
        /// </summary>
        IList<TLocalizedModel> Locales { get; set; }
    }
}
