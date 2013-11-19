using System.Collections.Specialized;
using System.Web;
using Nop.Core.Fakes;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests
{
    [TestFixture]
    public class WebHelperTests
    {
        private HttpContextBase _httpContext;
        private IWebHelper _webHelper;

        [Test]
        public void Can_get_serverVariables()
        {
            var serverVariables = new NameValueCollection();
            serverVariables.Add("Key1", "Value1");
            serverVariables.Add("Key2", "Value2");
            _httpContext = new FakeHttpContext("~/", "GET", null, null, null, null, null, serverVariables);
            _webHelper = new WebHelper(_httpContext);
            _webHelper.ServerVariables("Key1").ShouldEqual("Value1");
            _webHelper.ServerVariables("Key2").ShouldEqual("Value2");
            _webHelper.ServerVariables("Key3").ShouldEqual("");
        }

        [Test]
        public void Can_get_storeHost_without_ssl()
        {
            var serverVariables = new NameValueCollection();
            serverVariables.Add("HTTP_HOST", "www.example.com");
            _httpContext = new FakeHttpContext("~/", "GET", null, null, null, null, null, serverVariables);
            _webHelper = new WebHelper(_httpContext);
            _webHelper.GetStoreHost(false).ShouldEqual("http://www.example.com/");
        }

        [Test]
        public void Can_get_storeHost_with_ssl()
        {
            var serverVariables = new NameValueCollection();
            serverVariables.Add("HTTP_HOST", "www.example.com");
            _httpContext = new FakeHttpContext("~/", "GET", null, null, null, null, null, serverVariables);
            _webHelper = new WebHelper(_httpContext);
            _webHelper.GetStoreHost(true).ShouldEqual("https://www.example.com/");
        }

        [Test]
        public void Can_get_storeLocation_without_ssl()
        {
            var serverVariables = new NameValueCollection();
            serverVariables.Add("HTTP_HOST", "www.example.com");
            _httpContext = new FakeHttpContext("~/", "GET", null, null, null, null, null, serverVariables);
            _webHelper = new WebHelper(_httpContext);
            _webHelper.GetStoreLocation(false).ShouldEqual("http://www.example.com/");
        }

        [Test]
        public void Can_get_storeLocation_with_ssl()
        {
            var serverVariables = new NameValueCollection();
            serverVariables.Add("HTTP_HOST", "www.example.com");
            _httpContext = new FakeHttpContext("~/", "GET", null, null, null, null, null, serverVariables);
            _webHelper = new WebHelper(_httpContext);
            _webHelper.GetStoreLocation(true).ShouldEqual("https://www.example.com/");
        }

        [Test]
        public void Can_get_storeLocation_in_virtual_directory()
        {
            var serverVariables = new NameValueCollection();
            serverVariables.Add("HTTP_HOST", "www.example.com");
            _httpContext = new FakeHttpContext("~/nopCommercepath", "GET", null, null, null, null, null, serverVariables);
            _webHelper = new WebHelper(_httpContext);
            _webHelper.GetStoreLocation(false).ShouldEqual("http://www.example.com/nopcommercepath/");
        }

        [Test]
        public void Get_storeLocation_should_return_lowerCased_result()
        {
            var serverVariables = new NameValueCollection();
            serverVariables.Add("HTTP_HOST", "www.Example.com");
            _httpContext = new FakeHttpContext("~/", "GET", null, null, null, null, null, serverVariables);
            _webHelper = new WebHelper(_httpContext);
            _webHelper.GetStoreLocation(false).ShouldEqual("http://www.example.com/");
        }
        
        [Test]
        public void Can_get_queryString()
        {
            var queryStringParams = new NameValueCollection();
            queryStringParams.Add("Key1", "Value1");
            queryStringParams.Add("Key2", "Value2");
            _httpContext = new FakeHttpContext("~/", "GET", null, null, queryStringParams, null, null, null);
            _webHelper = new WebHelper(_httpContext);
            _webHelper.QueryString<string>("Key1").ShouldEqual("Value1");
            _webHelper.QueryString<string>("Key2").ShouldEqual("Value2");
            _webHelper.QueryString<string>("Key3").ShouldEqual(null);
        }
        
        [Test]
        public void Can_remove_queryString()
        {
            _httpContext = new FakeHttpContext("~/", "GET", null, null, null, null, null, null);
            _webHelper = new WebHelper(_httpContext);
            //first param (?)
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value2", "param1")
                .ShouldEqual("http://www.example.com/?param2=value2");
            //second param (&)
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value2", "param2")
                .ShouldEqual("http://www.example.com/?param1=value1");
            //non-existing param
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value2", "param3")
                .ShouldEqual("http://www.example.com/?param1=value1&param2=value2");
        }

        [Test]
        public void Can_remove_queryString_should_return_lowerCased_result()
        {
            _httpContext = new FakeHttpContext("~/", "GET", null, null, null, null, null, null);
            _webHelper = new WebHelper(_httpContext);
            _webHelper.RemoveQueryString("htTp://www.eXAmple.com/?param1=value1&parAm2=value2", "paRAm1")
                .ShouldEqual("http://www.example.com/?param2=value2");
        }

        [Test]
        public void Can_remove_queryString_should_ignore_input_parameter_case()
        {
            _httpContext = new FakeHttpContext("~/", "GET", null, null, null, null, null, null);
            _webHelper = new WebHelper(_httpContext);
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&parAm2=value2", "paRAm1")
                .ShouldEqual("http://www.example.com/?param2=value2");
        }

        [Test]
        public void Can_modify_queryString()
        {
            _httpContext = new FakeHttpContext("~/", "GET", null, null, null, null, null, null);
            _webHelper = new WebHelper(_httpContext);
            //first param (?)
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value2", "param1=value3", null)
                .ShouldEqual("http://www.example.com/?param1=value3&param2=value2");
            //second param (&)
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value2", "param2=value3", null)
                .ShouldEqual("http://www.example.com/?param1=value1&param2=value3");
            //non-existing param
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value2", "param3=value3", null)
                .ShouldEqual("http://www.example.com/?param1=value1&param2=value2&param3=value3");
        }

        [Test]
        public void Can_modify_queryString_with_anchor()
        {
            _httpContext = new FakeHttpContext("~/", "GET", null, null, null, null, null, null);
            _webHelper = new WebHelper(_httpContext);
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value2", "param1=value3", "Test")
                .ShouldEqual("http://www.example.com/?param1=value3&param2=value2#test");
        }

        [Test]
        public void Can_modify_queryString_new_anchor_should_remove_previous_one()
        {
            _httpContext = new FakeHttpContext("~/", "GET", null, null, null, null, null, null);
            _webHelper = new WebHelper(_httpContext);
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value2#test1", "param1=value3", "Test2")
                .ShouldEqual("http://www.example.com/?param1=value3&param2=value2#test2");
        }
    }
}
