namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface IGeocodeService
    {
        (double lat, double lng) GeocodeZip(string zip);
    }
}