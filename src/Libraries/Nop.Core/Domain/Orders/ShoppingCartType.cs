namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a shopping cart type
    /// </summary>
    public enum ShoppingCartType
    {
        /// <summary>
        /// Shopping cart
        /// </summary>
        ShoppingCart = 1,

        /// <summary>
        /// Wishlist
        /// </summary>
        Wishlist = 2,

        //customization

        ShortListedMe = 3,

        InterestSent = 4,

        InterestReceived = 5,

        AcceptedByMe = 6,

        AcceptedMe = 7,

        DeclinedByMe = 8,

        DeclinedMe = 9,

        ViewedByMe = 10,

        ViewedMe = 11,

        BlockedByMe = 12,

        BlockedMe = 13
    }
}
