<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ForumTopicControl"
    CodeBehind="ForumTopic.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumPost" Src="~/Modules/ForumPost.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumBreadcrumb" Src="~/Modules/ForumBreadcrumb.ascx" %>
<div class="forumtopic">
    <nopCommerce:ForumBreadcrumb ID="ctrlForumBreadcrumb" runat="server" />
    <div class="title">
        <h2 class="topicname"><asp:Label ID="lblTopicSubject" runat="server" /></h2>
        <div class="manage">
            <asp:LinkButton runat="server" ID="btnEdit" Text="<% $NopResources:Forum.EditTopic %>"
                OnClick="btnEdit_Click" CssClass="edittopiclinkbutton" />
            <asp:LinkButton runat="server" ID="btnDelete" Text="<% $NopResources:Forum.DeleteTopic %>"
                OnClick="btnDelete_Click" CssClass="deletetopiclinkbutton" />
            <asp:LinkButton runat="server" ID="btnMoveTopic" Text="<% $NopResources:Forum.MoveTopic %>"
                OnClick="btnMoveTopic_Click" CssClass="movetopiclinkbutton" />
        </div>
    </div>
    <div class="clear">
    </div>
    <div class="topicheader">
        <div class="topicoptions">
            <asp:LinkButton runat="server" ID="btnReply" Text="<% $NopResources:Forum.Reply %>"
                OnClick="btnReply_Click" CssClass="replytopiclinkbutton" />
            <asp:LinkButton runat="server" ID="btnWatchTopic" Text="Watch topic" OnClick="btnWatchTopic_Click"
                CssClass="watchtopiclinkbutton" />
        </div>
        <div class="pager">
            <nopCommerce:Pager runat="server" ID="postsPager1" QueryStringProperty="p" FirstButtonText="<% $NopResources:Pager.First %>"
                LastButtonText="<% $NopResources:Pager.Last %>" NextButtonText="<% $NopResources:Pager.Next %>"
                PreviousButtonText="<% $NopResources:Pager.Previous %>" CurrentPageText="Pager.CurrentPage" />
        </div>
    </div>
    <div class="clear">
    </div>
    <div class="posts">
        <asp:Repeater ID="rptrPosts" runat="server">
            <ItemTemplate>
                <nopCommerce:ForumPost ID="ctrlForumPost" runat="server" ForumPost='<%# Container.DataItem %>' />
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
<div class="topicfooter">
    <div class="topicoptions">
        <asp:LinkButton runat="server" ID="btnReply2" Text="<% $NopResources:Forum.Reply %>"
            OnClick="btnReply_Click" CssClass="replytopiclinkbutton" />
        <asp:LinkButton runat="server" ID="btnWatchTopic2" Text="Watch topic" OnClick="btnWatchTopic_Click"
            CssClass="watchtopiclinkbutton" />
    </div>
    <div class="pager">
        <nopCommerce:Pager runat="server" ID="postsPager2" QueryStringProperty="p" FirstButtonText="<% $NopResources:Pager.First %>"
            LastButtonText="<% $NopResources:Pager.Last %>" NextButtonText="<% $NopResources:Pager.Next %>"
            PreviousButtonText="<% $NopResources:Pager.Previous %>" CurrentPageText="Pager.CurrentPage" />
    </div>
</div>
<div class="clear">
</div>
