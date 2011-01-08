using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class LocaleStringResourcePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_lst()
        {
            var lst = new LocaleStringResource
            {
                ResourceName = "ResourceName1",
                ResourceValue = "ResourceValue2",
                Language = new Language()
                               {
                                   Name = "English",
                                   LanguageCulture = "en-Us",
                                   FlagImageFileName = "us.png",
                                   Published = true,
                                   DisplayOrder = 1
                               }
            };

            var fromDb = SaveAndLoadEntity(lst);
            fromDb.ResourceName.ShouldEqual("ResourceName1");
            fromDb.ResourceValue.ShouldEqual("ResourceValue2");

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }
    }
}
