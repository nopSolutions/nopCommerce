using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Widgets.AbcPickupInStore.Models
{
    public class PickupInStoreModel
    {
        [Required]
        [NopResourceDisplayName(PickStoreWidget.LocaleKey.PickupInStoreText)]
        public string PickupInStoreText { get; set; }
    }
}
