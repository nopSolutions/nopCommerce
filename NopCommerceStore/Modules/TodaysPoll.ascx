<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.TodaysPollControl" Codebehind="TodaysPoll.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="Poll" Src="~/Modules/Poll.ascx" %>
<div class="todays-poll-box">
    <div class="title">
        <%=GetLocaleResourceString("Polls.TodaysPoll")%>
    </div>
    <div class="clear">
    </div>
    <div class="poll-item">
        <asp:Repeater runat="server" ID="rptPollBlocks">
            <ItemTemplate>
                <nopCommerce:Poll ID="PollControl" runat="server" PollId='<%#Eval("PollId")%>' />
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
