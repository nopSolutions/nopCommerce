<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ACLControl"
    CodeBehind="ACL.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ACL.Title")%>" />
        <%=GetLocaleResourceString("Admin.ACL.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.ACL.SaveButton.Text %>" CssClass="adminButtonBlue"
            ID="btnSave" ValidationGroup="ACLSettings" OnClick="btnSave_Click" ToolTip="<% $NopResources:Admin.ACL.SaveButton.Tooltip %>" /><nopCommerce:ConfirmationBox
                runat="server" ID="cbSave" TargetControlID="btnSave" YesText="<% $NopResources:Admin.Common.Yes %>"
                NoText="<% $NopResources:Admin.Common.No %>" ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
    </div>
</div>
<table width="100%">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblACLEnabled" Text="<% $NopResources:Admin.ACL.ACLEnabled %>"
                ToolTip="<% $NopResources:Admin.ACL.ACLEnabled.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbACLEnabled"></asp:CheckBox>
        </td>
    </tr>
</table>
<table class="adminContent">
    <tr>
        <td>
            <asp:GridView ID="gvACL" runat="server" AutoGenerateColumns="False" Width="100%">
            </asp:GridView>
            <br />
            <asp:Label runat="server" ID="lblMessage"></asp:Label>
        </td>
    </tr>
</table>
