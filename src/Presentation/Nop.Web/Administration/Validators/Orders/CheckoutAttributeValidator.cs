using FluentValidation;
using Nop.Admin.Models.Orders;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Orders
{
    public partial class CheckoutAttributeValidator : BaseNopValidator<CheckoutAttributeModel>
    {
        public CheckoutAttributeValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.CheckoutAttributes.Fields.Name.Required"));

            SetStringPropertiesMaxLength<CheckoutAttribute>(dbContext);
        }
    }
}