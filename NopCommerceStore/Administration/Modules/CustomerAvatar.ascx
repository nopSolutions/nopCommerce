<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerAvatarControl" CodeBehind="CustomerAvatar.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>

<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerAvatarImage" Text="<% $NopResources:Admin.CustomerAvatar.Image %>"
                ToolTip="<% $NopResources:Admin.CustomerAvatar.Image.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Image ID="imgAvatar" runat="server" AlternateText="Avatar" />
            <br />
            <asp:FileUpload ID="uplAvatar" runat="server" ToolTip="Choose a new avatar image to upload." />
            <br />
            <asp:Button ID="btnUploadAvatar" runat="server" OnClick="btnUploadAvatar_Click" Text="<% $NopResources:Admin.CustomerAvatar.UploadAvatar %>" />
            <br style="line-height: 6px;" />
            <%=GetLocaleResourceString("Admin.CustomerAvatar.UploadAvatarRules")%>
            <br style="line-height: 6px;" />
            <asp:Button ID="btnRemoveAvatar" runat="server" OnClick="btnRemoveAvatar_Click" Text="<% $NopResources:Admin.CustomerAvatar.RemoveAvatar %>" CausesValidation="false" />
        </td>
    </tr>
</table>