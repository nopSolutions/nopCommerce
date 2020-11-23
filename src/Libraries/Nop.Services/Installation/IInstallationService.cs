using System.Globalization;

namespace Nop.Services.Installation
{
    /// <summary>
    /// Installation service
    /// </summary>
    public partial interface IInstallationService
    {
        /// <summary>
        /// Install required data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        /// <param name="defaultUserPassword">Default user password</param>
        /// <param name="languagePackDownloadLink">Language pack download link</param>
        /// <param name="regionInfo">RegionInfo</param>
        /// <param name="cultureInfo">CultureInfo</param>
        void InstallRequiredData(string defaultUserEmail, string defaultUserPassword,
            string languagePackDownloadLink, RegionInfo regionInfo, CultureInfo cultureInfo);

        /// <summary>
        /// Install sample data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        void InstallSampleData(string defaultUserEmail);
    }
}
