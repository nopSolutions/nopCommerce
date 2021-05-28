using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nop.Plugin.Misc.BuyAmScraper.Models;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using PuppeteerSharp;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Services.Customers;
using Nop.Services.Vendors;
using System.Collections.Generic;
using Nop.Services.Media;
using Nop.Core;
using Nop.Services.Seo;

namespace Nop.Plugin.BuyAmScraper.Service
{
    class BuyAmScraperTask : Services.Tasks.IScheduleTask
    {
        private const int SCROLL_HEIGHT = 300;
        private const string CARREFOUR_CUSTOMER_NAME = "Carrefour";
        private readonly string[] _categoryUrlsToScrape;

        private ILogger _logger;
        private IProductService _productService;
        private ICustomerService _customerService;
        private IVendorService _vendorService;
        private ICategoryService _categoryService;
        private IPictureService _pictureService;
        private IUrlRecordService _urlRecordService;

        public BuyAmScraperTask(ILogger logger, 
            IProductService productService, 
            ICustomerService customerService, 
            IVendorService vendorService,
            ICategoryService categoryService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService)
        {
            this._categoryUrlsToScrape = new string[] {
                //"https://buy.am/hy/carrefour/bakery-pastry",
                //"https://buy.am/hy/carrefour/fresh-fruit-vegetable",
                //"https://buy.am/hy/carrefour/dairy-eggs",
                //"https://buy.am/hy/carrefour/frozen-products",
                //"https://buy.am/hy/carrefour/breakfast-coffee-tea",
                //"https://buy.am/hy/carrefour/bio-organic",
                //"https://buy.am/hy/carrefour/sweets-snacks",
                //"https://buy.am/hy/carrefour/juices-drinks",
                //"https://buy.am/hy/carrefour/alcoholic-beverages-cigarettes",
                
                "https://buy.am/hy/carrefour/shop-in-shop",
                "https://buy.am/hy/carrefour/household-goods",
            };
            this._logger = logger;
            this._productService = productService;
            this._customerService = customerService;
            this._vendorService = vendorService;
            this._categoryService = categoryService;
            this._pictureService = pictureService;
            this._urlRecordService = urlRecordService;
        }

        private async Task ScrollUntilEnd(Page page)
        {
            int scrolledHeight = 0;
            while (true)
            {
                await page.Mouse.WheelAsync(0, SCROLL_HEIGHT);
                scrolledHeight += SCROLL_HEIGHT;
                await Task.Delay(300);
                var bodyScrollHeight = await page.EvaluateExpressionAsync<int>("document.body.scrollHeight");
                if (scrolledHeight >= bodyScrollHeight)
                {
                    await _logger.InformationAsync("Scrolled to the page end");
                    break;
                }
            }
        }

        private async Task<byte[]> DownloadImage(string url)
        {
            using var httpClient = new WebClient();
            try
            {
                return await httpClient.DownloadDataTaskAsync(url);
            }
            catch (Exception exc)
            {
                await _logger.ErrorAsync($"Exception while downloading image: {url}, message: {exc.Message}");
            }

            return Array.Empty<byte>();
        }

