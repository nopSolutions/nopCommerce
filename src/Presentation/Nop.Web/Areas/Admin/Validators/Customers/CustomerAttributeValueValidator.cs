using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Customers;

public partial class CustomerAttributeValueValidator : BaseNopValidator<CustomerAttributeValueModel>
{
    public CustomerAttributeValueValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Admin.Customers.CustomerAttributes.Values.Fields.Name.Required");

        SetDatabaseValidationRules<CustomerAttributeValue>();
    }
}