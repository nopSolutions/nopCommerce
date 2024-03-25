using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.AddressAutocomplete.Models;

namespace AbcWarehouse.Plugin.Widgets.AddressAutocomplete
{
    public class AddressAutocompleteSettings : ISettings
    {
        public string GooglePlacesApiKey { get; private set; }

        public static AddressAutocompleteSettings FromModel(ConfigModel model)
        {
            return new AddressAutocompleteSettings()
            {
                GooglePlacesApiKey = model.GooglePlacesApiKey
            };
        }

        public ConfigModel ToModel()
        {
            var model = new ConfigModel();

            model.GooglePlacesApiKey = GooglePlacesApiKey;

            return model;
        }
    }
}
