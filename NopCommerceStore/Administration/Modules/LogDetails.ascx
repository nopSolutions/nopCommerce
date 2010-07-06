<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.LogDetailsControl"
    CodeBehind="LogDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-system.png" alt="<%=GetLocaleResourceString("Admin.LogDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.LogDetails.Title")%>
        <a href="Logs.aspx" title="<%=GetLocaleResourceString("Admin.LogDetails.BackToLog")%>">
            (<%=GetLocaleResourceString("Admin.LogDetails.BackToLog")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.LogDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.LogDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLogTypeTitle" Text="<% $NopResources:Admin.LogDetails.LogType %>"
                ToolTip="<% $NopResources:Admin.LogDetails.LogType.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblLogType" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSeverityTitle" Text="<% $NopResources:Admin.LogDetails.Severity %>"
                ToolTip="<% $NopResources:Admin.LogDetails.Severity.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblSeverity" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMessageTitle" Text="<% $NopResources:Admin.LogDetails.Message %>"
                ToolTip="<% $NopResources:Admin.LogDetails.Message.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblExceptionTitle" Text="<% $NopResources:Admin.LogDetails.Exception %>"
                ToolTip="<% $NopResources:Admin.LogDetails.Exception.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblException" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblIPAddressTitle" Text="<% $NopResources:Admin.LogDetails.IPAddress %>"
                ToolTip="<% $NopResources:Admin.LogDetails.IPAddress.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblIPAddress" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerTitle" Text="<% $NopResources:Admin.LogDetails.Customer %>"
                ToolTip="<% $NopResources:Admin.LogDetails.Customer.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCustomer" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPageURLTitle" Text="<% $NopResources:Admin.LogDetails.PageURL %>"
                ToolTip="<% $NopResources:Admin.LogDetails.PageURL.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblPageURL" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblReferrerURLTitle" Text="<% $NopResources:Admin.LogDetails.ReferrerURL %>"
                ToolTip="<% $NopResources:Admin.LogDetails.ReferrerURL.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblReferrerURL" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTitle" Text="<% $NopResources:Admin.LogDetails.CreatedOn %>"
                ToolTip="<% $NopResources:Admin.LogDetails.CreatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCreatedOn" runat="server"></asp:Label>
        </td>
    </tr>
</table>
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
