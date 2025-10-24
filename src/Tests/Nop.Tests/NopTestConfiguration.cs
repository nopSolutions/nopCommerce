using System.Diagnostics;
using Nop.Core.Domain.ArtificialIntelligence;

namespace Nop.Tests;

public static partial class NopTestConfiguration
{
    /// <summary>
    ///  Gets the connection string for MySQL server
    /// </summary>
    public static string MySqlServerConnectionString => "";

    /// <summary>
    /// Gets the connection string for PostgreSQL server
    /// </summary>
    public static string PostgreSqlServerConnectionString => "";

    /// <summary>
    /// Gets the connection string for MS SQL server
    /// </summary>
    public static string SqlServerConnectionString => "";

    /// <summary>
    /// Gets the connection string for SQLite server
    /// </summary>
    public static string SqliteConnectionString => "Data Source=nopCommerceTest.sqlite;Mode=Memory;Cache=Shared";

    /// <summary>
    /// Gets the artificial intelligence settings for testing purposes
    /// </summary>
    public static ArtificialIntelligenceSettings GetArtificialIntelligenceSettings(
        ArtificialIntelligenceProviderType providerType = ArtificialIntelligenceProviderType.ChatGpt)
    {
        return new()
        {
            RequestTimeout = 30,
            ProviderType = providerType,
            ChatGptApiKey = string.Empty,
            GeminiApiKey = string.Empty,
            DeepSeekApiKey = string.Empty
        };
    }

    /// <summary>
    /// Checks if artificial intelligence provider configured
    /// </summary>
    /// <param name="settings">Artificial intelligence settings instance</param>
    /// <returns>True if provider configured, otherwise - false</returns>
    public static bool IsProviderConfigured(this ArtificialIntelligenceSettings settings)
    {
        var result = settings.ProviderType switch
        {
            ArtificialIntelligenceProviderType.Gemini => !string.IsNullOrEmpty(settings.GeminiApiKey),
            ArtificialIntelligenceProviderType.ChatGpt => !string.IsNullOrEmpty(settings.ChatGptApiKey),
            ArtificialIntelligenceProviderType.DeepSeek => !string.IsNullOrEmpty(settings.DeepSeekApiKey),
            _ => throw new ArgumentOutOfRangeException()
        };

        Debug.WriteLineIf(!result, $"Artificial intelligence provider {settings.ProviderType.ToString()} not configured");

        return result;
    }
}
