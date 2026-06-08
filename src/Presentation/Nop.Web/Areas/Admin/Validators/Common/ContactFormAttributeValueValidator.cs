using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Common;

public partial class ContactFormAttributeValueValidator : BaseNopValidator<ContactFormAttributeValueModel>
{
    public ContactFormAttributeValueValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.ContactFormAttributes.Values.Fields.Name.Required"));

        SetDatabaseValidationRules<ContactFormAttributeValue>();
    }
}