using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#if NET451
using System.Web.Mvc;
#endif
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Validators.Install;

namespace Nop.Web.Models.Install
{
    [Validator(typeof(InstallValidator))]
    public partial class InstallModel : BaseNopModel
    {
        public InstallModel()
        {
#if NET451
            this.AvailableLanguages = new List<SelectListItem>();
#endif
        }

#if NET451
		[AllowHtml]
#endif
        public string AdminEmail { get; set; }
        	
#if NET451
		[AllowHtml]
#endif
        [NoTrim]
        [DataType(DataType.Password)]
        public string AdminPassword { get; set; }
        	
#if NET451
		[AllowHtml]
#endif
        [NoTrim]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        	
#if NET451
		[AllowHtml]
#endif
        public string DatabaseConnectionString { get; set; }
        public string DataProvider { get; set; }
        public bool DisableSqlCompact { get; set; }
        //SQL Server properties
        public string SqlConnectionInfo { get; set; }
        	
#if NET451
		[AllowHtml]
#endif
        public string SqlServerName { get; set; }
        	
#if NET451
		[AllowHtml]
#endif
        public string SqlDatabaseName { get; set; }
        	
#if NET451
		[AllowHtml]
#endif
        public string SqlServerUsername { get; set; }
        	
#if NET451
		[AllowHtml]
#endif
        public string SqlServerPassword { get; set; }
        public string SqlAuthenticationType { get; set; }
        public bool SqlServerCreateDatabase { get; set; }

        public bool UseCustomCollation { get; set; }
        	
#if NET451
		[AllowHtml]
#endif
        public string Collation { get; set; }


        public bool DisableSampleDataOption { get; set; }
        public bool InstallSampleData { get; set; }

#if NET451
        public List<SelectListItem> AvailableLanguages { get; set; }
#endif
    }
}