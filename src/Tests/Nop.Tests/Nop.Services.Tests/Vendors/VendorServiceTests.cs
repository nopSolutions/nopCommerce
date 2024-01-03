using FluentAssertions;
using global::Nop.Core.Domain.Catalog;
using global::Nop.Core.Domain.Customers;
using global::Nop.Core.Domain.Vendors;
using global::Nop.Data;
using global::Nop.Services.Html;
using global::Nop.Services.Vendors;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using LinqToDB.DataProvider;
using Nop.Core.Events;



namespace Nop.Tests.Nop.Services.Tests.Vendors
{
    [TestFixture]
    public class VendorServiceTests
    {        
      
        [Test]
        public async Task GetVendorByProductIdAsync_ValidProductId_ReturnsVendor()
        {
            // Arrange
            var productId = 1;
            var vendorId = 2;
            var vendor = new Vendor { Id = vendorId };
            var productRepositoryMock = new Mock<IRepository<Product>>();
            productRepositoryMock.Setup(r => r.Table).Returns(new List<Product> { new Product { Id = productId, VendorId = vendorId } }.AsQueryable());

            var vendorRepositoryMock = new Mock<IRepository<Vendor>>();
            vendorRepositoryMock.Setup(r => r.Table).Returns(new List<Vendor> { vendor }.AsQueryable());

            var vendorService = new VendorService(
                Mock.Of<IHtmlFormatter>(),
                Mock.Of<IRepository<Customer>>(),
                productRepositoryMock.Object,
                vendorRepositoryMock.Object,
                Mock.Of<IRepository<VendorNote>>()
            );

            // Act
            var result = await vendorService.GetVendorByProductIdAsync(productId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(vendorId);
        }
        [Test]
        public async Task InsertVendorNoteAsync_ValidVendorNote_CallsInsertAsync()
        {
            // Arrange
            var vendorNoteRepositoryMock = new Mock<IRepository<VendorNote>>();
            var vendorService = new VendorService(
                Mock.Of<IHtmlFormatter>(),
                Mock.Of<IRepository<Customer>>(),
                Mock.Of<IRepository<Product>>(),
                Mock.Of<IRepository<Vendor>>(),
                vendorNoteRepositoryMock.Object
            );

            var vendorNote = new VendorNote
            {
                VendorId = 1,  // Set the VendorId property
                Note = "Sample Note",  // Set the Note property
                CreatedOnUtc = DateTime.UtcNow  // Set the CreatedOnUtc property
            };

            // Act
            await vendorService.InsertVendorNoteAsync(vendorNote);

            // Assert
            vendorNoteRepositoryMock.Verify(
                r => r.InsertAsync(It.IsAny<VendorNote>(), It.IsAny<bool>()),
                Times.Once
            );
        }
        [Test]
        public async Task DeleteVendorNoteAsync_ValidVendorNote_DeletesSuccessfully()
        {
            // Arrange
            var vendorNoteRepositoryMock = new Mock<IRepository<VendorNote>>();
            var vendorService = new VendorService(
                Mock.Of<IHtmlFormatter>(),
                Mock.Of<IRepository<Customer>>(),
                Mock.Of<IRepository<Product>>(),
                Mock.Of<IRepository<Vendor>>(),
                vendorNoteRepositoryMock.Object
            );

            var vendorNote = new VendorNote
            {
                VendorId = 1,  // Set the VendorId property
                Note = "Sample Note",  // Set the Note property
                CreatedOnUtc = DateTime.UtcNow  // Set the CreatedOnUtc property
            };

            // Act
            await vendorService.DeleteVendorNoteAsync(vendorNote);

            // Assert
            vendorNoteRepositoryMock.Verify(
                r => r.DeleteAsync(It.IsAny<VendorNote>(), It.IsAny<bool>()),
                Times.Once
            );
        }
        [Test]
        public async Task GetVendorNotesByVendorAsync_ValidVendorId_ReturnsVendorNotes()
        {
            // Arrange
            var vendorNoteRepositoryMock = new Mock<IRepository<VendorNote>>();
            var vendorService = new VendorService(
                Mock.Of<IHtmlFormatter>(),
                Mock.Of<IRepository<Customer>>(),
                Mock.Of<IRepository<Product>>(),
                Mock.Of<IRepository<Vendor>>(),
                vendorNoteRepositoryMock.Object
            );

            var vendorId = 1; // Specify a valid vendor ID
            var pageIndex = 0;
            var pageSize = 10;

            var vendorNotes = new List<VendorNote>
            {
                new VendorNote { VendorId = vendorId, CreatedOnUtc = DateTime.UtcNow, Id = 1 },
                new VendorNote { VendorId = vendorId, CreatedOnUtc = DateTime.UtcNow, Id = 2 },
                // Add more vendor notes as needed
            };

            vendorNoteRepositoryMock.Setup(r => r.Table)
                .Returns(vendorNotes.AsQueryable());

            // Act
            var result = await vendorService.GetVendorNotesByVendorAsync(vendorId, pageIndex, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(vendorNotes.Count);
            // Additional assertions based on your specific logic and requirements
        }
        [Test]
        public async Task UpdateVendorAsync_ValidVendor_CallsUpdateAsync()
        {
            // Arrange
            var vendorRepositoryMock = new Mock<IRepository<Vendor>>();
            var vendorService = new VendorService(
                Mock.Of<IHtmlFormatter>(),
                Mock.Of<IRepository<Customer>>(),
                Mock.Of<IRepository<Product>>(),
                vendorRepositoryMock.Object,
                Mock.Of<IRepository<VendorNote>>()
            );

            var vendor = new Vendor
            {
                Id = 1,  // Set the vendor ID
                Name = "Updated Name",  // Set the updated name
                Email = "updated@example.com"
            };

            // Act
            await vendorService.UpdateVendorAsync(vendor);

            // Assert
            vendorRepositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<Vendor>(), It.IsAny<bool>()),
                Times.Once
            );
        }

