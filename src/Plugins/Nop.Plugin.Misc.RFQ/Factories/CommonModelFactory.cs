using Nop.Core.Domain.Catalog;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.RFQ.Factories;

public abstract class CommonModelFactory
{
    #region Fields

    private readonly IPictureService _pictureService;

    #endregion

    #region Ctor

    protected CommonModelFactory(IPictureService pictureService)
    {
        _pictureService = pictureService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare the picture model
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Product attributes xml</param>
    /// <param name="productName">Product name</param>
    /// <param name="imageSize">Image size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture model
    /// </returns>
    protected async Task<string> GetPictureUrlAsync(Product product, string attributesXml, string productName, int imageSize = 200)
    {
        var sciPicture = await _pictureService.GetProductPictureAsync(product, attributesXml);

        return (await _pictureService.GetPictureUrlAsync(sciPicture, imageSize)).Url;
    }

    #endregion
}