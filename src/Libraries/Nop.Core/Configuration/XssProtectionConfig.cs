using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents XSS protection parameters
    /// </summary>
    public partial class XssProtectionConfig : IConfig
    {
        /// <summary>
        /// Gets or sets a value specifing which code points are allowed to be represented unescaped by the encoders.
        /// </summary>
        public string[] TextEncoderAllowedRanges { get; set; } = new[] { nameof(UnicodeRanges.BasicLatin) };

        /// <summary>
        /// Gets a value of TextEncoderSettings.
        /// </summary>
        public TextEncoderSettings TextEncoderSettings
        { 
            get 
            {
                var allowedRanges = TextEncoderAllowedRanges
                    .Select(r => typeof(UnicodeRanges).GetProperty(r.Split('.').Last()).GetValue(null))
                    .Cast<UnicodeRange>()
                    .ToArray();

                return new TextEncoderSettings(allowedRanges);
            } 
        }

        /// <summary>
        /// Get or set a value of HtmlSanitizer parameters
        /// </summary>
        public string HtmlSanitizerSettings { get; set; } = "for future use of mganss/HtmlSanitizer";
    }
}
