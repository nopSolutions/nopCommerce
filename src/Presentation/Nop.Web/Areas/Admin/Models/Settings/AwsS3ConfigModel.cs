using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents Aws S3 storage configuration model
    /// </summary>

    public partial record AwsS3ConfigModel : BaseNopModel, IConfigModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Configuration.AppSettings.AwsS3.Region")]
        public string Region { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.AwsS3.Bucket")]
        public string Bucket { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.AwsS3.AccessKeyId")]
        public string AccessKeyId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.AwsS3.SecretAccessKey")]
        public bool SecretAccessKey { get; set; }

        #endregion
    }
}
