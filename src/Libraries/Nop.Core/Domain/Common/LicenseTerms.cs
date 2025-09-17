using Newtonsoft.Json;

namespace Nop.Core.Domain.Common;

public partial class LicenseTerms : List<LicenseTermsInfo>;

public partial class LicenseTermsInfo
{
    #region Properties

    [JsonProperty(PropertyName = "version")]
    public string Version { get; set; }

    [JsonProperty(PropertyName = "installationDate")]
    public DateTime? InstallationDate { get; set; }

    [JsonProperty(PropertyName = "acceptedLicenseTerms")]
    public bool AcceptedLicenseTerms { get; set; }

    [JsonProperty(PropertyName = "lastCheckDate")]
    public DateTime? LastCheckDate { get; set; }

    #endregion
}