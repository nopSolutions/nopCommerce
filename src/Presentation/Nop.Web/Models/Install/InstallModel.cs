using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Install;

namespace Nop.Web.Models.Install
{
    [Validator(typeof(InstallValidator))]
    public partial class InstallModel : BaseNopModel
    {
        public InstallModel()
        {
            this.AvailableLanguages = new List<SelectListItem>();
        }
        [AllowHtml]
        public string AdminEmail { get; set; }
        [AllowHtml]
        [NoTrim]
        [DataType(DataType.Password)]
        public string AdminPassword { get; set; }
        [AllowHtml]
        [NoTrim]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        [AllowHtml]
        public string DatabaseConnectionString { get; set; }
        public string DataProvider { get; set; }
        public bool DisableSqlCompact { get; set; }
        //SQL Server properties
        public string SqlConnectionInfo { get; set; }
        [AllowHtml]
        public string SqlServerName { get; set; }
        [AllowHtml]
        public string SqlDatabaseName { get; set; }
        [AllowHtml]
        public string SqlServerUsername { get; set; }
        [AllowHtml]
        public string SqlServerPassword { get; set; }
        public string SqlAuthenticationType { get; set; }
        public bool SqlServerCreateDatabase { get; set; }

        public bool UseCustomCollation { get; set; }
        [AllowHtml]
        public string Collation { get; set; }


        public bool DisableSampleDataOption { get; set; }
        public bool InstallSampleData { get; set; }

        public List<SelectListItem> AvailableLanguages { get; set; }
    }
}