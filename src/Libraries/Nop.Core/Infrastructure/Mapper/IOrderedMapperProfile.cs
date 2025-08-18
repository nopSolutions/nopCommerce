using Mapster;

namespace Nop.Core.Infrastructure.Mapper;

/// <summary>
/// Mapper profile registrar interface
/// </summary>
public partial interface IOrderedMapperProfile
{
    /// <summary>
    /// Gets order of this configuration implementation
    /// </summary>
    int Order { get; }
    
    /// <summary>
    /// Configure mappings for Mapster
    /// </summary>
    /// <param name="config">Type adapter configuration</param>
    void Configure(TypeAdapterConfig config);
}