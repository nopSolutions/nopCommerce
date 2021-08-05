using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Customers
{
    public partial class CustomerAttributeValueValidator : BaseNopValidator<CustomerAttributeValueModel>
    {
        public CustomerAttributeValueValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Customers.CustomerAttributes.Values.Fields.Name.Required"));

            SetDatabaseValidationRules<CustomerAttributeValue>(mappingEntityAccessor);
        }
    }
}