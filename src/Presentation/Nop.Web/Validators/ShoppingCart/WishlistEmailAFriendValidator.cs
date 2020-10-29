using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Validators.ShoppingCart
{
    public partial class WishlistEmailAFriendValidator : BaseNopValidator<WishlistEmailAFriendModel>
    {
        public WishlistEmailAFriendValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FriendEmail).NotEmpty().WithMessage(localizationService.GetResourceAsync("Wishlist.EmailAFriend.FriendEmail.Required").Result);
            RuleFor(x => x.FriendEmail).EmailAddress().WithMessage(localizationService.GetResourceAsync("Common.WrongEmail").Result);

            RuleFor(x => x.YourEmailAddress).NotEmpty().WithMessage(localizationService.GetResourceAsync("Wishlist.EmailAFriend.YourEmailAddress.Required").Result);
            RuleFor(x => x.YourEmailAddress).EmailAddress().WithMessage(localizationService.GetResourceAsync("Common.WrongEmail").Result);
        }
    }
}