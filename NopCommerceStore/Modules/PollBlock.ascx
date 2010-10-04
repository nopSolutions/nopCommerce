<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Modules.PollBlockControl" Codebehind="PollBlock.ascx.cs" %>
    <%@ Register TagPrefix="nopCommerce" TagName="Poll" Src="~/Modules/Poll.ascx" %>

<div class="block block-poll">
    <div class="title">
        <%=GetLocaleResourceString("PollBlock.Title")%>
    </div>
    <div class="clear">
    </div>
    <div class="listbox">
        <nopCommerce:Poll ID="ctrlPoll" runat="server" />
    </div>
</div>
