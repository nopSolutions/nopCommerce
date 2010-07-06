<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ForumControl"
    CodeBehind="Forum.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumLastPost" Src="~/Modules/ForumLastPost.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumBreadcrumb" Src="~/Modules/ForumBreadcrumb.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumSearchBox" Src="~/Modules/ForumSearchBox.ascx" %>
<div class="forum">
    <nopCommerce:ForumBreadcrumb ID="ctrlForumBreadcrumb" runat="server" />
    <div class="top">
        <div class="foruminfo">
            <asp:Label ID="lblForumName" runat="server" CssClass="forumname" />
            <br />
            <asp:Label ID="lblForumDescription" runat="server" CssClass="forumdescription" />
        </div>
        <div class="actions">
            <asp:HyperLink ID="hlNewTopic" runat="server" Text="<% $NopResources:Forum.NewTopic %>"
                CssClass="newtopic" />
            <asp:LinkButton runat="server" ID="btnWatchForum" Text="Watch forum" OnClick="btnWatchForum_Click"
                CssClass="watchforum" />
            <br />
            <nopCommerce:ForumSearchBox ID="ctrlForumSearchBox" runat="server" />
        </div>
    </div>
    <div class="clear">
    </div>
    <div class="pager">
        <nopCommerce:Pager runat="server" ID="topicsPager1" QueryStringProperty="p" FirstButtonText="<% $NopResources:Pager.First %>"
            LastButtonText="<% $NopResources:Pager.Last %>" NextButtonText="<% $NopResources:Pager.Next %>"
            PreviousButtonText="<% $NopResources:Pager.Previous %>" CurrentPageText="Pager.CurrentPage" />
    </div>
    <div class="topicsgroup">
        <asp:Repeater ID="rptrTopics" runat="server" OnItemDataBound="rptrTopics_ItemDataBound">
            <HeaderTemplate>
                <table class="topics">
                    <tr class="forumheader">
                        <td class="topicname" colspan="2">
                            <%#GetLocaleResourceString("Forum.TopicTitle")%>
                        </td>
                        <td class="replies">
                            <%#GetLocaleResourceString("Forum.Replies")%>
                        </td>
                        <td class="views">
                            <%#GetLocaleResourceString("Forum.Views")%>
                        </td>
                        <td class="lastpost">
                            <%#GetLocaleResourceString("Forum.LatestPost")%>
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="topic">
                    <td>
                        <asp:Panel runat="server" ID="pnlTopicImage" />
                    </td>
                    <td class="topicname">
                        <asp:Label runat="server" ID="lblTopicType" CssClass="topictype"></asp:Label>
                        <asp:HyperLink ID="hlTopic" runat="server" CssClass="topictitle" />
                        <asp:Literal ID="lPager" runat="server"></asp:Literal>
                        <br />
                        <span class="topicstarter">
                            <asp:Label runat="server" ID="lblAuthor" Text="<% $NopResources:Forum.Author %>" />:
                            <asp:HyperLink ID="hlTopicStarter" runat="server" />
                            <asp:Label ID="lblTopicStarter" runat="server" />
                        </span>
                    </td>
                    <td class="replies">
                        <%#Eval("NumReplies")%>
                    </td>
                    <td class="views">
                        <%#Eval("Views")%>
                    </td>
                    <td class="lastpost">
                        <nopCommerce:ForumLastPost ID="ctrlForumLastPost" runat="server" ForumPost='<%#Eval("LastPost")%>' />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div class="clear">
    </div>
    <div class="pager">
        <nopCommerce:Pager runat="server" ID="topicsPager2" QueryStringProperty="p" FirstButtonText="<% $NopResources:Pager.First %>"
            LastButtonText="<% $NopResources:Pager.Last %>" NextButtonText="<% $NopResources:Pager.Next %>"
            PreviousButtonText="<% $NopResources:Pager.Previous %>" CurrentPageText="Pager.CurrentPage" />
    </div>
</div>
