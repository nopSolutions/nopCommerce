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
        <p>Here's some information about the requesting device:</p>
        <ul>
            <li>Manufacturer: <% =Request.Browser.MobileDeviceManufacturer %></li>
            <li>Model: <% =Request.Browser.MobileDeviceModel %></li>
            <li>Screen Width: <% =Request.Browser.ScreenPixelsWidth %></li>
            <li>Screen Height: <% =Request.Browser.ScreenPixelsHeight %></li>
            <li>Tablet Device: <% =Request.Browser["is_tablet"] %></li>
            <li>Pointing Method: <% =Request.Browser["pointing_method"] %></li>
        </ul>
        <p>See <a href="http://msdn.microsoft.com/en-us/library/system.web.httpbrowsercapabilities_properties.aspx" title="MSDN HttpBrowserCapabilities Documentation">MSDN</a> for details of Request.Browser properties.</p>
        <p>See <a href="http://wurfl.sourceforge.net/help_doc.php" title="WURFL Capabilities Documentation">WURFL</a> for details of other WURFL capabilities.</p>
        <p>See <a href="http://51degrees.codeplex.com/documentation" title="51Degrees.mobi Documentation">51Degrees.mobi</a> for user guide.</p>
        <hr />
        <% if (Request.Browser.IsMobileDevice == false) { %>
            <p>The requesting device isn't a mobile. The page must have been requested directly.</p>
            <p>Try accessing the web site from a mobile device, or a mobile device emulator. A list of popular mobile emulators can be found <a href="http://51degrees.mobi/Support/FAQs/MobileEmulators.aspx" title="Mobile Emulators">here</a>.</p>
        <% } %>
    </div>
    </form>
</body>
</html>

