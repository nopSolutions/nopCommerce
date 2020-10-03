using FluentValidation;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Validators
{
    /// <summary>
    /// Represents an <see cref="TokenModel"/> validator.
    /// </summary>
    public class TokenModelValidator : BaseNopValidator<TokenModel>
    {
        public TokenModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.Token).NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Required"));
            RuleFor(model => model.Token).Matches(@"^[0-9]{6,6}$")
                .WithMessage(localizationService.GetResource("Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Wrong"));
        }
    }
}
