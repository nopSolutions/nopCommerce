using System.Collections.Generic;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Plugins;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Plugins
{
    [Validator(typeof(PluginValidator))]
    public partial class PluginModel : BaseNopModel, ILocalizedModel<PluginLocalizedModel>
    {
        public PluginModel()
        {
            Locales = new List<PluginLocalizedModel>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Group")]
        public string Group { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
        public string FriendlyName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.SystemName")]
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Version")]
        public string Version { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Author")]
        public string Author { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Configure")]
        public string ConfigurationUrl { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Installed")]
        public bool Installed { get; set; }
        
        public string Description { get; set; }

        public bool CanChangeEnabled { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.IsEnabled")]
        public bool IsEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.Logo")]
        public string LogoUrl { get; set; }

        public IList<PluginLocalizedModel> Locales { get; set; }

        //ACL (customer roles)
        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.AclCustomerRoles")]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        //store mapping
        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }

    public partial class PluginLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
        public string FriendlyName { get; set; }
    }
}