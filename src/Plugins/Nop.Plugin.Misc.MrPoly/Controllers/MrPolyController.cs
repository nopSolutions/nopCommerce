using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace Nop.Plugin.Misc.MrPoly.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class MrPolyController : BasePluginController
    {
        private readonly IProductService _productService;
        private readonly IRepository<Product> _productRepository;
        private readonly IPictureService _pictureService;

        public MrPolyController(IProductService productService, IRepository<Product> productRepository, IPictureService pictureService)
        {
            _productService = productService;
            _productRepository = productRepository;
            _pictureService = pictureService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Plugins/Misc.MrPoly/Views/Index.cshtml");
        }

        [HttpPost]
        [Route("api/mrpoly/save_product/{barcode}")]
        public async Task<IActionResult> SaveProduct(string barcode)
        {
            RemoteWebDriver driver = null; 

            try
            {
                if (string.IsNullOrWhiteSpace(barcode))
                {
                    return BadRequest("Invalid barcode");
                }

                if (!Regex.IsMatch(barcode, @"^[0-9]*$"))
                {
                    return BadRequest("Invalid barcode. Expecting numbers only.");
                }

                var existingProduct = _productRepository.Table.FirstOrDefault(x => x.Gtin == barcode);

                if (existingProduct != null)
                {
                    return BadRequest($"Product with GTIN {barcode} already exists ({existingProduct.Name})");
                }

                driver = new ChromeDriver(@"C:\selenium");

                driver.Navigate().GoToUrl($"https://www.amazon.com/s?k={barcode}&ref=nb_sb_noss");

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                var searchResults = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("s-result-item")));

                const string noProductMsg = "Sorry, could not find a product with this barcode. Try another product.";

                if (searchResults.Text.Contains("No results"))
                {
                    return BadRequest(noProductMsg);
                }

                var results = driver.FindElementsByCssSelector(".s-result-item a .a-text-normal");

                if (!results.Any())
                {
                    return BadRequest(noProductMsg);
                }

                results[0].Click();

                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("altImages")));
                var titleElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("productTitle")));

                string title = null;
                if (titleElem != null)
                {
                    title = titleElem.Text;
                }

                string brand = null;
                var brandElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("bylineInfo")));
                if (brandElem != null)
                {
                    brand = brandElem.Text.Replace("Brand: ", string.Empty);
                }

                List<string> features = new List<string>();

                var featureBulletsElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("feature-bullets")));
                if (featureBulletsElem != null)
                {
                    var bullets = featureBulletsElem.FindElements(By.CssSelector(".a-list-item"));

                    foreach (var bullet in bullets)
                    {
                        features.Add(bullet.Text);
                    }
                }

                var mainImage = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("imgTagWrapperId")));

                mainImage.Click();

                var ivTabViewContainer = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("iv-tab-view-container")));

                var imageThumbs = ivTabViewContainer.FindElements(By.CssSelector("#ivThumbs .ivThumb"));

                var imageUrls = new List<string>();

                int i = 0;
                foreach (var thumb in imageThumbs)
                {

                    if (thumb.Displayed && thumb.Enabled)
                    {
                        thumb.Click();

                        int continueCnt = 0;

                        while (true)
                        {
                            await Task.Delay(50);
                            var largeImg = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#ivLargeImage img")));

                            if (largeImg != null)
                            {
                                var src = largeImg.GetAttribute("src");

                                if (continueCnt++ > 100)
                                {
                                    break;
                                }
                                if (imageUrls.Any() && imageUrls.Last() == src)
                                {
                                    continue;
                                }

                                if (src != null)
                                {
                                    imageUrls.Add(src);
                                }

                                break;
                            }
                        }
                    }

                    i++;
                }

                var popoverCloseBtn = driver.FindElementByCssSelector(".a-popover .a-button-close");

                if (popoverCloseBtn != null)
                {
                    popoverCloseBtn.Click();
                }

                StringBuilder sb = new StringBuilder();

                if (features.Any())
                {
                    sb.Append("<ul>");
                    foreach (var feature in features)
                    {
                        sb.Append("<li>");
                        sb.Append(feature);
                        sb.Append("</li>");
                    }
                    sb.Append("</ul>");
                }

                var product = new Product
                {
                    Name = title.ToUpper(),
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    Gtin = barcode,
                    FullDescription = sb.ToString(),
                    Price = 12,
                    StockQuantity = 1,
                    ManageInventoryMethodId = (int)ManageInventoryMethod.ManageStock,
                    MinStockQuantity = 1,
                    LowStockActivityId = (int)LowStockActivity.Unpublish,
                    DisplayStockQuantity = true,
                    DisplayStockAvailability = true,
                    Published = true,
                    VisibleIndividually = true,
                    AllowCustomerReviews = true,
                    MarkAsNew = true,
                    NotifyAdminForQuantityBelow = 1,
                    OrderMinimumQuantity = 1,
                    OrderMaximumQuantity = 10000
                };

                await _productService.InsertProductAsync(product);

                for (int j = 0; j < imageUrls.Count; j++)
                {
                    var imageUrl = imageUrls[j];

                    using (var httpClient = new HttpClient())
                    {
                        var bytes = await httpClient.GetByteArrayAsync(imageUrl);

                        string mimeType;
                        using (var stream = new MemoryStream(bytes))
                        {
                            stream.Position = 0;

                            var image = Image.FromStream(stream);
                            ImageFormat format = image.RawFormat;
                            ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == format.Guid);
                            mimeType = codec.MimeType;
                        }

                        Uri uri = new Uri(imageUrl);
                        string filename = System.IO.Path.GetFileName(uri.LocalPath);

                        var seoName = await _pictureService.GetPictureSeNameAsync(filename);

                        var picture = await _pictureService.InsertPictureAsync(bytes, mimeType, seoName);

                        var productPicture = new ProductPicture
                        {
                            DisplayOrder = j + 1,
                            ProductId = product.Id,
                            PictureId = picture.Id
                        };
                        await _productService.InsertProductPictureAsync(productPicture);
                    }
                }

                var nextUrl = $"/Admin/Product/Edit/{product.Id}";

                return Ok(new { nextUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.StackTrace);
            }
            finally
            {
                if (driver != null)
                {
                    driver.Close();
                }         
            }

        }
    }
}
