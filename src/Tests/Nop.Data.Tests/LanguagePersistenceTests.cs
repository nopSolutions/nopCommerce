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
                LanguageCulture = "en-GB",
                FlagImageFileName = string.Empty,
                Published = true,
                DisplayOrder = 1
            };

            var fromDb = SaveAndLoadEntity(lang);
            fromDb.Name.ShouldEqual("English");
        }
    }
}
