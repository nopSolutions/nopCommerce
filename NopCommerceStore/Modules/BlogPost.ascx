<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.BlogPostControl"
    CodeBehind="BlogPost.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="BlogComment" Src="~/Modules/BlogComment.ascx" %>
<div class="blogpost">
    <div class="page-title">
        <h1><asp:Literal runat="server" ID="lBlogPostTitle" EnableViewState="false"></asp:Literal></h1>
    </div>
    <div class="clear">
    </div>
    <div class="postdate">
        <asp:Literal runat="server" ID="lCreatedOn" EnableViewState="false"></asp:Literal>
    </div>
    <div class="postbody">
        <asp:Literal runat="server" ID="lBlogPostBody" EnableViewState="false"></asp:Literal>
    </div>
    <div class="clear">
    </div>
    <div class="tags">
        <asp:Literal runat="server" ID="lTags" EnableViewState="false"></asp:Literal>
    </div>
    <div class="clear">
    </div>
    <div id="pnlComments" runat="server" class="blogcomments">
        <div class="title">
            <%=GetLocaleResourceString("Blog.Comments")%>
        </div>
        <div class="clear">
        </div>
        <div class="newcomment">
            <table>
                <tr>
                    <td colspan="2" class="leavetitle">
                        <strong>
                            <asp:Literal runat="server" ID="lblLeaveYourComment" EnableViewState="false"></asp:Literal>
                        </strong>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%=GetLocaleResourceString("Blog.CommentText")%>:
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtComment" TextMode="MultiLine" ValidationGroup="NewComment"
                            SkinID="BlogAddCommentCommentText"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvComment" runat="server" ControlToValidate="txtComment"
                            ErrorMessage="<% $NopResources:Blog.PleaseEnterCommentText %>" ToolTip="<% $NopResources:Blog.PleaseEnterCommentText %>"
                            ValidationGroup="NewComment">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr runat="server" id="pnlError">
                    <td class="message-error" colspan="2">
                        <asp:Literal ID="lErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="button">
                        <asp:Button runat="server" ID="btnComment" Text="<% $NopResources:Blog.NewCommentButton %>"
                            ValidationGroup="NewComment" OnClick="btnComment_Click" CssClass="blogpostaddcommentbutton">
                        </asp:Button>
                    </td>
                </tr>
            </table>
        </div>
        <div class="clear">
        </div>
        <div class="commentlist">
            <asp:Repeater ID="rptrComments" runat="server">
                <ItemTemplate>
                    <nopCommerce:BlogComment ID="ctrlBlogComment" runat="server" BlogComment='<%# Container.DataItem %>' />
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>
