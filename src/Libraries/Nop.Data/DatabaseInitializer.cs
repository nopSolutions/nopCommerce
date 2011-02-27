
using System;
using System.Collections.Generic;
using System.Data.Entity.Database;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;


namespace Nop.Data
{
    /// <summary>
    /// Database initializer
    /// </summary>
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<NopObjectContext>
    {
        protected override void Seed(NopObjectContext context)
        {
            #region Settings

            var settings = new List<Setting>
                               {
                                   new Setting
                                       {
                                           Name = "TestSetting1",
                                           Value = "Value1",
                                           Description = string.Empty
                                       },
                                   new Setting
                                       {
                                           Name = "TestSetting2",
                                           Value = "Value2",
                                           Description = string.Empty
                                       }
                               };
            settings.ForEach(s => context.Settings.Add(s));
            context.SaveChanges();

            #endregion

            #region Measures

            context.MeasureDimensions.Add(new MeasureDimension()
                {
                    Name = "inch(es)",
                    SystemKeyword = "inches",
                    Ratio = 1M,
                    DisplayOrder = 1,
                });
            context.MeasureDimensions.Add(new MeasureDimension()
            {
                Name = "feet",
                SystemKeyword = "feet",
                Ratio = 0.0833M,
                DisplayOrder = 2,
            });
            context.MeasureDimensions.Add(new MeasureDimension()
            {
                Name = "meter(s)",
                SystemKeyword = "meters",
                Ratio = 0.0254M,
                DisplayOrder = 3,
            });
            context.MeasureDimensions.Add(new MeasureDimension()
            {
                Name = "millimetre(s)",
                SystemKeyword = "millimetres",
                Ratio = 25.4M,
                DisplayOrder = 4,
            });
            context.SaveChanges();


            context.MeasureWeights.Add(new MeasureWeight()
            {
                Name = "ounce(s)",
                SystemKeyword = "ounce",
                Ratio = 16M,
                DisplayOrder = 1,
            });
            context.MeasureWeights.Add(new MeasureWeight()
            {
                Name = "lb(s)",
                SystemKeyword = "lb",
                Ratio = 1M,
                DisplayOrder = 2,
            });
            context.MeasureWeights.Add(new MeasureWeight()
            {
                Name = "kg(s)",
                SystemKeyword = "kg",
                Ratio = 0.4536M,
                DisplayOrder = 3,
            });
            context.MeasureWeights.Add(new MeasureWeight()
            {
                Name = "gram(s)",
                SystemKeyword = "grams",
                Ratio = 453.59M,
                DisplayOrder = 4,
            });
            context.SaveChanges();

            #endregion

            #region Tax classes

            var taxCategories = new List<TaxCategory>
                               {
                                   new TaxCategory
                                       {
                                           Name = "Books",
                                           DisplayOrder = 1,
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Electronics & Software",
                                           DisplayOrder = 5,
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Downloadable Products",
                                           DisplayOrder = 10,
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Jewelry",
                                           DisplayOrder = 15,
                                       },
                                   new TaxCategory
                                       {
                                           Name = "Apparel & Shoes",
                                           DisplayOrder = 20,
                                       },
                               };
            taxCategories.ForEach(tc => context.TaxCategories.Add(tc));
            context.SaveChanges();

            #endregion

            #region Language & locale resources

            var language1 = new Language
                               {
                                           Name = "English",
                                           LanguageCulture = "en-US",
                                           FlagImageFileName = "us.png",
                                           Published= true,
                                           DisplayOrder= 1
                               };
            //insert default sting resources (temporary solution). Requires some performance optimization
            //TODO find better way to insert default locale string resources
            //TODO use IStorageProvider instead of HostingEnvironment.MapPath
            foreach (var resFile in Directory.EnumerateFiles(HostingEnvironment.MapPath("~/App_Data/"), "*.nopres.xml"))
            {
                var resXml = new XmlDocument();
                resXml.Load(resFile);
                var resNodeList = resXml.SelectNodes(@"//Language/LocaleResource");
                foreach (XmlNode resNode in resNodeList)
                {
                    if (resNode.Attributes != null && resNode.Attributes["Name"] != null)
                    {
                        string resName = resNode.Attributes["Name"].InnerText.Trim();
                        string resValue = resNode.SelectSingleNode("Value").InnerText;
                        if (!String.IsNullOrEmpty(resName))
                        {
                            var lsr = new LocaleStringResource
                            {
                                ResourceName = resName,
                                ResourceValue = resValue
                            };
                            language1.LocaleStringResources.Add(lsr);
                        }
                    }
                }
            }
            context.Languages.Add(language1);
            context.SaveChanges();

            #endregion

            #region Currency

            var currency1 = new Currency
            {
                Name = "US Dollar",
                CurrencyCode = "USD",
                Rate = 1,
                DisplayLocale = "en-US",
                CustomFormatting = "CustomFormatting 1",
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
            context.Currencies.Add(currency1);
            context.SaveChanges();

            #endregion

            #region Customers & Users

            var customers = new List<Customer>
                                {
                                    new Customer
                                        {
                                            CustomerGuid = Guid.NewGuid(),
                                            Email = "admin@yourStore.com",
                                            Username = "admin@yourStore.com",
                                            AdminComment = string.Empty,
                                            Active = true,
                                            Deleted = false,
                                            CreatedOnUtc = DateTime.UtcNow,
                                        }
                                };
            customers.ForEach(c => context.Customers.Add(c));
            context.SaveChanges();

            var customerRoles = new List<CustomerRole>
                                {
                                    new CustomerRole
                                        {
                                            Name = "Administrators",
                                            Active = true,
                                            IsSystemRole = true,
                                            SystemName = SystemCustomerRoleNames.Administrators,
                                            Customers = new List<Customer>()
                                            {
                                                customers.FirstOrDefault()
                                            }
                                        },
                                    new CustomerRole
                                        {
                                            Name = "Registered",
                                            Active = true,
                                            IsSystemRole = true,
                                            SystemName = SystemCustomerRoleNames.Registered,
                                            Customers = new List<Customer>()
                                            {
                                                customers.FirstOrDefault()
                                            }
                                        },
                                    new CustomerRole
                                        {
                                            Name = "Guests",
                                            Active = true,
                                            IsSystemRole = true,
                                            SystemName = SystemCustomerRoleNames.Guests,
                                        }
                                };
            customerRoles.ForEach(cr => context.CustomerRoles.Add(cr));
            context.SaveChanges();

            var users = new List<User>
                                {
                                    new User
                                        {
                                            Username = "admin@yourStore.com",
                                            Password = "admin",
                                            PasswordFormat = PasswordFormat.Clear,
                                            PasswordSalt = "",
                                            FirstName = "John",
                                            LastName = "Smith",
                                            Email = "admin@yourStore.com",
                                            SecurityQuestion = "",
                                            SecurityAnswer = "",
                                            IsApproved = true,
                                            IsLockedOut = false,
                                            CreatedOnUtc = DateTime.UtcNow
                                        }
                                };
            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();

            #endregion

            #region Categories

            for (int i = 0; i < 100; i++)
            {
                var cat = new Category()
                {
                    Name = "sample category" + i,
                    Description = "Some description 1",
                    MetaKeywords = string.Empty,
                    MetaDescription = string.Empty,
                    MetaTitle = string.Empty,
                    SeName = string.Empty,
                    ParentCategoryId = 0,
                    PageSize = 3,
                    PriceRanges = string.Empty,
                    Published = true,
                    DisplayOrder = i,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };
                context.Categories.Add(cat);
                context.SaveChanges();
            }
            Category category1 = context.Categories.FirstOrDefault();

            //var category1Localization1 = new LocalizedProperty()
            //{ 
            //    Language = language1,
            //    LocaleKeyGroup = "Category",
            //    LocaleKey = "Name",
            //    LocaleValue = "Jewelry localized",
            //    EntityId = category1.Id
            //};
            //context.LocalizedProperties.Add(category1Localization1);
            //context.SaveChanges();

            #endregion

            #region Manufacturers

            var manufacturer1 = new Manufacturer()
            {
                Name = "Manufacturer 1",
                Description = "Some description 1",
                MetaKeywords = string.Empty,
                MetaDescription = string.Empty,
                MetaTitle = string.Empty,
                SeName = string.Empty,
                PageSize = 3,
                PriceRanges = string.Empty,
                Published = true,
                DisplayOrder = 7,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            context.Manufacturers.Add(manufacturer1);
            context.SaveChanges();

            #endregion

            #region Products

            var product1 = new Product()
                               {
                                   Name = "Black & White Diamond Heart",
                                   ShortDescription = "Heart Pendant 1/4 Carat (ctw) in Sterling Silver",
                                   FullDescription =
                                       "<p>Bold black diamonds alternate with sparkling white diamonds along a crisp sterling silver heart to create a look that is simple and beautiful. This sleek and stunning 1/4 carat (ctw) diamond heart pendant which includes an 18 inch silver chain, and a free box of godiva chocolates makes the perfect Valentine's Day gift.</p>",
                                   AdminComment = string.Empty,
                                   ShowOnHomePage = false,
                                   MetaKeywords = string.Empty,
                                   MetaDescription = string.Empty,
                                   MetaTitle = string.Empty,
                                   SeName = string.Empty,
                                   AllowCustomerReviews = true,
                                   AllowCustomerRatings = true,
                                   RatingSum = 0,
                                   TotalRatingVotes = 0,
                                   Published = true,
                                   CreatedOnUtc = DateTime.UtcNow,
                                   UpdatedOnUtc = DateTime.UtcNow
                               };
            var productVariant1 = new ProductVariant()
                                      {
                                          Name = string.Empty,
                                          Sku = string.Empty,
                                          Description = string.Empty,
                                          AdminComment = string.Empty,
                                          ManufacturerPartNumber = string.Empty,
                                          UserAgreementText = string.Empty,
                                          IsShipEnabled = true,
                                          ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                                          LowStockActivity = LowStockActivity.Unpublish,
                                          BackorderMode = BackorderMode.NoBackorders,
                                          OrderMinimumQuantity = 1,
                                          OrderMaximumQuantity = 10000,
                                          Price = 130.12345M,
                                          Weight = 1,
                                          Length = 1,
                                          Width = 1,
                                          Height = 1,
                                          Published = true,
                                          CreatedOnUtc = DateTime.UtcNow,
                                          UpdatedOnUtc = DateTime.UtcNow
                                      };
            product1.ProductVariants = new List<ProductVariant>();
            product1.ProductVariants.Add(productVariant1);

            var pcm1 = new ProductCategory()
                           {
                               Category = category1,
                               Product = product1,
                               DisplayOrder = 1
                           };
            context.ProductCategories.Add(pcm1);
            context.Products.Add(product1);
            context.SaveChanges();

            #endregion

            base.Seed(context);
        }
    }
}