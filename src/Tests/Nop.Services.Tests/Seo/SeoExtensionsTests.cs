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
            _urlRecordService.GetSeName("tEsT", false, false).ShouldEqual("test");
        }

        [Test]
        public void Should_allow_all_latin_chars()
        {
            _urlRecordService.GetSeName("abcdefghijklmnopqrstuvwxyz1234567890", false, false).ShouldEqual("abcdefghijklmnopqrstuvwxyz1234567890");
        }

        [Test]
        public void Should_remove_illegal_chars()
        {
            _urlRecordService.GetSeName("test!@#$%^&*()+<>?/", false, false).ShouldEqual("test");
        }

        [Test]
        public void Should_replace_space_with_dash()
        {
            _urlRecordService.GetSeName("test test", false, false).ShouldEqual("test-test");
            _urlRecordService.GetSeName("test     test", false, false).ShouldEqual("test-test");
        }

        [Test]
        public void Can_convert_non_western_chars()
        {
            //German letters with diacritics
            _urlRecordService.GetSeName("testäöü", true, false).ShouldEqual("testaou");
            _urlRecordService.GetSeName("testäöü", false, false).ShouldEqual("test");
        }

        [Test]
        public void Can_allow_unicode_chars()
        {
            //Russian letters
            _urlRecordService.GetSeName("testтест", false, true).ShouldEqual("testтест");
            _urlRecordService.GetSeName("testтест", false, false).ShouldEqual("test");
        }
    }
}



