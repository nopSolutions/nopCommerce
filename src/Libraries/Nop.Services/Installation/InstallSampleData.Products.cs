using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Media;

namespace Nop.Services.Installation;

public partial class InstallationService
{
    #region Fields

    protected Dictionary<string, ProductTag> _tags = new(comparer: StringComparer.InvariantCultureIgnoreCase);

    #endregion

    #region Utilities

    /// <summary>
    /// Inserts the product picture
    /// </summary>
    /// <param name="product">Product to insert picture</param>
    /// <param name="fileName">Picture file name</param>
    /// <param name="displayOrder">Display order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the identifier of inserted picture
    /// </returns>
    protected virtual async Task<int> InsertProductPictureAsync(Product product, string fileName, int displayOrder = 1)
    {
        var pictureId = await InsertPictureAsync(fileName, product.Name);

        await _dataProvider.InsertEntityAsync(
            new ProductPicture
            {
                ProductId = product.Id,
                PictureId = pictureId,
                DisplayOrder = displayOrder
            });

        return pictureId;
    }

    /// <summary>
    /// Gets a specification attribute option identifier
    /// </summary>
    /// <param name="specAttributeName">The spec attribute name</param>
    /// <param name="specAttributeOptionName">The spec attribute option name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<int> GetSpecificationAttributeOptionIdAsync(string specAttributeName, string specAttributeOptionName)
    {
        var specificationAttribute = await Table<SpecificationAttribute>()
            .SingleAsync(sa => sa.Name == specAttributeName);

        var specificationAttributeOption = await Table<SpecificationAttributeOption>()
            .SingleAsync(sao => sao.Name == specAttributeOptionName && sao.SpecificationAttributeId == specificationAttribute.Id);

        return specificationAttributeOption.Id;
    }

