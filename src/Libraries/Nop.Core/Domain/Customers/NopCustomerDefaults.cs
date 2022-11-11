namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents default values related to customers data
    /// </summary>
    public static partial class NopCustomerDefaults
    {
        #region System customer roles

        /// <summary>
        /// Gets a system name of 'administrators' customer role
        /// </summary>
        public static string AdministratorsRoleName => "Administrators";

        /// <summary>
        /// Gets a system name of 'forum moderators' customer role
        /// </summary>
        public static string ForumModeratorsRoleName => "ForumModerators";

        /// <summary>
        /// Gets a system name of 'registered' customer role
        /// </summary>
        public static string RegisteredRoleName => "Registered";

        /// <summary>
        /// Gets a system name of 'guests' customer role
        /// </summary>
        public static string GuestsRoleName => "Guests";

        /// <summary>
        /// Gets a system name of 'vendors' customer role
        /// </summary>
        public static string VendorsRoleName => "Vendors";

        #endregion

        #region System customers

        /// <summary>
        /// Gets a system name of 'search engine' customer object
        /// </summary>
        public static string SearchEngineCustomerName => "SearchEngine";

        /// <summary>
        /// Gets a system name of 'background task' customer object
        /// </summary>
        public static string BackgroundTaskCustomerName => "BackgroundTask";

        #endregion

        #region Customer attributes

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'DiscountCouponCode'
        /// </summary>
        public static string DiscountCouponCodeAttribute => "DiscountCouponCode";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'GiftCardCouponCodes'
        /// </summary>
        public static string GiftCardCouponCodesAttribute => "GiftCardCouponCodes";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'AvatarPictureId'
        /// </summary>
        public static string AvatarPictureIdAttribute => "AvatarPictureId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'ForumPostCount'
        /// </summary>
        public static string ForumPostCountAttribute => "ForumPostCount";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Signature'
        /// </summary>
        public static string SignatureAttribute => "Signature";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'PasswordRecoveryToken'
        /// </summary>
        public static string PasswordRecoveryTokenAttribute => "PasswordRecoveryToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'PasswordRecoveryTokenDateGenerated'
        /// </summary>
        public static string PasswordRecoveryTokenDateGeneratedAttribute => "PasswordRecoveryTokenDateGenerated";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'AccountActivationToken'
        /// </summary>
        public static string AccountActivationTokenAttribute => "AccountActivationToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'EmailRevalidationToken'
        /// </summary>
        public static string EmailRevalidationTokenAttribute => "EmailRevalidationToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LastVisitedPage'
        /// </summary>
        public static string LastVisitedPageAttribute => "LastVisitedPage";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'ImpersonatedCustomerId'
        /// </summary>
        public static string ImpersonatedCustomerIdAttribute => "ImpersonatedCustomerId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'AdminAreaStoreScopeConfiguration'
        /// </summary>
        public static string AdminAreaStoreScopeConfigurationAttribute => "AdminAreaStoreScopeConfiguration";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'SelectedPaymentMethod'
        /// </summary>
        public static string SelectedPaymentMethodAttribute => "SelectedPaymentMethod";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'SelectedShippingOption'
        /// </summary>
        public static string SelectedShippingOptionAttribute => "SelectedShippingOption";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'SelectedPickupPoint'
        /// </summary>
        public static string SelectedPickupPointAttribute => "SelectedPickupPoint";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'CheckoutAttributes'
        /// </summary>
        public static string CheckoutAttributes => "CheckoutAttributes";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'OfferedShippingOptions'
        /// </summary>
        public static string OfferedShippingOptionsAttribute => "OfferedShippingOptions";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LastContinueShoppingPage'
        /// </summary>
        public static string LastContinueShoppingPageAttribute => "LastContinueShoppingPage";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'NotifiedAboutNewPrivateMessages'
        /// </summary>
        public static string NotifiedAboutNewPrivateMessagesAttribute => "NotifiedAboutNewPrivateMessages";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'WorkingThemeName'
        /// </summary>
        public static string WorkingThemeNameAttribute => "WorkingThemeName";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'UseRewardPointsDuringCheckout'
        /// </summary>
        public static string UseRewardPointsDuringCheckoutAttribute => "UseRewardPointsDuringCheckout";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'EuCookieLawAccepted'
        /// </summary>
        public static string EuCookieLawAcceptedAttribute => "EuCookieLaw.Accepted";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'SelectedMultiFactorAuthProvider'
        /// </summary>
        public static string SelectedMultiFactorAuthenticationProviderAttribute => "SelectedMultiFactorAuthProvider";

        /// <summary>
        /// Gets a name of session key
        /// </summary>
        public static string CustomerMultiFactorAuthenticationInfo => "CustomerMultiFactorAuthenticationInfo";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'HideConfigurationSteps'
        /// </summary>
        public static string HideConfigurationStepsAttribute => "HideConfigurationSteps";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'CloseConfigurationSteps'
        /// </summary>
        public static string CloseConfigurationStepsAttribute => "CloseConfigurationSteps";

        #endregion
    }
}