using System.Collections.Specialized;
using System.Web;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework
{
    /// <summary>
    /// Represents a RemotePost helper class
    /// </summary>
    public partial class RemotePost
    {
        private readonly HttpContextBase _httpContext;
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
        /// A value indicating whether we should create a new "input" HTML element for each value (in case if there are more than one) for the same "name" attributes.
        /// </summary>
        public bool NewInputForEachValue { get; set; }

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
        {
            _inputValues = new NameValueCollection();
            Url = "http://www.someurl.com";
            Method = "post";
            FormName = "formName";
            //TODO inject 
            _httpContext = EngineContext.Current.Resolve<HttpContextBase>();
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
            _httpContext.Response.Clear();
            _httpContext.Response.Write("<html><head>");
            _httpContext.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
            _httpContext.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
            if (NewInputForEachValue)
            {
                foreach (string key in _inputValues.Keys)
                {
                    foreach (string value in _inputValues[key].Split(new char[] { ',' }))
                        _httpContext.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", HttpUtility.HtmlEncode(key), HttpUtility.HtmlEncode(value)));
                }
            }
            else
            {
                for (int i = 0; i < _inputValues.Keys.Count; i++)
                    _httpContext.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", HttpUtility.HtmlEncode(_inputValues.Keys[i]), HttpUtility.HtmlEncode(_inputValues[_inputValues.Keys[i]])));
            }
            _httpContext.Response.Write("</form>");
            _httpContext.Response.Write("</body></html>");
            _httpContext.Response.End();
        }
    }
}