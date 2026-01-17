using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Validators.Catalog;

public partial class ProductEmailAFriendValidator : BaseNopValidator<ProductEmailAFriendModel>
{
    public ProductEmailAFriendValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.FriendEmail).NotEmpty().WithMessage("Products.EmailAFriend.FriendEmail.Required");
        RuleFor(x => x.FriendEmail)
            .IsEmailAddress()
            .WithMessage("Common.WrongEmail");

        RuleFor(x => x.YourEmailAddress).NotEmpty().WithMessage("Products.EmailAFriend.YourEmailAddress.Required");
        RuleFor(x => x.YourEmailAddress)
            .IsEmailAddress()
            .WithMessage("Common.WrongEmail");
    }
}