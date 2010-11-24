<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CustomerAvatarControl"CodeBehind="CustomerAvatar.ascx.cs" %>
<div class="customer-avatar">
    <asp:Panel runat="server" ID="pnlCustomerAvatarError" CssClass="error-block">
        <div class="message-error">
            <asp:Literal ID="lCustomerAvatarErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
        </div>
    </asp:Panel>
    <div class="clear">
    </div>
    <div class="section-body">
        <asp:Image ID="iAvatar" runat="server" AlternateText="Avatar" />
        <br />
        <asp:FileUpload ID="fuAvatar" runat="server" ToolTip="Choose a new avatar image to upload." />
    </div>
    <div class="clear">
    </div>
    <div class="button">
        <asp:Button ID="btnUploadAvatar" runat="server" OnClick="btnUploadAvatar_Click" CssClass="uploadavatarbutton"
            Text="<% $NopResources:Account.UploadAvatar %>" />
        <asp:Button ID="btnRemoveAvatar" runat="server" OnClick="btnRemoveAvatar_Click" CssClass="removeavatarbutton"
            Text="<% $NopResources:Account.RemoveAvatar %>" CausesValidation="false" />
    </div>
    <div class="clear">
    </div>
    <div>
        <%=GetLocaleResourceString("Account.UploadAvatarRules")%></div>
</div>
