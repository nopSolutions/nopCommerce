using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Data;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Install
{
    public partial class InstallModel : BaseNopModel
    {
        public InstallModel()
        {
            AvailableLanguages = new List<SelectListItem>();
        }

        public string AdminEmail { get; set; }

        [NoTrim]
        [DataType(DataType.Password)]
        public string AdminPassword { get; set; }

        [NoTrim]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public bool SqlServerCreateDatabase { get; set; }

        public bool UseCustomCollation { get; set; }
        public string Collation { get; set; }

        public bool DisableSampleDataOption { get; set; }

        public bool InstallSampleData { get; set; }

        public List<SelectListItem> AvailableLanguages { get; set; }
    }
}