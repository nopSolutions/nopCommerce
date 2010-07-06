<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.PollControl"
    CodeBehind="Poll.ascx.cs" %>
<div class="poll-block">
    <asp:Label ID="lblPollName" runat="server" CssClass="poll-display-text"></asp:Label>
    <asp:Panel ID="pnlTakePoll" runat="server" CssClass="poll-take-poll">
        <asp:RadioButtonList ID="rblPollAnswers" runat="server" DataTextField="Name" DataValueField="PollAnswerId">
        </asp:RadioButtonList>
        <asp:Button ID="btnSubmitVoteRecord" runat="server" OnClick="btnSubmitVoteRecord_Click"
            Text="<% $NopResources:Polls.SubmitVoteRecordButton %>" CssClass="submitpollvotebutton" />
        <asp:RequiredFieldValidator ID="rfvPollAnswers" runat="server" ControlToValidate="rblPollAnswers"
            Display="Dynamic" ErrorMessage="<% $NopResources:Polls.SelectAnOption %>" ToolTip="<% $NopResources:Polls.SelectAnOption %>"></asp:RequiredFieldValidator>
    </asp:Panel>
    <asp:Panel ID="pnlPollResults" runat="server" CssClass="poll-results">
        <asp:DataList ID="dlResults" runat="server" DataKeyField="PollAnswerId" OnItemDataBound="dlResults_ItemDataBound">
            <ItemTemplate>
                <%# Eval("Name") %>
                (<%# Eval("Count") %>
                <%=GetLocaleResourceString("Polls.votes")%>)<br />
                <asp:Image ID="imgPercentage" runat="server" Height="7px" ImageUrl="~/images/PollPercentage.jpg"
                    AlternateText="Votes" />
                <asp:Label ID="lblPercentage" runat="server"></asp:Label><br />
                <br />
            </ItemTemplate>
        </asp:DataList>
        <br />
        <br />
        <asp:Label runat="server" ID="lblTotalVotes" CssClass="poll-total-votes"></asp:Label>
    </asp:Panel>
</div>
