using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Shipping;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Shipping
{
    [Validator(typeof(DeliveryDateValidator))]
    public partial class DeliveryDateModel : BaseNopEntityModel, ILocalizedModel<DeliveryDateLocalizedModel>
    {
        public DeliveryDateModel()
        {
            Locales = new List<DeliveryDateLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.Configuration.Shipping.DeliveryDates.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.DeliveryDates.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<DeliveryDateLocalizedModel> Locales { get; set; }
    }

    public partial class DeliveryDateLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.DeliveryDates.Fields.Name")]
        public string Name { get; set; }
    }
}