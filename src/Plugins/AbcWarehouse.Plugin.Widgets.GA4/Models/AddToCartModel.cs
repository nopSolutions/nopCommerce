using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.GA4.Models
{
    public class AddToCartModel
    {
        public int Id { get; set; }
        public string ButtonId { get; set; }
        public GA4OrderItem Item { get; set; }
    }
}
