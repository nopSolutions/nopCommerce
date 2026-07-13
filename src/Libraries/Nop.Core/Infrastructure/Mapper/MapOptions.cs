using System.Linq.Expressions;

namespace Nop.Core.Infrastructure.Mapper;

/// <summary>
/// Represents options for mapping between source and destination objects
/// </summary>
public partial class MapOptions
{
    /// <summary>
    /// Indicates whether the mapping should be ignored
    /// </summary>
    public virtual bool IsIgnored { get; protected set; }

    /// <summary>
    /// Set the mapping to be ignored
    /// </summary>
    public void Ignore()
    {
        IsIgnored = true;
    }

    /// <summary>
    /// Gets or sets the source expression for mapping
    /// </summary>
    public virtual Expression<Func<object, object>> Source { get; set; }

    /// <summary>
    /// Sets the source expression for mapping
    /// </summary>
    /// <param name="source"> The source expression for mapping</param>
    public void MapFrom(Expression<Func<object, object>> source)
    {
        Source = source;
    }
}