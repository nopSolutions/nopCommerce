<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.EmailAccountInfoControl"
    CodeBehind="EmailAccountInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="EmailTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                ID="lblEmailAddress" Text="<% $NopResources:Admin.EmailAccountInfo.Email %>"
                ToolTip="<% $NopResources:Admin.EmailAccountInfo.Email.Tooltip %>" />
        </td>
        <td class="adminData">
            <nopCommerce:EmailTextBox runat="server" CssClass="adminInput" ID="txtEmailAddress">
            </nopCommerce:EmailTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                ID="lblDisplayName" Text="<% $NopResources:Admin.EmailAccountInfo.EmailDisplayName %>"
                ToolTip="<% $NopResources:Admin.EmailAccountInfo.EmailDisplayName.Tooltip %>" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtDisplayName" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                ID="lblHost" Text="<% $NopResources:Admin.EmailAccountInfo.EmailHost %>"
                ToolTip="<% $NopResources:Admin.EmailAccountInfo.EmailHost.Tooltip %>" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtHost" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                ID="lblPort" Text="<% $NopResources:Admin.EmailAccountInfo.EmailPort %>"
                ToolTip="<% $NopResources:Admin.EmailAccountInfo.EmailPort.Tooltip %>" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtPort" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                ID="lblUser" Text="<% $NopResources:Admin.EmailAccountInfo.User %>"
                ToolTip="<% $NopResources:Admin.EmailAccountInfo.User.Tooltip %>" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtUser" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                ID="lblPassword" Text="<% $NopResources:Admin.EmailAccountInfo.Password %>"
                ToolTip="<% $NopResources:Admin.EmailAccountInfo.Password.Tooltip %>" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtPassword" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                ID="lblMailSSL" Text="<% $NopResources:Admin.EmailAccountInfo.SSL %>"
                ToolTip="<% $NopResources:Admin.EmailAccountInfo.SSL.Tooltip %>" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbEnableSsl" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                ID="lblUseDefaultCredentials" Text="<% $NopResources:Admin.EmailAccountInfo.DefaultCredentials %>"
                ToolTip="<% $NopResources:Admin.EmailAccountInfo.DefaultCredentials.Tooltip %>" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUseDefaultCredentials" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <asp:PlaceHolder runat="server" ID="phTestEmail">
        <tr class="adminSeparator">
            <td colspan="2">
                <hr />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <p>
                    <strong>
                        <%=GetLocaleResourceString("Admin.EmailAccountInfo.SendTestEmail")%></strong>
                </p>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                    ID="lblTestEmailTo" Text="<% $NopResources:Admin.EmailAccountInfo.TestEmailTo %>"
                    ToolTip="<% $NopResources:Admin.EmailAccountInfo.TestEmailTo.Tooltip %>" />
            </td>
            <td class="adminData">
                <nopCommerce:EmailTextBox runat="server" CssClass="adminInput" ID="txtSendEmailTo"
                    ValidationGroup="SendTestEmail"></nopCommerce:EmailTextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
            </td>
            <td class="adminData">
                <asp:Button ID="btnSendTestEmail" runat="server" Text="<% $NopResources:Admin.EmailAccountInfo.SendTestEmailButton.Text %>"
                    CssClass="adminButton" OnClick="btnSendTestEmail_Click" ValidationGroup="SendTestEmail"
                    ToolTip="<% $NopResources:Admin.EmailAccountInfo.SendTestEmailButton.Tooltip %>" />
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
            </td>
            <td class="adminData" style="color: red">
                <asp:Label ID="lblSendTestEmailResult" runat="server" EnableViewState="false">
                </asp:Label>
            </td>
        </tr>
    </asp:PlaceHolder>
</table>
