using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Common;

public partial class AddressAttributeValidator : BaseNopValidator<AddressAttributeModel>
{
    public AddressAttributeValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.AddressAttributes.Fields.Name.Required"));

        SetDatabaseValidationRules<AddressAttribute>();
    }
}