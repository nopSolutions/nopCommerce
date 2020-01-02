using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    /// <summary>
    /// Represents a customer attribute value model
    /// </summary>
    public partial class CustomerAttributeValueModel : BaseNopEntityModel, ILocalizedModel<CustomerAttributeValueLocalizedModel>
    {
        #region Ctor

        public CustomerAttributeValueModel()
        {
            Locales = new List<CustomerAttributeValueLocalizedModel>();
        }

        #endregion

        #region Properties

        public int CustomerAttributeId { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<CustomerAttributeValueLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial class CustomerAttributeValueLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.Name")]
        public string Name { get; set; }
    }
}