        private async Task<IReadOnlyList<ProductDTO>> ExtractProducts(Page page)
        {
            var products = new List<ProductDTO>();
            var productsInDom = await page.QuerySelectorAllAsync("div.product--box.box--minimal .image--media img");
            var categoryName = await page.EvaluateExpressionAsync<string>("document.querySelector('li.is--active span.breadcrumb--title').innerText");

            if(categoryName == null)
            {
                await _logger.ErrorAsync($"Category for page {page.Url} is null");
                return products;
            }

            foreach (var current in productsInDom)
            {
                try
                {
                    await current.ClickAsync();
                    var nameTag = await page.WaitForSelectorAsync("div.product--details h1.product--title", new WaitForSelectorOptions { Visible = true });

                    if (nameTag == null)
                    {
                        await _logger.InformationAsync("Product name tag wasn't found!");
                        continue;
                    }

                    var parseResults = await page.EvaluateFunctionAsync(@"() => {
                        return {  
                            'Name': document.querySelector('div.product--details h1.product--title').innerText,
                            'SubCategory': Array.from(document.querySelectorAll('.product--buybox .entry--property')).map((n) => {return {'key': n.children[0].innerText, 'value': n.children[1].innerText}}).find((n) => n.key == 'Բաժին')?.value,
                            'Description': document.querySelector('div .product--description.im-collapse-content')?.innerText.replaceAll(/\s{2,}/ig, '\n').trim(),
                            'Price': document.querySelector('.price--content.content--default').children[0].attributes['content'].value,
                            'Code': Array.from(document.querySelectorAll('span.entry--content')).find((n) => 'itemprop' in n.attributes && n.attributes['itemprop'].value == 'sku').innerText,
                            'ImageUrl': document.querySelector('.image-slider--item span.image--element').attributes['data-img-original'].value
                        }
                        }
                    ");

                    var description = parseResults.Value<string>("Description") ?? string.Empty;
                    var code = parseResults.Value<int>("Code");
                    var name = parseResults.Value<string>("Name");
                    var subCategory = parseResults.Value<string>("SubCategory") ?? string.Empty;
                    var priceAsString = parseResults.Value<string>("Price");
                    int price = 0;
                    var imageUrl = parseResults.Value<string>("ImageUrl");

                    if (name == null || priceAsString == null ||
                        !int.TryParse(priceAsString, System.Globalization.NumberStyles.AllowDecimalPoint, null, out price) || imageUrl == null)
                    {
                        await _logger.WarningAsync($"Field is missing from product. code={code}, description={description}, name={name}, subCategory={subCategory}, priceAsString={priceAsString}, imageUrl={imageUrl}");
                        continue;
                    }

                    var imageUrlParsed = new Uri(imageUrl);

                    var product = new ProductDTO
                    {
                        Category = categoryName,
                        Name = name,
                        SubCategory = subCategory,
                        ShortDescription = $"Code: {code}\n" + description.Substring(0, Math.Min(100, description.Length)),
                        FullDescription = $"Code: {code}\n" + description,
                        Price = price,
                        Code = code,
                        Image = await DownloadImage(imageUrl),
                        ImageFileName = imageUrlParsed.Segments[imageUrlParsed.Segments.Length - 1]
                    };

                    products.Add(product);

                    await _logger.InformationAsync($"Parsed product, name: {product.Name}");
                }
                catch (Exception exc)
                {
                    await _logger.WarningAsync($"Exception thrown when parsing: URL={page.Url}", exc);
                    continue;
                }
                finally
                {
                    try
                    {
                        var closeButton = await page.QuerySelectorAsync(".quickview-close-btn");
                        await closeButton.ClickAsync();
                        await page.WaitForSelectorAsync(".quick-view.is--active", new WaitForSelectorOptions { Hidden = true });
                        await page.WaitForTimeoutAsync(1000);
                    }
                    catch
                    {}
                }
            }

            return products;
        }

        private async Task<Core.Domain.Catalog.Category> GetExactCategoryByName(string name)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(name);
            return categories.FirstOrDefault(c => string.Equals(c.Name, name));
        }

