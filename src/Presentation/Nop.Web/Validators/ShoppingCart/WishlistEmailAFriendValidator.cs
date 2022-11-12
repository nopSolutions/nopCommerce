<<<<<<< HEAD
﻿using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Validators.ShoppingCart
{
    public partial class WishlistEmailAFriendValidator : BaseNopValidator<WishlistEmailAFriendModel>
    {
        public WishlistEmailAFriendValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FriendEmail).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Wishlist.EmailAFriend.FriendEmail.Required"));
            RuleFor(x => x.FriendEmail).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));

            RuleFor(x => x.YourEmailAddress).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Wishlist.EmailAFriend.YourEmailAddress.Required"));
            RuleFor(x => x.YourEmailAddress).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
        }
    }
=======
﻿using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Validators.ShoppingCart
{
    public partial class WishlistEmailAFriendValidator : BaseNopValidator<WishlistEmailAFriendModel>
    {
        public WishlistEmailAFriendValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FriendEmail).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Wishlist.EmailAFriend.FriendEmail.Required"));
            RuleFor(x => x.FriendEmail).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));

            RuleFor(x => x.YourEmailAddress).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Wishlist.EmailAFriend.YourEmailAddress.Required"));
            RuleFor(x => x.YourEmailAddress).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
        }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}