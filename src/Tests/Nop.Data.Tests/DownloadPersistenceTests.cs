using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Media;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class DownloadPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_download()
        {
            var download = new Download
            {
                UseDownloadUrl= true,
                DownloadUrl = "http://www.someUrl.com/file.zip",
                DownloadBinary = new byte[] { 1, 2, 3 },
                ContentType = "application/x-zip-co",
                Filename = "file",
                Extension = ".zip",
                IsNew = true
            };

            var fromDb = SaveAndLoadEntity(download);
            fromDb.ShouldNotBeNull();
            fromDb.UseDownloadUrl.ShouldEqual(true);
            fromDb.DownloadUrl.ShouldEqual("http://www.someUrl.com/file.zip");
            fromDb.DownloadBinary.ShouldEqual(new byte[] { 1, 2, 3 });
            fromDb.ContentType.ShouldEqual("application/x-zip-co");
            fromDb.Filename.ShouldEqual("file");
            fromDb.Extension.ShouldEqual(".zip");
            fromDb.IsNew.ShouldEqual(true);
        }
    }
}
