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
    public class CustomerContentPersistenceTests : PersistenceTest
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
            fromDb.ShouldNotBeNull();
            fromDb.IpAddress.ShouldEqual("192.168.1.1");
            fromDb.IsApproved.ShouldEqual(true);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
            
            fromDb.Customer.ShouldNotBeNull();
        }

        [Test]
        public void Can_get_customer_content_by_type()
        {
            var customer = SaveAndLoadEntity<Customer>(GetTestCustomer(), false);
            var product = SaveAndLoadEntity<Product>(GetTestProduct(), false);

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
            
            var productReviewHelpfulness = new ProductReviewHelpfulness
            {
                Customer = customer,
                ProductReview = productReview,
                WasHelpful = true,
                IpAddress = "192.168.1.1",
                IsApproved = true,
                CreatedOnUtc = new DateTime(2010, 01, 03),
                UpdatedOnUtc = new DateTime(2010, 01, 04)
            };

            context.Set<CustomerContent>().Add(productReview);
            context.Set<CustomerContent>().Add(productReviewHelpfulness);

            context.SaveChanges();

            context.Dispose();
            context = new NopObjectContext(GetTestDbName());

            var query = context.Set<CustomerContent>();
            query.ToList().Count.ShouldEqual(2);

            var dbReviews = query.OfType<ProductReview>().ToList();
            dbReviews.Count().ShouldEqual(1);
            dbReviews.First().ReviewText.ShouldEqual("A review");

            var dbHelpfulnessRecords = query.OfType<ProductReviewHelpfulness>().ToList();
            dbHelpfulnessRecords.Count().ShouldEqual(1);
            dbHelpfulnessRecords.First().WasHelpful.ShouldEqual(true);
        }

        [Test]
        public void Can_save_productReview_with_helpfulness()
        {
            var customer = SaveAndLoadEntity<Customer>(GetTestCustomer(), false);
            var product = SaveAndLoadEntity<Product>(GetTestProduct(), false);

            var productReview = new ProductReview
            {
                Customer = customer,
                Product = product,
                Title = "Test",
                ReviewText = "A review",
                IpAddress = "192.168.1.1",
                IsApproved = true,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02)
            };
            productReview.ProductReviewHelpfulnessEntries.Add
                (
                    new ProductReviewHelpfulness
                    {
                        Customer = customer,
                        WasHelpful = true,
                        IpAddress = "192.168.1.1",
                        IsApproved = true,
                        CreatedOnUtc = new DateTime(2010, 01, 03),
                        UpdatedOnUtc = new DateTime(2010, 01, 04)
                    } 
                );
            var fromDb = SaveAndLoadEntity(productReview);
            fromDb.ShouldNotBeNull();
            fromDb.ReviewText.ShouldEqual("A review");


            fromDb.ProductReviewHelpfulnessEntries.ShouldNotBeNull();
            (fromDb.ProductReviewHelpfulnessEntries.Count == 1).ShouldBeTrue();
            fromDb.ProductReviewHelpfulnessEntries.First().WasHelpful.ShouldEqual(true);
        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
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
                Published = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }
    }
}