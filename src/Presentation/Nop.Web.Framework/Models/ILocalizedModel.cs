using System.Collections.Generic;

namespace Nop.Web.Framework.Models
{
    /// <summary>
    /// Represents localized model
    /// </summary>
    public interface ILocalizedModel
    {
    }

    /// <summary>
    /// Represents generic localized model
    /// </summary>
    /// <typeparam name="TLocalizedModel">Localized model type</typeparam>
    public interface ILocalizedModel<TLocalizedModel> : ILocalizedModel
    {
        /// <summary>
        /// Gets or sets localized locale models
        /// </summary>
        IList<TLocalizedModel> Locales { get; set; }
    }
}