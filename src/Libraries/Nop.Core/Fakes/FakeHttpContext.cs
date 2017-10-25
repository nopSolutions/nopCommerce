using System;
using System.Collections;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;

namespace Nop.Core.Fakes
{
    /// <summary>
    /// 模拟的Http上下文
    /// </summary>
    public class FakeHttpContext : HttpContextBase
    {
        private readonly HttpCookieCollection _cookies;
        private readonly NameValueCollection _formParams;
        private IPrincipal _principal;
        private readonly NameValueCollection _queryStringParams;
        private readonly string _relativeUrl;
        private readonly string _method;
        private readonly SessionStateItemCollection _sessionItems;
        private readonly NameValueCollection _serverVariables;
        private HttpResponseBase _response;
        private HttpRequestBase _request;
        private readonly IDictionary _items;

        public static FakeHttpContext Root()
        {
            return new FakeHttpContext("~/");
        }

        public FakeHttpContext(string relativeUrl, string method)
            : this(relativeUrl, method, null, null, null, null, null, null)
        {
        }

        public FakeHttpContext(string relativeUrl)
            : this(relativeUrl, null, null, null, null, null, null)
        {
        }

        public FakeHttpContext(string relativeUrl,
            IPrincipal principal, 
            NameValueCollection formParams,
            NameValueCollection queryStringParams, 
            HttpCookieCollection cookies,
            SessionStateItemCollection sessionItems, 
            NameValueCollection serverVariables)
            : this(relativeUrl, null, principal, formParams, queryStringParams, cookies, sessionItems, serverVariables)
        {
        }

        /// <summary>
        /// 构造模拟的Http上下文对象
        /// </summary>
        /// <param name="relativeUrl">相对地址</param>
        /// <param name="method"></param>
        /// <param name="principal"></param>
        /// <param name="formParams">form窗体变量集合</param>
        /// <param name="queryStringParams">当前请求的 URL 中由客户端发送的查询字符串变量集合</param>
        /// <param name="cookies"></param>
        /// <param name="sessionItems"></param>
        /// <param name="serverVariables"></param>
        public FakeHttpContext(string relativeUrl, 
            string method,
            IPrincipal principal, 
            NameValueCollection formParams,
            NameValueCollection queryStringParams, 
            HttpCookieCollection cookies,
            SessionStateItemCollection sessionItems, 
            NameValueCollection serverVariables)
        {
            _relativeUrl = relativeUrl;
            _method = method;
            _principal = principal;
            _formParams = formParams;
            _queryStringParams = queryStringParams;
            _cookies = cookies;
            _sessionItems = sessionItems;
            _serverVariables = serverVariables;

            _items = new Hashtable();
        }

        public override HttpRequestBase Request
        {
            get
            {
                return _request ??
                       new FakeHttpRequest(_relativeUrl, _method, _formParams, _queryStringParams, _cookies, _serverVariables);
            }
        }

        public void SetRequest(HttpRequestBase request)
        {
            _request = request;
        }

        public override HttpResponseBase Response
        {
            get
            {
                return _response ?? new FakeHttpResponse();
            }
        }

        public void SetResponse(HttpResponseBase response)
        {
            _response = response;
        }

        public override IPrincipal User
        {
            get { return _principal; }
            set { _principal = value; }
        }

        public override HttpSessionStateBase Session
        {
            get { return new FakeHttpSessionState(_sessionItems); }
        }

        public override IDictionary Items
        {
            get
            {
                return _items;
            }
        }


        public override bool SkipAuthorization { get; set; }

        public override object GetService(Type serviceType)
        {
            return null;
        }
    }
}