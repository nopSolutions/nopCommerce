<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.NewsCommentControl"
    CodeBehind="NewsComment.ascx.cs" %>
<div class="newscomment">
    <div class="commentinfo">
        <div class="userinfo">
            <asp:HyperLink ID="hlUser" runat="server" CssClass="username" />
            <asp:Label ID="lblUser" runat="server" CssClass="username" />
            <div class="avatar">
                <asp:Image runat="server" ID="imgAvatar" AlternateText="Avatar" CssClass="avatar-img" />
            </div>
        </div>
    </div>
    <div class="commentcontent">
        <div class="commenttime">
            <%=GetLocaleResourceString("News.CommentCreatedOn")%>:
            <asp:Label ID="lblCreatedOn" CssClass="statvalue" runat="server" />
        </div>
        <div class="commenttitle">
            <asp:Label ID="lblTitle" CssClass="commenttext" runat="server"></asp:Label>
        </div>
        <div class="commentbody">
            <asp:Label ID="lblComment" CssClass="commenttext" runat="server"></asp:Label>
            <asp:Label ID="lblNewsCommentId" runat="server" Visible="false"></asp:Label>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
