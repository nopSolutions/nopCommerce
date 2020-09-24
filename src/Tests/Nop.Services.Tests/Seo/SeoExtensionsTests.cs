using FluentAssertions;
using Nop.Services.Seo;
using NUnit.Framework;

namespace Nop.Services.Tests.Seo
{
    [TestFixture]
    public class SeoExtensionsTests : ServiceTest
    {
        private IUrlRecordService _urlRecordService;

        [SetUp]
        public void SetUp()
        {
            _urlRecordService = GetService<IUrlRecordService>();
        }

        [Test]
        public void ShouldReturnLowercase()
        {
            _urlRecordService.GetSeName("tEsT", false, false).Should().Be("test");
        }

        [Test]
        public void ShouldAllowAllLatinChars()
        {
            _urlRecordService.GetSeName("abcdefghijklmnopqrstuvwxyz1234567890", false, false).Should()
                .Be("abcdefghijklmnopqrstuvwxyz1234567890");
        }

        [Test]
        public void ShouldRemoveIllegalChars()
        {
            _urlRecordService.GetSeName("test!@#$%^&*()+<>?/", false, false).Should().Be("test");
        }

        [Test]
        public void ShouldReplaceSpaceWithDash()
        {
            _urlRecordService.GetSeName("test test", false, false).Should().Be("test-test");
            _urlRecordService.GetSeName("test     test", false, false).Should().Be("test-test");
        }

        [Test]
        public void CanConvertNonWesternChars()
        {
            //German letters with diacritics
            _urlRecordService.GetSeName("testäöü", true, false).Should().Be("testaou");
            _urlRecordService.GetSeName("testäöü", false, false).Should().Be("test");
        }

        [Test]
        public void CanAllowUnicodeChars()
        {
            //Russian letters
            _urlRecordService.GetSeName("testтест", false, true).Should().Be("testтест");
            _urlRecordService.GetSeName("testтест", false, false).Should().Be("test");
        }
    }
}