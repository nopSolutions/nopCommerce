using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents Aws S3 storage configuration parameters
    /// </summary>
    public partial class AwsS3Config : IConfig
    {
        /// <summary>
        /// Gets or sets region for Aws S3 storage
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets S3 bucket name
        /// </summary>
        public string Bucket { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Aws key Id
        /// </summary>
        public string AccessKeyId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Aws secret access key
        /// </summary>
        public string SecretAccessKey { get; set; } = string.Empty;      

        /// <summary>
        /// Gets a value indicating whether we should use Aws S3 storage
        /// </summary>
        [JsonIgnore]
        public bool Enabled => !string.IsNullOrEmpty(SecretAccessKey) && !string.IsNullOrEmpty(AccessKeyId);

        /// <summary>
        /// Aws S3 object url template
        /// </summary>
        public readonly string ObjectUrlTemplate = "https://$bucket$.s3-$region$.amazonaws.com/$fileName$";
    }
}
