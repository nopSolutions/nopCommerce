<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.LiveChatBlockControl" CodeBehind="LiveChatBlock.ascx.cs" %>

<div class="block block-livechat">
    <div class="title">
        <%=GetLocaleResourceString("Content.LiveChat")%>
    </div>
    <div class="clear">
    </div>
    <div class="listbox">
        <asp:Literal runat="server" ID="lblLiveChatBtn" EnableViewState="false" />
    </div>
</div>
