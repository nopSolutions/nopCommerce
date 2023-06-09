namespace Nop.Plugin.Misc.InfigoProductProvider;

public class InfigoProductProviderDefaults
{
    public static string SystemName => "Misc.InfigoProductProvider";
    public static string ConfigurationRouteName => "Plugin.Misc.InfigoProductProvider.Configure";
    public static string ConfigurationPath => "~/Plugins/Misc.InfigoProductProvider/Views/Configure.cshtml";
    public static string SearchByExternalIdPath =>
        "~/Plugins/Misc.InfigoProductProvider/Views/SearchByExternalId.cshtml";
    public static string SpecificationAttributeForExternalId => "InfigoExternalId";
    public static string DefaultPictureUrl =>
        "https://c2318.qa.infigosoftware.rocks/-4856815/Handler/Picture/PI/T/0000659_nop_200.jpeg";
}