<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.HomePagePollControl" Codebehind="HomePagePoll.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="Poll" Src="~/Modules/Poll.ascx" %>
<div class="todays-poll-box">
    <div class="poll-item">
        <asp:Repeater runat="server" ID="rptPollBlocks">
            <ItemTemplate>
                <nopCommerce:Poll ID="PollControl" runat="server" PollId='<%#Eval("PollId")%>' />
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
