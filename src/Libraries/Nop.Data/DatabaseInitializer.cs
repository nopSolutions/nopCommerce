//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity.Database;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;


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

            #region Language

            var language1 = new Language
                               {
                                           Name = "English",
                                           LanguageCulture = "en-US",
                                           FlagImageFileName = string.Empty,
                                           Published= true,
                                           DisplayOrder= 1
                               };
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
                                            ApplicationName = "NopCommerce",
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

            var category1 = new Category()
            {
                Name = "Jewelry",
                Description = "Some description 1",
                TemplateId = 0, //TODO: set TemplateId
                MetaKeywords = string.Empty,
                MetaDescription = string.Empty,
                MetaTitle = string.Empty,
                SeName = string.Empty,
                ParentCategoryId = 0,
                PageSize = 3,
                PriceRanges = string.Empty,
                Published = true,
                DisplayOrder = 7,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            context.Categories.Add(category1);
            context.SaveChanges();


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
                TemplateId = 0, //TODO: set TemplateId
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
                                   TemplateId = 0, //TODO: set TemplateId
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