using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Plugin.Misc.MediaMigration.Data;
using Nop.Services.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.MediaMigration.Controllers;


[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]

public class MediaMigrationController : BasePluginController
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IProductService _productService;
    private readonly IPictureService _pictureService;
    private readonly IVideoService _videoService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly ILogger _logger;
    private readonly OldStoreDbContext _oldDb;



    public MediaMigrationController(IHttpClientFactory httpClientFactory, IPictureService pictureService, IUrlRecordService urlRecordService, OldStoreDbContext oldDb, IProductService productService, IVideoService videoService, ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _pictureService = pictureService;
        _urlRecordService = urlRecordService;
        _oldDb = oldDb;
        _productService = productService;
        _videoService = videoService;
        _logger = logger;
    }

    public async Task<IActionResult> ImportProductImages()
    {
        decimal[] medias = [8, 9, 10, 11, 12, 13, 14, 15, 16, 17];
        decimal[] videotypes = [18, 19, 20];

        var products = await _productService.SearchProductsAsync(showHidden: true);
        var productIds = products.Select(c => c.Id).ToList();
        var oldProducts = await _oldDb.Persistent_Map_Products.Where(c => productIds.Contains(c.NewId)).ToListAsync();
        var productMedias = await _oldDb.ProductMedias.Where(c => !c.PM_Deleted).ToListAsync();
        foreach (var product in products)
        {
            try
            {
                var oldProduct = oldProducts.FirstOrDefault(c => c.NewId == product.Id);
                if (oldProduct != null)
                {
                    var oldProductId = oldProduct.OldId;

                    // تصاویر
                    var images = productMedias.Where(c => c.PM_ProductID == oldProductId && medias.Contains(c.PM_MediaTypeID))
                        .OrderBy(c => c.PM_MediaTypeID)
                        .ToList();
                    foreach (var image in images)
                    {
                        if (!string.IsNullOrEmpty(image?.PM_Picture))
                        {
                            var pictureId = await DownloadAndSaveImageAsync(image.PM_Picture, product?.Name ?? Guid.NewGuid().ToString());
                            if (pictureId > 0)
                            {
                                await _productService.InsertProductPictureAsync(new ProductPicture
                                {
                                    ProductId = product.Id,
                                    PictureId = pictureId
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Error processing product ID {product.Id}", ex);
            }

        }
        return RedirectToAction("List", "Product");
    }


    /// <summary>
    /// Download category image and save it in NopCommerce
    /// </summary>
    /// 
    [NonAction]
    private async Task<int> DownloadAndSaveImageAsync(string imageUrl, string name)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return 0;
        imageUrl = "https://ddl.etminanshop.com/" + imageUrl;
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(imageUrl);

        if (!response.IsSuccessStatusCode)
            return 0;

        var imageBytes = await response.Content.ReadAsByteArrayAsync();
        if (imageBytes.Length == 0)
            return 0;


        var picture = await _pictureService.InsertPictureAsync(imageBytes, $"image/webp", await _urlRecordService.GetSeNameAsync(name, true, true));
        if (picture == null || picture.Id <= 0)
            return 0;

        return picture.Id;
    }


    /// <summary>
    /// Download category image and save it in NopCommerce
    /// </summary>
    /// 
    [NonAction]
    private async Task<int> DownloadAndSaveVideoAsync(string videoUrl, string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoUrl))
            return 0;
        videoUrl = "https://ddl.etminanshop.com/" + videoUrl;
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(videoUrl, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return 0;

        var videoBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        if (videoBytes.Length == 0)
            return 0;


        var video = await _videoService.InsertVideoAsync(new Video
        {
            VideoUrl = videoUrl,
        });
        if (video == null || video.Id <= 0)
            return 0;

        return video.Id;
    }

}