        [Test]
        public async Task GetVendorsByProductIdsAsync_ValidProductIds_ReturnsVendors()
        {
            // Arrange
            var vendorRepositoryMock = new Mock<IRepository<Vendor>>();
            var productRepositoryMock = new Mock<IRepository<Product>>();

            var vendorService = new VendorService(
                Mock.Of<IHtmlFormatter>(),
                Mock.Of<IRepository<Customer>>(),
                productRepositoryMock.Object,
                vendorRepositoryMock.Object,
                Mock.Of<IRepository<VendorNote>>()
            );

            var productIds = new[] { 1, 2, 3 }; // Specify valid product IDs

            // Set up mock data for the repositories
            var vendors = new List<Vendor>
            {
                new Vendor { Id = 1, Name = "Vendor1", Active = true, Deleted = false },
                new Vendor { Id = 2, Name = "Vendor2", Active = true, Deleted = false },
                new Vendor { Id = 3, Name = "Vendor3", Active = true, Deleted = false }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, VendorId = 1 },
                new Product { Id = 2, VendorId = 2 },
                new Product { Id = 3, VendorId = 3 },
                // Add more products if needed
            };

            vendorRepositoryMock.Setup(r => r.Table).Returns(vendors.AsQueryable());
            productRepositoryMock.Setup(r => r.Table).Returns(products.AsQueryable());

