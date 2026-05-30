namespace Nop.Core.Domain.Media;

/// <summary>
/// Represents a video
/// </summary>
public partial class Video : BaseEntity
{
    /// <summary>
    /// Gets or sets the URL of video
    /// </summary>
    public string VideoUrl { get; set; }
}