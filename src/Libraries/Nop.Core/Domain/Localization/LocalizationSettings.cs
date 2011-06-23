using Nop.Core.Configuration;

namespace Nop.Core.Domain.Localization
{
    public class LocalizationSettings : ISettings
    {
        /// <summary>
        /// Default admin area language identifer
        /// </summary>
        public int DefaultAdminLanguageId { get; set; }

        /// <summary>
        /// Use images for language selection
        /// </summary>
        public bool UseImagesForLanguageSelection { get; set; }
    }
}