<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.BlogCommentControl"
    CodeBehind="BlogComment.ascx.cs" %>
<div class="blogcomment">
    <div class="commentinfo">
        <div class="userinfo">
            <asp:HyperLink ID="hlUser" runat="server" CssClass="username" EnableViewState="false" />
            <asp:Label ID="lblUser" runat="server" CssClass="username" EnableViewState="false" />
            <div class="avatar">
                <asp:Image runat="server" ID="imgAvatar" AlternateText="Avatar" CssClass="avatar-img" EnableViewState="false" />
            </div>
        </div>
    </div>
    <div class="commentcontent">
        <div class="commenttime">
            <%=GetLocaleResourceString("Blog.CommentCreatedOn")%>:
            <asp:Label ID="lblCreatedOn" CssClass="statvalue" runat="server" EnableViewState="false" />
        </div>
        <div class="commentbody">
            <asp:Label ID="lblComment" CssClass="commenttext" runat="server" EnableViewState="false"></asp:Label>
            <asp:Label ID="lblBlogCommentId" runat="server" Visible="false" EnableViewState="false"></asp:Label>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