    /// <summary>
    /// Insert product tags mappings
    /// </summary>
    /// <param name="product"></param>
    /// <param name="tags"></param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InsertProductTagMappingAsync(Product product, string[] tags)
    {
        if (!_tags.Any())
            _tags = Table<ProductTag>().AsEnumerable().GroupBy(p => p.Name, p => p)
                .ToDictionary(p => p.Key, p => p.FirstOrDefault());

        var newProductTags = await tags.Distinct().Where(tag => !_tags.ContainsKey(tag))
            .Select(item => new ProductTag { Name = item }).ToListAsync();

        await _dataProvider.BulkInsertEntitiesAsync(newProductTags);
        await InsertSearchEngineNamesAsync(newProductTags, productTag => productTag.Name);

        foreach (var productTag in newProductTags)
            _tags.Add(productTag.Name, productTag);

        await _dataProvider.BulkInsertEntitiesAsync(tags.Select(tag =>
            new ProductProductTagMapping { ProductTagId = _tags[tag].Id, ProductId = product.Id }));
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallComputersAsync(int productTemplateSimpleId, List<Product> allProducts, List<RelatedProduct> relatedProducts)
    {
        var productBuildComputer = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Build your own computer",
            Sku = "COMP_CUST",
            ShortDescription = "Build it",
            FullDescription = "<p>Fight back against cluttered workspaces with the stylish IBM zBC12 All-in-One desktop PC, featuring powerful computing resources and a stunning 20.1-inch widescreen display with stunning XBRITE-HiColor LCD technology. The black IBM zBC12 has a built-in microphone and MOTION EYE camera with face-tracking technology that allows for easy communication with friends and family. And it has a built-in DVD burner and Sony's Movie Store software so you can create a digital entertainment library for personal viewing at your convenience. Easy to setup and even easier to use, this JS-series All-in-One includes an elegantly designed keyboard and a USB mouse.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "build-your-own-computer",
            AllowCustomerReviews = true,
            Price = 1200M,
            IsShipEnabled = true,
            IsFreeShipping = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            ShowOnHomepage = true,
            MarkAsNew = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };

        allProducts.Add(productBuildComputer);

        await _dataProvider.InsertEntityAsync(productBuildComputer);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productBuildComputer.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Desktops")).Id,
            DisplayOrder = 1
        });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductPicture
            {
                ProductId = productBuildComputer.Id,
                PictureId = await InsertPictureAsync("product_Desktops_1.jpeg", productBuildComputer.Name),
                DisplayOrder = 1
            },
            new ProductPicture
            {
                ProductId = productBuildComputer.Id,
                PictureId = await InsertPictureAsync("product_Desktops_2.jpeg", productBuildComputer.Name),
                DisplayOrder = 2
            }});

        var pamProcessor = await _dataProvider.InsertEntityAsync(new ProductAttributeMapping
        {
            ProductId = productBuildComputer.Id,
            ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "Processor")).Id,
            AttributeControlType = AttributeControlType.DropdownList,
            IsRequired = true
        });

        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamProcessor.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "2.2 GHz Intel Pentium Dual-Core E2200",
                DisplayOrder = 1
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamProcessor.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "2.5 GHz Intel Pentium Dual-Core E2200",
                IsPreSelected = true,
                PriceAdjustment = 15,
                DisplayOrder = 2
            }
        });

        var pamRam = await _dataProvider.InsertEntityAsync(new ProductAttributeMapping
        {
            ProductId = productBuildComputer.Id,
            ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "RAM")).Id,
            AttributeControlType = AttributeControlType.DropdownList,
            IsRequired = true
        });

        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamRam.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "2 GB",
                DisplayOrder = 1
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamRam.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "4GB",
                PriceAdjustment = 20,
                DisplayOrder = 2
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamRam.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "8GB",
                PriceAdjustment = 60,
                DisplayOrder = 3
            }
        });

        var pamHdd = await _dataProvider.InsertEntityAsync(
            new ProductAttributeMapping
            {
                ProductId = productBuildComputer.Id,
                ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "HDD")).Id,
                AttributeControlType = AttributeControlType.RadioList,
                IsRequired = true
            });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamHdd.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "320 GB",
                DisplayOrder = 1
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamHdd.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "400 GB",
                PriceAdjustment = 100,
                DisplayOrder = 2
            }});

        var pamOs = await _dataProvider.InsertEntityAsync(
            new ProductAttributeMapping
            {
                ProductId = productBuildComputer.Id,
                ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "OS")).Id,
                AttributeControlType = AttributeControlType.RadioList,
                IsRequired = true
            });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamOs.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Vista Home",
                PriceAdjustment = 50,
                IsPreSelected = true,
                DisplayOrder = 1
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamOs.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Vista Premium",
                PriceAdjustment = 60,
                DisplayOrder = 2
            }});

        var pamSoftware = await _dataProvider.InsertEntityAsync(new ProductAttributeMapping
        {
            ProductId = productBuildComputer.Id,
            ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "Software")).Id,
            AttributeControlType = AttributeControlType.Checkboxes
        });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamSoftware.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Microsoft Office",
                PriceAdjustment = 50,
                IsPreSelected = true,
                DisplayOrder = 1
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamSoftware.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Acrobat Reader",
                PriceAdjustment = 10,
                DisplayOrder = 2
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamSoftware.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Total Commander",
                PriceAdjustment = 5,
                DisplayOrder = 2
            }});

        await InsertProductTagMappingAsync(productBuildComputer, new[] { "awesome", "computer" });

        var productDigitalStorm = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Digital Storm VANQUISH Custom Performance PC",
            Sku = "DS_VA3_PC",
            ShortDescription = "Digital Storm Vanquish 3 Desktop PC",
            FullDescription = "<p>Blow the doors off today’s most demanding games with maximum detail, speed, and power for an immersive gaming experience without breaking the bank.</p><p>Stay ahead of the competition, VANQUISH 3 is fully equipped to easily handle future upgrades, keeping your system on the cutting edge for years to come.</p><p>Each system is put through an extensive stress test, ensuring you experience zero bottlenecks and get the maximum performance from your hardware.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "compaq-presario-sr1519x-pentium-4-desktop-pc-with-cdrw",
            AllowCustomerReviews = true,
            Price = 1259M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productDigitalStorm);

        await _dataProvider.InsertEntityAsync(productDigitalStorm);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productDigitalStorm.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Desktops")).Id,
            DisplayOrder = 1
        });

        await _dataProvider.InsertEntityAsync(new ProductPicture
        {
            ProductId = productDigitalStorm.Id,
            PictureId = await InsertPictureAsync("product_DigitalStorm.jpeg", productDigitalStorm.Name),
            DisplayOrder = 1
        });

        await InsertProductTagMappingAsync(productDigitalStorm, new[] { "cool", "computer" });

        var productLenovoIdeaCentre = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Lenovo IdeaCentre",
            Sku = "LE_IC_600",
            ShortDescription = string.Empty,
            FullDescription = "<p>The A600 features a 21.5in screen, DVD or optional Blu-Ray drive, support for the full beans 1920 x 1080 HD, Dolby Home Cinema certification and an optional hybrid analogue/digital TV tuner.</p><p>Connectivity is handled by 802.11a/b/g - 802.11n is optional - and an ethernet port. You also get four USB ports, a Firewire slot, a six-in-one card reader and a 1.3- or two-megapixel webcam.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "hp-iq506-touchsmart-desktop-pc",
            AllowCustomerReviews = true,
            Price = 500M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productLenovoIdeaCentre);

        await _dataProvider.InsertEntityAsync(productLenovoIdeaCentre);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productLenovoIdeaCentre.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Desktops")).Id,
            DisplayOrder = 1
        });

        await _dataProvider.InsertEntityAsync(new ProductPicture
        {
            ProductId = productLenovoIdeaCentre.Id,
            PictureId = await InsertPictureAsync("product_LenovoIdeaCentre.jpeg", productLenovoIdeaCentre.Name),
            DisplayOrder = 1
        });

        await InsertProductTagMappingAsync(productLenovoIdeaCentre, new[] { "awesome", "computer" });

        var productAppleMacBookPro = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Apple MacBook Pro",
            Sku = "AP_MBP_13",
            ShortDescription = "A groundbreaking Retina display. A new force-sensing trackpad. All-flash architecture. Powerful dual-core and quad-core Intel processors. Together, these features take the notebook to a new level of performance. And they will do the same for you in everything you create.",
            FullDescription = "<p>With fifth-generation Intel Core processors, the latest graphics, and faster flash storage, the incredibly advanced MacBook Pro with Retina display moves even further ahead in performance and battery life.* *Compared with the previous generation.</p><p>Retina display with 2560-by-1600 resolution</p><p>Fifth-generation dual-core Intel Core i5 processor</p><p>Intel Iris Graphics</p><p>Up to 9 hours of battery life1</p><p>Faster flash storage2</p><p>802.11ac Wi-Fi</p><p>Two Thunderbolt 2 ports for connecting high-performance devices and transferring data at lightning speed</p><p>Two USB 3 ports (compatible with USB 2 devices) and HDMI</p><p>FaceTime HD camera</p><p>Pages, Numbers, Keynote, iPhoto, iMovie, GarageBand included</p><p>OS X, the world's most advanced desktop operating system</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "asus-eee-pc-1000ha-10-inch-netbook",
            AllowCustomerReviews = true,
            Price = 1800M,
            IsShipEnabled = true,
            IsFreeShipping = true,
            Weight = 3,
            Length = 3,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 2,
            OrderMaximumQuantity = 10000,
            Published = true,
            ShowOnHomepage = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productAppleMacBookPro);

        await _dataProvider.InsertEntityAsync(productAppleMacBookPro);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productAppleMacBookPro.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Notebooks")).Id,
            DisplayOrder = 1
        });

        await _dataProvider.InsertEntityAsync(new ProductManufacturer
        {
            ProductId = productAppleMacBookPro.Id,
            ManufacturerId = (await Table<Manufacturer>().SingleAsync(c => c.Name == "Apple")).Id,
            DisplayOrder = 2
        });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductPicture
        {
            ProductId = productAppleMacBookPro.Id,
            PictureId = await InsertPictureAsync("product_macbook_1.jpeg", productAppleMacBookPro.Name),
            DisplayOrder = 1
        }, new ProductPicture
        {
            ProductId = productAppleMacBookPro.Id,
            PictureId = await InsertPictureAsync("product_macbook_2.jpeg", productAppleMacBookPro.Name),
            DisplayOrder = 2
        }});

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductSpecificationAttribute
            {
                ProductId = productAppleMacBookPro.Id,
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Screensize", "13.0''")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productAppleMacBookPro.Id,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 2,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("CPU Type", "Intel Core i5")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productAppleMacBookPro.Id,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 3,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Memory", "4 GB")
            }});

        await InsertProductTagMappingAsync(productAppleMacBookPro, new[] { "compact", "awesome", "computer" });

        var productAsusN551JK = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Asus Laptop",
            Sku = "AS_551_LP",
            ShortDescription = "Laptop Asus N551JK Intel Core i7-4710HQ 2.5 GHz, RAM 16GB, HDD 1TB, Video NVidia GTX 850M 4GB, BluRay, 15.6, Full HD, Win 8.1",
            FullDescription = "<p>The ASUS N550JX combines cutting-edge audio and visual technology to deliver an unsurpassed multimedia experience. A full HD wide-view IPS panel is tailor-made for watching movies and the intuitive touchscreen makes for easy, seamless navigation. ASUS has paired the N550JX’s impressive display with SonicMaster Premium, co-developed with Bang & Olufsen ICEpower® audio experts, for true surround sound. A quad-speaker array and external subwoofer combine for distinct vocals and a low bass that you can feel.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "asus-eee-pc-900ha-89-inch-netbook-black",
            AllowCustomerReviews = true,
            Price = 1500M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };

        allProducts.Add(productAsusN551JK);

        await _dataProvider.InsertEntityAsync(productAsusN551JK);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productAsusN551JK.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Notebooks")).Id,
            DisplayOrder = 1
        });

        await _dataProvider.InsertEntityAsync(new ProductPicture
        {
            ProductId = productAsusN551JK.Id,
            PictureId = await InsertPictureAsync("product_asuspc_N551JK.jpeg", productAsusN551JK.Name),
            DisplayOrder = 1
        });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductSpecificationAttribute
            {
                ProductId = productAsusN551JK.Id,
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Screensize", "15.6''")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productAsusN551JK.Id,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 2,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("CPU Type", "Intel Core i7")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productAsusN551JK.Id,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 3,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Memory", "16 GB")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productAsusN551JK.Id,
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 4,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Hard drive", "1 TB")
            }});

        await InsertProductTagMappingAsync(productAsusN551JK, new[] { "compact", "awesome", "computer" });

        var productSamsungSeries = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Samsung Premium Ultrabook",
            Sku = "SM_900_PU",
            ShortDescription = "Samsung Series 9 NP900X4C-A06US 15-Inch Ultrabook (1.70 GHz Intel Core i5-3317U Processor, 8GB DDR3, 128GB SSD, Windows 8) Ash Black",
            FullDescription = "<p>Designed with mobility in mind, Samsung's durable, ultra premium, lightweight Series 9 laptop (model NP900X4C-A01US) offers mobile professionals and power users a sophisticated laptop equally suited for work and entertainment. Featuring a minimalist look that is both simple and sophisticated, its polished aluminum uni-body design offers an iconic look and feel that pushes the envelope with an edge just 0.58 inches thin. This Series 9 laptop also includes a brilliant 15-inch SuperBright Plus display with HD+ technology, 128 GB Solid State Drive (SSD), 8 GB of system memory, and up to 10 hours of battery life.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "hp-pavilion-artist-edition-dv2890nr-141-inch-laptop",
            AllowCustomerReviews = true,
            Price = 1590M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            //ShowOnHomepage = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productSamsungSeries);

        await _dataProvider.InsertEntityAsync(productSamsungSeries);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productSamsungSeries.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Notebooks")).Id,
            DisplayOrder = 1
        });

        await _dataProvider.InsertEntityAsync(new ProductPicture
        {
            ProductId = productSamsungSeries.Id,
            PictureId = await InsertPictureAsync("product_SamsungNP900X4C.jpeg", productSamsungSeries.Name),
            DisplayOrder = 1
        });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductSpecificationAttribute
            {
                ProductId = productSamsungSeries.Id,
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Screensize", "15.0''")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productSamsungSeries.Id,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 2,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("CPU Type", "Intel Core i5")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productSamsungSeries.Id,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 3,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Memory", "8 GB")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productSamsungSeries.Id,
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 4,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Hard drive", "128 GB")
            }});

        await InsertProductTagMappingAsync(productSamsungSeries, new[] { "nice", "computer", "compact" });

        var productHpSpectre = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "HP Spectre XT Pro UltraBook",
            Sku = "HP_SPX_UB",
            ShortDescription = "HP Spectre XT Pro UltraBook / Intel Core i5-2467M / 13.3 / 4GB / 128GB / Windows 7 Professional / Laptop",
            FullDescription = "<p>Introducing HP ENVY Spectre XT, the Ultrabook designed for those who want style without sacrificing substance. It's sleek. It's thin. And with Intel. Corer i5 processor and premium materials, it's designed to go anywhere from the bistro to the boardroom, it's unlike anything you've ever seen from HP.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "hp-pavilion-elite-m9150f-desktop-pc",
            AllowCustomerReviews = true,
            Price = 1350M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productHpSpectre);

        await _dataProvider.InsertEntityAsync(productHpSpectre);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productHpSpectre.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Notebooks")).Id,
            DisplayOrder = 1
        });

        await _dataProvider.InsertEntityAsync(new ProductManufacturer
        {
            ProductId = productHpSpectre.Id,
            ManufacturerId = (await Table<Manufacturer>().SingleAsync(c => c.Name == "HP")).Id,
            DisplayOrder = 3
        });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductPicture
            {
                ProductId = productHpSpectre.Id,
                PictureId = await InsertPictureAsync("product_HPSpectreXT_1.jpeg", productHpSpectre.Name),
                DisplayOrder = 1
            },
            new ProductPicture
            {
                ProductId = productHpSpectre.Id,
                PictureId = await InsertPictureAsync("product_HPSpectreXT_2.jpeg", productHpSpectre.Name),
                DisplayOrder = 2
            }});

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductSpecificationAttribute
            {
                ProductId = productHpSpectre.Id,
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Screensize", "13.3''")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productHpSpectre.Id,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 2,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("CPU Type", "Intel Core i5")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productHpSpectre.Id,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 3,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Memory", "4 GB")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productHpSpectre.Id,
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 4,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Hard drive", "128 GB")
            }});

        await InsertProductTagMappingAsync(productHpSpectre, new[] { "nice", "computer" });

        var productHpEnvy = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "HP Envy 15.6-Inch Sleekbook",
            Sku = "HP_ESB_15",
            ShortDescription = "HP ENVY 6-1202ea Ultrabook Beats Audio, 3rd generation Intel® CoreTM i7-3517U processor, 8GB RAM, 500GB HDD, Microsoft Windows 8, AMD Radeon HD 8750M (2 GB DDR3 dedicated)",
            FullDescription = "The UltrabookTM that's up for anything. Thin and light, the HP ENVY is the large screen UltrabookTM with Beats AudioTM. With a soft-touch base that makes it easy to grab and go, it's a laptop that's up for anything.<br /><br /><b>Features</b><br /><br />- Windows 8 or other operating systems available<br /><br /><b>Top performance. Stylish design. Take notice.</b><br /><br />- At just 19.8 mm (0.78 in) thin, the HP ENVY UltrabookTM is slim and light enough to take anywhere. It's the laptop that gets you noticed with the power to get it done.<br />- With an eye-catching metal design, it's a laptop that you want to carry with you. The soft-touch, slip-resistant base gives you the confidence to carry it with ease.<br /><br /><b>More entertaining. More gaming. More fun.</b><br /><br />- Own the UltrabookTM with Beats AudioTM, dual speakers, a subwoofer, and an awesome display. Your music, movies and photo slideshows will always look and sound their best.<br />- Tons of video memory let you experience incredible gaming and multimedia without slowing down. Create and edit videos in a flash. And enjoy more of what you love to the fullest.<br />- The HP ENVY UltrabookTM is loaded with the ports you'd expect on a world-class laptop, but on a Sleekbook instead. Like HDMI, USB, RJ-45, and a headphone jack. You get all the right connections without compromising size.<br /><br /><b>Only from HP.</b><br /><br />- Life heats up. That's why there's HP CoolSense technology, which automatically adjusts your notebook's temperature based on usage and conditions. It stays cool. You stay comfortable.<br />- With HP ProtectSmart, your notebook's data stays safe from accidental bumps and bruises. It senses motion and plans ahead, stopping your hard drive and protecting your entire digital life.<br />- Keep playing even in dimly lit rooms or on red eye flights. The optional backlit keyboard[1] is full-size so you don't compromise comfort. Backlit keyboard. Another bright idea.<br /><br />",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "hp-pavilion-g60-230us-160-inch-laptop",
            AllowCustomerReviews = true,
            Price = 1460M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productHpEnvy);

        await _dataProvider.InsertEntityAsync(productHpEnvy);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productHpEnvy.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Notebooks")).Id,
            DisplayOrder = 1
        });

        await _dataProvider.InsertEntityAsync(new ProductManufacturer
        {
            ProductId = productHpEnvy.Id,
            ManufacturerId = (await Table<Manufacturer>().SingleAsync(c => c.Name == "HP")).Id,
            DisplayOrder = 4
        });

        await _dataProvider.InsertEntityAsync(new ProductPicture
        {
            ProductId = productHpEnvy.Id,
            PictureId = await InsertPictureAsync("product_HpEnvy6.jpeg", productHpEnvy.Name),
            DisplayOrder = 1
        });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductSpecificationAttribute
            {
                ProductId = productHpEnvy.Id,
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Screensize", "15.6''")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productHpEnvy.Id,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 2,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("CPU Type", "Intel Core i7")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productHpEnvy.Id,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 3,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Memory", "8 GB")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productHpEnvy.Id,
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 4,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Hard drive", "500 GB")
            }});

        await InsertProductTagMappingAsync(productHpEnvy, new[] { "computer", "cool", "compact" });

        var productLenovoThinkpad = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Lenovo Thinkpad Carbon Laptop",
            Sku = "LE_TX1_CL",
            ShortDescription = "Lenovo Thinkpad X1 Carbon Touch Intel Core i7 14 Ultrabook",
            FullDescription = "<p>The X1 Carbon brings a new level of quality to the ThinkPad legacy of high standards and innovation. It starts with the durable, carbon fiber-reinforced roll cage, making for the best Ultrabook construction available, and adds a host of other new features on top of the old favorites. Because for 20 years, we haven't stopped innovating. And you shouldn't stop benefiting from that.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "toshiba-satellite-a305-s6908-154-inch-laptop",
            AllowCustomerReviews = true,
            Price = 1360M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productLenovoThinkpad);

        await _dataProvider.InsertEntityAsync(productLenovoThinkpad);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productLenovoThinkpad.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Notebooks")).Id,
            DisplayOrder = 1
        });

        await _dataProvider.InsertEntityAsync(new ProductPicture
        {
            ProductId = productLenovoThinkpad.Id,
            PictureId = await InsertPictureAsync("product_LenovoThinkpad.jpeg", productLenovoThinkpad.Name),
            DisplayOrder = 1
        });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductSpecificationAttribute
            {
                ProductId = productLenovoThinkpad.Id,
                AllowFiltering = false,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Screensize", "14.0''")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productLenovoThinkpad.Id,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 2,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("CPU Type", "Intel Core i7")
            }});

        await InsertProductTagMappingAsync(productLenovoThinkpad, new[] { "awesome", "computer", "compact" });

        var productAdobePhotoshop = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Adobe Photoshop",
            Sku = "AD_CS4_PH",
            ShortDescription = "Easily find and view all your photos",
            FullDescription = "<p>Adobe Photoshop CS4 software combines power and simplicity so you can make ordinary photos extraordinary; tell engaging stories in beautiful, personalized creations for print and web; and easily find and view all your photos. New Photoshop.com membership* works with Photoshop CS4 so you can protect your photos with automatic online backup and 2 GB of storage; view your photos anywhere you are; and share your photos in fun, interactive ways with invitation-only Online Albums.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "adobe-photoshop-elements-7",
            AllowCustomerReviews = true,
            Price = 75M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 3,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productAdobePhotoshop);

        await _dataProvider.InsertEntityAsync(productAdobePhotoshop);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productAdobePhotoshop.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Software")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productAdobePhotoshop, "product_AdobePhotoshop.jpeg");

        await InsertProductTagMappingAsync(productAdobePhotoshop, new[] { "computer", "awesome" });

        var productWindows8Pro = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Microsoft Windows OS",
            Sku = "MS_WIN_8P",
            ShortDescription = "Windows 8 is a Microsoft operating system that was released in 2012 as part of the company's Windows NT OS family. ",
            FullDescription = "<p>Windows 8 Pro is comparable to Windows 7 Professional and Ultimate and is targeted towards enthusiasts and business users; it includes all the features of Windows 8. Additional features include the ability to receive Remote Desktop connections, the ability to participate in a Windows Server domain, Encrypting File System, Hyper-V, and Virtual Hard Disk Booting, Group Policy as well as BitLocker and BitLocker To Go. Windows Media Center functionality is available only for Windows 8 Pro as a separate software package.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "corel-paint-shop-pro-photo-x2",
            AllowCustomerReviews = true,
            Price = 65M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 3,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productWindows8Pro);

        await _dataProvider.InsertEntityAsync(productWindows8Pro);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productWindows8Pro.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Software")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productWindows8Pro, "product_Windows.jpeg");

        await InsertProductTagMappingAsync(productWindows8Pro, new[] { "awesome", "computer" });

        var productSoundForge = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Sound Forge Pro (recurring)",
            Sku = "SF_PRO_11",
            ShortDescription = "Advanced audio waveform editor.",
            FullDescription = "<p>Sound Forge™ Pro is the application of choice for a generation of creative and prolific artists, producers, and editors. Record audio quickly on a rock-solid platform, address sophisticated audio processing tasks with surgical precision, and render top-notch master files with ease. New features include one-touch recording, metering for the new critical standards, more repair and restoration tools, and exclusive round-trip interoperability with SpectraLayers Pro. Taken together, these enhancements make this edition of Sound Forge Pro the deepest and most advanced audio editing platform available.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "major-league-baseball-2k9",
            IsRecurring = true,
            RecurringCycleLength = 30,
            RecurringCyclePeriod = RecurringProductCyclePeriod.Months,
            RecurringTotalCycles = 12,
            AllowCustomerReviews = true,
            Price = 54.99M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productSoundForge);

        await _dataProvider.InsertEntityAsync(productSoundForge);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productSoundForge.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Software")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productSoundForge, "product_SoundForge.jpeg");

        await InsertProductTagMappingAsync(productSoundForge, new[] { "game", "computer", "cool" });

        relatedProducts.AddRange(new[]
        {
            new RelatedProduct
            {
                ProductId1 = productLenovoIdeaCentre.Id,
                ProductId2 = productDigitalStorm.Id
            },
            new RelatedProduct
            {
                ProductId1 = productDigitalStorm.Id,
                ProductId2 = productBuildComputer.Id
            },
            new RelatedProduct
            {
                ProductId1 = productDigitalStorm.Id,
                ProductId2 = productLenovoIdeaCentre.Id
            },
            new RelatedProduct
            {
                ProductId1 = productDigitalStorm.Id,
                ProductId2 = productLenovoThinkpad.Id
            },
            new RelatedProduct
            {
                ProductId1 = productDigitalStorm.Id,
                ProductId2 = productAppleMacBookPro.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLenovoIdeaCentre.Id,
                ProductId2 = productBuildComputer.Id
            },
            new RelatedProduct
            {
                ProductId1 = productAsusN551JK.Id,
                ProductId2 = productLenovoThinkpad.Id
            },
            new RelatedProduct
            {
                ProductId1 = productAsusN551JK.Id,
                ProductId2 = productAppleMacBookPro.Id
            },
            new RelatedProduct
            {
                ProductId1 = productAsusN551JK.Id,
                ProductId2 = productSamsungSeries.Id
            },
            new RelatedProduct
            {
                ProductId1 = productAsusN551JK.Id,
                ProductId2 = productHpSpectre.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLenovoThinkpad.Id,
                ProductId2 = productAsusN551JK.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLenovoThinkpad.Id,
                ProductId2 = productAppleMacBookPro.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLenovoThinkpad.Id,
                ProductId2 = productSamsungSeries.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLenovoThinkpad.Id,
                ProductId2 = productHpEnvy.Id
            },
            new RelatedProduct
            {
                ProductId1 = productAppleMacBookPro.Id,
                ProductId2 = productLenovoThinkpad.Id
            },
            new RelatedProduct
            {
                ProductId1 = productAppleMacBookPro.Id,
                ProductId2 = productSamsungSeries.Id
            },
            new RelatedProduct
            {
                ProductId1 = productAppleMacBookPro.Id,
                ProductId2 = productAsusN551JK.Id
            },
            new RelatedProduct
            {
                ProductId1 = productAppleMacBookPro.Id,
                ProductId2 = productHpSpectre.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHpSpectre.Id,
                ProductId2 = productLenovoThinkpad.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHpSpectre.Id,
                ProductId2 = productSamsungSeries.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHpSpectre.Id,
                ProductId2 = productAsusN551JK.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHpSpectre.Id,
                ProductId2 = productHpEnvy.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHpEnvy.Id,
                ProductId2 = productAsusN551JK.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHpEnvy.Id,
                ProductId2 = productAppleMacBookPro.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHpEnvy.Id,
                ProductId2 = productHpSpectre.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHpEnvy.Id,
                ProductId2 = productSamsungSeries.Id
            },
            new RelatedProduct
            {
                ProductId1 = productSamsungSeries.Id,
                ProductId2 = productAsusN551JK.Id
            },
            new RelatedProduct
            {
                ProductId1 = productSamsungSeries.Id,
                ProductId2 = productAppleMacBookPro.Id
            },
            new RelatedProduct
            {
                ProductId1 = productSamsungSeries.Id,
                ProductId2 = productHpEnvy.Id
            },
            new RelatedProduct
            {
                ProductId1 = productSamsungSeries.Id,
                ProductId2 = productHpSpectre.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLenovoIdeaCentre.Id,
                ProductId2 = productLenovoThinkpad.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLenovoIdeaCentre.Id,
                ProductId2 = productAppleMacBookPro.Id
            }
        });
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallElectronicsAsync(int productTemplateSimpleId, int productTemplateGroupedId, List<Product> allProducts, List<RelatedProduct> relatedProducts)
    {
        //this one is a grouped product with two associated ones
        var productNikonD5500DSLR = new Product
        {
            ProductType = ProductType.GroupedProduct,
            VisibleIndividually = true,
            Name = "Nikon D5500 DSLR",
            Sku = "N5500DS_0",
            ShortDescription = "Slim, lightweight Nikon D5500 packs a vari-angle touchscreen",
            FullDescription = "Nikon has announced its latest DSLR, the D5500. A lightweight, compact DX-format camera with a 24.2MP sensor, it’s the first of its type to offer a vari-angle touchscreen. The D5500 replaces the D5300 in Nikon’s range, and while it offers much the same features the company says it’s a much slimmer and lighter prospect. There’s a deep grip for easier handling and built-in Wi-Fi that lets you transfer and share shots via your phone or tablet.",
            ProductTemplateId = productTemplateGroupedId,
            //SeName = "canon-digital-slr-camera",
            AllowCustomerReviews = true,
            Published = true,
            Price = 670M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productNikonD5500DSLR);

        await _dataProvider.InsertEntityAsync(productNikonD5500DSLR);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productNikonD5500DSLR.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Camera & photo")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productNikonD5500DSLR, "product_NikonCamera_1.jpeg");
        await InsertProductPictureAsync(productNikonD5500DSLR, "product_NikonCamera_2.jpeg", 2);

        await InsertProductTagMappingAsync(productNikonD5500DSLR, new[] { "cool", "camera" });

        var productNikonD5500DslrAssociated1 = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = false, //hide this products
            ParentGroupedProductId = productNikonD5500DSLR.Id,
            Name = "Nikon D5500 DSLR - Black",
            Sku = "N5500DS_B",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "canon-digital-slr-camera-black",
            AllowCustomerReviews = true,
            Published = true,
            Price = 670M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productNikonD5500DslrAssociated1);

        await _dataProvider.InsertEntityAsync(productNikonD5500DslrAssociated1);

        await InsertProductPictureAsync(productNikonD5500DslrAssociated1, "product_NikonCamera_black.jpeg");

        var productNikonD5500DslrAssociated2 = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = false, //hide this products
            ParentGroupedProductId = productNikonD5500DSLR.Id,
            Name = "Nikon D5500 DSLR - Red",
            Sku = "N5500DS_R",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "canon-digital-slr-camera-silver",
            AllowCustomerReviews = true,
            Published = true,
            Price = 630M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productNikonD5500DslrAssociated2);

        await _dataProvider.InsertEntityAsync(productNikonD5500DslrAssociated2);

        await InsertProductPictureAsync(productNikonD5500DslrAssociated2, "product_NikonCamera_red.jpeg");

        var productLeica = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Leica T Mirrorless Digital Camera",
            Sku = "LT_MIR_DC",
            ShortDescription = "Leica T (Typ 701) Silver",
            FullDescription = "<p>The new Leica T offers a minimalist design that's crafted from a single block of aluminum.  Made in Germany and assembled by hand, this 16.3 effective mega pixel camera is easy to use.  With a massive 3.7 TFT LCD intuitive touch screen control, the user is able to configure and save their own menu system.  The Leica T has outstanding image quality and also has 16GB of built in memory.  This is Leica's first system camera to use Wi-Fi.  Add the T-App to your portable iOS device and be able to transfer and share your images (free download from the Apple App Store)</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "canon-vixia-hf100-camcorder",
            AllowCustomerReviews = true,
            Price = 530M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productLeica);

        await _dataProvider.InsertEntityAsync(productLeica);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productLeica.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Camera & photo")).Id,
            DisplayOrder = 3
        });

        await InsertProductPictureAsync(productLeica, "product_LeicaT.jpeg");

        await InsertProductTagMappingAsync(productLeica, new[] { "camera", "cool" });

        var productAppleICam = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Apple iCam",
            Sku = "APPLE_CAM",
            ShortDescription = "Photography becomes smart",
            FullDescription = "<p>A few months ago we featured the amazing WVIL camera, by many considered the future of digital photography. This is another very good looking concept, iCam is the vision of Italian designer Antonio DeRosa, the idea is to have a device that attaches to the iPhone 5, which then allows the user to have a camera with interchangeable lenses. The device would also feature a front-touch screen and a projector. Would be great if apple picked up on this and made it reality.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "panasonic-hdc-sdt750k-high-definition-3d-camcorder",
            AllowCustomerReviews = true,
            Price = 1300M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productAppleICam);

        await _dataProvider.InsertEntityAsync(productAppleICam);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productAppleICam.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Camera & photo")).Id,
            DisplayOrder = 2
        });

        await _dataProvider.InsertEntityAsync(new ProductManufacturer
        {
            ProductId = productAppleICam.Id,
            ManufacturerId = (await Table<Manufacturer>().SingleAsync(c => c.Name == "Apple")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productAppleICam, "product_iCam.jpeg");

        var productHtcOne = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "HTC smartphone",
            Sku = "M8_HTC_5L",
            ShortDescription = "HTC - One (M8) 4G LTE Cell Phone with 32GB Memory - Gunmetal (Sprint)",
            FullDescription = "<p><b>HTC One (M8) Cell Phone for Sprint:</b> With its brushed-metal design and wrap-around unibody frame, the HTC One (M8) is designed to fit beautifully in your hand. It's fun to use with amped up sound and a large Full HD touch screen, and intuitive gesture controls make it seem like your phone almost knows what you need before you do. <br /><br />Sprint Easy Pay option available in store.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "blackberry-bold-9000-phone-black-att",
            AllowCustomerReviews = true,
            Price = 245M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            ShowOnHomepage = true,
            MarkAsNew = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productHtcOne);

        await _dataProvider.InsertEntityAsync(productHtcOne);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productHtcOne.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Cell phones")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productHtcOne, "product_HTC_One_M8.jpeg");

        await InsertProductTagMappingAsync(productHtcOne, new[] { "cell", "compact", "awesome" });

        var productHtcOneMini = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "HTC One Mini Blue",
            Sku = "OM_HTC_BL",
            ShortDescription = "HTC One and HTC One Mini now available in bright blue hue",
            FullDescription = "<p>HTC One mini smartphone with 4.30-inch 720x1280 display powered by 1.4GHz processor alongside 1GB RAM and 4-Ultrapixel rear camera.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "samsung-rugby-a837-phone-black-att",
            AllowCustomerReviews = true,
            Price = 100M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            MarkAsNew = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productHtcOneMini);

        await _dataProvider.InsertEntityAsync(productHtcOneMini);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productHtcOneMini.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Cell phones")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productHtcOneMini, "product_HTC_One_Mini_1.jpeg");
        await InsertProductPictureAsync(productHtcOneMini, "product_HTC_One_Mini_2.jpeg", 2);

        await InsertProductTagMappingAsync(productHtcOneMini, new[] { "awesome", "compact", "cell" });

        var productNokiaLumia = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Nokia Lumia 1020",
            Sku = "N_1020_LU",
            ShortDescription = "Nokia Lumia 1020 4G Cell Phone (Unlocked)",
            FullDescription = "<p>Capture special moments for friends and family with this Nokia Lumia 1020 32GB WHITE cell phone that features an easy-to-use 41.0MP rear-facing camera and a 1.2MP front-facing camera. The AMOLED touch screen offers 768 x 1280 resolution for crisp visuals.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "sony-dcr-sr85-1mp-60gb-hard-drive-handycam-camcorder",
            AllowCustomerReviews = true,
            Price = 349M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productNokiaLumia);

        await _dataProvider.InsertEntityAsync(productNokiaLumia);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productNokiaLumia.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Cell phones")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productNokiaLumia, "product_Lumia1020.jpeg");

        await InsertProductTagMappingAsync(productNokiaLumia, new[] { "awesome", "cool", "camera" });

        var productIphone = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Apple iPhone 16 128GB",
            Sku = "A_16_128T",
            ShortDescription = "Apple iPhone 16 128GB Teal with 6.1 inches screen and 48 megapixels rear-facing camera",
            FullDescription = "<p>iPhone 16 brings you Dynamic Island, a 48MP Main camera and USB-C — all in a durable colour-infused glass and aluminium design. iPhone 16 has the Dynamic Island, an innovative way to interact with important alerts and Live Activities.</p>",
            ProductTemplateId = productTemplateSimpleId,
            AllowCustomerReviews = true,
            Price = 799M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            MarkAsNew = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productIphone);

        await _dataProvider.InsertEntityAsync(productIphone);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productIphone.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Cell phones")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productIphone, "product_iphone_16_128.png");
        await InsertProductTagMappingAsync(productIphone, new[] { "cool", "cell" });

        var productSamsungGalaxy = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Samsung Galaxy S24 256GB",
            Sku = "SG_24_256B",
            ShortDescription = "Samsung - Galaxy S24 256GB - Onyx Black with Dynamic LTPO AMOLED 2X, 120Hz, HDR10+",
            FullDescription = "<p>The Samsung Galaxy S24 combines a fast processor, a bright display, sharp cameras, and helpful AI tools in a pocket-friendly size for a reasonable price. In addition to a top-end Qualcomm processor and excellent cameras, the Galaxy S24 gets all of Samsung’s cutting-edge AI features.</p>",
            ProductTemplateId = productTemplateSimpleId,
            AllowCustomerReviews = true,
            Price = 859M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            MarkAsNew = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productSamsungGalaxy);

        await _dataProvider.InsertEntityAsync(productSamsungGalaxy);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productSamsungGalaxy.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Cell phones")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productSamsungGalaxy, "product_samsung_galaxy_s24.png");
        await InsertProductTagMappingAsync(productSamsungGalaxy, new[] { "awesome", "cell" });


        var productBeatsPill = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Beats Pill Wireless Speaker",
            Sku = "BP_20_WSP",
            ShortDescription = "<b>Pill 2.0 Portable Bluetooth Speaker (1-Piece):</b> Watch your favorite movies and listen to music with striking sound quality. This lightweight, portable speaker is easy to take with you as you travel to any destination, keeping you entertained wherever you are. ",
            FullDescription = "<ul><li>Pair and play with your Bluetooth® device with 30 foot range</li><li>Built-in speakerphone</li><li>7 hour rechargeable battery</li><li>Power your other devices with USB charge out</li><li>Tap two Beats Pills™ together for twice the sound with Beats Bond™</li></ul>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "acer-aspire-one-89-mini-notebook-case-black",
            AllowCustomerReviews = true,
            Price = 79.99M,
            IsShipEnabled = true,
            IsFreeShipping = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 3,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            MarkAsNew = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productBeatsPill);

        await _dataProvider.InsertEntityAsync(productBeatsPill);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productBeatsPill.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Others")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productBeatsPill, "product_PillBeats_1.jpeg");
        await InsertProductPictureAsync(productBeatsPill, "product_PillBeats_2.jpeg", 2);

        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new TierPrice {
                Quantity = 2,
                Price = 19,
                ProductId = productBeatsPill.Id
            },
            new TierPrice {
                Quantity = 5,
                Price = 17,
                ProductId = productBeatsPill.Id
            },
            new TierPrice {
                Quantity = 10,
                Price = 15,
                StartDateTimeUtc = DateTime.UtcNow.AddDays(-7),
                EndDateTimeUtc = DateTime.UtcNow.AddDays(7),
                ProductId = productBeatsPill.Id
            }
        });

        await InsertProductTagMappingAsync(productBeatsPill, new[] { "computer", "cool" });

        var productUniversalTabletCover = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Universal 7-8 Inch Tablet Cover",
            Sku = "TC_78I_UN",
            ShortDescription = "Universal protection for 7-inch & 8-inch tablets",
            FullDescription = "<p>Made of durable polyurethane, our Universal Cover is slim, lightweight, and strong, with protective corners that stretch to hold most 7 and 8-inch tablets securely. This tough case helps protects your tablet from bumps, scuffs, and dings.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "apc-back-ups-rs-800va-ups-800-va-ups-battery-lead-acid-br800blk",
            AllowCustomerReviews = true,
            Price = 39M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 3,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productUniversalTabletCover);

        await _dataProvider.InsertEntityAsync(productUniversalTabletCover);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productUniversalTabletCover.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Others")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productUniversalTabletCover, "product_TabletCover.jpeg");

        await InsertProductTagMappingAsync(productUniversalTabletCover, new[] { "computer", "cool" });

        var productPortableSoundSpeakers = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Portable Sound Speakers",
            Sku = "PT_SPK_SN",
            ShortDescription = "Universall portable sound speakers",
            FullDescription = "<p>Your phone cut the cord, now it's time for you to set your music free and buy a Bluetooth speaker. Thankfully, there's one suited for everyone out there.</p><p>Some Bluetooth speakers excel at packing in as much functionality as the unit can handle while keeping the price down. Other speakers shuck excess functionality in favor of premium build materials instead. Whatever path you choose to go down, you'll be greeted with many options to suit your personal tastes.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "microsoft-bluetooth-notebook-mouse-5000-macwindows",
            AllowCustomerReviews = true,
            Price = 37M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Electronics & Software")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productPortableSoundSpeakers);

        await _dataProvider.InsertEntityAsync(productPortableSoundSpeakers);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productPortableSoundSpeakers.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Others")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productPortableSoundSpeakers, "product_Speakers.jpeg");

        relatedProducts.AddRange(new[]
        {
            new RelatedProduct
            {
                ProductId1 = productLeica.Id,
                ProductId2 = productHtcOneMini.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLeica.Id,
                ProductId2 = productNikonD5500DSLR.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLeica.Id,
                ProductId2 = productAppleICam.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLeica.Id,
                ProductId2 = productNokiaLumia.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHtcOne.Id,
                ProductId2 = productHtcOneMini.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHtcOne.Id,
                ProductId2 = productNokiaLumia.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHtcOne.Id,
                ProductId2 = productBeatsPill.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHtcOne.Id,
                ProductId2 = productPortableSoundSpeakers.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHtcOneMini.Id,
                ProductId2 = productHtcOne.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHtcOneMini.Id,
                ProductId2 = productNokiaLumia.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHtcOneMini.Id,
                ProductId2 = productBeatsPill.Id
            },
            new RelatedProduct
            {
                ProductId1 = productHtcOneMini.Id,
                ProductId2 = productPortableSoundSpeakers.Id
            },
            new RelatedProduct
            {
                ProductId1 = productNokiaLumia.Id,
                ProductId2 = productHtcOne.Id
            },
            new RelatedProduct
            {
                ProductId1 = productNokiaLumia.Id,
                ProductId2 = productHtcOneMini.Id
            },
            new RelatedProduct
            {
                ProductId1 = productNokiaLumia.Id,
                ProductId2 = productBeatsPill.Id
            },
            new RelatedProduct
            {
                ProductId1 = productNokiaLumia.Id,
                ProductId2 = productPortableSoundSpeakers.Id
            }
        });
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallApparelAsync(int productTemplateSimpleId, List<Product> allProducts, List<RelatedProduct> relatedProducts)
    {
        //product availability range
        var productAvailabilityRangeId = await GetFirstEntityIdAsync<ProductAvailabilityRange>() ?? throw new Exception("No default product availability range could be loaded");

        var productNikeFloral = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Nike Floral Roshe Customized Running Shoes",
            Sku = "NK_FRC_RS",
            ShortDescription = "When you ran across these shoes, you will immediately fell in love and needed a pair of these customized beauties.",
            FullDescription = "<p>Each Rosh Run is personalized and exclusive, handmade in our workshop Custom. Run Your Rosh creations born from the hand of an artist specialized in sneakers, more than 10 years of experience.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "adidas-womens-supernova-csh-7-running-shoe",
            AllowCustomerReviews = true,
            Price = 40M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Apparel")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productNikeFloral);

        await _dataProvider.InsertEntityAsync(productNikeFloral);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productNikeFloral.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Shoes")).Id,
            DisplayOrder = 1
        });

        await _dataProvider.InsertEntityAsync(new ProductManufacturer
        {
            ProductId = productNikeFloral.Id,
            ManufacturerId = (await Table<Manufacturer>().SingleAsync(c => c.Name == "Nike")).Id,
            DisplayOrder = 2
        });

        var picProductNikeFloralShoe1Id = await InsertProductPictureAsync(productNikeFloral, "product_NikeFloralShoe_1.jpg");
        var picProductNikeFloralShoe2Id = await InsertProductPictureAsync(productNikeFloral, "product_NikeFloralShoe_2.jpg", 2);

        await _dataProvider.InsertEntityAsync(new ProductSpecificationAttribute
        {
            ProductId = productNikeFloral.Id,
            AllowFiltering = true,
            ShowOnProductPage = false,
            DisplayOrder = 1,
            SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Color", "Grey")
        });

        var pamSize = await _dataProvider.InsertEntityAsync(
            new ProductAttributeMapping
            {
                ProductId = productNikeFloral.Id,
                ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "Size")).Id,
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true
            });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "8",
                DisplayOrder = 1
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "9",
                DisplayOrder = 2
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "10",
                DisplayOrder = 3
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "11",
                DisplayOrder = 4
            }});

        var pamColor = await _dataProvider.InsertEntityAsync(
            new ProductAttributeMapping
            {
                ProductId = productNikeFloral.Id,
                ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "Color")).Id,
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true
            });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamColor.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "White/Blue",
                DisplayOrder = 1
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamColor.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "White/Black",
                DisplayOrder = 2
            }});

        var pamPrint = await _dataProvider.InsertEntityAsync(
            new ProductAttributeMapping
            {
                ProductId = productNikeFloral.Id,
                ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "Print")).Id,
                AttributeControlType = AttributeControlType.ImageSquares,
                IsRequired = true
            });

        var pavNatural = await _dataProvider.InsertEntityAsync(
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamPrint.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Natural",
                DisplayOrder = 1,
                ImageSquaresPictureId = await InsertPictureAsync("p_attribute_print_2.jpg", "Natural Print")
            });

        await _dataProvider.InsertEntityAsync(new ProductAttributeValuePicture
        {
            PictureId = picProductNikeFloralShoe1Id,
            ProductAttributeValueId = pavNatural.Id
        });

        var pavFresh = await _dataProvider.InsertEntityAsync(
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamPrint.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Fresh",
                DisplayOrder = 2,
                ImageSquaresPictureId = await InsertPictureAsync("p_attribute_print_1.jpg", "Fresh Print")
            });

        await _dataProvider.InsertEntityAsync(
            new ProductAttributeValuePicture
            {
                PictureId = picProductNikeFloralShoe2Id,
                ProductAttributeValueId = pavFresh.Id
            });

        await InsertProductTagMappingAsync(productNikeFloral, new[] { "cool", "shoes", "apparel" });

        await _dataProvider.UpdateEntityAsync(productNikeFloral);

        var productAdidas = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "adidas Consortium Campus 80s Running Shoes",
            Sku = "AD_C80_RS",
            ShortDescription = "adidas Consortium Campus 80s Primeknit Light Maroon/Running Shoes",
            FullDescription = "<p>One of three colorways of the adidas Consortium Campus 80s Primeknit set to drop alongside each other. This pair comes in light maroon and running white. Featuring a maroon-based primeknit upper with white accents. A limited release, look out for these at select adidas Consortium accounts worldwide.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "etnies-mens-digit-sneaker",
            AllowCustomerReviews = true,
            Price = 27.56M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Apparel")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            //ShowOnHomepage = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productAdidas);

        await _dataProvider.InsertEntityAsync(productAdidas);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productAdidas.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Shoes")).Id,
            DisplayOrder = 1
        });

        var picProductAdidasId = await InsertProductPictureAsync(productAdidas, "product_adidas.jpg");
        var picProductAdidas2Id = await InsertProductPictureAsync(productAdidas, "product_adidas_2.jpg", 2);
        var picProductAdidas3Id = await InsertProductPictureAsync(productAdidas, "product_adidas_3.jpg", 3);

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductSpecificationAttribute
            {
                ProductId = productAdidas.Id,
                AllowFiltering = true,
                ShowOnProductPage = false,
                DisplayOrder = 1,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Color", "Grey")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productAdidas.Id,
                AllowFiltering = true,
                ShowOnProductPage = false,
                DisplayOrder = 2,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Color", "Red")
            },
            new ProductSpecificationAttribute
            {
                ProductId = productAdidas.Id,
                AllowFiltering = true,
                ShowOnProductPage = false,
                DisplayOrder = 3,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Color", "Blue")
            }});

        var pamAdidasSize = await _dataProvider.InsertEntityAsync(
            new ProductAttributeMapping
            {
                ProductId = productAdidas.Id,
                ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "Size")).Id,
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true
            });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamAdidasSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "8",
                DisplayOrder = 1
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamAdidasSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "9",
                DisplayOrder = 2
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamAdidasSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "10",
                DisplayOrder = 3
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamAdidasSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "11",
                DisplayOrder = 4
            }});

        var pamAdidasColor = await _dataProvider.InsertEntityAsync(
            new ProductAttributeMapping
            {
                ProductId = productAdidas.Id,
                ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "Color")).Id,
                AttributeControlType = AttributeControlType.ColorSquares,
                IsRequired = true
            });

        var pavRed = await _dataProvider.InsertEntityAsync(
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamAdidasColor.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Red",
                IsPreSelected = true,
                ColorSquaresRgb = "#663030",
                DisplayOrder = 1
            });

        await _dataProvider.InsertEntityAsync(
            new ProductAttributeValuePicture
            {
                PictureId = picProductAdidasId,
                ProductAttributeValueId = pavRed.Id
            });

        var pavBlue = await _dataProvider.InsertEntityAsync(new ProductAttributeValue
        {
            ProductAttributeMappingId = pamAdidasColor.Id,
            AttributeValueType = AttributeValueType.Simple,
            Name = "Blue",
            ColorSquaresRgb = "#363656",
            DisplayOrder = 2
        });

        await _dataProvider.InsertEntityAsync(
            new ProductAttributeValuePicture
            {
                PictureId = picProductAdidas2Id,
                ProductAttributeValueId = pavBlue.Id
            });

        var pavSilver = await _dataProvider.InsertEntityAsync(
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamAdidasColor.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Silver",
                ColorSquaresRgb = "#c5c5d5",
                DisplayOrder = 3
            });

        await _dataProvider.InsertEntityAsync(
            new ProductAttributeValuePicture
            {
                PictureId = picProductAdidas3Id,
                ProductAttributeValueId = pavSilver.Id
            });

        await InsertProductTagMappingAsync(productAdidas, new[] { "cool", "shoes", "apparel" });

        await _dataProvider.UpdateEntityAsync(productAdidas);

        var productNikeZoom = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Nike SB Zoom Stefan Janoski \"Medium Mint\"",
            Sku = "NK_ZSJ_MM",
            ShortDescription = "Nike SB Zoom Stefan Janoski Dark Grey Medium Mint Teal ...",
            FullDescription = "The newly Nike SB Zoom Stefan Janoski gets hit with a \"Medium Mint\" accents that sits atop a Dark Grey suede. Expected to drop in October.",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "v-blue-juniors-cuffed-denim-short-with-rhinestones",
            AllowCustomerReviews = true,
            Price = 30M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Apparel")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };

        allProducts.Add(productNikeZoom);

        await _dataProvider.InsertEntityAsync(productNikeZoom);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productNikeZoom.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Shoes")).Id,
            DisplayOrder = 1
        });

        await _dataProvider.InsertEntityAsync(new ProductManufacturer
        {
            ProductId = productNikeZoom.Id,
            ManufacturerId = (await Table<Manufacturer>().SingleAsync(c => c.Name == "Nike")).Id,
            DisplayOrder = 2
        });

        await InsertProductPictureAsync(productNikeZoom, "product_NikeZoom.jpg");

        await _dataProvider.InsertEntityAsync(new ProductSpecificationAttribute
        {
            ProductId = productNikeZoom.Id,
            AllowFiltering = true,
            ShowOnProductPage = false,
            DisplayOrder = 1,
            SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Color", "Grey")
        });

        await InsertProductTagMappingAsync(productNikeZoom, new[] { "jeans", "cool", "apparel" });

        var productNikeTailwind = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Nike Tailwind Loose Short-Sleeve Running Shirt",
            Sku = "NK_TLS_RS",
            ShortDescription = string.Empty,
            FullDescription = "<p>Boost your adrenaline with the Nike® Women's Tailwind Running Shirt. The lightweight, slouchy fit is great for layering, and moisture-wicking fabrics keep you feeling at your best. This tee has a notched hem for an enhanced range of motion, while flat seams with reinforcement tape lessen discomfort and irritation over longer distances. Put your keys and card in the side zip pocket and take off in your Nike® running t-shirt.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "50s-rockabilly-polka-dot-top-jr-plus-size",
            AllowCustomerReviews = true,
            Published = true,
            Price = 15M,
            IsShipEnabled = true,
            Weight = 1,
            Length = 2,
            Width = 3,
            Height = 3,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Apparel")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productNikeTailwind);

        await _dataProvider.InsertEntityAsync(productNikeTailwind);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productNikeTailwind.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Clothing")).Id,
            DisplayOrder = 1
        });

        await _dataProvider.InsertEntityAsync(new ProductManufacturer
        {
            ProductId = productNikeTailwind.Id,
            ManufacturerId = (await Table<Manufacturer>().SingleAsync(c => c.Name == "Nike")).Id,
            DisplayOrder = 2
        });

        await InsertProductPictureAsync(productNikeTailwind, "product_NikeShirt.jpg");

        var pamNikeSize = await _dataProvider.InsertEntityAsync(
            new ProductAttributeMapping
            {
                ProductId = productNikeTailwind.Id,
                ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "Size")).Id,
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true
            });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamNikeSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Small",
                DisplayOrder = 1
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamNikeSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "1X",
                DisplayOrder = 2
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamNikeSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "2X",
                DisplayOrder = 3
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamNikeSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "3X",
                DisplayOrder = 4
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamNikeSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "4X",
                DisplayOrder = 5
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamNikeSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "5X",
                DisplayOrder = 6
            }});

        await InsertProductTagMappingAsync(productNikeTailwind, new[] { "cool", "apparel", "shirt" });

        var productOversizedWomenTShirt = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Oversized Women T-Shirt",
            Sku = "WM_OVR_TS",
            ShortDescription = string.Empty,
            FullDescription = "<p>This oversized women t-Shirt needs minimum ironing. It is a great product at a great value!</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "arrow-mens-wrinkle-free-pinpoint-solid-long-sleeve",
            AllowCustomerReviews = true,
            Price = 24M,
            IsShipEnabled = true,
            Weight = 4,
            Length = 3,
            Width = 3,
            Height = 3,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Apparel")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };

        allProducts.Add(productOversizedWomenTShirt);

        await _dataProvider.InsertEntityAsync(productOversizedWomenTShirt);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productOversizedWomenTShirt.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Clothing")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productOversizedWomenTShirt, "product_WomenTShirt.jpg");

        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new TierPrice {
                Quantity = 3,
                Price = 21,
                ProductId = productOversizedWomenTShirt.Id
            },
            new TierPrice {
                Quantity = 7,
                Price = 19,
                ProductId = productOversizedWomenTShirt.Id
            },
            new TierPrice {
                Quantity = 10,
                Price = 16,
                ProductId = productOversizedWomenTShirt.Id
            }
        });

        await InsertProductTagMappingAsync(productOversizedWomenTShirt, new[] { "cool", "apparel", "shirt" });

        var productCustomTShirt = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Custom T-Shirt",
            Sku = "CS_TSHIRT",
            ShortDescription = "T-Shirt - Add Your Content",
            FullDescription = "<p>Comfort comes in all shapes and forms, yet this tee out does it all. Rising above the rest, our classic cotton crew provides the simple practicality you need to make it through the day. Tag-free, relaxed fit wears well under dress shirts or stands alone in laid-back style. Reinforced collar and lightweight feel give way to long-lasting shape and breathability. One less thing to worry about, rely on this tee to provide comfort and ease with every wear.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "custom-t-shirt",
            AllowCustomerReviews = true,
            Price = 15M,
            IsShipEnabled = true,
            Weight = 4,
            Length = 3,
            Width = 3,
            Height = 3,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Apparel")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productCustomTShirt);

        await _dataProvider.InsertEntityAsync(productCustomTShirt);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productCustomTShirt.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Clothing")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productCustomTShirt, "product_CustomTShirt.jpeg");

        await _dataProvider.InsertEntityAsync(
            new ProductAttributeMapping
            {
                ProductId = productCustomTShirt.Id,
                ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "Custom Text")).Id,
                TextPrompt = "Enter your text:",
                AttributeControlType = AttributeControlType.TextBox,
                IsRequired = true
            });

        await InsertProductTagMappingAsync(productCustomTShirt, new[] { "cool", "shirt", "apparel" });

        var productLeviJeans = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Levi's 511 Jeans",
            Sku = "LV_511_JN",
            ShortDescription = "Levi's Faded Black 511 Jeans ",
            FullDescription = "<p>Between a skinny and straight fit, our 511&trade; slim fit jeans are cut close without being too restricting. Slim throughout the thigh and leg opening for a long and lean look.</p><ul><li>Slouch1y at top; sits below the waist</li><li>Slim through the leg, close at the thigh and straight to the ankle</li><li>Stretch for added comfort</li><li>Classic five-pocket styling</li><li>99% Cotton, 1% Spandex, 11.2 oz. - Imported</li></ul>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "levis-skinny-511-jeans",
            AllowCustomerReviews = true,
            Price = 43.5M,
            OldPrice = 55M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Apparel")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productLeviJeans);

        await _dataProvider.InsertEntityAsync(productLeviJeans);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productLeviJeans.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Clothing")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productLeviJeans, "product_LeviJeans_1.jpg");
        await InsertProductPictureAsync(productLeviJeans, "product_LeviJeans_2.jpg", 2);

        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new TierPrice {
                Quantity = 3,
                Price = 40,
                ProductId = productLeviJeans.Id
            },
            new TierPrice {
                Quantity = 6,
                Price = 38,
                ProductId = productLeviJeans.Id
            },
            new TierPrice {
                Quantity = 10,
                Price = 35,
                ProductId = productLeviJeans.Id
            }
        });

        await InsertProductTagMappingAsync(productLeviJeans, new[] { "cool", "jeans", "apparel" });

        var productObeyHat = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Obey Propaganda Hat",
            Sku = "OB_HAT_PR",
            ShortDescription = string.Empty,
            FullDescription = "<p>Printed poplin 5 panel camp hat with debossed leather patch and web closure</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "indiana-jones-shapeable-wool-hat",
            AllowCustomerReviews = true,
            Price = 30M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Apparel")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productObeyHat);

        await _dataProvider.InsertEntityAsync(productObeyHat);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productObeyHat.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Accessories")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productObeyHat, "product_hat.jpg");

        var pamObeyHatSize = await _dataProvider.InsertEntityAsync(
            new ProductAttributeMapping
            {
                ProductId = productObeyHat.Id,
                ProductAttributeId = (await Table<ProductAttribute>().SingleAsync(x => x.Name == "Size")).Id,
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true
            });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamObeyHatSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Small",
                DisplayOrder = 1
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamObeyHatSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Medium",
                DisplayOrder = 2
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamObeyHatSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "Large",
                DisplayOrder = 3
            },
            new ProductAttributeValue
            {
                ProductAttributeMappingId = pamObeyHatSize.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = "X-Large",
                DisplayOrder = 4
            }});

        await InsertProductTagMappingAsync(productObeyHat, new[] { "apparel", "cool" });

        var productBelt = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Reversible Horseferry Check Belt",
            Sku = "RH_CHK_BL",
            ShortDescription = "Reversible belt in Horseferry check with smooth leather trim",
            FullDescription = "<p>Reversible belt in Horseferry check with smooth leather trim</p><p>Leather lining, polished metal buckle</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "nike-golf-casual-belt",
            AllowCustomerReviews = true,
            Price = 45M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Apparel")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            ProductAvailabilityRangeId = productAvailabilityRangeId,
            StockQuantity = 0,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productBelt);

        await _dataProvider.InsertEntityAsync(productBelt);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productBelt.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Accessories")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productBelt, "product_Belt.jpeg");

        var productSunglasses = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Ray Ban Aviator Sunglasses",
            Sku = "RB_AVR_SG",
            ShortDescription = "Aviator sunglasses are one of the first widely popularized styles of modern day sunwear.",
            FullDescription = "<p>Since 1937, Ray-Ban can genuinely claim the title as the world's leading sunglasses and optical eyewear brand. Combining the best of fashion and sports performance, the Ray-Ban line of Sunglasses delivers a truly classic style that will have you looking great today and for years to come.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "ray-ban-aviator-sunglasses-rb-3025",
            AllowCustomerReviews = true,
            Price = 25M,
            IsShipEnabled = true,
            Weight = 7,
            Length = 7,
            Width = 7,
            Height = 7,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Apparel")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productSunglasses);

        await _dataProvider.InsertEntityAsync(productSunglasses);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productSunglasses.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Accessories")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productSunglasses, "product_Sunglasses.jpg");

        await InsertProductTagMappingAsync(productSunglasses, new[] { "apparel", "cool" });

        relatedProducts.AddRange(new[]
        {
            new RelatedProduct
            {
                ProductId1 = productAdidas.Id,
                ProductId2 = productLeviJeans.Id
            },
            new RelatedProduct
            {
                ProductId1 = productAdidas.Id,
                ProductId2 = productNikeFloral.Id
            },
            new RelatedProduct
            {
                ProductId1 = productAdidas.Id,
                ProductId2 = productNikeZoom.Id
            },
            new RelatedProduct
            {
                ProductId1 = productAdidas.Id,
                ProductId2 = productNikeTailwind.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLeviJeans.Id,
                ProductId2 = productAdidas.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLeviJeans.Id,
                ProductId2 = productNikeFloral.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLeviJeans.Id,
                ProductId2 = productNikeZoom.Id
            },
            new RelatedProduct
            {
                ProductId1 = productLeviJeans.Id,
                ProductId2 = productNikeTailwind.Id
            },

            new RelatedProduct
            {
                ProductId1 = productCustomTShirt.Id,
                ProductId2 = productLeviJeans.Id
            },
            new RelatedProduct
            {
                ProductId1 = productCustomTShirt.Id,
                ProductId2 = productNikeTailwind.Id
            },
            new RelatedProduct
            {
                ProductId1 = productCustomTShirt.Id,
                ProductId2 = productOversizedWomenTShirt.Id
            },
            new RelatedProduct
            {
                ProductId1 = productCustomTShirt.Id,
                ProductId2 = productObeyHat.Id
            }
        });
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallDigitalDownloadsAsync(int productTemplateSimpleId, List<Product> allProducts, List<RelatedProduct> relatedProducts)
    {
        //downloads
        //TODO: avoid using service
        var downloadService = EngineContext.Current.Resolve<IDownloadService>();

        var sampleDownloadsPath = _fileProvider.GetAbsolutePath(NopInstallationDefaults.SampleImagesPath);

        var downloadNightVision1 = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            ContentType = MimeTypes.ApplicationXZipCo,
            DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + "product_NightVision_1.zip"),
            Extension = ".zip",
            Filename = "Night_Vision_1",
            IsNew = true
        };
        await downloadService.InsertDownloadAsync(downloadNightVision1);
        var downloadNightVision2 = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            ContentType = MimeTypes.TextPlain,
            DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + "product_NightVision_2.txt"),
            Extension = ".txt",
            Filename = "Night_Vision_1",
            IsNew = true
        };
        await downloadService.InsertDownloadAsync(downloadNightVision2);
        var productNightVision = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Night Visions",
            Sku = "NIGHT_VSN",
            ShortDescription = "Night Visions is the debut studio album by American rock band Imagine Dragons.",
            FullDescription = "<p>Original Release Date: September 4, 2012</p><p>Release Date: September 4, 2012</p><p>Genre - Alternative rock, indie rock, electronic rock</p><p>Label - Interscope/KIDinaKORNER</p><p>Copyright: (C) 2011 Interscope Records</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "poker-face",
            AllowCustomerReviews = true,
            Price = 2.8M,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Downloadable Products")).Id,
            ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            IsDownload = true,
            DownloadId = downloadNightVision1.Id,
            DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
            UnlimitedDownloads = true,
            HasUserAgreement = false,
            HasSampleDownload = true,
            SampleDownloadId = downloadNightVision2.Id,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productNightVision);

        await _dataProvider.InsertEntityAsync(productNightVision);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productNightVision.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Digital downloads")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productNightVision, "product_NightVisions.jpeg");

        await InsertProductTagMappingAsync(productNightVision, new[] { "awesome", "digital" });

        var downloadIfYouWait1 = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            ContentType = MimeTypes.ApplicationXZipCo,
            DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + "product_IfYouWait_1.zip"),
            Extension = ".zip",
            Filename = "If_You_Wait_1",
            IsNew = true
        };
        await downloadService.InsertDownloadAsync(downloadIfYouWait1);
        var downloadIfYouWait2 = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            ContentType = MimeTypes.TextPlain,
            DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + "product_IfYouWait_2.txt"),
            Extension = ".txt",
            Filename = "If_You_Wait_1",
            IsNew = true
        };
        await downloadService.InsertDownloadAsync(downloadIfYouWait2);
        var productIfYouWait = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "If You Wait (donation)",
            Sku = "IF_YOU_WT",
            ShortDescription = "If You Wait is the debut studio album by English indie pop band London Grammar",
            FullDescription = "<p>Original Release Date: September 6, 2013</p><p>Genre - Electronica, dream pop downtempo, pop</p><p>Label - Metal & Dust/Ministry of Sound</p><p>Producer - Tim Bran, Roy Kerr London, Grammar</p><p>Length - 43:22</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "single-ladies-put-a-ring-on-it",
            CustomerEntersPrice = true,
            MinimumCustomerEnteredPrice = 0.5M,
            MaximumCustomerEnteredPrice = 100M,
            AllowCustomerReviews = true,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Downloadable Products")).Id,
            ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            IsDownload = true,
            DownloadId = downloadIfYouWait1.Id,
            DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
            UnlimitedDownloads = true,
            HasUserAgreement = false,
            HasSampleDownload = true,
            SampleDownloadId = downloadIfYouWait2.Id,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productIfYouWait);

        await _dataProvider.InsertEntityAsync(productIfYouWait);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productIfYouWait.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Digital downloads")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productIfYouWait, "product_IfYouWait.jpeg");

        await InsertProductTagMappingAsync(productIfYouWait, new[] { "digital", "awesome" });

        var downloadScienceAndFaith = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            ContentType = MimeTypes.ApplicationXZipCo,
            DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + "product_ScienceAndFaith_1.zip"),
            Extension = ".zip",
            Filename = "Science_And_Faith",
            IsNew = true
        };
        await downloadService.InsertDownloadAsync(downloadScienceAndFaith);
        var productScienceAndFaith = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Science & Faith",
            Sku = "SCI_FAITH",
            ShortDescription = "Science & Faith is the second studio album by Irish pop rock band The Script.",
            FullDescription = "<p># Original Release Date: September 10, 2010<br /># Label: RCA, Epic/Phonogenic(America)<br /># Copyright: 2010 RCA Records.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "the-battle-of-los-angeles",
            AllowCustomerReviews = true,
            CustomerEntersPrice = true,
            MinimumCustomerEnteredPrice = 0.5M,
            MaximumCustomerEnteredPrice = 1000M,
            Price = decimal.Zero,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Downloadable Products")).Id,
            ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            IsDownload = true,
            DownloadId = downloadScienceAndFaith.Id,
            DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
            UnlimitedDownloads = true,
            HasUserAgreement = false,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productScienceAndFaith);

        await _dataProvider.InsertEntityAsync(productScienceAndFaith);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productScienceAndFaith.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Digital downloads")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productScienceAndFaith, "product_ScienceAndFaith.jpeg");

        await InsertProductTagMappingAsync(productScienceAndFaith, new[] { "digital", "awesome" });

        relatedProducts.AddRange(new[]
        {
            new RelatedProduct
            {
                ProductId1 = productIfYouWait.Id,
                ProductId2 = productNightVision.Id
            },
            new RelatedProduct
            {
                ProductId1 = productIfYouWait.Id,
                ProductId2 = productScienceAndFaith.Id
            },
            new RelatedProduct
            {
                ProductId1 = productNightVision.Id,
                ProductId2 = productIfYouWait.Id
            },
            new RelatedProduct
            {
                ProductId1 = productNightVision.Id,
                ProductId2 = productScienceAndFaith.Id
            }
        });
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallBooksAsync(int productTemplateSimpleId, List<Product> allProducts, List<RelatedProduct> relatedProducts)
    {
        var productFahrenheit = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Fahrenheit 451 by Ray Bradbury",
            Sku = "FR_451_RB",
            ShortDescription = "Fahrenheit 451 is a dystopian novel by Ray Bradbury published in 1953. It is regarded as one of his best works.",
            FullDescription = "<p>The novel presents a future American society where books are outlawed and firemen burn any that are found. The title refers to the temperature that Bradbury understood to be the autoignition point of paper.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "best-grilling-recipes",
            AllowCustomerReviews = true,
            Price = 27M,
            OldPrice = 30M,
            IsShipEnabled = true,
            IsFreeShipping = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Books")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productFahrenheit);

        await _dataProvider.InsertEntityAsync(productFahrenheit);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productFahrenheit.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Books")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productFahrenheit, "product_Fahrenheit451.jpeg");

        await InsertProductTagMappingAsync(productFahrenheit, new[] { "awesome", "book", "nice" });

        var productFirstPrizePies = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "First Prize Pies",
            Sku = "FIRST_PRP",
            ShortDescription = "Allison Kave made pies as a hobby, until one day her boyfriend convinced her to enter a Brooklyn pie-making contest. She won. In fact, her pies were such a hit that she turned pro.",
            FullDescription = "<p>First Prize Pies, a boutique, made-to-order pie business that originated on New York's Lower East Side, has become synonymous with tempting and unusual confections. For the home baker who is passionate about seasonal ingredients and loves a creative approach to recipes, First Prize Pies serves up 52 weeks of seasonal and eclectic pastries in an interesting pie-a-week format. Clear instructions, technical tips and creative encouragement guide novice bakers as well as pie mavens. With its nostalgia-evoking photos of homemade pies fresh out of the oven, First Prize Pies will be as giftable as it is practical.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "eatingwell-in-season",
            AllowCustomerReviews = true,
            Price = 51M,
            OldPrice = 67M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Books")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productFirstPrizePies);

        await _dataProvider.InsertEntityAsync(productFirstPrizePies);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productFirstPrizePies.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Books")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productFirstPrizePies, "product_FirstPrizePies.jpeg");

        await InsertProductTagMappingAsync(productFirstPrizePies, new[] { "book" });

        var productPrideAndPrejudice = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Pride and Prejudice",
            Sku = "PRIDE_PRJ",
            ShortDescription = "Pride and Prejudice is a novel of manners by Jane Austen, first published in 1813.",
            FullDescription = "<p>Set in England in the early 19th century, Pride and Prejudice tells the story of Mr and Mrs Bennet's five unmarried daughters after the rich and eligible Mr Bingley and his status-conscious friend, Mr Darcy, have moved into their neighbourhood. While Bingley takes an immediate liking to the eldest Bennet daughter, Jane, Darcy has difficulty adapting to local society and repeatedly clashes with the second-eldest Bennet daughter, Elizabeth.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "the-best-skillet-recipes",
            AllowCustomerReviews = true,
            Price = 24M,
            OldPrice = 35M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Books")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productPrideAndPrejudice);

        await _dataProvider.InsertEntityAsync(productPrideAndPrejudice);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productPrideAndPrejudice.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Books")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productPrideAndPrejudice, "product_PrideAndPrejudice.jpeg");

        await InsertProductTagMappingAsync(productPrideAndPrejudice, new[] { "book" });

        relatedProducts.AddRange(new[]
        {
            new RelatedProduct
            {
                ProductId1 = productPrideAndPrejudice.Id,
                ProductId2 = productFirstPrizePies.Id
            },
            new RelatedProduct
            {
                ProductId1 = productPrideAndPrejudice.Id,
                ProductId2 = productFahrenheit.Id
            },
            new RelatedProduct
            {
                ProductId1 = productFirstPrizePies.Id,
                ProductId2 = productPrideAndPrejudice.Id
            },
            new RelatedProduct
            {
                ProductId1 = productFirstPrizePies.Id,
                ProductId2 = productFahrenheit.Id
            },
            new RelatedProduct
            {
                ProductId1 = productFahrenheit.Id,
                ProductId2 = productFirstPrizePies.Id
            },
            new RelatedProduct
            {
                ProductId1 = productFahrenheit.Id,
                ProductId2 = productPrideAndPrejudice.Id
            }
        });
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallJewelryAsync(int productTemplateSimpleId, List<Product> allProducts, List<RelatedProduct> relatedProducts)
    {
        var productElegantGemstoneNecklace = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Elegant Gemstone Necklace (rental)",
            Sku = "EG_GEM_NL",
            ShortDescription = "Classic and elegant gemstone necklace now available in our store",
            FullDescription = "<p>For those who like jewelry, creating their ownelegant jewelry from gemstone beads provides an economical way to incorporate genuine gemstones into your jewelry wardrobe. Manufacturers create beads from all kinds of precious gemstones and semi-precious gemstones, which are available in bead shops, craft stores, and online marketplaces.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "diamond-pave-earrings",
            AllowCustomerReviews = true,
            IsRental = true,
            RentalPriceLength = 1,
            RentalPricePeriod = RentalPricePeriod.Days,
            Price = 30M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Jewelry")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            MarkAsNew = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productElegantGemstoneNecklace);

        await _dataProvider.InsertEntityAsync(productElegantGemstoneNecklace);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productElegantGemstoneNecklace.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Jewelry")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productElegantGemstoneNecklace, "product_GemstoneNecklaces.jpg");

        await InsertProductTagMappingAsync(productElegantGemstoneNecklace, new[] { "jewelry", "awesome" });

        var productFlowerGirlBracelet = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Flower Girl Bracelet",
            Sku = "FL_GIRL_B",
            ShortDescription = "Personalised Flower Braceled",
            FullDescription = "<p>This is a great gift for your flower girl to wear on your wedding day. A delicate bracelet that is made with silver plated soldered cable chain, gives this bracelet a dainty look for young wrist. A Swarovski heart, shown in Rose, hangs off a silver plated flower. Hanging alongside the heart is a silver plated heart charm with Flower Girl engraved on both sides. This is a great style for the younger flower girl.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "diamond-tennis-bracelet",
            AllowCustomerReviews = true,
            Price = 360M,
            IsShipEnabled = true,
            IsFreeShipping = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Jewelry")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productFlowerGirlBracelet);

        await _dataProvider.InsertEntityAsync(productFlowerGirlBracelet);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productFlowerGirlBracelet.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Jewelry")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productFlowerGirlBracelet, "product_FlowerBracelet.jpg");

        await InsertProductTagMappingAsync(productFlowerGirlBracelet, new[] { "awesome", "jewelry" });

        var productEngagementRing = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "Vintage Style Engagement Ring",
            Sku = "VS_ENG_RN",
            ShortDescription = "1.24 Carat (ctw) in 14K White Gold (Certified)",
            FullDescription = "<p>Dazzle her with this gleaming 14 karat white gold vintage proposal. A ravishing collection of 11 decadent diamonds come together to invigorate a superbly ornate gold shank. Total diamond weight on this antique style engagement ring equals 1 1/4 carat (ctw). Item includes diamond certificate.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "vintage-style-three-stone-diamond-engagement-ring",
            AllowCustomerReviews = true,
            Price = 2100M,
            IsShipEnabled = true,
            Weight = 2,
            Length = 2,
            Width = 2,
            Height = 2,
            TaxCategoryId = (await Table<TaxCategory>().SingleAsync(tc => tc.Name == "Jewelry")).Id,
            ManageInventoryMethod = ManageInventoryMethod.ManageStock,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            DisplayStockAvailability = true,
            LowStockActivity = LowStockActivity.DisableBuyButton,
            BackorderMode = BackorderMode.NoBackorders,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(productEngagementRing);

        await _dataProvider.InsertEntityAsync(productEngagementRing);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = productEngagementRing.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Jewelry")).Id,
            DisplayOrder = 1
        });

        await InsertProductPictureAsync(productEngagementRing, "product_EngagementRing_1.jpg");

        await InsertProductTagMappingAsync(productEngagementRing, new[] { "jewelry", "awesome" });

        relatedProducts.AddRange(new[]
        {
            new RelatedProduct
            {
                ProductId1 = productFlowerGirlBracelet.Id,
                ProductId2 = productEngagementRing.Id
            },
            new RelatedProduct
            {
                ProductId1 = productFlowerGirlBracelet.Id,
                ProductId2 = productElegantGemstoneNecklace.Id
            },
            new RelatedProduct
            {
                ProductId1 = productEngagementRing.Id,
                ProductId2 = productFlowerGirlBracelet.Id
            },
            new RelatedProduct
            {
                ProductId1 = productEngagementRing.Id,
                ProductId2 = productElegantGemstoneNecklace.Id
            },
            new RelatedProduct
            {
                ProductId1 = productElegantGemstoneNecklace.Id,
                ProductId2 = productFlowerGirlBracelet.Id
            },
            new RelatedProduct
            {
                ProductId1 = productElegantGemstoneNecklace.Id,
                ProductId2 = productEngagementRing.Id
            }
        });
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallGiftCardsAsync(int productTemplateSimpleId, List<Product> allProducts, List<RelatedProduct> relatedProducts)
    {
        //delivery date
        var deliveryDateId = await GetFirstEntityIdAsync<DeliveryDate>() ?? throw new Exception("No default deliveryDate could be loaded");

        var product25GiftCard = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "$25 Virtual Gift Card",
            Sku = "VG_CR_025",
            ShortDescription = "$25 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible products.",
            FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible products. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "25-virtual-gift-card",
            AllowCustomerReviews = true,
            Price = 25M,
            IsGiftCard = true,
            GiftCardType = GiftCardType.Virtual,
            ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            Published = true,
            ShowOnHomepage = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(product25GiftCard);

        await _dataProvider.InsertEntityAsync(product25GiftCard);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = product25GiftCard.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Gift Cards")).Id,
            DisplayOrder = 2
        });

        await InsertProductPictureAsync(product25GiftCard, "product_25giftcart.jpeg");

        await InsertProductTagMappingAsync(product25GiftCard, new[] { "nice", "gift" });

        var product50GiftCard = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "$50 Physical Gift Card",
            Sku = "PG_CR_050",
            ShortDescription = "$50 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible products.",
            FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible products. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "50-physical-gift-card",
            AllowCustomerReviews = true,
            Price = 50M,
            IsGiftCard = true,
            GiftCardType = GiftCardType.Physical,
            IsShipEnabled = true,
            IsFreeShipping = true,
            DeliveryDateId = deliveryDateId,
            Weight = 1,
            Length = 1,
            Width = 1,
            Height = 1,
            ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            Published = true,
            MarkAsNew = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(product50GiftCard);

        await _dataProvider.InsertEntityAsync(product50GiftCard);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = product50GiftCard.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Gift Cards")).Id,
            DisplayOrder = 3
        });

        await InsertProductPictureAsync(product50GiftCard, "product_50giftcart.jpeg");

        var product100GiftCard = new Product
        {
            ProductType = ProductType.SimpleProduct,
            VisibleIndividually = true,
            Name = "$100 Physical Gift Card",
            Sku = "PG_CR_100",
            ShortDescription = "$100 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible products.",
            FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible products. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
            ProductTemplateId = productTemplateSimpleId,
            //SeName = "100-physical-gift-card",
            AllowCustomerReviews = true,
            Price = 100M,
            IsGiftCard = true,
            GiftCardType = GiftCardType.Physical,
            IsShipEnabled = true,
            DeliveryDateId = deliveryDateId,
            Weight = 1,
            Length = 1,
            Width = 1,
            Height = 1,
            ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
            OrderMinimumQuantity = 1,
            OrderMaximumQuantity = 10000,
            StockQuantity = 10000,
            NotifyAdminForQuantityBelow = 1,
            AllowBackInStockSubscriptions = false,
            Published = true,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        allProducts.Add(product100GiftCard);

        await _dataProvider.InsertEntityAsync(product100GiftCard);

        await _dataProvider.InsertEntityAsync(new ProductCategory
        {
            ProductId = product100GiftCard.Id,
            CategoryId = (await Table<Category>().SingleAsync(c => c.Name == "Gift Cards")).Id,
            DisplayOrder = 4
        });

        await InsertProductPictureAsync(product100GiftCard, "product_100giftcart.jpeg");
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallProductsAsync()
    {
        var productTemplateSimpleId = await GetFirstEntityIdAsync<ProductTemplate>(pt => pt.Name == "Simple product") ?? throw new Exception("Simple product template could not be loaded");
        var productTemplateGroupedId = await GetFirstEntityIdAsync<ProductTemplate>(pt => pt.Name == "Grouped product (with variants)") ?? throw new Exception("Grouped product template could not be loaded");

        //products
        var allProducts = new List<Product>();

        //related products
        var relatedProducts = new List<RelatedProduct>();

        //desktops, notebooks, software
        await InstallComputersAsync(productTemplateSimpleId, allProducts, relatedProducts);
        //camera & photo, cell phones, others
        await InstallElectronicsAsync(productTemplateSimpleId, productTemplateGroupedId, allProducts, relatedProducts);
        //shoes, clothing, accessories
        await InstallApparelAsync(productTemplateSimpleId, allProducts, relatedProducts);
        //digital downloads
        await InstallDigitalDownloadsAsync(productTemplateSimpleId, allProducts, relatedProducts);
        //books
        await InstallBooksAsync(productTemplateSimpleId, allProducts, relatedProducts);
        //jewelry
        await InstallJewelryAsync(productTemplateSimpleId, allProducts, relatedProducts);
        //gift cards
        await InstallGiftCardsAsync(productTemplateSimpleId, allProducts, relatedProducts);

        //search engine names
        foreach (var product in allProducts)
            await _dataProvider.InsertEntityAsync(new UrlRecord
            {
                EntityId = product.Id,
                EntityName = nameof(Product),
                LanguageId = 0,
                IsActive = true,
                Slug = await ValidateSeNameAsync(product, product.Name)
            });

        //related products
        await _dataProvider.BulkInsertEntitiesAsync(relatedProducts);

        //reviews
        using (var random = new SecureRandomNumberGenerator())
            foreach (var product in allProducts)
            {
                if (product.ProductType != ProductType.SimpleProduct)
                    continue;

                //only 3 of 4 products will have reviews
                if (random.Next(4) == 3)
                    continue;

                //rating from 4 to 5
                var rating = random.Next(4, 6);

                await _dataProvider.InsertEntityAsync(new ProductReview
                {
                    CustomerId = await GetDefaultCustomerIdAsync(),
                    ProductId = product.Id,
                    StoreId = await GetDefaultStoreIdAsync(),
                    IsApproved = true,
                    Title = "Some sample review",
                    ReviewText = $"This sample review is for the {product.Name}. I've been waiting for this product to be available. It is priced just right.",
                    //random (4 or 5)
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    CreatedOnUtc = DateTime.UtcNow
                });

                product.ApprovedRatingSum = rating;
                product.ApprovedTotalReviews = 1;
            }

        await _dataProvider.UpdateEntitiesAsync(allProducts);

        //stock quantity history
        foreach (var product in allProducts)
            if (product.StockQuantity > 0)
                await _dataProvider.InsertEntityAsync(new StockQuantityHistory
                {
                    ProductId = product.Id,
                    WarehouseId = product.WarehouseId > 0 ? product.WarehouseId : null,
                    QuantityAdjustment = product.StockQuantity,
                    StockQuantity = product.StockQuantity,
                    Message = "The stock quantity has been edited",
                    CreatedOnUtc = DateTime.UtcNow
                });
    }

    #endregion
}
