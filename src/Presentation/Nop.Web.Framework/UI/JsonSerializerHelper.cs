using System.Text.Json;

namespace Nop.Web.Framework.UI;

public static class JsonSerializerHelper
{
    /// <summary>
    /// Define the default serialization options for the JSON serializer.
    /// </summary>
    /// <returns></returns>
    public static JsonSerializerOptions DefaultSerializationOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}