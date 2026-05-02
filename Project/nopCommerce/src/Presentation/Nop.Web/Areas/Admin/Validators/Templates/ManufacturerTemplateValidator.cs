using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Templates;

public partial class ManufacturerTemplateValidator : BaseNopValidator<ManufacturerTemplateModel>
{
    public ManufacturerTemplateValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Manufacturer.Name.Required"));
        RuleFor(x => x.ViewPath).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Manufacturer.ViewPath.Required"));

        SetDatabaseValidationRules<ManufacturerTemplate>();
    }
}