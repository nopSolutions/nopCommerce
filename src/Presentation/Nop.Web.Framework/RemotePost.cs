using System.Collections.Specialized;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework
{
    /// <summary>
    /// Represents a RemotePost helper class
    /// </summary>
    public partial class RemotePost
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly NameValueCollection _inputValues;

        /// <summary>
        /// Gets or sets a remote URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets a form name
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// Gets or sets a form character-sets the server can handle for form-data.
        /// </summary>
        public string AcceptCharset { get; set; }

        /// <summary>
        /// A value indicating whether we should create a new "input" HTML element for each value (in case if there are more than one) for the same "name" attributes.
        /// </summary>
        public bool NewInputForEachValue { get; set; }

        /// <summary>
        /// Parames
        /// </summary>
        public NameValueCollection Params
        {
            get
            {
                return _inputValues;
            }
        }

        /// <summary>
        /// Creates a new instance of the RemotePost class
        /// </summary>
        public RemotePost()
            : this(EngineContext.Current.Resolve<IHttpContextAccessor>(), EngineContext.Current.Resolve<IWebHelper>())
        {
        }

        /// <summary>
        /// Creates a new instance of the RemotePost class
        /// </summary>
        /// <param name="httpContextAccessor">HTTP Context accessor</param>
        /// <param name="webHelper">Web helper</param>
        public RemotePost(IHttpContextAccessor httpContextAccessor, IWebHelper webHelper)
        {
            _inputValues = new NameValueCollection();
            Url = "http://www.someurl.com";
            Method = "post";
            FormName = "formName";

            _httpContextAccessor = httpContextAccessor;
            _webHelper = webHelper;
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary (to be posted).
        /// </summary>
        /// <param name="name">The key of the element to add</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(string name, string value)
        {
            _inputValues.Add(name, value);
        }
        
        /// <summary>
        /// Post
        /// </summary>
        public void Post()
        {
            //text
            var sb = new StringBuilder();
            sb.Append("<html><head>");
            sb.Append($"</head><body onload=\"document.{FormName}.submit()\">");
            if (!string.IsNullOrEmpty(AcceptCharset))
            {
                //AcceptCharset specified
                sb.Append(
                    $"<form name=\"{FormName}\" method=\"{Method}\" action=\"{Url}\" accept-charset=\"{AcceptCharset}\">");
            }
            else
            {
                //no AcceptCharset specified
                sb.Append($"<form name=\"{FormName}\" method=\"{Method}\" action=\"{Url}\" >");
            }
            if (NewInputForEachValue)
            {
                foreach (string key in _inputValues.Keys)
                {
                    var values = _inputValues.GetValues(key);
                    if (values != null)
                    {
                        foreach (var value in values)
                        {
                            sb.Append(
                                $"<input name=\"{WebUtility.HtmlEncode(key)}\" type=\"hidden\" value=\"{WebUtility.HtmlEncode(value)}\">");
                        }
                    }
                }
            }
            else
            {
                for (var i = 0; i < _inputValues.Keys.Count; i++)
                    sb.Append(
                        $"<input name=\"{WebUtility.HtmlEncode(_inputValues.Keys[i])}\" type=\"hidden\" value=\"{WebUtility.HtmlEncode(_inputValues[_inputValues.Keys[i]])}\">");
            }
            sb.Append("</form>");
            sb.Append("</body></html>");


            //post
            var httpContext = _httpContextAccessor.HttpContext;
            var response = httpContext.Response;
            response.Clear();
            var data = Encoding.UTF8.GetBytes(sb.ToString());
            response.ContentType = "text/html; charset=utf-8";
            response.ContentLength = data.Length;

            response.Body.Write(data, 0, data.Length);

            //store a value indicating whether POST has been done
            _webHelper.IsPostBeingDone = true;
        }
    }
}