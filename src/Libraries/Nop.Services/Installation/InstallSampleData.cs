using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Media;

namespace Nop.Services.Installation;

public partial class InstallationService
{
    #region Fields

    protected int? _defaultLanguageId;
    protected int? _defaultStoreId;
    protected int? _defaultCustomerId;

    #endregion

    #region Utilities

    /// <summary>
    /// Gets default language identifier
    /// </summary>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains the identifier of default language</returns>
    protected virtual async Task<int> GetDefaultLanguageIdAsync()
    {
        if (_defaultLanguageId.HasValue)
            return _defaultLanguageId.Value;

        var lang = await Table<Language>().FirstOrDefaultAsync() ?? throw new Exception("Default language could not be loaded");

        _defaultLanguageId = lang.Id;

        return lang.Id;
    }

    /// <summary>
    /// Gets default store identifier
    /// </summary>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains the identifier of default store</returns>
    protected virtual async Task<int> GetDefaultStoreIdAsync()
    {
        if (_defaultStoreId.HasValue)
            return _defaultStoreId.Value;

        var store = await Table<Store>().FirstOrDefaultAsync() ?? throw new Exception("No default store could be loaded");

        _defaultStoreId = store.Id;

        return store.Id;
    }

    /// <summary>
    /// Gets default customer identifier
    /// </summary>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains the identifier of default customer</returns>
    protected virtual async Task<int> GetDefaultCustomerIdAsync()
    {
        if (_defaultCustomerId.HasValue)
            return _defaultCustomerId.Value;

        var customer = await Table<Customer>().FirstOrDefaultAsync(x => x.Email == _defaultCustomerEmail) ?? throw new Exception("Cannot load default customer");

        _defaultCustomerId = customer.Id;

        return customer.Id;
    }

    /// <summary>
    /// Inserts search engine names fore entities
    /// </summary>
    /// <param name="entities">Entities fore insert search engine names</param>
    /// <param name="getName">Function to getting the name fore creating the slug</param>
    /// <param name="languageId">The language identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task InsertSearchEngineNamesAsync<TEntity>(IEnumerable<TEntity> entities, Func<TEntity, string> getName, int languageId=0) where TEntity : BaseEntity
    {
        await _dataProvider.BulkInsertEntitiesAsync(await entities.SelectAwait(async entity => new UrlRecord
        {
            EntityId = entity.Id,
            EntityName = typeof(TEntity).Name,
            LanguageId = languageId,
            IsActive = true,
            Slug = await ValidateSeNameAsync(entity, getName(entity))
        }).ToListAsync());
    }

