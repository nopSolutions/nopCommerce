<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ForumActiveDiscussionsControl"
    CodeBehind="ForumActiveDiscussions.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumLastPost" Src="~/Modules/ForumLastPost.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumBreadcrumb" Src="~/Modules/ForumBreadcrumb.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumSearchBox" Src="~/Modules/ForumSearchBox.ascx" %>
<div class="activediscussions">
    <div class="activediscussionstitle">
        <%=GetLocaleResourceString("Forum.ActiveDiscussions")%>
        <%if (!this.HideViewAllLink)
          { %>
        <div class="viewall">
            [<a href="<%= SEOHelper.GetForumActiveDiscussionsUrl() %>"><%=GetLocaleResourceString("Forum.ActiveDiscussions.ViewAll")%></a>]
        </div>
        <%} %>
    </div>
    <div class="activediscussionstopics">
        <asp:Repeater ID="rptrTopics" runat="server" OnItemDataBound="rptrTopics_ItemDataBound">
            <HeaderTemplate>
                <table class="topics">
                    <tr class="activediscussionsheader">
                        <td class="topicname">
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
                    <td class="topicname">
                        <asp:HyperLink ID="hlTopic" runat="server" CssClass="topictitle" />
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
</div>
