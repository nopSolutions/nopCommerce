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
using Nop.Core.Domain;
using Nop.Core;


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

            #region Customers

            string password = "admin";
            string saltKey = SecurityHelper.CreateSalt(5);
            string passwordHash = SecurityHelper.CreatePasswordHash(password, saltKey, "SHA1");
            var customers = new List<Customer>
                                {
                                    new Customer
                                        {
                                            CustomerGuid = Guid.NewGuid(),
                                            Email = "admin@yourStore.com",
                                            Username = "admin@yourStore.com",
                                            SaltKey = saltKey,
                                            PasswordHash = passwordHash,
                                            AdminComment = string.Empty,
                                            Active = true,
                                            Deleted = false,
                                            RegistrationDateUtc = DateTime.UtcNow
                                        }
                                };
            customers.ForEach(c => context.Customers.Add(c));
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
                                   TemplateId = 0,
                                   //TODO: set TemplateId
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
                                          LowStockActivity = LowStockActivity.Nothing,
                                          BackorderMode = BackorderMode.NoBackorders,
                                          OrderMinimumQuantity = 1,
                                          OrderMaximumQuantity = 10000,
                                          Price = 130.12345M,
                                          Weight = 1,
                                          Length = 1,
                                          Width = 1,
                                          Height = 1,
                                          Published = true,
                                          Product = product1,
                                          CreatedOnUtc = DateTime.UtcNow,
                                          UpdatedOnUtc = DateTime.UtcNow
                                      };
            product1.ProductVariants = new List<ProductVariant>();
            product1.ProductVariants.Add(productVariant1);
            context.Products.Add(product1);
            //context.ProductVariants.Add(productVariant1);
            context.SaveChanges();

            #endregion

            base.Seed(context);
        }
    }
}