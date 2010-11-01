<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SystemInformationControl"
    CodeBehind="SystemInformation.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-system.png" alt="<%=GetLocaleResourceString("Admin.SystemInformation.Title")%>" />
        <%=GetLocaleResourceString("Admin.SystemInformation.Title")%>
    </div>
    <div class="options">
        
    </div>
</div>
<table width="100%" class="adminContent">
    <tr>
        <td class="adminTitle">
           <%=GetLocaleResourceString("Admin.SystemInformation.nopVersion")%>
        </td>
        <td class="adminData">
            <asp:Label runat="server" ID="lblNopVersion" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <%=GetLocaleResourceString("Admin.SystemInformation.OperatingSystem")%>
        </td>
        <td class="adminData">
            <asp:Label runat="server" ID="lblOperatingSystem" />
        </td>
    </tr> 
    <tr>
        <td class="adminTitle">
            <%=GetLocaleResourceString("Admin.SystemInformation.ASPNETInfo")%>
        </td>
        <td class="adminData">
            <asp:Label runat="server" ID="lblASPNETInfo" />
        </td>
    </tr> 
    <tr>
        <td class="adminTitle">
            <%=GetLocaleResourceString("Admin.SystemInformation.IsFullTrust")%>
        </td>
        <td class="adminData">
            <asp:Label runat="server" ID="lblIsFullTrust" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <%=GetLocaleResourceString("Admin.SystemInformation.ServerTimeZone")%>
        </td>
        <td class="adminData">
            <asp:Label runat="server" ID="lblServerTimeZone" />
        </td>
    </tr> 
    <tr>
        <td class="adminTitle">
            <%=GetLocaleResourceString("Admin.SystemInformation.ServerLocalTime")%>
        </td>
        <td class="adminData">
            <asp:Label runat="server" ID="lblServerLocalTime" />
        </td>
    </tr> 
    <tr>
        <td class="adminTitle">
            <%=GetLocaleResourceString("Admin.SystemInformation.UTCTime")%>
        </td>
        <td class="adminData">
            <asp:Label runat="server" ID="lblUTCTime" />
        </td>
    </tr>
</table>
