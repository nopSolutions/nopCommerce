using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class CustomerContentTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customerContent()
        {
            var customerContent = new CustomerContent
                               {
                                   IpAddress = "192.168.1.1",
                                   IsApproved = true,
                                   CreatedOnUtc = new DateTime(2010, 01, 01),
                                   UpdatedOnUtc = new DateTime(2010, 01, 02),
                                   Customer = GetTestCustomer()
                               };

            var fromDb = SaveAndLoadEntity(customerContent);
            fromDb.IpAddress.ShouldEqual("192.168.1.1");
            fromDb.IsApproved.ShouldEqual(true);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
            
            fromDb.Customer.ShouldNotBeNull();
            fromDb.Customer.Email.ShouldEqual("admin@yourStore.com");
        }

        [Test]
        public void Can_get_customer_content_by_type()
        {
            var customer = SaveAndLoadEntity<Customer>(GetTestCustomer());
            var product = SaveAndLoadEntity<Product>(GetTestProduct());

            var productReview = new ProductReview
            {
                Customer = customer,
                Product = product,
                Title = "Test",
                ReviewText = "A review",
                IpAddress = "192.168.1.1",
                IsApproved = true,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            var productRating = new ProductRating
            {
                Customer = customer,
                Product = product,
                Rating = 10,
                IpAddress = "192.168.1.1",
                IsApproved = true,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02)
            };

            context.Set<CustomerContent>().Add(productReview);
            context.Set<CustomerContent>().Add(productRating);

            context.SaveChanges();

            context.Dispose();
            context = new NopObjectContext(GetTestDbName());

            var query = context.Set<CustomerContent>();
            query.ToList().Count.ShouldEqual(2);

            var dbReviews = query.OfType<ProductReview>().ToList();
            dbReviews.Count().ShouldEqual(1);
            dbReviews.First().ReviewText.ShouldEqual("A review");

            var dbRatings = query.OfType<ProductRating>().ToList();
            dbRatings.Count().ShouldEqual(1);
            dbRatings.First().Rating.ShouldEqual(10);
        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = "admin@yourStore.com",
                Username = "admin@yourStore.com",
                AdminComment = "some comment here",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        protected Product GetTestProduct()
        {
            return new Product
            {
                Name = "Name 1",
                ShortDescription = "ShortDescription 1",
                FullDescription = "FullDescription 1",
                AdminComment = "AdminComment 1",
                TemplateId = 1,
                ShowOnHomePage = false,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                SeName = "SE name",
                AllowCustomerReviews = true,
                AllowCustomerRatings = true,
                RatingSum = 2,
                TotalRatingVotes = 3,
                Published = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }
    }
}