using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Moq;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using NUnit.Framework;

namespace Nop.Core.Tests
{
    [TestFixture]
    public class WebHelperTests
    {
        private DefaultHttpContext _httpContext;
        private Mock<IActionContextAccessor> _actionContextAccessor;
        private Mock<IHostApplicationLifetime> _applicationLifetime;
        private Mock<INopFileProvider> _fileProvider;
        private Mock<IUrlHelperFactory> _urlHelperFactory;
        private IWebHelper _webHelper;

        [SetUp]
        public void SetUp()
        {
            _httpContext = new DefaultHttpContext();
            var queryString = new QueryString("");
            queryString = queryString.Add("Key1", "Value1");
            queryString = queryString.Add("Key2", "Value2");
            _httpContext.Request.QueryString = queryString;
            _httpContext.Request.Headers.Add(HeaderNames.Host, "www.Example.com");

            _actionContextAccessor = new Mock<IActionContextAccessor>();
            _applicationLifetime = new Mock<IHostApplicationLifetime>();
            _fileProvider = new Mock<INopFileProvider>();
            _urlHelperFactory = new Mock<IUrlHelperFactory>();
            _actionContextAccessor.Setup(x => x.ActionContext).Returns(new ActionContext(_httpContext, new RouteData(), new ActionDescriptor()));
            _urlHelperFactory.Setup(x => x.GetUrlHelper(_actionContextAccessor.Object.ActionContext))
                .Returns(new UrlHelper(_actionContextAccessor.Object.ActionContext));

            _webHelper = new WebHelper(new HostingConfig(), _actionContextAccessor.Object, _applicationLifetime.Object, new FakeHttpContextAccessor(_httpContext), _fileProvider.Object, _urlHelperFactory.Object);
        }

        [Test]
        public void Can_get_storeHost_without_ssl()
        {
            _webHelper.GetStoreHost(false).Should().Be("http://www.Example.com/");
        }

        [Test]
        public void Can_get_storeHost_with_ssl()
        {
            _webHelper.GetStoreHost(true).Should().Be("https://www.Example.com/");
        }

        [Test]
        public void Can_get_storeLocation_without_ssl()
        {
            _webHelper.GetStoreLocation(false).Should().Be("http://www.Example.com/");
        }

        [Test]
        public void Can_get_storeLocation_with_ssl()
        {
            _webHelper.GetStoreLocation(true).Should().Be("https://www.Example.com/");
        }

        [Test]
        public void Can_get_storeLocation_in_virtual_directory()
        {
            _httpContext.Request.PathBase = "/nopCommercepath";
            _webHelper.GetStoreLocation(false).Should().Be("http://www.Example.com/nopCommercepath/");
        }

        [Test]
        public void Can_get_queryString()
        {
            _webHelper.QueryString<string>("Key1").Should().Be("Value1");
            _webHelper.QueryString<string>("Key2").Should().Be("Value2");
            _webHelper.QueryString<string>("Key3").Should().Be(null);
        }

        [Test]
        public void Can_remove_queryString()
        {
            //empty URL
            _webHelper.RemoveQueryString(null, null).Should().Be(string.Empty);
            //empty key
            _webHelper.RemoveQueryString("http://www.example.com/", null).Should().Be("http://www.example.com/");
            //non-existing param with fragment
            _webHelper.RemoveQueryString("http://www.example.com/#fragment", "param").Should().Be("http://www.example.com/#fragment");
            //first param (?)
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value1", "param1")
                .Should().Be("http://www.example.com/?param2=value1");
            //second param (&)
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value1", "param2")
                .Should().Be("http://www.example.com/?param1=value1");
            //non-existing param
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value1", "param3")
                .Should().Be("http://www.example.com/?param1=value1&param2=value1");
            //with fragment
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value1#fragment", "param1")
                .Should().Be("http://www.example.com/?param2=value1#fragment");
            //specific value
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param1=value2&param2=value1", "param1", "value1")
                .Should().Be("http://www.example.com/?param1=value2&param2=value1");
            //all values
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param1=value2&param2=value1", "param1")
                .Should().Be("http://www.example.com/?param2=value1");
        }

        [Test]
        public void Can_modify_queryString()
        {
            //empty URL
            _webHelper.ModifyQueryString(null, null).Should().Be(string.Empty);
            //empty key
            _webHelper.ModifyQueryString("http://www.example.com/", null).Should().Be("http://www.example.com/");
            //empty value
            _webHelper.ModifyQueryString("http://www.example.com/", "param").Should().Be("http://www.example.com/?param=");
            //first param (?)
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value1", "Param1", "value2")
                .Should().Be("http://www.example.com/?param1=value2&param2=value1");
            //second param (&)
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value1", "param2", "value2")
                .Should().Be("http://www.example.com/?param1=value1&param2=value2");
            //non-existing param
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value1", "param3", "value1")
                .Should().Be("http://www.example.com/?param1=value1&param2=value1&param3=value1");
            //multiple values
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value1", "param1", "value1", "value2", "value3")
                .Should().Be("http://www.example.com/?param1=value1,value2,value3&param2=value1");
            //with fragment
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value1#fragment", "param1", "value2")
                .Should().Be("http://www.example.com/?param1=value2&param2=value1#fragment");
        }

        [Test]
        public void Can_modify_queryString_in_virtual_directory()
        {
            _httpContext.Request.PathBase = "/nopCommercepath";
            _webHelper.ModifyQueryString("/nopCommercepath/Controller/Action", "param1", "value1").Should().Be("/nopCommercepath/Controller/Action?param1=value1");
        }
    }

    public class FakeHttpContextAccessor : IHttpContextAccessor
    {
        public FakeHttpContextAccessor(HttpContext httpContext)
        {
            HttpContext = httpContext;
        }

        public HttpContext HttpContext { get; set; }
    }
}
