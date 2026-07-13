using System.Collections.Specialized;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Nop.Services.Helpers;

namespace Nop.Web.Framework;

/// <summary>
/// Represents a RemotePost helper class
/// </summary>
public partial class RemotePost
{
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IWebHelper _webHelper;

    /// <summary>
    /// Gets or sets a remote URL
    /// </summary>
    public string Url { get; set; }
    
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
    /// Params
    /// </summary>
    public NameValueCollection Params { get; }

    /// <summary>
    /// Creates a new instance of the RemotePost class
    /// </summary>
    /// <param name="httpContextAccessor">HTTP Context accessor</param>
    /// <param name="webHelper">Web helper</param>
    public RemotePost(IHttpContextAccessor httpContextAccessor, IWebHelper webHelper)
    {
        Params = new NameValueCollection();
        Url = "http://www.someurl.com";
        FormName = "formName";

        _httpContextAccessor = httpContextAccessor;
        _webHelper = webHelper;
    }

    /// <summary>
    /// Adds the specified key and value to the dictionary (to be posted).
    /// </summary>
    /// <param name="name">The key of the element to add</param>
    /// <param name="value">The value of the element to add.</param>
    public virtual void Add(string name, string value)
    {
        Params.Add(name, value);
    }

    /// <summary>
    /// Post
    /// </summary>
    public virtual async Task PostAsync()
    {
        //text
        var sb = new StringBuilder();
        sb.Append("<html><head>");
        sb.Append($"</head><body onload=\"document.{FormName}.submit()\">");
        
        if (!string.IsNullOrEmpty(AcceptCharset))
        {
            //AcceptCharset specified
            sb.Append(
                $"<form name=\"{FormName}\" method=\"{HttpMethod.Post}\" action=\"{Url}\" accept-charset=\"{AcceptCharset}\">");
        }
        else
        {
            //no AcceptCharset specified
            sb.Append($"<form name=\"{FormName}\" method=\"{HttpMethod.Post}\" action=\"{Url}\" >");
        }

        if (NewInputForEachValue)
        {
            foreach (string key in Params.Keys)
            {
                var values = Params.GetValues(key);

                if (values == null)
                    continue;

                foreach (var value in values)
                {
                    sb.Append(
                        $"<input name=\"{WebUtility.HtmlEncode(key)}\" type=\"hidden\" value=\"{WebUtility.HtmlEncode(value)}\">");
                }
            }
        }
        else
        {
            for (var i = 0; i < Params.Keys.Count; i++)
            {
                sb.Append(
                    $"<input name=\"{WebUtility.HtmlEncode(Params.Keys[i])}\" type=\"hidden\" value=\"{WebUtility.HtmlEncode(Params[Params.Keys[i]])}\">");
            }
        }
        sb.Append("</form>");
        sb.Append("</body></html>");

        var data = Encoding.UTF8.GetBytes(sb.ToString());

        //modify the response
        var httpContext = _httpContextAccessor.HttpContext;
        var response = httpContext?.Response ?? throw new NullReferenceException("Can't access response");
        
        //We do not use the response.Clear() method because it deletes the .NopCustomer cookie, which is important for guest orders.
        //This behavior leads to the problem of a new guest user getting created after returning from the payment gateway.
        //So we manually clear only the body and status.
        //If some problem will start with this approach in the future,
        //we can check the changes on the response.Clear() method and then use it in our code.
        response.StatusCode = 200;
        response.HttpContext.Features.GetRequiredFeature<IHttpResponseFeature>().ReasonPhrase = null;

        if (response.Body.CanSeek)
            response.Body.SetLength(0);

        response.ContentType = "text/html; charset=utf-8";
        response.ContentLength = data.Length;

        await response.Body.WriteAsync(data, 0, data.Length);

        //store a value indicating whether POST has been done
        _webHelper.IsPostBeingDone = true;
    }
}