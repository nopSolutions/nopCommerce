using System;
using System.ComponentModel;
using System.Linq;
using Nop.Core.Configuration;

namespace Nop.Services.Configuration
{
    public class ConfigurationProvider<TSettings> : IConfiguration<TSettings> where TSettings : ISettings, new()
    {
        readonly ISettingService settingService;

        public ConfigurationProvider(ISettingService settingService) 
        {
            this.settingService = settingService;
            this.BuildConfiguration();
        }
        
        public TSettings Settings { get; private set; }

        private void BuildConfiguration() 
        {
            Settings = Activator.CreateInstance<TSettings>();

            // get properties we can write to
            //TODO ensure that we can convert Enum (e.g., TaxSettings.TaxBasedOn)
            //TODO support default values (set using attributes)
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             let setting = settingService.GetSettingByKey<string>(typeof(TSettings).Name + "." + prop.Name)
                             where setting != null
                             where TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string))
                             let value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting)
                             select new { prop, value };

            // assign properties
            properties.ToList().ForEach(p => p.prop.SetValue(Settings, p.value, null));
        }
    }
}
