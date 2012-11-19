<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="$rootnamespace$.Mobile.Default" %>
<%@ Import Namespace="System.Collections" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Welcome to the Mobile Home Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Welcome to the Mobile Home Page</h1>
        <% 
            try
            {
                var version = System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion();
                if (version.StartsWith("v2"))
                { %>
            <p style="color: Red;"><b>
            IMPORTANT: The NuGet version of 51Degrees.mobi requires .NET v4 or greater to run automatically.
            Please change your project to run under .NET v4 then uninstall and reinstall the 51Degrees.mobi NuGet package. 
            See the <a href="http://51degrees.mobi/Support/Documentation/Foundation/UserGuide.aspx">User Guide</a> for more information.
            </b></p>    
        <%  } }
            catch { }
        %>
        <p>Here's some information about the requesting device:</p>
        <ul>
            <li>Screen Width: <% =Request.Browser.ScreenPixelsWidth %></li>
            <li>Screen Height: <% =Request.Browser.ScreenPixelsHeight %></li>
            <li>LayoutEngine: <% =Request.Browser["LayoutEngine"] %></li>
            <li>AnimationTiming: <% =Request.Browser["AnimationTiming"] %></li>
            <li>BlobBuilder: <% =Request.Browser["BlobBuilder"] %></li>
            <li>CssBackground: <% =Request.Browser["CssBackground"] %></li>
            <li>CssBorderImage: <% =Request.Browser["CssBorderImage"] %></li>
            <li>CssCanvas: <% =Request.Browser["CssCanvas"] %></li>
            <li>CssColor: <% =Request.Browser["CssColor"] %></li>
            <li>CssColumn: <% =Request.Browser["CssColumn"] %></li>
            <li>CssFlexbox: <% =Request.Browser["CssFlexbox"] %></li>
            <li>CssFont: <% =Request.Browser["CssFont"] %></li>
            <li>CssMediaQueries: <% =Request.Browser["CssMediaQueries"] %></li>
            <li>CssMinMax: <% =Request.Browser["CssMinMax"] %></li>
            <li>CssOverflow: <% =Request.Browser["CssOverflow"] %></li>
            <li>CssPosition: <% =Request.Browser["CssPosition"] %></li>
            <li>CssText: <% =Request.Browser["CssText"] %></li>
            <li>CssTransforms: <% =Request.Browser["CssTransforms"] %></li>
            <li>CssTransitions: <% =Request.Browser["CssTransitions"] %></li>
            <li>CssUI: <% =Request.Browser["CssUI"] %></li>
            <li>DataSet: <% =Request.Browser["DataSet"] %></li>
            <li>DataUrl: <% =Request.Browser["DataUrl"] %></li>
            <li>DeviceOrientation: <% =Request.Browser["DeviceOrientation"] %></li>
            <li>FileReader: <% =Request.Browser["FileReader"] %></li>
            <li>FileSaver: <% =Request.Browser["FileSaver"] %></li>
            <li>FileWriter: <% =Request.Browser["FileWriter"] %></li>
            <li>FormData: <% =Request.Browser["FormData"] %></li>
            <li>Fullscreen: <% =Request.Browser["Fullscreen"] %></li>
            <li>GeoLocation: <% =Request.Browser["GeoLocation"] %></li>
            <li>History: <% =Request.Browser["History"] %></li>
            <li>Html5: <% =Request.Browser["Html5"] %></li>
            <li>Html-Media-Capture: <% =Request.Browser["Html-Media-Capture"] %></li>
            <li>Iframe: <% =Request.Browser["Iframe"] %></li>
            <li>IndexedDB: <% =Request.Browser["IndexedDB"] %></li>
            <li>Json: <% =Request.Browser["Json"] %></li>
            <li>Masking: <% =Request.Browser["Masking"] %></li>
            <li>PostMessage: <% =Request.Browser["PostMessage"] %></li>
            <li>PreferencesForFrames: <% =Request.Browser["PreferencesForFrames"] %></li>
            <li>Progress: <% =Request.Browser["Progress"] %></li>
            <li>Prompts: <% =Request.Browser["Prompts"] %></li>
            <li>Selector: <% =Request.Browser["Selector"] %></li>
            <li>Svg: <% =Request.Browser["Svg"] %></li>
            <li>TouchEvents: <% =Request.Browser["TouchEvents"] %></li>
            <li>Track: <% =Request.Browser["Track"] %></li>
            <li>Video: <% =Request.Browser["Video"] %></li>
            <li>Viewport: <% =Request.Browser["Viewport"] %></li>
        </ul>
        <p>See <a href="http://msdn.microsoft.com/en-us/library/system.web.httpbrowsercapabilities_properties.aspx" title="MSDN HttpBrowserCapabilities Documentation">MSDN</a> for details of Request.Browser properties.</p>
        <p>See <a href="http://51degrees.codeplex.com/documentation" title="51Degrees.mobi Documentation">51Degrees.mobi</a> for user guide.</p>
		<p>See <a href="http://51degrees.mobi/Products/DeviceData.aspx" title="51Degrees.mobi Device Data">51Degrees.mobi Device Data</a> for details of other data properties and to get weekly data updates.</p>
        <hr />
        <% if (Request.Browser.IsMobileDevice == false) { %>
            <p>The requesting device isn't a mobile. The page must have been requested directly.</p>
            <p>Try accessing the web site from a mobile device, or a mobile device emulator. A list of popular mobile emulators can be found <a href="http://51degrees.mobi/Support/FAQs/MobileEmulators.aspx" title="Mobile Emulators">here</a>.</p>
        <% } %>
        <p>Data Published: <% =FiftyOne.Foundation.Mobile.Detection.Factory.ActiveProvider.PublishedDate %></p>
    </div>
    </form>
</body>
</html>