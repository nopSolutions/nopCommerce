
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    public class PdfSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating PDF is supported
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a PDF logo file path
        /// </summary>
        public string LogoFilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to render order notes in PDf reports
        /// </summary>
        public bool RenderOrderNotes { get; set; }
    }
}