namespace AO.Services.Services
{
    public interface IImageEditingService
    {
        void CropImageToSquare(string inputFilePath, string outputFilePath, string newImageName, int targetSize);
    }
}