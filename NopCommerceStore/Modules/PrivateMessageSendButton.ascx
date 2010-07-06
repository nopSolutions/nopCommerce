<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.PrivateMessageSendButtonControl"
    CodeBehind="PrivateMessageSendButton.ascx.cs" %>
<div class="sendpmbox">
    <asp:LinkButton runat="server" ID="btnSendPM" Text="<% $NopResources:PrivateMessages.PM %>"
        OnClick="btnSendPM_Click" CssClass="pmlinkbutton" />
</div>
