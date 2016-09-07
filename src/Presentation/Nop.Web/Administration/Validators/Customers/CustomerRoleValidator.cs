using FluentValidation;
using Nop.Admin.Models.Customers;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Customers
{
    public partial class CustomerRoleValidator : BaseNopValidator<CustomerRoleModel>
    {
        public CustomerRoleValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.Name.Required"));

            SetStringPropertiesMaxLength<CustomerRole>(dbContext);
        }
    }
}