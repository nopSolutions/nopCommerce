using Nop.Core.Configuration;
using Nop.Plugin.Misc.AbcCore.Models;
using System.Configuration;
using System.Data.Odbc;

namespace Nop.Plugin.Misc.AbcCore
{
    public class CoreSettings : ISettings
    {
        public string BackendDbConnectionString { get; private set; }
        public bool AreExternalCallsSkipped { get; private set; }
        public bool IsDebugMode { get; private set; }
        public string MobilePhoneNumber { get; private set; }
        public string GoogleMapsGeocodingAPIKey { get; private set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(BackendDbConnectionString) &&
                   !string.IsNullOrWhiteSpace(MobilePhoneNumber) &&
                   !string.IsNullOrWhiteSpace(GoogleMapsGeocodingAPIKey);
        }

        public static CoreSettings FromModel(ConfigurationModel model)
        {
            return new CoreSettings()
            {
                BackendDbConnectionString = model.BackendDbConnectionString,
                AreExternalCallsSkipped = model.AreExternalCallsSkipped,
                IsDebugMode = model.IsDebugMode,
                MobilePhoneNumber = model.MobilePhoneNumber,
                GoogleMapsGeocodingAPIKey = model.GoogleMapsGeocodingAPIKey
            };
        }

        public ConfigurationModel ToModel()
        {
            return new ConfigurationModel
            {
                BackendDbConnectionString = BackendDbConnectionString,
                AreExternalCallsSkipped = AreExternalCallsSkipped,
                IsDebugMode = IsDebugMode,
                MobilePhoneNumber = MobilePhoneNumber,
                GoogleMapsGeocodingAPIKey = GoogleMapsGeocodingAPIKey
            };
        }
        public OdbcConnection GetBackendDbConnection()
        {
            if (string.IsNullOrWhiteSpace(BackendDbConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "Backend DB connection string is not set, " +
                    "please set in AbcCore configuration.");
            }

            return new OdbcConnection(BackendDbConnectionString);
        }
    }
}