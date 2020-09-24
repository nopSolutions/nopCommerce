using System.Collections.Generic;

namespace Nop.Services.Themes
{
    /// <summary>
    /// Themes uploaded event
    /// </summary>
    public class ThemesUploadedEvent
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="uploadedThemes">Uploaded themes</param>
        public ThemesUploadedEvent(IList<ThemeDescriptor> uploadedThemes)
        {
            UploadedThemes = uploadedThemes;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Uploaded themes
        /// </summary>
        public IList<ThemeDescriptor> UploadedThemes { get; }

        #endregion
    }
}