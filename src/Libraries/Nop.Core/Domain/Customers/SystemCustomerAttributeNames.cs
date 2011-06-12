
namespace Nop.Core.Domain.Customers
{
    public static class SystemCustomerAttributeNames
    {
        //Form fields

        public static string FirstName { get { return "FirstName"; } }

        public static string LastName { get { return "LastName"; } }

        public static string Gender { get { return "Gender"; } }

        public static string DateOfBirth { get { return "DateOfBirth"; } }

        public static string Company { get { return "Company"; } }

        public static string AvatarPictureId { get { return "AvatarPictureId"; } }

        //Other attributes
        public static string LastShippingOption { get { return "LastShippingOption  "; } }

        public static string PasswordRecoveryToken { get { return "PasswordRecoveryToken"; } }

        public static string AccountActivationToken { get { return "AccountActivationToken"; } }

        public static string LastContinueShoppingPage { get { return "LastContinueShoppingPage"; } }
    }
}