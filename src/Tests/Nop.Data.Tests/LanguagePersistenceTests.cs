using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class LanguagePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_language()
        {
            var lang = new Language
            {
                Name = "English",
                LanguageCulture = "en-Us",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };

            var fromDb = SaveAndLoadEntity(lang);
            fromDb.Name.ShouldEqual("English");
            fromDb.LanguageCulture.ShouldEqual("en-Us");
            fromDb.FlagImageFileName.ShouldEqual("us.png");
            fromDb.Published.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);
        }
    }
}
