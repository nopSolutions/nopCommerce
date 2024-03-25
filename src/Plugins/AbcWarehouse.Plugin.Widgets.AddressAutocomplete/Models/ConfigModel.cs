using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.AddressAutocomplete.Models
{
    public class ConfigModel
    {
        [NopResourceDisplayName(AddressAutocompleteLocales.GooglePlacesApiKey)]
        public string GooglePlacesApiKey { get; set; }
    }
}