    /// <summary>
    /// Inserts a picture
    /// </summary>
    /// <param name="fileName">Picture file name</param>
    /// <param name="name">Picture name to create the SE name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the identifier of inserted picture
    /// </returns>
    protected virtual async Task<int> InsertPictureAsync(string fileName, string name)
    {
        var sampleImagesPath = _fileProvider.GetAbsolutePath(NopInstallationDefaults.SampleImagesPath);

        //TODO: avoid using service
        var pictureService = EngineContext.Current.Resolve<IPictureService>();
        var pictureBinary = await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, fileName));
        var seName = await pictureService.GetPictureSeNameAsync(name);

        var picture = await pictureService.InsertPictureAsync(pictureBinary, MimeTypes.ImageJpeg, seName);

        return picture?.Id ?? 0;
    }

    /// <summary>
    /// Installs a sample customers
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    protected virtual async Task InstallSampleCustomersAsync()
    {
        var crRegistered = await Table<CustomerRole>()
            .FirstOrDefaultAsync(customerRole => customerRole.SystemName == NopCustomerDefaults.RegisteredRoleName);

        ArgumentNullException.ThrowIfNull(crRegistered);

        //default store 
        var defaultStore = await Table<Store>().FirstOrDefaultAsync() ?? throw new Exception("No default store could be loaded");

        var storeId = defaultStore.Id;

        Customer createCustomer(Address address)
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = address.Email,
                Username = address.Email,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId,
                BillingAddressId = address.Id,
                ShippingAddressId = address.Id,
                FirstName = address.FirstName,
                LastName = address.LastName
            };
        }

        var addresses = new[]
        {
            new Address
            {
                FirstName = "Steve",
                LastName = "Gates",
                PhoneNumber = "87654321",
                Email = "steve_gates@nopCommerce.com",
                FaxNumber = string.Empty,
                Company = "Steve Company",
                Address1 = "750 Bel Air Rd.",
                Address2 = string.Empty,
                City = "Los Angeles",
                StateProvinceId = await GetFirstEntityIdAsync<StateProvince>(sp => sp.Name == "California"),
                CountryId = await GetFirstEntityIdAsync<Country>(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "90077",
                CreatedOnUtc = DateTime.UtcNow
            },
            new Address
            {
                FirstName = "Arthur",
                LastName = "Holmes",
                PhoneNumber = "111222333",
                Email = "arthur_holmes@nopCommerce.com",
                FaxNumber = string.Empty,
                Company = "Holmes Company",
                Address1 = "221B Baker Street",
                Address2 = string.Empty,
                City = "London",
                CountryId = await GetFirstEntityIdAsync<Country>(c => c.ThreeLetterIsoCode == "GBR"),
                ZipPostalCode = "NW1 6XE",
                CreatedOnUtc = DateTime.UtcNow
            },
            new Address
            {
                FirstName = "James",
                LastName = "Pan",
                PhoneNumber = "369258147",
                Email = "james_pan@nopCommerce.com",
                FaxNumber = string.Empty,
                Company = "Pan Company",
                Address1 = "St Katharine’s West 16",
                Address2 = string.Empty,
                City = "St Andrews",
                CountryId = await GetFirstEntityIdAsync<Country>(c => c.ThreeLetterIsoCode == "GBR"),
                ZipPostalCode = "KY16 9AX",
                CreatedOnUtc = DateTime.UtcNow
            },
            new Address
            {
                FirstName = "Brenda",
                LastName = "Lindgren",
                PhoneNumber = "14785236",
                Email = "brenda_lindgren@nopCommerce.com",
                FaxNumber = string.Empty,
                Company = "Brenda Company",
                Address1 = "1249 Tongass Avenue, Suite B",
                Address2 = string.Empty,
                City = "Ketchikan",
                StateProvinceId = await GetFirstEntityIdAsync<StateProvince>(sp => sp.Name == "Alaska"),
                CountryId = await GetFirstEntityIdAsync<Country>(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "99901",
                CreatedOnUtc = DateTime.UtcNow
            },
            new Address
            {
                FirstName = "Victoria",
                LastName = "Terces",
                PhoneNumber = "45612378",
                Email = "victoria_victoria@nopCommerce.com",
                FaxNumber = string.Empty,
                Company = "Terces Company",
                Address1 = "201 1st Avenue South",
                Address2 = string.Empty,
                City = "Saskatoon",
                StateProvinceId = await GetFirstEntityIdAsync<StateProvince>(sp => sp.Name == "Saskatchewan"),
                CountryId = await GetFirstEntityIdAsync<Country>(c => c.ThreeLetterIsoCode == "CAN"),
                ZipPostalCode = "S7K 1J9",
                CreatedOnUtc = DateTime.UtcNow
            }
        };
        await _dataProvider.BulkInsertEntitiesAsync(addresses);

        var customers = addresses.Select(createCustomer).ToList();
        await _dataProvider.BulkInsertEntitiesAsync(customers);

        await _dataProvider.BulkInsertEntitiesAsync(customers.Select(customer => new CustomerAddressMapping
        {
            CustomerId = customer.Id,
            AddressId = customer.BillingAddressId ?? 0
        }));

        await _dataProvider.BulkInsertEntitiesAsync(customers.Select(customer => new CustomerCustomerRoleMapping
        {
            CustomerId = customer.Id,
            CustomerRoleId = crRegistered.Id
        }));

        await _dataProvider.BulkInsertEntitiesAsync(customers.Select(customer => new CustomerPassword
        {
            CustomerId = customer.Id,
            Password = "123456",
            PasswordFormat = PasswordFormat.Clear,
            PasswordSalt = string.Empty,
            CreatedOnUtc = DateTime.UtcNow
        }));
    }

    /// <summary>
    /// Installs a sample checkout attributes
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallCheckoutAttributesAsync()
    {
        var checkoutAttribute = await _dataProvider.InsertEntityAsync(new CheckoutAttribute
        {
            Name = "Gift wrapping",
            IsRequired = true,
            ShippableProductRequired = true,
            AttributeControlType = AttributeControlType.DropdownList,
            DisplayOrder = 1
        });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new CheckoutAttributeValue
            {
                Name = "No",
                PriceAdjustment = 0,
                DisplayOrder = 1,
                IsPreSelected = true,
                AttributeId = checkoutAttribute.Id
            },
            new CheckoutAttributeValue
            {
                Name = "Yes",
                PriceAdjustment = 10,
                DisplayOrder = 2,
                AttributeId = checkoutAttribute.Id
            }});
    }

    /// <summary>
    /// Installs a sample specification attributes
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallSpecificationAttributesAsync()
    {
        var specificationAttributeGroup = await _dataProvider.InsertEntityAsync(
            new SpecificationAttributeGroup
            {
                Name = "System unit"
            });

        var specificationAttribute1 = new SpecificationAttribute
        {
            Name = "Screensize",
            DisplayOrder = 1
        };

        var specificationAttribute2 = new SpecificationAttribute
        {
            Name = "CPU Type",
            DisplayOrder = 2,
            SpecificationAttributeGroupId = specificationAttributeGroup.Id
        };

        var specificationAttribute3 = new SpecificationAttribute
        {
            Name = "Memory",
            DisplayOrder = 3,
            SpecificationAttributeGroupId = specificationAttributeGroup.Id
        };

        var specificationAttribute4 = new SpecificationAttribute
        {
            Name = "Hard drive",
            DisplayOrder = 5,
            SpecificationAttributeGroupId = specificationAttributeGroup.Id
        };

        var specificationAttribute5 = new SpecificationAttribute
        {
            Name = "Color",
            DisplayOrder = 1
        };

        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            specificationAttribute1,
            specificationAttribute2,
            specificationAttribute3,
            specificationAttribute4,
            specificationAttribute5
        });

        await _dataProvider.BulkInsertEntitiesAsync(new[]{
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute1.Id,
                Name = "13.0''",
                DisplayOrder = 2
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute1.Id,
                Name = "13.3''",
                DisplayOrder = 3
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute1.Id,
                Name = "14.0''",
                DisplayOrder = 4
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute1.Id,
                Name = "15.0''",
                DisplayOrder = 4
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute1.Id,
                Name = "15.6''",
                DisplayOrder = 5
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute2.Id,
                Name = "Intel Core i5",
                DisplayOrder = 1
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute2.Id,
                Name = "Intel Core i7",
                DisplayOrder = 2
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute3.Id,
                Name = "4 GB",
                DisplayOrder = 1
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute3.Id,
                Name = "8 GB",
                DisplayOrder = 2
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute3.Id,
                Name = "16 GB",
                DisplayOrder = 3
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute4.Id,
                Name = "128 GB",
                DisplayOrder = 7
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute4.Id,
                Name = "500 GB",
                DisplayOrder = 4
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute4.Id,
                Name = "1 TB",
                DisplayOrder = 3
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute5.Id,
                Name = "Grey",
                DisplayOrder = 2,
                ColorSquaresRgb = "#8a97a8"
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute5.Id,
                Name = "Red",
                DisplayOrder = 3,
                ColorSquaresRgb = "#8a374a"
            },
            new SpecificationAttributeOption
            {
                SpecificationAttributeId = specificationAttribute5.Id,
                Name = "Blue",
                DisplayOrder = 4,
                ColorSquaresRgb = "#47476f"
            }});
    }

    /// <summary>
    /// Installs a sample product attributes
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallProductAttributesAsync()
    {
        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new ProductAttribute { Name = "Color" },
            new ProductAttribute { Name = "Print" },
            new ProductAttribute { Name = "Custom Text" },
            new ProductAttribute { Name = "HDD" },
            new ProductAttribute { Name = "OS" },
            new ProductAttribute { Name = "Processor" },
            new ProductAttribute { Name = "RAM" },
            new ProductAttribute { Name = "Size" },
            new ProductAttribute { Name = "Software" }
        });
    }
    
    /// <summary>
    /// Installs a sample categories
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallCategoriesAsync()
    {
        var categoryTemplateInGridAndLines = await Table<CategoryTemplate>().FirstOrDefaultAsync(pt => pt.Name == "Products in Grid or Lines") ?? throw new Exception("Category template cannot be loaded");

        async Task<Category> createCategory(string name, string imageFileName, int displayOrder, bool priceRangeFiltering = true, int parentCategoryId = 0, bool showOnHomepage = false)
        {
            var category = new Category
            {
                Name = name,
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = parentCategoryId,
                PictureId = await InsertPictureAsync(imageFileName, name),
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = displayOrder,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ShowOnHomepage = showOnHomepage
            };

            if (!priceRangeFiltering)
                return category;

            category.PriceRangeFiltering = true;
            category.ManuallyPriceRange = true;
            category.PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
            category.PriceTo = NopCatalogDefaults.DefaultPriceRangeTo;

            return category;
        }

        var categoryComputers = await _dataProvider.InsertEntityAsync(await createCategory("Computers", "category_computers.jpeg", 1, false));

        var categoryDesktops = await createCategory("Desktops", "category_desktops.jpg", 1, parentCategoryId: categoryComputers.Id);
        var categoryNotebooks = await createCategory("Notebooks", "category_notebooks.jpg", 2, false, categoryComputers.Id);
        var categorySoftware = await createCategory("Software", "category_software.jpg", 3, false, categoryComputers.Id);

        await _dataProvider.BulkInsertEntitiesAsync(new[] { categoryDesktops, categoryNotebooks, categorySoftware });

        var categoryElectronics = await _dataProvider.InsertEntityAsync(await createCategory("Electronics", "category_electronics.jpeg", 2, false, showOnHomepage: true));

        var categoryCameraPhoto = await createCategory("Camera & photo", "category_camera_photo.jpeg", 1, parentCategoryId: categoryElectronics.Id);
        var categoryCellPhones = await createCategory("Cell phones", "category_cell_phones.jpeg", 2, false, categoryElectronics.Id);
        var categoryOthers = await createCategory("Others", "category_accessories.jpg", 3, parentCategoryId: categoryElectronics.Id);

        await _dataProvider.BulkInsertEntitiesAsync(new[] { categoryCameraPhoto, categoryCellPhones, categoryOthers });

        var categoryApparel = await _dataProvider.InsertEntityAsync(await createCategory("Apparel", "category_apparel.jpeg", 3, false, showOnHomepage: true));

        var categoryShoes = await createCategory("Shoes", "category_shoes.jpeg", 1, parentCategoryId: categoryApparel.Id);
        var categoryClothing = await createCategory("Clothing", "category_clothing.jpeg", 2, false, categoryApparel.Id);
        var categoryAccessories = await createCategory("Accessories", "category_apparel_accessories.jpg", 3, parentCategoryId: categoryApparel.Id);

        await _dataProvider.BulkInsertEntitiesAsync(new[] { categoryShoes, categoryClothing, categoryAccessories });

        var categoryDigitalDownloads = await createCategory("Digital downloads", "category_digital_downloads.jpeg", 4, false, showOnHomepage: true);
        var categoryBooks = await createCategory("Books", "category_book.jpeg", 5);
        var categoryJewelry = await createCategory("Jewelry", "category_jewelry.jpeg", 6);
        var categoryGiftCards = await createCategory("Gift Cards", "category_gift_cards.jpeg", 7, false);

        await _dataProvider.BulkInsertEntitiesAsync(new[] { categoryDigitalDownloads, categoryBooks, categoryJewelry, categoryGiftCards });

        //search engine names
        await InsertSearchEngineNamesAsync(
            new[]
            {
                categoryComputers, categoryDesktops, categoryNotebooks, categorySoftware, categoryElectronics,
                categoryCameraPhoto, categoryCellPhones, categoryOthers, categoryApparel, categoryShoes,
                categoryClothing, categoryAccessories, categoryDigitalDownloads, categoryBooks, categoryJewelry,
                categoryGiftCards
            }, category => category.Name);
    }

    /// <summary>
    /// Installs a sample manufacturers
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallManufacturersAsync()
    {
        var manufacturerTemplateInGridAndLines = await Table<ManufacturerTemplate>()
            .FirstOrDefaultAsync(pt => pt.Name == "Products in Grid or Lines") ?? throw new Exception("Manufacturer template cannot be loaded");

        async Task<Manufacturer> createManufacturer(string name, string imageFileName, int displayOrder, bool priceRangeFiltering = true)
        {
            var manufacturer = new Manufacturer
            {
                Name = name,
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                Published = true,
                PictureId = await InsertPictureAsync(imageFileName, name),
                DisplayOrder = displayOrder,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            if (!priceRangeFiltering)
                return manufacturer;

            manufacturer.PriceRangeFiltering = true;
            manufacturer.ManuallyPriceRange = true;
            manufacturer.PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
            manufacturer.PriceTo = NopCatalogDefaults.DefaultPriceRangeTo;

            return manufacturer;
        }

        var allManufacturers = new[]
        {
            await createManufacturer("Apple", "manufacturer_apple.jpg", 1),
            await createManufacturer("HP", "manufacturer_hp.jpg", 5),
            await createManufacturer("Nike", "manufacturer_nike.jpg", 5, false)
        };

        await _dataProvider.BulkInsertEntitiesAsync(allManufacturers);

        //search engine names
        await _dataProvider.BulkInsertEntitiesAsync(await allManufacturers.SelectAwait(async manufacturer => new UrlRecord
        {
            EntityId = manufacturer.Id,
            EntityName = nameof(Manufacturer),
            LanguageId = 0,
            IsActive = true,
            Slug = await ValidateSeNameAsync(manufacturer, manufacturer.Name)
        }).ToListAsync());
    }

    /// <summary>
    /// Installs a sample forums
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallForumsAsync()
    {
        var forumGroup = await _dataProvider.InsertEntityAsync(new ForumGroup
        {
            Name = "General",
            DisplayOrder = 5,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        });

        Forum createForum(string name, string description, int displayOrder)
        {
            return new Forum
            {
                ForumGroupId = forumGroup.Id,
                Name = name,
                Description = description,
                NumTopics = 0,
                NumPosts = 0,
                LastPostCustomerId = 0,
                LastPostTime = null,
                DisplayOrder = displayOrder,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
        }

        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            createForum("New Products", "Discuss new products and industry trends", 1),
            createForum("Mobile Devices Forum", "Discuss the mobile phone market", 10),
            createForum("Packaging & Shipping", "Discuss packaging & shipping", 20),
        });
    }

    /// <summary>
    /// Installs a sample discounts
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallDiscountsAsync()
    {
        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new Discount
            {
                IsActive = true,
                Name = "Sample discount with coupon code",
                DiscountType = DiscountType.AssignedToSkus,
                DiscountLimitation = DiscountLimitationType.Unlimited,
                UsePercentage = false,
                DiscountAmount = 10,
                RequiresCouponCode = true,
                CouponCode = "123"
            },
            new Discount
            {
                IsActive = true,
                Name = "'20% order total' discount",
                DiscountType = DiscountType.AssignedToOrderTotal,
                DiscountLimitation = DiscountLimitationType.Unlimited,
                UsePercentage = true,
                DiscountPercentage = 20,
                StartDateUtc = new DateTime(2010, 1, 1),
                EndDateUtc = new DateTime(2020, 1, 1),
                RequiresCouponCode = true,
                CouponCode = "456"
            }
        });
    }

    /// <summary>
    /// Installs a sample blog posts
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallBlogPostsAsync()
    {
        var blogPosts = new List<BlogPost>
        {
            new()
            {
                AllowComments = true,
                LanguageId = await GetDefaultLanguageIdAsync(),
                Title = "How a blog can help your growing e-Commerce business",
                BodyOverview = "<p>When you start an online business, your main aim is to sell the products, right? As a business owner, you want to showcase your store to more audience. So, you decide to go on social media, why? Because everyone is doing it, then why shouldn&rsquo;t you? It is tempting as everyone is aware of the hype that it is the best way to market your brand.</p><p>Do you know having a blog for your online store can be very helpful? Many businesses do not understand the importance of having a blog because they don&rsquo;t have time to post quality content.</p><p>Today, we will talk about how a blog can play an important role for the growth of your e-Commerce business. Later, we will also discuss some tips that will be helpful to you for writing business related blog posts.</p>",
                Body = "<p>When you start an online business, your main aim is to sell the products, right? As a business owner, you want to showcase your store to more audience. So, you decide to go on social media, why? Because everyone is doing it, then why shouldn&rsquo;t you? It is tempting as everyone is aware of the hype that it is the best way to market your brand.</p><p>Do you know having a blog for your online store can be very helpful? Many businesses do not understand the importance of having a blog because they don&rsquo;t have time to post quality content.</p><p>Today, we will talk about how a blog can play an important role for the growth of your e-Commerce business. Later, we will also discuss some tips that will be helpful to you for writing business related blog posts.</p><h3>1) Blog is useful in educating your customers</h3><p>Blogging is one of the best way by which you can educate your customers about your products/services that you offer. This helps you as a business owner to bring more value to your brand. When you provide useful information to the customers about your products, they are more likely to buy products from you. You can use your blog for providing tutorials in regard to the use of your products.</p><p><strong>For example:</strong> If you have an online store that offers computer parts. You can write tutorials about how to build a computer or how to make your computer&rsquo;s performance better. While talking about these things, you can mention products in the tutorials and provide link to your products within the blog post from your website. Your potential customers might get different ideas of using your product and will likely to buy products from your online store.</p><h3>2) Blog helps your business in Search Engine Optimization (SEO)</h3><p>Blog posts create more internal links to your website which helps a lot in SEO. Blog is a great way to have quality content on your website related to your products/services which is indexed by all major search engines like Google, Bing and Yahoo. The more original content you write in your blog post, the better ranking you will get in search engines. SEO is an on-going process and posting blog posts regularly keeps your site active all the time which is beneficial when it comes to search engine optimization.</p><p><strong>For example:</strong> Let&rsquo;s say you sell &ldquo;Sony Television Model XYZ&rdquo; and you regularly publish blog posts about your product. Now, whenever someone searches for &ldquo;Sony Television Model XYZ&rdquo;, Google will crawl on your website knowing that you have something to do with this particular product. Hence, your website will show up on the search result page whenever this item is being searched.</p><h3>3) Blog helps in boosting your sales by convincing the potential customers to buy</h3><p>If you own an online business, there are so many ways you can share different stories with your audience in regard your products/services that you offer. Talk about how you started your business, share stories that educate your audience about what&rsquo;s new in your industry, share stories about how your product/service was beneficial to someone or share anything that you think your audience might find interesting (it does not have to be related to your product). This kind of blogging shows that you are an expert in your industry and interested in educating your audience. It sets you apart in the competitive market. This gives you an opportunity to showcase your expertise by educating the visitors and it can turn your audience into buyers.</p><p><strong>Fun Fact:</strong> Did you know that 92% of companies who decided to blog acquired customers through their blog?</p><p><a href=\"https://www.nopcommerce.com/\">nopCommerce</a> is great e-Commerce solution that also offers a variety of CMS features including blog. A store owner has full access for managing the blog posts and related comments.</p>",
                Tags = "e-commerce, blog, moey",
                CreatedOnUtc = DateTime.UtcNow
            },
            new()
            {
                AllowComments = true,
                LanguageId = await GetDefaultLanguageIdAsync(),
                Title = "Why your online store needs a wish list",
                BodyOverview = "<p>What comes to your mind, when you hear the term&rdquo; wish list&rdquo;? The application of this feature is exactly how it sounds like: a list of things that you wish to get. As an online store owner, would you like your customers to be able to save products in a wish list so that they review or buy them later? Would you like your customers to be able to share their wish list with friends and family for gift giving?</p><p>Offering your customers a feature of wish list as part of shopping cart is a great way to build loyalty to your store site. Having the feature of wish list on a store site allows online businesses to engage with their customers in a smart way as it allows the shoppers to create a list of what they desire and their preferences for future purchase.</p>",
                Body = "<p>What comes to your mind, when you hear the term&rdquo; wish list&rdquo;? The application of this feature is exactly how it sounds like: a list of things that you wish to get. As an online store owner, would you like your customers to be able to save products in a wish list so that they review or buy them later? Would you like your customers to be able to share their wish list with friends and family for gift giving?</p><p>Offering your customers a feature of wish list as part of shopping cart is a great way to build loyalty to your store site. Having the feature of wish list on a store site allows online businesses to engage with their customers in a smart way as it allows the shoppers to create a list of what they desire and their preferences for future purchase.</p><p>Does every e-Commerce store needs a wish list? The answer to this question in most cases is yes, because of the following reasons:</p><p><strong>Understanding the needs of your customers</strong> - A wish list is a great way to know what is in your customer&rsquo;s mind. Try to think the purchase history as a small portion of the customer&rsquo;s preferences. But, the wish list is like a wide open door that can give any online business a lot of valuable information about their customer and what they like or desire.</p><p><strong>Shoppers like to share their wish list with friends and family</strong> - Providing your customers a way to email their wish list to their friends and family is a pleasant way to make online shopping enjoyable for the shoppers. It is always a good idea to make the wish list sharable by a unique link so that it can be easily shared though different channels like email or on social media sites.</p><p><strong>Wish list can be a great marketing tool</strong> &ndash; Another way to look at wish list is a great marketing tool because it is extremely targeted and the recipients are always motivated to use it. For example: when your younger brother tells you that his wish list is on a certain e-Commerce store. What is the first thing you are going to do? You are most likely to visit the e-Commerce store, check out the wish list and end up buying something for your younger brother.</p><p>So, how a wish list is a marketing tool? The reason is quite simple, it introduce your online store to new customers just how it is explained in the above example.</p><p><strong>Encourage customers to return to the store site</strong> &ndash; Having a feature of wish list on the store site can increase the return traffic because it encourages customers to come back and buy later. Allowing the customers to save the wish list to their online accounts gives them a reason return to the store site and login to the account at any time to view or edit the wish list items.</p><p><strong>Wish list can be used for gifts for different occasions like weddings or birthdays. So, what kind of benefits a gift-giver gets from a wish list?</strong></p><ul><li>It gives them a surety that they didn&rsquo;t buy a wrong gift</li><li>It guarantees that the recipient will like the gift</li><li>It avoids any awkward moments when the recipient unwraps the gift and as a gift-giver you got something that the recipient do not want</li></ul><p><strong>Wish list is a great feature to have on a store site &ndash; So, what kind of benefits a business owner gets from a wish list</strong></p><ul><li>It is a great way to advertise an online store as many people do prefer to shop where their friend or family shop online</li><li>It allows the current customers to return to the store site and open doors for the new customers</li><li>It allows store admins to track what&rsquo;s in customers wish list and run promotions accordingly to target specific customer segments</li></ul><p><a href=\"https://www.nopcommerce.com/\">nopCommerce</a> offers the feature of wish list that allows customers to create a list of products that they desire or planning to buy in future.</p>",
                Tags = "e-commerce, nopCommerce, sample tag, money",
                CreatedOnUtc = DateTime.UtcNow.AddSeconds(1)
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(blogPosts);
        await InsertSearchEngineNamesAsync(blogPosts, blogPost => blogPost.Title, await GetDefaultLanguageIdAsync());
        
        await _dataProvider.BulkInsertEntitiesAsync(await blogPosts.SelectAwait(async blogPost => new BlogComment
        {
            BlogPostId = blogPost.Id,
            CustomerId = await GetDefaultCustomerIdAsync(),
            CommentText = "This is a sample comment for this blog post",
            IsApproved = true,
            StoreId = await GetDefaultStoreIdAsync(),
            CreatedOnUtc = DateTime.UtcNow
        }).ToListAsync());
    }

    /// <summary>
    /// Installs a sample news
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallNewsAsync()
    {
        var news = new List<NewsItem>
        {
            new()
            {
                AllowComments = true,
                LanguageId = await GetDefaultLanguageIdAsync(),
                Title = "About nopCommerce",
                Short = "It's stable and highly usable. From downloads to documentation, www.nopCommerce.com offers a comprehensive base of information, resources, and support to the nopCommerce community.",
                Full = "<p>For full feature list go to <a href=\"https://www.nopCommerce.com\">nopCommerce.com</a></p><p>Providing outstanding custom search engine optimization, web development services and e-commerce development solutions to our clients at a fair price in a professional manner.</p>",
                Published = true,
                CreatedOnUtc = DateTime.UtcNow
            },
            new()
            {
                AllowComments = true,
                LanguageId = await GetDefaultLanguageIdAsync(),
                Title = "nopCommerce new release!",
                Short = "nopCommerce includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included! nopCommerce is a fully customizable shopping cart",
                Full = "<p>nopCommerce includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included!</p>",
                Published = true,
                CreatedOnUtc = DateTime.UtcNow.AddSeconds(1)
            },
            new()
            {
                AllowComments = true,
                LanguageId = await GetDefaultLanguageIdAsync(),
                Title = "New online store is open!",
                Short = "The new nopCommerce store is open now! We are very excited to offer our new range of products. We will be constantly adding to our range so please register on our site.",
                Full = "<p>Our online store is officially up and running. Stock up for the holiday season! We have a great selection of items. We will be constantly adding to our range so please register on our site, this will enable you to keep up to date with any new products.</p><p>All shipping is worldwide and will leave the same day an order is placed! Happy Shopping and spread the word!!</p>",
                Published = true,
                CreatedOnUtc = DateTime.UtcNow.AddSeconds(2)
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(news);
        await InsertSearchEngineNamesAsync(news, newsItem => newsItem.Title, await GetDefaultLanguageIdAsync());

        await _dataProvider.BulkInsertEntitiesAsync(await news.SelectAwait(async newsItem => new NewsComment
        {
            NewsItemId = newsItem.Id,
            CustomerId = await GetDefaultCustomerIdAsync(),
            CommentTitle = "Sample comment title",
            CommentText = "This is a sample comment...",
            IsApproved = true,
            StoreId = await GetDefaultStoreIdAsync(),
            CreatedOnUtc = DateTime.UtcNow
        }).ToListAsync());
    }

    /// <summary>
    /// Installs a sample polls
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallPollsAsync()
    {
        var poll = await _dataProvider.InsertEntityAsync(new Poll
        {
            LanguageId = await GetDefaultLanguageIdAsync(),
            Name = "Do you like nopCommerce?",
            SystemKeyword = string.Empty,
            Published = true,
            ShowOnHomepage = true,
            DisplayOrder = 1
        });
        
        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new PollAnswer { Name = "Excellent", DisplayOrder = 1, PollId = poll.Id },
            new PollAnswer { Name = "Good", DisplayOrder = 2, PollId = poll.Id },
            new PollAnswer { Name = "Poor", DisplayOrder = 3, PollId = poll.Id },
            new PollAnswer { Name = "Very bad", DisplayOrder = 4, PollId = poll.Id }
        });
    }

    /// <summary>
    /// Installs a sample warehouses
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallWarehousesAsync()
    {
        var address1 = new Address
        {
            Address1 = "21 West 52nd Street",
            City = "New York",
            StateProvinceId = await GetFirstEntityIdAsync<StateProvince>(sp => sp.Name == "New York"),
            CountryId = await GetFirstEntityIdAsync<Country>(c => c.ThreeLetterIsoCode == "USA"),
            ZipPostalCode = "10021",
            CreatedOnUtc = DateTime.UtcNow
        };

        var address2 = new Address
        {
            Address1 = "300 South Spring Stree",
            City = "Los Angeles",
            StateProvinceId = await GetFirstEntityIdAsync<StateProvince>(sp => sp.Name == "California"),
            CountryId = await GetFirstEntityIdAsync<Country>(c => c.ThreeLetterIsoCode == "USA"),
            ZipPostalCode = "90013",
            CreatedOnUtc = DateTime.UtcNow
        };

        await _dataProvider.BulkInsertEntitiesAsync(new[] { address1, address2 });
        
        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new Warehouse { Name = "Warehouse 1 (New York)", AddressId = address1.Id },
            new Warehouse { Name = "Warehouse 2 (Los Angeles)", AddressId = address2.Id }
        });
    }

    /// <summary>
    /// Installs a sample vendors
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallVendorsAsync()
    {
        var vendors = new List<Vendor>
        {
            new()
            {
                Name = "Vendor 1",
                Email = "vendor1email@gmail.com",
                Description = "Some description...",
                AdminComment = string.Empty,
                PictureId = 0,
                Active = true,
                DisplayOrder = 1,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9, 18",
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = NopCatalogDefaults.DefaultPriceRangeTo,
            },
            new()
            {
                Name = "Vendor 2",
                Email = "vendor2email@gmail.com",
                Description = "Some description...",
                AdminComment = string.Empty,
                PictureId = 0,
                Active = true,
                DisplayOrder = 2,
                PageSize = 6,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9, 18"
            }
        };

        await _dataProvider.BulkInsertEntitiesAsync(vendors);
        await InsertSearchEngineNamesAsync(vendors, vendor => vendor.Name);
    }

    /// <summary>
    /// Installs a sample affiliates
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallAffiliatesAsync()
    {
        var affiliateAddress = await _dataProvider.InsertEntityAsync(new Address
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "affiliate_email@gmail.com",
            Company = "Company name here...",
            City = "New York",
            Address1 = "21 West 52nd Street",
            ZipPostalCode = "10021",
            PhoneNumber = "123456789",
            StateProvinceId = await GetFirstEntityIdAsync<StateProvince>(sp => sp.Name == "New York"),
            CountryId = await GetFirstEntityIdAsync<Country>(c => c.ThreeLetterIsoCode == "USA"),
            CreatedOnUtc = DateTime.UtcNow
        });
        
        await _dataProvider.InsertEntityAsync(new Affiliate
        {
            Active = true,
            AddressId = affiliateAddress.Id
        });
    }

    /// <summary>
    /// Installs a sample orders
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallOrdersAsync()
    {
        static Address cloneAddress(Address address)
        {
            return new Address
            {
                FirstName = address.FirstName,
                LastName = address.LastName,
                Email = address.Email,
                Company = address.Company,
                CountryId = address.CountryId,
                StateProvinceId = address.StateProvinceId,
                County = address.County,
                City = address.City,
                Address1 = address.Address1,
                Address2 = address.Address2,
                ZipPostalCode = address.ZipPostalCode,
                PhoneNumber = address.PhoneNumber,
                FaxNumber = address.FaxNumber,
                CustomAttributes = address.CustomAttributes,
                CreatedOnUtc = address.CreatedOnUtc
            };
        }

        //default store
        var defaultStoreId = await GetFirstEntityIdAsync<Store>() ?? throw new Exception("No default store could be loaded");

        #region Customers

        var customers = Table<Customer>().AsEnumerable().GroupBy(p => p.Email, p => p).ToDictionary(p => p.Key, p => p.First());

        var firstCustomer = customers["steve_gates@nopCommerce.com"];
        var secondCustomer = customers["arthur_holmes@nopCommerce.com"];
        var thirdCustomer = customers["james_pan@nopCommerce.com"];
        var fourthCustomer = customers["brenda_lindgren@nopCommerce.com"];
        var fifthCustomer = customers["victoria_victoria@nopCommerce.com"];

        #endregion

        #region Addresses

        var firstCustomerBillingAddress = cloneAddress(await GetByIdAsync<Address>(firstCustomer.BillingAddressId));
        var firstCustomerShippingAddress = cloneAddress(await GetByIdAsync<Address>(firstCustomer.ShippingAddressId));
        var secondCustomerBillingAddress = cloneAddress(await GetByIdAsync<Address>(secondCustomer.BillingAddressId));
        var secondCustomerShippingAddress = cloneAddress(await GetByIdAsync<Address>(secondCustomer.ShippingAddressId));
        var thirdCustomerBillingAddress = cloneAddress(await GetByIdAsync<Address>(thirdCustomer.BillingAddressId));
        var fourthCustomerBillingAddress = cloneAddress(await GetByIdAsync<Address>(fourthCustomer.BillingAddressId));
        var fourthCustomerShippingAddress = cloneAddress(await GetByIdAsync<Address>(fourthCustomer.ShippingAddressId));
        var fourthCustomerPickupAddress = cloneAddress(await GetByIdAsync<Address>(fourthCustomer.ShippingAddressId));
        var fifthCustomerBillingAddress = cloneAddress(await GetByIdAsync<Address>(fifthCustomer.BillingAddressId));
        var fifthCustomerShippingAddress = cloneAddress(await GetByIdAsync<Address>(fifthCustomer.ShippingAddressId));

        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            firstCustomerBillingAddress, firstCustomerShippingAddress, secondCustomerBillingAddress,
            secondCustomerShippingAddress, thirdCustomerBillingAddress, fourthCustomerBillingAddress,
            fourthCustomerShippingAddress, fourthCustomerPickupAddress, fifthCustomerBillingAddress,
            fifthCustomerShippingAddress
        });

        #endregion

        #region Orders
        
        var firstOrder = new Order
        {
            StoreId = defaultStoreId,
            OrderGuid = Guid.NewGuid(),
            CustomerId = firstCustomer.Id,
            CustomerLanguageId = await GetDefaultLanguageIdAsync(),
            CustomerIp = "127.0.0.1",
            OrderSubtotalInclTax = 1855M,
            OrderSubtotalExclTax = 1855M,
            OrderSubTotalDiscountInclTax = decimal.Zero,
            OrderSubTotalDiscountExclTax = decimal.Zero,
            OrderShippingInclTax = decimal.Zero,
            OrderShippingExclTax = decimal.Zero,
            PaymentMethodAdditionalFeeInclTax = decimal.Zero,
            PaymentMethodAdditionalFeeExclTax = decimal.Zero,
            TaxRates = "0:0;",
            OrderTax = decimal.Zero,
            OrderTotal = 1855M,
            RefundedAmount = decimal.Zero,
            OrderDiscount = decimal.Zero,
            CheckoutAttributeDescription = string.Empty,
            CheckoutAttributesXml = string.Empty,
            CustomerCurrencyCode = "USD",
            CurrencyRate = 1M,
            AffiliateId = 0,
            OrderStatus = OrderStatus.Processing,
            AllowStoringCreditCardNumber = false,
            CardType = string.Empty,
            CardName = string.Empty,
            CardNumber = string.Empty,
            MaskedCreditCardNumber = string.Empty,
            CardCvv2 = string.Empty,
            CardExpirationMonth = string.Empty,
            CardExpirationYear = string.Empty,
            PaymentMethodSystemName = "Payments.CheckMoneyOrder",
            AuthorizationTransactionId = string.Empty,
            AuthorizationTransactionCode = string.Empty,
            AuthorizationTransactionResult = string.Empty,
            CaptureTransactionId = string.Empty,
            CaptureTransactionResult = string.Empty,
            SubscriptionTransactionId = string.Empty,
            PaymentStatus = PaymentStatus.Paid,
            PaidDateUtc = DateTime.UtcNow,
            BillingAddressId = firstCustomerBillingAddress.Id,
            ShippingAddressId = firstCustomerShippingAddress.Id,
            ShippingStatus = ShippingStatus.NotYetShipped,
            ShippingMethod = "Ground",
            PickupInStore = false,
            ShippingRateComputationMethodSystemName = "Shipping.FixedByWeightByTotal",
            CustomValuesXml = string.Empty,
            VatNumber = string.Empty,
            CreatedOnUtc = DateTime.UtcNow,
            CustomOrderNumber = string.Empty
        };
        
        var secondOrder = new Order
        {
            StoreId = defaultStoreId,
            OrderGuid = Guid.NewGuid(),
            CustomerId = secondCustomer.Id,
            CustomerLanguageId = await GetDefaultLanguageIdAsync(),
            CustomerIp = "127.0.0.1",
            OrderSubtotalInclTax = 2460M,
            OrderSubtotalExclTax = 2460M,
            OrderSubTotalDiscountInclTax = decimal.Zero,
            OrderSubTotalDiscountExclTax = decimal.Zero,
            OrderShippingInclTax = decimal.Zero,
            OrderShippingExclTax = decimal.Zero,
            PaymentMethodAdditionalFeeInclTax = decimal.Zero,
            PaymentMethodAdditionalFeeExclTax = decimal.Zero,
            TaxRates = "0:0;",
            OrderTax = decimal.Zero,
            OrderTotal = 2460M,
            RefundedAmount = decimal.Zero,
            OrderDiscount = decimal.Zero,
            CheckoutAttributeDescription = string.Empty,
            CheckoutAttributesXml = string.Empty,
            CustomerCurrencyCode = "USD",
            CurrencyRate = 1M,
            AffiliateId = 0,
            OrderStatus = OrderStatus.Pending,
            AllowStoringCreditCardNumber = false,
            CardType = string.Empty,
            CardName = string.Empty,
            CardNumber = string.Empty,
            MaskedCreditCardNumber = string.Empty,
            CardCvv2 = string.Empty,
            CardExpirationMonth = string.Empty,
            CardExpirationYear = string.Empty,
            PaymentMethodSystemName = "Payments.CheckMoneyOrder",
            AuthorizationTransactionId = string.Empty,
            AuthorizationTransactionCode = string.Empty,
            AuthorizationTransactionResult = string.Empty,
            CaptureTransactionId = string.Empty,
            CaptureTransactionResult = string.Empty,
            SubscriptionTransactionId = string.Empty,
            PaymentStatus = PaymentStatus.Pending,
            PaidDateUtc = null,
            BillingAddressId = secondCustomerBillingAddress.Id,
            ShippingAddressId = secondCustomerShippingAddress.Id,
            ShippingStatus = ShippingStatus.NotYetShipped,
            ShippingMethod = "Next Day Air",
            PickupInStore = false,
            ShippingRateComputationMethodSystemName = "Shipping.FixedByWeightByTotal",
            CustomValuesXml = string.Empty,
            VatNumber = string.Empty,
            CreatedOnUtc = DateTime.UtcNow,
            CustomOrderNumber = string.Empty
        };

        var thirdOrder = new Order
        {
            StoreId = defaultStoreId,
            OrderGuid = Guid.NewGuid(),
            CustomerId = thirdCustomer.Id,
            CustomerLanguageId = await GetDefaultLanguageIdAsync(),
            CustomerIp = "127.0.0.1",
            OrderSubtotalInclTax = 8.80M,
            OrderSubtotalExclTax = 8.80M,
            OrderSubTotalDiscountInclTax = decimal.Zero,
            OrderSubTotalDiscountExclTax = decimal.Zero,
            OrderShippingInclTax = decimal.Zero,
            OrderShippingExclTax = decimal.Zero,
            PaymentMethodAdditionalFeeInclTax = decimal.Zero,
            PaymentMethodAdditionalFeeExclTax = decimal.Zero,
            TaxRates = "0:0;",
            OrderTax = decimal.Zero,
            OrderTotal = 8.80M,
            RefundedAmount = decimal.Zero,
            OrderDiscount = decimal.Zero,
            CheckoutAttributeDescription = string.Empty,
            CheckoutAttributesXml = string.Empty,
            CustomerCurrencyCode = "USD",
            CurrencyRate = 1M,
            AffiliateId = 0,
            OrderStatus = OrderStatus.Pending,
            AllowStoringCreditCardNumber = false,
            CardType = string.Empty,
            CardName = string.Empty,
            CardNumber = string.Empty,
            MaskedCreditCardNumber = string.Empty,
            CardCvv2 = string.Empty,
            CardExpirationMonth = string.Empty,
            CardExpirationYear = string.Empty,
            PaymentMethodSystemName = "Payments.CheckMoneyOrder",
            AuthorizationTransactionId = string.Empty,
            AuthorizationTransactionCode = string.Empty,
            AuthorizationTransactionResult = string.Empty,
            CaptureTransactionId = string.Empty,
            CaptureTransactionResult = string.Empty,
            SubscriptionTransactionId = string.Empty,
            PaymentStatus = PaymentStatus.Pending,
            PaidDateUtc = null,
            BillingAddressId = thirdCustomerBillingAddress.Id,
            ShippingStatus = ShippingStatus.ShippingNotRequired,
            ShippingMethod = string.Empty,
            PickupInStore = false,
            ShippingRateComputationMethodSystemName = string.Empty,
            CustomValuesXml = string.Empty,
            VatNumber = string.Empty,
            CreatedOnUtc = DateTime.UtcNow,
            CustomOrderNumber = string.Empty
        };

        var fourthOrder = new Order
        {
            StoreId = defaultStoreId,
            OrderGuid = Guid.NewGuid(),
            CustomerId = fourthCustomer.Id,
            CustomerLanguageId = await GetDefaultLanguageIdAsync(),
            CustomerIp = "127.0.0.1",
            OrderSubtotalInclTax = 102M,
            OrderSubtotalExclTax = 102M,
            OrderSubTotalDiscountInclTax = decimal.Zero,
            OrderSubTotalDiscountExclTax = decimal.Zero,
            OrderShippingInclTax = decimal.Zero,
            OrderShippingExclTax = decimal.Zero,
            PaymentMethodAdditionalFeeInclTax = decimal.Zero,
            PaymentMethodAdditionalFeeExclTax = decimal.Zero,
            TaxRates = "0:0;",
            OrderTax = decimal.Zero,
            OrderTotal = 102M,
            RefundedAmount = decimal.Zero,
            OrderDiscount = decimal.Zero,
            CheckoutAttributeDescription = string.Empty,
            CheckoutAttributesXml = string.Empty,
            CustomerCurrencyCode = "USD",
            CurrencyRate = 1M,
            AffiliateId = 0,
            OrderStatus = OrderStatus.Processing,
            AllowStoringCreditCardNumber = false,
            CardType = string.Empty,
            CardName = string.Empty,
            CardNumber = string.Empty,
            MaskedCreditCardNumber = string.Empty,
            CardCvv2 = string.Empty,
            CardExpirationMonth = string.Empty,
            CardExpirationYear = string.Empty,
            PaymentMethodSystemName = "Payments.CheckMoneyOrder",
            AuthorizationTransactionId = string.Empty,
            AuthorizationTransactionCode = string.Empty,
            AuthorizationTransactionResult = string.Empty,
            CaptureTransactionId = string.Empty,
            CaptureTransactionResult = string.Empty,
            SubscriptionTransactionId = string.Empty,
            PaymentStatus = PaymentStatus.Paid,
            PaidDateUtc = DateTime.UtcNow,
            BillingAddressId = fourthCustomerBillingAddress.Id,
            ShippingAddressId = fourthCustomerShippingAddress.Id,
            ShippingStatus = ShippingStatus.Shipped,
            ShippingMethod = "Pickup in store",
            PickupInStore = true,
            PickupAddressId = fourthCustomerPickupAddress.Id,
            ShippingRateComputationMethodSystemName = "Pickup.PickupInStore",
            CustomValuesXml = string.Empty,
            VatNumber = string.Empty,
            CreatedOnUtc = DateTime.UtcNow,
            CustomOrderNumber = string.Empty
        };

        var fifthOrder = new Order
        {
            StoreId = defaultStoreId,
            OrderGuid = Guid.NewGuid(),
            CustomerId = fifthCustomer.Id,
            CustomerLanguageId = await GetDefaultLanguageIdAsync(),
            CustomerIp = "127.0.0.1",
            OrderSubtotalInclTax = 43.50M,
            OrderSubtotalExclTax = 43.50M,
            OrderSubTotalDiscountInclTax = decimal.Zero,
            OrderSubTotalDiscountExclTax = decimal.Zero,
            OrderShippingInclTax = decimal.Zero,
            OrderShippingExclTax = decimal.Zero,
            PaymentMethodAdditionalFeeInclTax = decimal.Zero,
            PaymentMethodAdditionalFeeExclTax = decimal.Zero,
            TaxRates = "0:0;",
            OrderTax = decimal.Zero,
            OrderTotal = 43.50M,
            RefundedAmount = decimal.Zero,
            OrderDiscount = decimal.Zero,
            CheckoutAttributeDescription = string.Empty,
            CheckoutAttributesXml = string.Empty,
            CustomerCurrencyCode = "USD",
            CurrencyRate = 1M,
            AffiliateId = 0,
            OrderStatus = OrderStatus.Complete,
            AllowStoringCreditCardNumber = false,
            CardType = string.Empty,
            CardName = string.Empty,
            CardNumber = string.Empty,
            MaskedCreditCardNumber = string.Empty,
            CardCvv2 = string.Empty,
            CardExpirationMonth = string.Empty,
            CardExpirationYear = string.Empty,
            PaymentMethodSystemName = "Payments.CheckMoneyOrder",
            AuthorizationTransactionId = string.Empty,
            AuthorizationTransactionCode = string.Empty,
            AuthorizationTransactionResult = string.Empty,
            CaptureTransactionId = string.Empty,
            CaptureTransactionResult = string.Empty,
            SubscriptionTransactionId = string.Empty,
            PaymentStatus = PaymentStatus.Paid,
            PaidDateUtc = DateTime.UtcNow,
            BillingAddressId = fifthCustomerBillingAddress.Id,
            ShippingAddressId = fifthCustomerShippingAddress.Id,
            ShippingStatus = ShippingStatus.Delivered,
            ShippingMethod = "Ground",
            PickupInStore = false,
            ShippingRateComputationMethodSystemName = "Shipping.FixedByWeightByTotal",
            CustomValuesXml = string.Empty,
            VatNumber = string.Empty,
            CreatedOnUtc = DateTime.UtcNow,
            CustomOrderNumber = string.Empty
        };

        var allOrders = new[] { firstOrder, secondOrder, thirdOrder, fourthOrder, fifthOrder };

        await _dataProvider.BulkInsertEntitiesAsync(allOrders);

        foreach (var order in allOrders) 
            order.CustomOrderNumber = order.Id.ToString();

        await _dataProvider.UpdateEntitiesAsync(allOrders);

        #endregion

        #region Order items

        //item Apple iCam
        var firstOrderItem1 = new OrderItem
        {
            OrderItemGuid = Guid.NewGuid(),
            OrderId = firstOrder.Id,
            ProductId = (await Table<Product>().FirstAsync(p => p.Name == "Apple iCam")).Id,
            UnitPriceInclTax = 1300M,
            UnitPriceExclTax = 1300M,
            PriceInclTax = 1300M,
            PriceExclTax = 1300M,
            OriginalProductCost = decimal.Zero,
            AttributeDescription = string.Empty,
            AttributesXml = string.Empty,
            Quantity = 1,
            DiscountAmountInclTax = decimal.Zero,
            DiscountAmountExclTax = decimal.Zero,
            DownloadCount = 0,
            IsDownloadActivated = false,
            LicenseDownloadId = 0,
            ItemWeight = null,
            RentalStartDateUtc = null,
            RentalEndDateUtc = null
        };

        //item Leica T Mirrorless Digital Camera
        var firstOrderItem2 = new OrderItem
        {
            OrderItemGuid = Guid.NewGuid(),
            OrderId = firstOrder.Id,
            ProductId = (await Table<Product>().FirstAsync(p => p.Name == "Leica T Mirrorless Digital Camera")).Id,
            UnitPriceInclTax = 530M,
            UnitPriceExclTax = 530M,
            PriceInclTax = 530M,
            PriceExclTax = 530M,
            OriginalProductCost = decimal.Zero,
            AttributeDescription = string.Empty,
            AttributesXml = string.Empty,
            Quantity = 1,
            DiscountAmountInclTax = decimal.Zero,
            DiscountAmountExclTax = decimal.Zero,
            DownloadCount = 0,
            IsDownloadActivated = false,
            LicenseDownloadId = 0,
            ItemWeight = null,
            RentalStartDateUtc = null,
            RentalEndDateUtc = null
        };

        //item $25 Virtual Gift Card
        var firstOrderItem3 = new OrderItem
        {
            OrderItemGuid = Guid.NewGuid(),
            OrderId = firstOrder.Id,
            ProductId = (await Table<Product>().FirstAsync(p => p.Name == "$25 Virtual Gift Card")).Id,
            UnitPriceInclTax = 25M,
            UnitPriceExclTax = 25M,
            PriceInclTax = 25M,
            PriceExclTax = 25M,
            OriginalProductCost = decimal.Zero,
            AttributeDescription = "From: Steve Gates &lt;steve_gates@nopCommerce.com&gt;<br />For: Brenda Lindgren &lt;brenda_lindgren@nopCommerce.com&gt;",
            AttributesXml = "<Attributes><GiftCardInfo><RecipientName>Brenda Lindgren</RecipientName><RecipientEmail>brenda_lindgren@nopCommerce.com</RecipientEmail><SenderName>Steve Gates</SenderName><SenderEmail>steve_gates@gmail.com</SenderEmail><Message></Message></GiftCardInfo></Attributes>",
            Quantity = 1,
            DiscountAmountInclTax = decimal.Zero,
            DiscountAmountExclTax = decimal.Zero,
            DownloadCount = 0,
            IsDownloadActivated = false,
            LicenseDownloadId = 0,
            ItemWeight = null,
            RentalStartDateUtc = null,
            RentalEndDateUtc = null
        };

        //item Vintage Style Engagement Ring
        var secondOrderItem1 = new OrderItem
        {
            OrderItemGuid = Guid.NewGuid(),
            OrderId = secondOrder.Id,
            ProductId = (await Table<Product>().FirstAsync(p => p.Name == "Vintage Style Engagement Ring")).Id,
            UnitPriceInclTax = 2100M,
            UnitPriceExclTax = 2100M,
            PriceInclTax = 2100M,
            PriceExclTax = 2100M,
            OriginalProductCost = decimal.Zero,
            AttributeDescription = string.Empty,
            AttributesXml = string.Empty,
            Quantity = 1,
            DiscountAmountInclTax = decimal.Zero,
            DiscountAmountExclTax = decimal.Zero,
            DownloadCount = 0,
            IsDownloadActivated = false,
            LicenseDownloadId = 0,
            ItemWeight = null,
            RentalStartDateUtc = null,
            RentalEndDateUtc = null
        };

        //item Flower Girl Bracelet
        var secondOrderItem2 = new OrderItem
        {
            OrderItemGuid = Guid.NewGuid(),
            OrderId = secondOrder.Id,
            ProductId = (await Table<Product>().FirstAsync(p => p.Name == "Flower Girl Bracelet")).Id,
            UnitPriceInclTax = 360M,
            UnitPriceExclTax = 360M,
            PriceInclTax = 360M,
            PriceExclTax = 360M,
            OriginalProductCost = decimal.Zero,
            AttributeDescription = string.Empty,
            AttributesXml = string.Empty,
            Quantity = 1,
            DiscountAmountInclTax = decimal.Zero,
            DiscountAmountExclTax = decimal.Zero,
            DownloadCount = 0,
            IsDownloadActivated = false,
            LicenseDownloadId = 0,
            ItemWeight = null,
            RentalStartDateUtc = null,
            RentalEndDateUtc = null
        };

        //item If You Wait
        var thirdOrderItem1 = new OrderItem
        {
            OrderItemGuid = Guid.NewGuid(),
            OrderId = thirdOrder.Id,
            ProductId = (await Table<Product>().FirstAsync(p => p.Name == "If You Wait (donation)")).Id,
            UnitPriceInclTax = 3M,
            UnitPriceExclTax = 3M,
            PriceInclTax = 3M,
            PriceExclTax = 3M,
            OriginalProductCost = decimal.Zero,
            AttributeDescription = string.Empty,
            AttributesXml = string.Empty,
            Quantity = 1,
            DiscountAmountInclTax = decimal.Zero,
            DiscountAmountExclTax = decimal.Zero,
            DownloadCount = 0,
            IsDownloadActivated = false,
            LicenseDownloadId = 0,
            ItemWeight = null,
            RentalStartDateUtc = null,
            RentalEndDateUtc = null
        };

        //item Night Visions
        var thirdOrderItem2 = new OrderItem
        {
            OrderItemGuid = Guid.NewGuid(),
            OrderId = thirdOrder.Id,
            ProductId = (await Table<Product>().FirstAsync(p => p.Name == "Night Visions")).Id,
            UnitPriceInclTax = 2.8M,
            UnitPriceExclTax = 2.8M,
            PriceInclTax = 2.8M,
            PriceExclTax = 2.8M,
            OriginalProductCost = decimal.Zero,
            AttributeDescription = string.Empty,
            AttributesXml = string.Empty,
            Quantity = 1,
            DiscountAmountInclTax = decimal.Zero,
            DiscountAmountExclTax = decimal.Zero,
            DownloadCount = 0,
            IsDownloadActivated = false,
            LicenseDownloadId = 0,
            ItemWeight = null,
            RentalStartDateUtc = null,
            RentalEndDateUtc = null
        };

        //item Science & Faith
        var thirdOrderItem3 = new OrderItem
        {
            OrderItemGuid = Guid.NewGuid(),
            OrderId = thirdOrder.Id,
            ProductId = (await Table<Product>().FirstAsync(p => p.Name == "Science & Faith")).Id,
            UnitPriceInclTax = 3M,
            UnitPriceExclTax = 3M,
            PriceInclTax = 3M,
            PriceExclTax = 3M,
            OriginalProductCost = decimal.Zero,
            AttributeDescription = string.Empty,
            AttributesXml = string.Empty,
            Quantity = 1,
            DiscountAmountInclTax = decimal.Zero,
            DiscountAmountExclTax = decimal.Zero,
            DownloadCount = 0,
            IsDownloadActivated = false,
            LicenseDownloadId = 0,
            ItemWeight = null,
            RentalStartDateUtc = null,
            RentalEndDateUtc = null
        };

        //item Pride and Prejudice
        var fourthOrderItem1 = new OrderItem
        {
            OrderItemGuid = Guid.NewGuid(),
            OrderId = fourthOrder.Id,
            ProductId = (await Table<Product>().FirstAsync(p => p.Name == "Pride and Prejudice")).Id,
            UnitPriceInclTax = 24M,
            UnitPriceExclTax = 24M,
            PriceInclTax = 24M,
            PriceExclTax = 24M,
            OriginalProductCost = decimal.Zero,
            AttributeDescription = string.Empty,
            AttributesXml = string.Empty,
            Quantity = 1,
            DiscountAmountInclTax = decimal.Zero,
            DiscountAmountExclTax = decimal.Zero,
            DownloadCount = 0,
            IsDownloadActivated = false,
            LicenseDownloadId = 0,
            ItemWeight = null,
            RentalStartDateUtc = null,
            RentalEndDateUtc = null
        };

        //item First Prize Pies
        var fourthOrderItem2 = new OrderItem
        {
            OrderItemGuid = Guid.NewGuid(),
            OrderId = fourthOrder.Id,
            ProductId = (await Table<Product>().FirstAsync(p => p.Name == "First Prize Pies")).Id,
            UnitPriceInclTax = 51M,
            UnitPriceExclTax = 51M,
            PriceInclTax = 51M,
            PriceExclTax = 51M,
            OriginalProductCost = decimal.Zero,
            AttributeDescription = string.Empty,
            AttributesXml = string.Empty,
            Quantity = 1,
            DiscountAmountInclTax = decimal.Zero,
            DiscountAmountExclTax = decimal.Zero,
            DownloadCount = 0,
            IsDownloadActivated = false,
            LicenseDownloadId = 0,
            ItemWeight = null,
            RentalStartDateUtc = null,
            RentalEndDateUtc = null
        };

        //item Fahrenheit 451 by Ray Bradbury
        var fourthOrderItem3 = new OrderItem
        {
            OrderItemGuid = Guid.NewGuid(),
            OrderId = fourthOrder.Id,
            ProductId = (await Table<Product>().FirstAsync(p => p.Name == "Fahrenheit 451 by Ray Bradbury")).Id,
            UnitPriceInclTax = 27M,
            UnitPriceExclTax = 27M,
            PriceInclTax = 27M,
            PriceExclTax = 27M,
            OriginalProductCost = decimal.Zero,
            AttributeDescription = string.Empty,
            AttributesXml = string.Empty,
            Quantity = 1,
            DiscountAmountInclTax = decimal.Zero,
            DiscountAmountExclTax = decimal.Zero,
            DownloadCount = 0,
            IsDownloadActivated = false,
            LicenseDownloadId = 0,
            ItemWeight = null,
            RentalStartDateUtc = null,
            RentalEndDateUtc = null
        };

        //item Levi's 511 Jeans
        var fifthOrderItem1 = new OrderItem
        {
            OrderItemGuid = Guid.NewGuid(),
            OrderId = fifthOrder.Id,
            ProductId = (await Table<Product>().FirstAsync(p => p.Name == "Levi's 511 Jeans")).Id,
            UnitPriceInclTax = 43.50M,
            UnitPriceExclTax = 43.50M,
            PriceInclTax = 43.50M,
            PriceExclTax = 43.50M,
            OriginalProductCost = decimal.Zero,
            AttributeDescription = string.Empty,
            AttributesXml = string.Empty,
            Quantity = 1,
            DiscountAmountInclTax = decimal.Zero,
            DiscountAmountExclTax = decimal.Zero,
            DownloadCount = 0,
            IsDownloadActivated = false,
            LicenseDownloadId = 0,
            ItemWeight = null,
            RentalStartDateUtc = null,
            RentalEndDateUtc = null
        };

        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            firstOrderItem1, firstOrderItem2, firstOrderItem3, secondOrderItem1, secondOrderItem2, thirdOrderItem1,
            thirdOrderItem2, thirdOrderItem3, fourthOrderItem1, fourthOrderItem2, fourthOrderItem3, fifthOrderItem1
        });

        #endregion
        
        #region Shipments

        var fourthOrderShipment1 = new Shipment
        {
            OrderId = fourthOrder.Id,
            TrackingNumber = string.Empty,
            TotalWeight = 4M,
            ReadyForPickupDateUtc = DateTime.UtcNow,
            DeliveryDateUtc = DateTime.UtcNow,
            AdminComment = string.Empty,
            CreatedOnUtc = DateTime.UtcNow
        };

        var fourthOrderShipment2 = new Shipment
        {
            OrderId = fourthOrder.Id,
            TrackingNumber = string.Empty,
            TotalWeight = 2M,
            ReadyForPickupDateUtc = DateTime.UtcNow,
            DeliveryDateUtc = DateTime.UtcNow,
            AdminComment = string.Empty,
            CreatedOnUtc = DateTime.UtcNow
        };

        var fifthOrderShipment1 = new Shipment
        {
            OrderId = fifthOrder.Id,
            TrackingNumber = string.Empty,
            TotalWeight = 2M,
            ShippedDateUtc = DateTime.UtcNow,
            DeliveryDateUtc = DateTime.UtcNow,
            AdminComment = string.Empty,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            fourthOrderShipment1, fourthOrderShipment2, fifthOrderShipment1
        });

        #endregion

        #region Shipment items

        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new ShipmentItem
            {
                OrderItemId = fourthOrderItem1.Id,
                Quantity = 1,
                WarehouseId = 0,
                ShipmentId = fourthOrderShipment1.Id
            },
            new ShipmentItem
            {
                OrderItemId = fourthOrderItem2.Id,
                Quantity = 1,
                WarehouseId = 0,
                ShipmentId = fourthOrderShipment1.Id
            },
            new ShipmentItem
            {
                OrderItemId = fourthOrderItem3.Id,
                Quantity = 1,
                WarehouseId = 0,
                ShipmentId = fourthOrderShipment2.Id
            },
            new ShipmentItem
            {
                OrderItemId = fifthOrderItem1.Id,
                Quantity = 1,
                WarehouseId = 0,
                ShipmentId = fifthOrderShipment1.Id
            }
        });

        #endregion

        #region Gift cards

        await _dataProvider.InsertEntityAsync(new GiftCard
        {
            GiftCardType = GiftCardType.Virtual,
            PurchasedWithOrderItemId = firstOrderItem3.Id,
            Amount = 25M,
            IsGiftCardActivated = false,
            GiftCardCouponCode = string.Empty,
            RecipientName = "Brenda Lindgren",
            RecipientEmail = "brenda_lindgren@nopCommerce.com",
            SenderName = "Steve Gates",
            SenderEmail = "steve_gates@nopCommerce.com",
            Message = string.Empty,
            IsRecipientNotified = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        #endregion

        #region Order notes

        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new OrderNote { CreatedOnUtc = DateTime.UtcNow, Note = "Order placed", OrderId = firstOrder.Id },
            new OrderNote { CreatedOnUtc = DateTime.UtcNow, Note = "Order paid", OrderId = firstOrder.Id },
            new OrderNote { CreatedOnUtc = DateTime.UtcNow, Note = "Order placed", OrderId = secondOrder.Id },
            new OrderNote { CreatedOnUtc = DateTime.UtcNow, Note = "Order placed", OrderId = thirdOrder.Id },
            new OrderNote { CreatedOnUtc = DateTime.UtcNow, Note = "Order placed", OrderId = fourthOrder.Id },
            new OrderNote { CreatedOnUtc = DateTime.UtcNow, Note = "Order paid", OrderId = fourthOrder.Id },
            new OrderNote { CreatedOnUtc = DateTime.UtcNow, Note = "Order shipped", OrderId = fourthOrder.Id },
            new OrderNote { CreatedOnUtc = DateTime.UtcNow, Note = "Order placed", OrderId = fifthOrder.Id },
            new OrderNote { CreatedOnUtc = DateTime.UtcNow, Note = "Order paid", OrderId = fifthOrder.Id },
            new OrderNote { CreatedOnUtc = DateTime.UtcNow, Note = "Order shipped", OrderId = fifthOrder.Id },
            new OrderNote { CreatedOnUtc = DateTime.UtcNow, Note = "Order delivered", OrderId = fifthOrder.Id }
        });

        #endregion
    }

    /// <summary>
    /// Installs a sample activity logs
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallActivityLogsAsync()
    {
        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new ActivityLog
            {
                ActivityLogTypeId = await GetFirstEntityIdAsync<ActivityLogType>(alt => alt.SystemKeyword == "EditCategory") ?? throw new Exception("Cannot load LogType: EditCategory"),
                Comment = "Edited a category ('Computers')",
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = await GetDefaultCustomerIdAsync(),
                IpAddress = "127.0.0.1"
            },
            new ActivityLog
            {
                ActivityLogTypeId = await GetFirstEntityIdAsync<ActivityLogType>(alt => alt.SystemKeyword == "EditDiscount") ?? throw new Exception("Cannot load LogType: EditDiscount"),
                Comment = "Edited a discount ('Sample discount with coupon code')",
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = await GetDefaultCustomerIdAsync(),
                IpAddress = "127.0.0.1"
            },
            new ActivityLog
            {
                ActivityLogTypeId = await GetFirstEntityIdAsync<ActivityLogType>(alt => alt.SystemKeyword == "EditSpecAttribute") ?? throw new Exception("Cannot load LogType: EditSpecAttribute"),
                Comment = "Edited a specification attribute ('CPU Type')",
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = await GetDefaultCustomerIdAsync(),
                IpAddress = "127.0.0.1"
            },
            new ActivityLog
            {
                ActivityLogTypeId = await GetFirstEntityIdAsync<ActivityLogType>(alt => alt.SystemKeyword == "AddNewProductAttribute") ?? throw new Exception("Cannot load LogType: AddNewProductAttribute"),
                Comment = "Added a new product attribute ('Some attribute')",
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = await GetDefaultCustomerIdAsync(),
                IpAddress = "127.0.0.1"
            },
            new ActivityLog
            {
                ActivityLogTypeId = await GetFirstEntityIdAsync<ActivityLogType>(alt => alt.SystemKeyword == "DeleteGiftCard") ?? throw new Exception("Cannot load LogType: DeleteGiftCard"),
                Comment = "Deleted a gift card ('bdbbc0ef-be57')",
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = await GetDefaultCustomerIdAsync(),
                IpAddress = "127.0.0.1"
            }
        });
    }

    /// <summary>
    /// Installs a sample search terms
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallSearchTermsAsync()
    {
        await _dataProvider.BulkInsertEntitiesAsync(new[]
        {
            new SearchTerm { Count = 34, Keyword = "computer", StoreId = await GetDefaultStoreIdAsync() },
            new SearchTerm { Count = 30, Keyword = "camera", StoreId = await GetDefaultStoreIdAsync() },
            new SearchTerm { Count = 27, Keyword = "jewelry", StoreId = await GetDefaultStoreIdAsync() },
            new SearchTerm { Count = 26, Keyword = "shoes", StoreId = await GetDefaultStoreIdAsync() },
            new SearchTerm { Count = 19, Keyword = "jeans", StoreId = await GetDefaultStoreIdAsync() },
            new SearchTerm { Count = 10, Keyword = "gift", StoreId = await GetDefaultStoreIdAsync() }
        });
    }

    #endregion
}