        async Task AddProductsIfMissing(IReadOnlyList<ProductDTO> productDTOs)
        {
            var carrefourVendor = (await _vendorService.GetAllVendorsAsync(CARREFOUR_CUSTOMER_NAME)).FirstOrDefault();
            if(carrefourVendor == null)
            {
                await _logger.ErrorAsync($"Vendor with name {CARREFOUR_CUSTOMER_NAME} doesn't exist");
                return;
            }

            foreach (var productDTO in productDTOs)
            {
                var existingProduct = await _productService.GetProductBySkuAsync(productDTO.Code.ToString());
                if (existingProduct != null)
                {
                    await _logger.InformationAsync($"Product with code {productDTO.Code} exists, skipping");
                    continue;
                }

                var picture = await _pictureService.InsertPictureAsync(productDTO.Image, MimeTypes.ImageJpeg, productDTO.ImageFileName);

                var product = new Core.Domain.Catalog.Product()
                {
                    Sku = productDTO.Code.ToString(),
                    Name = productDTO.Name,
                    Price = productDTO.Price,
                    ShortDescription = productDTO.ShortDescription,
                    FullDescription = productDTO.FullDescription,
                    VendorId = carrefourVendor.Id,
                    IsShipEnabled = true,
                    DisableWishlistButton = true,
                    Published = true,
                    CreatedOnUtc = DateTime.Now,
                    UpdatedOnUtc = DateTime.Now,
                    RibbonEnable = false
                };

                await _productService.InsertProductAsync(product);

                var vendorCategory = await GetExactCategoryByName(CARREFOUR_CUSTOMER_NAME);
                if(vendorCategory == null)
                {
                    vendorCategory = new Core.Domain.Catalog.Category
                    {
                        Name = CARREFOUR_CUSTOMER_NAME,
                        IncludeInTopMenu = true,
                        CreatedOnUtc = DateTime.Now,
                        UpdatedOnUtc = DateTime.Now,
                        Published = true,
                        AllowCustomersToSelectPageSize = true,
                        PageSizeOptions = "6, 3, 9"
                    };

                    await _categoryService.InsertCategoryAsync(vendorCategory);
                    await _urlRecordService.InsertUrlRecordAsync(new Core.Domain.Seo.UrlRecord { 
                        EntityId = vendorCategory.Id,
                        EntityName = "Category",
                        IsActive = true,
                        LanguageId = 0,
                        Slug = vendorCategory.Name
                    });
                    
                }

                var productCategory = await GetExactCategoryByName(productDTO.Category);
                if(productCategory == null)
                {
                    productCategory = new Core.Domain.Catalog.Category
                    {
                        Name = productDTO.Category,
                        CreatedOnUtc = DateTime.Now,
                        UpdatedOnUtc = DateTime.Now,
                        ParentCategoryId = vendorCategory.Id,
                        Published = true,
                        AllowCustomersToSelectPageSize = true,
                        PageSizeOptions = "6, 3, 9"
                    };

                    await _categoryService.InsertCategoryAsync(productCategory);
                    await _urlRecordService.InsertUrlRecordAsync(new Core.Domain.Seo.UrlRecord
                    {
                        EntityId = productCategory.Id,
                        EntityName = "Category",
                        IsActive = true,
                        LanguageId = 0,
                        Slug = productCategory.Name
                    });
                }

                bool haveSubCategorySpecified = productDTO.SubCategory == null ? true : false;
                var productSubCategory = haveSubCategorySpecified ? await GetExactCategoryByName(productDTO.SubCategory) : null;
                if (haveSubCategorySpecified && productSubCategory == null)
                {
                    productSubCategory = new Core.Domain.Catalog.Category
                    {
                        Name = productDTO.SubCategory,
                        CreatedOnUtc = DateTime.Now,
                        UpdatedOnUtc = DateTime.Now,
                        ParentCategoryId = productCategory.Id,
                        Published = true,
                        AllowCustomersToSelectPageSize = true,
                        PageSizeOptions = "6, 3, 9"
                    };

                    await _categoryService.InsertCategoryAsync(productSubCategory);
                    await _urlRecordService.InsertUrlRecordAsync(new Core.Domain.Seo.UrlRecord
                    {
                        EntityId = productSubCategory.Id,
                        EntityName = "Category",
                        IsActive = true,
                        LanguageId = 0,
                        Slug = productSubCategory.Name
                    });
                }

                await _categoryService.InsertProductCategoryAsync(new Core.Domain.Catalog.ProductCategory {
                    ProductId = product.Id,
                    CategoryId = haveSubCategorySpecified ? productSubCategory.Id : productCategory.Id
                });

                await _productService.InsertProductPictureAsync(new Core.Domain.Catalog.ProductPicture
                {
                    PictureId = picture.Id,
                    ProductId = product.Id
                });
            }
            
        }

        public async Task<int> ScrapeAndAddProducts()
        {
            int result = 0;

            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions { Headless = false, DefaultViewport = new ViewPortOptions { Width = 1920, Height = 1024 } });

            foreach (var categoryUrl in _categoryUrlsToScrape)
            {
                await using var page = await browser.NewPageAsync();
                await page.GoToAsync(categoryUrl);
                await ScrollUntilEnd(page);

                var products = await ExtractProducts(page);
                await AddProductsIfMissing(products);

                result += products.Count;
            }

            return result;
        }

        public async Task ExecuteAsync()
        {
            await ScrapeAndAddProducts();
        }
    }
}
