<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.BlogControl"
    CodeBehind="Blog.ascx.cs" %>
<div class="blog">
    <div class="page-title">
        <table width="100%">
            <tr>
                <td style="text-align: left; vertical-align: middle;">
                    <h1>
                        <asp:Literal runat="server" ID="lTitle" /></h1>
                </td>
                <td style="text-align: right; vertical-align: middle;">
                    <a href="<%= GetBlogRSSUrl()%>">
                        <asp:Image ID="imgRSS" runat="server" ImageUrl="~/images/icon_rss.gif" ToolTip="<% $NopResources:BlogRSS.Tooltip %>"
                            AlternateText="RSS" EnableViewState="false" />
                    </a>
                </td>
            </tr>
        </table>
    </div>
    <div class="clear">
    </div>
    <div class="blogposts">
        <asp:Repeater ID="rptrBlogPosts" runat="server" OnItemDataBound="rptrBlogPosts_ItemDataBound" EnableViewState="false">
            <ItemTemplate>
                <div class="post">
                    <a class="blogtitle" href="<%#SEOHelper.GetBlogPostUrl(Convert.ToInt32(Eval("BlogPostId")))%>">
                        <%#Server.HtmlEncode(Eval("BlogPostTitle").ToString())%></a><span class="blogdate">
                            -
                            <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString("D")%>
                        </span>
                    <div class="blogbody">
                        <%#Eval("BlogPostBody")%>
                    </div>
                    <div class="clear">
                    </div>
                    <div class="tags">
                        <%#RenderBlogTags((BlogPost)Container.DataItem)%>
                    </div>
                    <a href="<%#SEOHelper.GetBlogPostUrl(Convert.ToInt32(Eval("BlogPostId")))%>" class="blogdetails">
                        <asp:Literal ID="lComments" runat="server"></asp:Literal>
                    </a>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div class="pager">
        <nopCommerce:Pager runat="server" ID="postsPager" QueryStringProperty="p" FirstButtonText="<% $NopResources:Pager.First %>"
            LastButtonText="<% $NopResources:Pager.Last %>" NextButtonText="<% $NopResources:Pager.Next %>"
            PreviousButtonText="<% $NopResources:Pager.Previous %>" CurrentPageText="Pager.CurrentPage" EnableViewState="false" />
    </div>
</div>
