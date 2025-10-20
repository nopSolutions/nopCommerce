using FluentValidation;
using Nop.Core.Domain.Menus;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Menus;

public partial class MenuValidator : BaseNopValidator<MenuModel>
{
    #region Ctor

    public MenuValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.Fields.Name.Required"));

        SetDatabaseValidationRules<Menu>();
    }

    #endregion
}
