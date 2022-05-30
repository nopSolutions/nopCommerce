using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Data;

namespace Nop.Web.Models.Install
{
    public partial record InstallModel : INopConnectionStringInfo
    {
        public InstallModel()
        {
            AvailableLanguages = new List<SelectListItem>();
            AvailableDataProviders = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
        }

        [DataType(DataType.EmailAddress)]
        public string AdminEmail { get; set; }
        [DataType(DataType.Password)]
        public string AdminPassword { get; set; }
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public bool UseCustomCollation { get; set; }

        public string Collation { get; set; }

        public bool CreateDatabaseIfNotExists { get; set; }
        public bool DisableSampleDataOption { get; set; }
        public bool InstallSampleData { get; set; }
        public bool ConnectionStringRaw { get; set; }

        public bool InstallRegionalResources { get; set; }

        public string DatabaseName { get; set; }
        public string ServerName { get; set; }

        public bool IntegratedSecurity { get; set; }

        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ConnectionString { get; set; }

        public List<SelectListItem> AvailableLanguages { get; set; }

        public List<SelectListItem> AvailableCountries { get; set; }

        public DataProviderType DataProvider { get; set; }

        public string Country { get; set; }

        public List<SelectListItem> AvailableDataProviders { get; set; }
        public IDictionary<string, string> RawDataSettings => new Dictionary<string, string>();

        public string RestartUrl { get; set; }
    }
}