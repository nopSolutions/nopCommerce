using FluentValidation;
using Nop.Plugin.DiscountRules.CustomerRoles.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.DiscountRules.CustomerRoles.Validators
{
    /// <summary>
    /// Represents an <see cref="RequirementModel"/> validator.
    /// </summary>
    public class RequirementModelValidator : BaseNopValidator<RequirementModel>
    {
        public RequirementModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.DiscountId)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Plugins.DiscountRules.CustomerRoles.Fields.DiscountId.Required").Result);
            RuleFor(model => model.CustomerRoleId)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRoleId.Required").Result);
        }
    }
}
