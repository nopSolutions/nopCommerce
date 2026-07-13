namespace Nop.Core.Infrastructure.Mapper;

/// <summary>
/// Mapper profile registrar interface
/// </summary>
public partial interface IOrderedMapperProfile
{
    /// <summary>
    /// Gets order of this configuration implementation
    /// </summary>
    /// <remarks>Works only with Init method implemented</remarks>
    int Order { get; }

    /// <summary>
    /// Initializes the mapping configuration for this profile. This method is called during the application startup to register the mapping rules defined in the profile
    /// </summary>
    void Init();
}