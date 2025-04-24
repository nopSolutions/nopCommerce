using Nop.Core.Domain.Menus;
using Nop.Web.Models.Menus;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the interface of the menu model factory
/// </summary>
public partial interface IMenuModelFactory
{
    /// <summary>
    /// Prepare the menu model
    /// </summary>
    /// <param name="menuType">Menu type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains list of menu models
    /// </returns>
    Task<IList<MenuModel>> PrepareMenuModelsAsync(MenuType menuType);
}
