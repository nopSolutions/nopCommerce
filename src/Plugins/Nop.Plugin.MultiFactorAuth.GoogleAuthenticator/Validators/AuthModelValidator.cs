using FluentValidation;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Validators
{
    /// <summary>
    /// Represents an <see cref="AuthModel"/> validator.
    /// </summary>
    public class AuthModelValidator : BaseNopValidator<AuthModel>
    {
        public AuthModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.Code).NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Required"));
            RuleFor(model => model.Code).Matches(@"^[0-9]{6,6}$")
                .WithMessage(localizationService.GetResource("Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Wrong"));
        }
    }
}
