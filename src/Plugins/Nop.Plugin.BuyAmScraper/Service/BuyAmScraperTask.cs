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

namespace Nop.Plugin.BuyAmScraper.Service
{
    class BuyAmScraperTask : Services.Tasks.IScheduleTask
    {
        private const int SCROLL_HEIGHT = 300;
//        private const string BUY_AM_CUSTOMER
        private readonly string[] _categoryUrlsToScrape;

        private ILogger _logger;
        private IProductService _productService;
        private ICustomerService _customerService;

        public BuyAmScraperTask(ILogger logger, IProductService productService, ICustomerService customerService)
        {
            this._categoryUrlsToScrape = new string[] {
                "https://buy.am/hy/carrefour/bakery-pastry",
                "https://buy.am/hy/carrefour/fresh-fruit-vegetable",
                "https://buy.am/hy/carrefour/dairy-eggs"
            };
            this._logger = logger;
            this._productService = productService;
            this._customerService = customerService;
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
            return await httpClient.DownloadDataTaskAsync(url);
        }

        private async Task<IReadOnlyList<ProductDTO>> ExtractProducts(Page page)
        {
            var products = new List<ProductDTO>();
            var productsInDom = await page.QuerySelectorAllAsync("div.product--box.box--minimal .image--media img");
            var categoryName = await page.EvaluateExpressionAsync<string>("document.querySelector('li.is--active span.breadcrumb--title').innerText");
            foreach (var current in productsInDom)
            {
                await current.ClickAsync();
                var nameTag = await page.WaitForSelectorAsync("div.product--details h1.product--title", new WaitForSelectorOptions { Visible = true });

                if (nameTag == null)
                {
                    await _logger.InformationAsync("Product name tag wasn't found!");
                    break;
                }

                var parseResults = await page.EvaluateFunctionAsync(@"() => {
                    return {  
                        'Name': document.querySelector('div.product--details h1.product--title').innerText,
                        'SubCategory': Array.from(document.querySelectorAll('.product--buybox .entry--property')).map((n) => {return {'key': n.children[0].innerText, 'value': n.children[1].innerText}}).find((n) => n.key == 'Բաժին').value,
                        'Description': document.querySelector('div .product--description.im-collapse-content').innerText.replaceAll(/\s{2,}/ig, '\n').trim(),
                        'Price': document.querySelector('.price--content.content--default').children[0].attributes['content'].value,
                        'Code': Array.from(document.querySelectorAll('span.entry--content')).find((n) => 'itemprop' in n.attributes && n.attributes['itemprop'].value == 'sku').innerText,
                        'ImageUrl': document.querySelector('.image-slider--item span.image--element').attributes['data-img-original'].value
                    }
                    }
                ");

                var closeButton = await page.QuerySelectorAsync(".quickview-close-btn");
                await closeButton.ClickAsync();
                await page.WaitForSelectorAsync(".quick-view.is--active", new WaitForSelectorOptions { Hidden = true });
                await page.WaitForTimeoutAsync(2000);

                var description = parseResults.Value<string>("Description");
                var code = parseResults.Value<int>("Code");
                var product = new ProductDTO
                {
                    Category = categoryName,
                    Name = parseResults.Value<string>("Name"),
                    SubCategory = parseResults.Value<string>("SubCategory"),
                    ShortDescription = $"Code: {code}\n" + description.Substring(0, Math.Min(100, description.Length)),
                    FullDescription = $"Code: {code}\n" + description,
                    Price = int.Parse(parseResults.Value<string>("Price"), System.Globalization.NumberStyles.AllowDecimalPoint),
                    Code = code,
                    Image = await DownloadImage(parseResults.Value<string>("ImageUrl"))
                };
                products.Add(product);

                await _logger.InformationAsync($"Parsed product, name: {product.Name}");
            }

            return products;
        }

        async Task AddProductsIfMissing(IReadOnlyList<ProductDTO> productDTOs)
        {
            foreach (var productDTO in productDTOs)
            {
                var existingProduct = await _productService.GetProductBySkuAsync(productDTO.Code.ToString());
                if(existingProduct != null)
                {
                    await _logger.InformationAsync($"Product with code {productDTO.Code} exists, skipping");
                    continue;
                }

                var product = new Core.Domain.Catalog.Product();
                product.Sku = productDTO.Code.ToString();
                product.Name = productDTO.Name;
                product.Price = productDTO.Price;
                product.ShortDescription = productDTO.ShortDescription;
                product.FullDescription = productDTO.FullDescription;
                //product.VendorId = _customerService.GetCustomerRolesAsync();
            }
            
        }

        public async Task<int> ScrapeAndAddProducts()
        {
            int result = 0;

            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions { Headless = true, DefaultViewport = new ViewPortOptions { Width = 1920, Height = 1024 } });

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
