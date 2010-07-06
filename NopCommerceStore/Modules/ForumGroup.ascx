<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ForumGroupControl"
    CodeBehind="ForumGroup.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumLastPost" Src="~/Modules/ForumLastPost.ascx" %>
<div class="forumgroup">
    <div class="grouptitle">
        <asp:HyperLink runat="server" ID="hlForumGroup"></asp:HyperLink>
    </div>
    <table class="groups">
        <tr class="groupheader">
            <td class="forumname" colspan="2">
                <%=GetLocaleResourceString("Forum.Forum")%>
            </td>
            <td class="topics">
                <%=GetLocaleResourceString("Forum.Topics")%>
            </td>
            <td class="posts">
                <%=GetLocaleResourceString("Forum.Posts")%>
            </td>
            <td class="lastpost">
                <%=GetLocaleResourceString("Forum.LatestPost")%>
            </td>
        </tr>
        <asp:Repeater ID="rptrForumList" runat="server">
            <ItemTemplate>
                <tr class="forum">
                    <td class="image">
                        <div />
                    </td>
                    <td class="forumname">
                        <span class="forumtitle"><a href="<%#SEOHelper.GetForumUrl(Convert.ToInt32(Eval("ForumId")))%>">
                            <%#Server.HtmlEncode(Eval("Name").ToString())%>
                        </a></span>
                        <br />
                        <span class="forumdescription">
                            <%#Server.HtmlEncode(Eval("Description").ToString())%>
                        </span>
                    </td>
                    <td class="topics">
                        <%#Eval("NumTopics")%>
                    </td>
                    <td class="posts">
                        <%#Eval("NumPosts")%>
                    </td>
                    <td class="lastpost">
                        <nopCommerce:ForumLastPost ID="ctrlForumLastPost" runat="server" ShowTopic="true"
                            ForumPost='<%#Eval("LastPost")%>' />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</div>
