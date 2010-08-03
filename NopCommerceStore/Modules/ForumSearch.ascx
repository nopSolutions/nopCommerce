<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ForumSearchControl"
    CodeBehind="ForumSearch.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumLastPost" Src="~/Modules/ForumLastPost.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumSelector" Src="~/Modules/ForumSelector.ascx" %>
<script type="text/javascript">
    $(document).ready(function () {
        toggleAdvancedSearch();
    });

    function toggleAdvancedSearch() {
        if (getE('<%=cbAdvancedSearch.ClientID %>').checked) {
            $('#<%=pnlAdvancedSearch.ClientID %>').show();
        }
        else {
            $('#<%=pnlAdvancedSearch.ClientID %>').hide();
        }
    }

</script>
<div class="forum-search-panel">
    <div class="page-title">
        <h1>
            <%=GetLocaleResourceString("Forum.Search")%></h1>
    </div>
    <div class="clear">
    </div>
    <div class="search-input">
        <asp:TextBox runat="server" ID="txtSearchTerm" SkinID="ForumSearchText" />&nbsp;
        <asp:Button runat="server" ID="btnSearch" OnClick="btnSearch_Click" Text="<% $NopResources:Forum.SearchButton %>"
            CssClass="forumsearchbutton" />
        <br />
        <table width="100%">
            <tbody>
                <tr>
                    <td class="title" colspan="2">
                        <asp:CheckBox runat="server" ID="cbAdvancedSearch" Text="<% $NopResources:ForumSearch.AdvancedSearch %>" />
                    </td>
                </tr>
                <tr>
                    <td class="title" colspan="2">
                        <table class="adv-search" runat="server" id="pnlAdvancedSearch">
                            <tbody>
                                <tr>
                                    <td class="title">
                                        <%=GetLocaleResourceString("ForumSearch.SearchInForum")%>
                                    </td>
                                    <td class="data">
                                        <nopCommerce:ForumSelector ID="ctrlForumSelector" runat="server" RootItemText="<% $NopResources:ForumSearch.SearchInForum.All %>" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="title">
                                        <%=GetLocaleResourceString("ForumSearch.SearchWithin")%>
                                    </td>
                                    <td class="data">
                                        <asp:DropDownList ID="ddlSearchWithin" runat="server">
                                            <asp:ListItem Text="<% $NopResources:ForumSearch.SearchWithin.All %>" Value="0" />
                                            <asp:ListItem Text="<% $NopResources:ForumSearch.SearchWithin.TopicTitlesOnly %>"
                                                Value="10" />
                                            <asp:ListItem Text="<% $NopResources:ForumSearch.SearchWithin.PostTextOnly %>"
                                                Value="20" />
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="title">
                                        <%=GetLocaleResourceString("ForumSearch.LimitResultsToPrevious")%>
                                    </td>
                                    <td class="data">
                                        <asp:DropDownList ID="ddlLimitResultsToPrevious" runat="server">
                                            <asp:ListItem Text="<% $NopResources:ForumSearch.LimitResultsToPrevious.AllResults %>"
                                                Value="0" />
                                            <asp:ListItem Text="<% $NopResources:ForumSearch.LimitResultsToPrevious.1day %>"
                                                Value="1" />
                                            <asp:ListItem Text="<% $NopResources:ForumSearch.LimitResultsToPrevious.7days %>"
                                                Value="7" />
                                            <asp:ListItem Text="<% $NopResources:ForumSearch.LimitResultsToPrevious.2weeks %>"
                                                Value="14" />
                                            <asp:ListItem Text="<% $NopResources:ForumSearch.LimitResultsToPrevious.1month %>"
                                                Value="30" />
                                            <asp:ListItem Text="<% $NopResources:ForumSearch.LimitResultsToPrevious.3months %>"
                                                Value="92" />
                                            <asp:ListItem Text="<% $NopResources:ForumSearch.LimitResultsToPrevious.6months %>"
                                                Value="183" />
                                            <asp:ListItem Text="<% $NopResources:ForumSearch.LimitResultsToPrevious.1year %>"
                                                Value="365" />
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
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