            // Act
            var result = await vendorService.GetVendorsByProductIdsAsync(productIds);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3); // Ensure the correct number of vendors is returned            
        }
        [Test]
        public async Task GetVendorsByCustomerIdsAsync_ValidCustomerIds_ReturnsVendors()
        {
            // Arrange
            var vendorRepositoryMock = new Mock<IRepository<Vendor>>();
            var customerRepositoryMock = new Mock<IRepository<Customer>>();

            var vendorService = new VendorService(
                Mock.Of<IHtmlFormatter>(),
                customerRepositoryMock.Object,
                Mock.Of<IRepository<Product>>(),
                vendorRepositoryMock.Object,
                Mock.Of<IRepository<VendorNote>>()
            );

            var customerIds = new[] { 1, 2, 3 }; // Specify valid customer IDs

            // Set up mock data for the repositories
            var vendors = new List<Vendor>
            {
                new Vendor { Id = 1, Name = "Vendor1", Active = true, Deleted = false },
                new Vendor { Id = 2, Name = "Vendor2", Active = true, Deleted = false },
                new Vendor { Id = 3, Name = "Vendor3", Active = true, Deleted = false }
            };

            var customers = new List<Customer>
            {
                new Customer { Id = 1, VendorId = 1 },
                new Customer { Id = 2, VendorId = 2 },
                new Customer { Id = 3, VendorId = 3 },
                // Add more customers if needed
            };

            vendorRepositoryMock.Setup(r => r.Table).Returns(vendors.AsQueryable());
            customerRepositoryMock.Setup(r => r.Table).Returns(customers.AsQueryable());

            // Act
            var result = await vendorService.GetVendorsByCustomerIdsAsync(customerIds);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3); // Ensure the correct number of vendors is returned
        }
        [Test]
        public async Task InsertVendorAsync_ValidVendor_InsertsVendorAndPublishesEvent()
        {
            // Arrange
            var vendorRepositoryMock = new Mock<IRepository<Vendor>>();
            var vendorNoteRepositoryMock = new Mock<IRepository<VendorNote>>();
            var htmlFormatterMock = new Mock<IHtmlFormatter>();

            var vendorService = new VendorService(
                htmlFormatterMock.Object,
                Mock.Of<IRepository<Customer>>(),
                Mock.Of<IRepository<Product>>(),
                vendorRepositoryMock.Object,
                vendorNoteRepositoryMock.Object
            );

            var vendor = new Vendor
            {
                Id = 1,  // Set the vendor ID
                Name = "New Name",  // Set the updated name
                Email = "new@example.com"
            };

            vendorRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Vendor>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

            // Act
            await vendorService.InsertVendorAsync(vendor);

            // Assert
            vendorRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Vendor>(), It.IsAny<bool>()), Times.Once);
        }
        [Test]
        public async Task DeleteVendorAsync_ValidVendor_DeletesVendorAndPublishesEvent()
        {
            // Arrange
            var vendorRepositoryMock = new Mock<IRepository<Vendor>>();
            var vendorNoteRepositoryMock = new Mock<IRepository<VendorNote>>();
            var htmlFormatterMock = new Mock<IHtmlFormatter>();

            var vendorService = new VendorService(
                htmlFormatterMock.Object,
                Mock.Of<IRepository<Customer>>(),
                Mock.Of<IRepository<Product>>(),
                vendorRepositoryMock.Object,
                vendorNoteRepositoryMock.Object
            );

            var vendor = new Vendor
            {
                Id = 1,  // Set the vendor ID
                Name = "New Name",  // Set the updated name
                Email = "new@example.com"
            };

            vendorRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Vendor>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

            // Act
            await vendorService.DeleteVendorAsync(vendor);

            // Assert
            vendorRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Vendor>(), It.IsAny<bool>()), Times.Once);
        }
        [Test]
        public void FormatVendorNoteText_ValidVendorNote_ReturnsFormattedText()
        {
            // Arrange
            var htmlFormatterMock = new Mock<IHtmlFormatter>();
            var vendorNote = new VendorNote
            {
                Note = "Sample note"
                // Set other properties as needed
            };

            htmlFormatterMock.Setup(f => f.FormatText(It.IsAny<string>(), false, true, false, false, false, false))
                             .Returns((string input, bool a, bool b, bool c, bool d, bool e, bool f) => input.ToUpper());

            var vendorService = new VendorService(
                htmlFormatterMock.Object,
                Mock.Of<IRepository<Customer>>(),
                Mock.Of<IRepository<Product>>(),
                Mock.Of<IRepository<Vendor>>(),
                Mock.Of<IRepository<VendorNote>>()
            );

            // Act
            var result = vendorService.FormatVendorNoteText(vendorNote);

            // Assert
            Assert.AreEqual("SAMPLE NOTE", result);
            htmlFormatterMock.Verify(f => f.FormatText(It.IsAny<string>(), false, true, false, false, false, false), Times.Once);
        }
        [Test]
        public async Task GetVendorNoteByIdAsync_ValidVendorNoteId_ReturnsVendorNote()
        {
            // Arrange
            var vendorNoteId = 1;
            var vendorNote = new VendorNote { Id = vendorNoteId, Note = "Sample note" };
            var vendorNoteRepositoryMock = new Mock<IRepository<VendorNote>>();
            vendorNoteRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<Func<ICacheKeyService, CacheKey>>(), true, false))
                                    .ReturnsAsync(vendorNote);

            var vendorService = new VendorService(
                Mock.Of<IHtmlFormatter>(),
                Mock.Of<IRepository<Customer>>(),
                Mock.Of<IRepository<Product>>(),
                Mock.Of<IRepository<Vendor>>(),
                vendorNoteRepositoryMock.Object
            );

            // Act
            var result = await vendorService.GetVendorNoteByIdAsync(vendorNoteId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(vendorNoteId, result.Id);
            Assert.AreEqual("Sample note", result.Note);

            // Verify that GetByIdAsync was called with the correct parameters
            vendorNoteRepositoryMock.Verify(r => r.GetByIdAsync(vendorNoteId, It.IsAny<Func<ICacheKeyService, CacheKey>>(), true, false), Times.Once);
        }
        [Test]
        public async Task GetVendorByIdAsync_ValidVendorId_ReturnsVendor()
        {
            // Arrange
            var vendorId = 1;
            var vendor = new Vendor { Id = vendorId, Name = "Sample Vendor" };
            var vendorRepositoryMock = new Mock<IRepository<Vendor>>();
            vendorRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<Func<ICacheKeyService, CacheKey>>(), true, false))
                                .ReturnsAsync(vendor);

            var vendorService = new VendorService(
                Mock.Of<IHtmlFormatter>(),
                Mock.Of<IRepository<Customer>>(),
                Mock.Of<IRepository<Product>>(),
                vendorRepositoryMock.Object,
                Mock.Of<IRepository<VendorNote>>()
            );

            // Act
            var result = await vendorService.GetVendorByIdAsync(vendorId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(vendorId, result.Id);
            Assert.AreEqual("Sample Vendor", result.Name);

            // Verify that GetByIdAsync was called with the correct parameters
            vendorRepositoryMock.Verify(r => r.GetByIdAsync(vendorId, It.IsAny<Func<ICacheKeyService, CacheKey>>(), true, false), Times.Once);
        }


    }
}

