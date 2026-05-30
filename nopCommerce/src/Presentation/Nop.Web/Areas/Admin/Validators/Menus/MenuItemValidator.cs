using FluentValidation;
using Nop.Core.Domain.Menus;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Menus;

public partial class MenuItemValidator : BaseNopValidator<MenuItemModel>
{
    #region Ctor

    public MenuItemValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.RouteName)
            .NotEmpty()
            .When(x => x.MenuItemTypeId == (int)MenuItemType.StandardPage)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItem.Fields.RouteName.Required"));

        RuleFor(x => x.ProductId)
            .NotNull()
            .When(x => x.MenuItemTypeId == (int)MenuItemType.Product)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItem.Fields.Product.Required"));

        RuleFor(x => x.TopicId)
            .GreaterThan(0)
            .When(x => x.MenuItemTypeId == (int)MenuItemType.TopicPage)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItem.Fields.Topic.Required"));

        RuleFor(x => x.Title)
            .NotEmpty()
            .When(x =>
                x.MenuItemTypeId == (int)MenuItemType.StandardPage ||
                x.MenuItemTypeId == (int)MenuItemType.CustomLink ||
                x.MenuItemTypeId == (int)MenuItemType.Text ||
                (x.MenuItemTypeId == (int)MenuItemType.Category && x.CategoryId == 0) ||
                (x.MenuItemTypeId == (int)MenuItemType.Manufacturer && x.ManufacturerId == 0) ||
                (x.MenuItemTypeId == (int)MenuItemType.Vendor && x.VendorId == 0))
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItem.Fields.Title.Required"));

        RuleFor(x => x.Url)
            .NotEmpty()
            .When(x => x.MenuItemTypeId == (int)MenuItemType.CustomLink)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Menus.MenuItem.Fields.Url.Required"));

        SetDatabaseValidationRules<MenuItem>();
    }

    #endregion
}
