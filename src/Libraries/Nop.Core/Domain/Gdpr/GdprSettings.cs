using Nop.Core.Configuration;

namespace Nop.Core.Domain.Gdpr
{
    /// <summary>
    /// GDPR settings
    /// </summary>
    public class GdprSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether GDPR is enabled
        /// </summary>
        public bool GdprEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should log "accept privacy policy" consent
        /// </summary>
        public bool LogPrivacyPolicyConsent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should log "newsletter" consent
        /// </summary>
        public bool LogNewsletterConsent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should log changes in user profile
        /// </summary>
        public bool LogUserProfileChanges { get; set; }
    }
}