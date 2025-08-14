namespace Nop.Services.Media;

/// <summary>
/// Represents default values related to media services
/// </summary>
public static partial class NopMediaDefaults
{
    /// <summary>
    /// Gets a multiple thumb directories length
    /// </summary>
    public static int MultipleThumbDirectoriesLength => 3;

    /// <summary>
    /// Gets a path to the image thumbs files
    /// </summary>
    public static string ImageThumbsPath => "thumbs";

    /// <summary>
    /// Gets a default path to the image files
    /// </summary>
    public static string DefaultImagesPath => "images";

    /// <summary>
    /// Gets a default avatar file name
    /// </summary>
    public static string DefaultAvatarFileName => "default-avatar.jpg";

    /// <summary>
    /// Gets a default image file name
    /// </summary>
    public static string DefaultImageFileName => "default-image.png";
}