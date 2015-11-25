using System;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Vendors;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Vendors
{
    [TestFixture]
    public class VendorNotePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_vendorNote()
        {
            var on = new VendorNote
            {
                Vendor = GetTestVendor(),
                Note = "Note1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
            };

            var fromDb = SaveAndLoadEntity(on);
            fromDb.ShouldNotBeNull();
            fromDb.Note.ShouldEqual("Note1");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.Vendor.ShouldNotBeNull();
        }
        
        protected Vendor GetTestVendor()
        {
            return new Vendor
            {
                Name = "Name 1",
                Email = "Email 1",
                Description = "Description 1",
                AdminComment = "AdminComment 1",
                PictureId = 1,
                Active = true,
                Deleted = true,
                DisplayOrder = 2,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
            };
        }

    }
}