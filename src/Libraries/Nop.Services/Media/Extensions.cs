using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;

namespace Nop.Services.Media;

/// <summary>
/// Extensions
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Get absolute path for local images
    /// </summary>
    /// <param name="fileProvider">Nop file provider</param>
    /// <param name="mediaSettings">Media settings</param>
    /// <param name="path"></param>
    /// <returns>Get local images path</returns>
    public static string GetLocalImagesPath(this INopFileProvider fileProvider, MediaSettings mediaSettings, string path = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            var imagePath = mediaSettings.PicturePath;

            path = string.IsNullOrEmpty(imagePath) ? NopMediaDefaults.DefaultImagesPath : imagePath;
        }

        if (!fileProvider.IsPathRooted(path))
            path = fileProvider.GetAbsolutePath(path);

        return path;
    }
}