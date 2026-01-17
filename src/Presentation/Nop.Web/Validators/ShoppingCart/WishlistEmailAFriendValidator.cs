using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Validators.ShoppingCart;

public partial class WishlistEmailAFriendValidator : BaseNopValidator<WishlistEmailAFriendModel>
{
    public WishlistEmailAFriendValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.FriendEmail).NotEmpty().WithMessage("Wishlist.EmailAFriend.FriendEmail.Required");
        RuleFor(x => x.FriendEmail)
            .IsEmailAddress()
            .WithMessage("Common.WrongEmail");

        RuleFor(x => x.YourEmailAddress).NotEmpty().WithMessage("Wishlist.EmailAFriend.YourEmailAddress.Required");
        RuleFor(x => x.YourEmailAddress)
            .IsEmailAddress()
            .WithMessage("Common.WrongEmail");
    }
}