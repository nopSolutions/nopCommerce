namespace Nop.Plugin.Misc.Omnisend.DTO;

public static class DtoExtensions
{
    public static string ToDtoString(this DateTime date)
    {
        return date.ToString("s") + "Z";
    }
}