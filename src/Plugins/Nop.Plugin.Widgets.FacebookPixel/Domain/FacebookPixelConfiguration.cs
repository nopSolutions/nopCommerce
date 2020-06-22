using Nop.Core;

namespace Nop.Plugin.Widgets.FacebookPixel.Domain
{
    /// <summary>
    /// Represents a Facebook Pixel configuration
    /// </summary>
    public class FacebookPixelConfiguration : BaseEntity
    {
        /// <summary>
        /// Gets or sets a Pixel identifier
        /// </summary>
        public string PixelId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Pixel is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable Pixel for users not accepting Cookie Consent (due to GDPR)
        /// </summary>
        public bool DisableForUsersNotAcceptingCookieConsent { get; set; }

        /// <summary>
        /// Gets or sets a store identifier in which Pixel is used
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to pass customer data
        /// </summary>
        public bool PassUserProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use advanced matching
        /// </summary>
        public bool UseAdvancedMatching { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track PageView event
        /// </summary>
        public bool TrackPageView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track AddToCart event
        /// </summary>
        public bool TrackAddToCart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track Purchase event
        /// </summary>
        public bool TrackPurchase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track ViewContent event
        /// </summary>
        public bool TrackViewContent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track AddToWishlist event
        /// </summary>
        public bool TrackAddToWishlist { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track InitiateCheckout event
        /// </summary>
        public bool TrackInitiateCheckout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track Search event
        /// </summary>
        public bool TrackSearch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track Contact event
        /// </summary>
        public bool TrackContact { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track CompleteRegistration event
        /// </summary>
        public bool TrackCompleteRegistration { get; set; }

        /// <summary>
        /// Gets or sets a serialized custom events configuration
        /// </summary>
        public string CustomEvents { get; set; }
    }
}