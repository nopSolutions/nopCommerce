namespace Nop.Plugin.Misc.AbcCore
{
    public class CoreLocales
    {
        public const string Base = "Plugins.Misc.AbcCore.Fields.";

        public const string BackendDbConnectionString =
            Base + "BackendDbConnectionString";
        public const string BackendDbConnectionStringHint =
            BackendDbConnectionString + ".Hint";

        public const string AreExternalCallsSkipped =
            Base + "AreExternalCallsSkipped";
        public const string AreExternalCallsSkippedHint =
            AreExternalCallsSkipped + ".Hint";

        public const string IsDebugMode =
            Base + "IsTraceMode";
        public const string IsDebugModeHint =
            IsDebugMode + ".Hint";

        public const string PLPDescription =
            Base + "PLPDescription";
        public const string PLPDescriptionHint =
            PLPDescription + ".Hint";

        public const string MobilePhoneNumber =
            Base + "MobilePhoneNumber";
        public const string MobilePhoneNumberHint =
            MobilePhoneNumber + ".Hint";

        public const string GoogleMapsGeocodingAPIKey =
            Base + "GoogleMapsGeocodingAPIKey";
        public const string GoogleMapsGeocodingAPIKeyHint =
            GoogleMapsGeocodingAPIKey + ".Hint";
        public const string IsFedExMode =
            Base + "IsFedExMode";
        public const string IsFedExModeHint =
            IsFedExMode + ".Hint";
    }
}