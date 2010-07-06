<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ForumSearchControl"
    CodeBehind="ForumSearch.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumLastPost" Src="~/Modules/ForumLastPost.ascx" %>
<div class="forum-search-panel">
    <div class="page-title">
        <h1><%=GetLocaleResourceString("Forum.Search")%></h1>
    </div>
    <div class="clear">
    </div>
    <div class="search-input">
        <asp:TextBox runat="server" ID="txtSearchTerm" SkinID="ForumSearchText" />&nbsp;
        <asp:Button runat="server" ID="btnSearch" OnClick="btnSearch_Click" Text="<% $NopResources:Forum.SearchButton %>"
            CssClass="forumsearchbutton" />
    </div>
    <div class="search-error">
        <asp:Label runat="server" ID="lblError" EnableViewState="false"></asp:Label>
    </div>
    <div class="clear">
    </div>
    <div class="pager">
        <nopCommerce:Pager runat="server" ID="searchPager1" QueryStringProperty="p" FirstButtonText="<% $NopResources:Pager.First %>"
            LastButtonText="<% $NopResources:Pager.Last %>" NextButtonText="<% $NopResources:Pager.Next %>"
            PreviousButtonText="<% $NopResources:Pager.Previous %>" CurrentPageText="Pager.CurrentPage" />
    </div>
    <div class="search-results">
        <asp:Label runat="server" ID="lblNoResults" Text="<% $NopResources:Forum.SearchNoResultsText %>"
            Visible="false" CssClass="result" />
        <div class="topicsgroup">
            <asp:Repeater ID="rptrSearchResults" runat="server" OnItemDataBound="rptrSearchResults_ItemDataBound">
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
                            <asp:Label runat="server" ID="lblTopicType" CssClass="topictype" />
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
    <div class="clear">
    </div>
    <div class="pager">
        <nopCommerce:Pager runat="server" ID="searchPager2" QueryStringProperty="p" FirstButtonText="<% $NopResources:Pager.First %>"
            LastButtonText="<% $NopResources:Pager.Last %>" NextButtonText="<% $NopResources:Pager.Next %>"
            PreviousButtonText="<% $NopResources:Pager.Previous %>" CurrentPageText="Pager.CurrentPage" />
    </div>
</div>
