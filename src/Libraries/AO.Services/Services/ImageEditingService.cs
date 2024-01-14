using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;

namespace AO.Services.Services
{
    public class ImageEditingService : IImageEditingService
    {
        public void CropImageToSquare(string inputFilePath, string outputFilePath, string newImageName, int targetSize)
        {
            // Validate input image path
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException($"Input image file not found: {inputFilePath}");
            }

            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                                    {
                                        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" // Add other formats if necessary
                                    };

            if (!allowedExtensions.Contains(Path.GetExtension(inputFilePath)))
            {
                throw new ArgumentException($"Invalid image format for: {inputFilePath}");
            }

            // Validate output directory
            if (!Directory.Exists(outputFilePath))
            {
                throw new DirectoryNotFoundException($"Output directory not found: {outputFilePath}");
            }

            using (var image = SixLabors.ImageSharp.Image.Load(inputFilePath))
            {
                int size = Math.Max(image.Width, image.Height);

                if (size > targetSize)
                {
                    size = targetSize;
                }

                // Resize the image to a square
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new SixLabors.ImageSharp.Size(size),
                    Mode = ResizeMode.Pad,
                    Position = AnchorPositionMode.Center,
                    PadColor = SixLabors.ImageSharp.Color.White
                }));

                // Save the image
                image.Save($@"{outputFilePath}\{newImageName}");
            }
        }
    }
}