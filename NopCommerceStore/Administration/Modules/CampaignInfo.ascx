<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CampaignInfoControl"
    CodeBehind="CampaignInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="EmailTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register Assembly="NopCommerceStore" Namespace="NopSolutions.NopCommerce.Web.Controls"
    TagPrefix="nopCommerce" %>
    
    <div runat="server" id="pnlSendCampaign">
    <table class="adminContent">
        <tr>
            <td colspan="2">
                <b>
                    <%=GetLocaleResourceString("Admin.CampaignInfo.Title")%>
                </b>
                <br />
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                    ID="lblSendTestEmailTo" Text="<% $NopResources:Admin.CampaignInfo.SendTestEmailTo %>"
                    ToolTip="<% $NopResources:Admin.CampaignInfo.SendTestEmailTo.Tooltip %>" />
            </td>
            <td class="adminData">
                <nopCommerce:EmailTextBox runat="server" CssClass="adminInput" ID="txtSendTestEmailTo"
                    ValidationGroup="SendTestEmail"></nopCommerce:EmailTextBox>
                &nbsp;&nbsp;<asp:Button ID="btnSendTestEmail" runat="server" Text="<% $NopResources:Admin.CampaignInfo.SendTestEmailButton.Text %>"
                    CssClass="adminButton" OnClick="btnSendTestEmail_Click" ValidationGroup="SendTestEmail"
                    ToolTip="<% $NopResources:Admin.CampaignInfo.SendTestEmailButton.Tooltip %>" />
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                    ID="lblAllowedTokensTitle" Text="<% $NopResources:Admin.CampaignInfo.AllowedTokens %>"
                    ToolTip="<% $NopResources:Admin.CampaignInfo.AllowedTokens.Tooltip %>" />
            </td>
            <td class="adminData">
                <asp:Label ID="lblAllowedTokens" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
            </td>
            <td class="adminData">
                <asp:Button ID="btnSendMassEmail" runat="server" Text="<% $NopResources:Admin.CampaignInfo.SendMassEmailButton.Text %>"
                    CssClass="adminButton" OnClick="btnSendMassEmail_Click" ValidationGroup="SendMassEmail"
                    ToolTip="<% $NopResources:Admin.CampaignInfo.SendMassEmailButton.Tooltip %>" />
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
            </td>
            <td class="adminData" style="color: red">
                <asp:Label ID="lblSendEmailResult" runat="server" EnableViewState="false">
                </asp:Label>
            </td>
        </tr>
    </table>
    <hr />
</div>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                ID="lblCampaignName" Text="<% $NopResources:Admin.CampaignInfo.CampaignName %>"
                ToolTip="<% $NopResources:Admin.CampaignInfo.CampaignName.Tooltip %>" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtName" ErrorMessage="<% $NopResources:Admin.CampaignInfo.CampaignName.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                ID="lblSubject" Text="<% $NopResources:Admin.CampaignInfo.Subject %>" ToolTip="<% $NopResources:Admin.CampaignInfo.Subject.Tooltip %>" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtSubject" ErrorMessage="<% $NopResources:Admin.CampaignInfo.Subject.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                ID="lblBody" Text="<% $NopResources:Admin.CampaignInfo.Body %>" ToolTip="<% $NopResources:Admin.CampaignInfo.Body.Tooltip %>" />
        </td>
        <td class="adminData">
            <nopCommerce:NopHTMLEditor ID="txtBody" runat="server" Height="350" />
        </td>
    </tr>
    <tr runat="server" id="pnlCreatedOn">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTitle" Text="<% $NopResources:Admin.CampaignInfo.CreatedOn %>"
                ToolTip="<% $NopResources:Admin.CampaignInfo.CreatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCreatedOn" runat="server"></asp:Label>
        </td>
    </tr>
</table>
