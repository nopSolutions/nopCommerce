<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.MessageTemplateDetailsControl"
    CodeBehind="MessageTemplateDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<%@ Register Assembly="NopCommerceStore" Namespace="NopSolutions.NopCommerce.Web.Controls"
    TagPrefix="nopCommerce" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.MessageTemplateDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.MessageTemplateDetails.Title")%>
        <a href="MessageTemplates.aspx" title="<%=GetLocaleResourceString("Admin.MessageTemplateDetails.BackToTemplates")%>">
            (<%=GetLocaleResourceString("Admin.MessageTemplateDetails.BackToTemplates")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.MessageTemplateDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.MessageTemplateDetails.SaveButton.Tooltip %>" />
    </div>
</div>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <strong>
                <nopcommerce:tooltiplabel runat="server" id="lblAllowedTokensTitle" text="<% $NopResources:Admin.MessageTemplateDetails.AllowedTokens %>"
                    tooltip="<% $NopResources:Admin.MessageTemplateDetails.AllowedTokens.Tooltip %>"
                    tooltipimage="~/Administration/Common/ico-help.gif" />
            </strong>
        </td>
        <td class="adminData">
            <br />
            <asp:Label ID="lblAllowedTokens" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblName" text="<% $NopResources:Admin.MessageTemplateDetails.Name %>"
                tooltip="<% $NopResources:Admin.MessageTemplateDetails.Name.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblTemplate" runat="server"></asp:Label>
        </td>
    </tr>
</table>
<div id="localizablecontentpanel" class="tabcontainer-usual">
    <ul class="idTabs">
        <asp:Repeater ID="rptrLanguageTabs" runat="server">
            <ItemTemplate>
                <li class="idTab"><a href="#idTab_Info<%# Container.ItemIndex+2 %>">
                    <asp:Image runat="server" ID="imgCFlag" Visible='<%# !String.IsNullOrEmpty(Eval("IconURL").ToString()) %>'
                        AlternateText='<%#Eval("Name")%>' ImageUrl='<%#Eval("IconURL").ToString()%>' />
                    <%#Server.HtmlEncode(Eval("Name").ToString())%></a></li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
    <asp:Repeater ID="rptrLanguageDivs" runat="server" OnItemDataBound="rptrLanguageDivs_ItemDataBound">
        <ItemTemplate>
            <div id="idTab_Info<%# Container.ItemIndex+2 %>" class="tab">
                <asp:Label ID="lblLanguageId" runat="server" Text='<%#Eval("LanguageId") %>' Visible="false"></asp:Label>
                <table class="adminContent">                    
                    <tr>
                        <td class="adminTitle">
                            <nopcommerce:tooltiplabel runat="server" id="lblEmailAccount" text="<% $NopResources:Admin.MessageTemplateDetails.EmailAccount %>"
                                tooltip="<% $NopResources:Admin.MessageTemplateDetails.EmailAccount.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:DropDownList runat="server" ID="ddlEmailAccount" />
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopcommerce:tooltiplabel runat="server" id="lblBCCEmailAddresses" text="<% $NopResources:Admin.MessageTemplateDetails.BCCEmailAddresses %>"
                                tooltip="<% $NopResources:Admin.MessageTemplateDetails.BCCEmailAddresses.Tooltip %>"
                                tooltipimage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtBCCEmailAddresses" CssClass="adminInput" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopcommerce:tooltiplabel runat="server" id="lblSubject" text="<% $NopResources:Admin.MessageTemplateDetails.Subject %>"
                                tooltip="<% $NopResources:Admin.MessageTemplateDetails.Subject.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" CssClass="adminInput" ID="txtSubject" />
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopcommerce:tooltiplabel runat="server" id="lblBody" text="<% $NopResources:Admin.MessageTemplateDetails.Body %>"
                                tooltip="<% $NopResources:Admin.MessageTemplateDetails.Body.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopcommerce:nophtmleditor id="txtBody" runat="server" height="350" />
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopcommerce:tooltiplabel runat="server" id="lblActive" text="<% $NopResources:Admin.MessageTemplateDetails.Active %>"
                                tooltip="<% $NopResources:Admin.MessageTemplateDetails.Active.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:CheckBox ID="cbActive" runat="server" Checked="true" />
                        </td>
                    </tr>
                </table>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>