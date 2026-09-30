namespace Nop.Plugin.Api.Rest;

/// <summary>
/// Represents plugin constants
/// </summary>
public static class ApiRestDefaults
{
    /// <summary>
    /// Gets the configuration route name
    /// </summary>
    public static string ConfigurationRouteName => "Plugin.Api.Rest.Configure";

    /// <summary>
    /// Gets the name of the folder the plugin is deployed to
    /// </summary>
    /// <remarks>
    /// Must match the OutputPath of the plugin project ("$(SolutionDir)\Presentation\Nop.Web\Plugins\{this value}")
    /// </remarks>
    public static string OutputFolderName => "Api.Rest";

    /// <summary>
    /// Gets the relative path of the Swagger UI page
    /// </summary>
    public static string SwaggerUiPath => "swagger/api-rest/index.html";

    /// <summary>
    /// Gets the relative path of the Swagger JSON document
    /// </summary>
    public static string SwaggerJsonPath => "swagger/v1/swagger.json";
}
