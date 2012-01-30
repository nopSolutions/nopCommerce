using Nop.Services.Seo;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Seo
{
    [TestFixture]
    public class SeoExtensionsTests
    {
        [Test]
        public void Should_return_lowercase()
        {
            SeoExtensions.GetSeName("tEsT", false, false).ShouldEqual("test");
        }

        [Test]
        public void Should_allow_all_latin_chars()
        {
            SeoExtensions.GetSeName("abcdefghijklmnopqrstuvwxyz1234567890", false, false).ShouldEqual("abcdefghijklmnopqrstuvwxyz1234567890");
        }

        [Test]
        public void Should_remove_illegal_chars()
        {
            SeoExtensions.GetSeName("test!@#$%^&*()+<>?/", false, false).ShouldEqual("test");
        }

        [Test]
        public void Should_replace_space_with_dash()
        {
            SeoExtensions.GetSeName("test test", false, false).ShouldEqual("test-test");
            SeoExtensions.GetSeName("test     test", false, false).ShouldEqual("test-test");
        }

        [Test]
        public void Can_convert_non_western_chars()
        {
            //german letters with diacritics
            SeoExtensions.GetSeName("testäöü", true, false).ShouldEqual("testaou");
            SeoExtensions.GetSeName("testäöü", false, false).ShouldEqual("test");
        }

        [Test]
        public void Can_allow_unicode_chars()
        {
            //russian letters
            SeoExtensions.GetSeName("testтест", true, true).ShouldEqual("testтест");
            SeoExtensions.GetSeName("testтест", true, false).ShouldEqual("test");
        }

        [Test]
        public void Can_encode_for_url()
        {
            //russian letters т should be encoded as "%d1%82"
            SeoExtensions.GetSeName("testт", true, true, true).ShouldEqual("test%d1%82");
        }
    }
}



