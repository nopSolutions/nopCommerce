<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.NewsItemControl"
    CodeBehind="NewsItem.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NewsComment" Src="~/Modules/NewsComment.ascx" %>
<div class="newsitem">
    <div class="page-title">
        <h1><asp:Literal runat="server" ID="lTitle" EnableViewState="false"></asp:Literal></h1>
    </div>
    <div class="clear">
    </div>
    <div class="newsdate">
        <asp:Literal runat="server" ID="lCreatedOn" EnableViewState="false"></asp:Literal>
    </div>
    <div class="newsbody">
        <asp:Literal runat="server" ID="lFull" EnableViewState="false"></asp:Literal>
    </div>
    <div id="pnlComments" runat="server" class="newscomments">
        <div class="title">
            <%=GetLocaleResourceString("News.Comments")%>
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
                        <%=GetLocaleResourceString("News.CommentTitle")%>:
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtTitle" SkinID="NewsAddCommentTitleText"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%=GetLocaleResourceString("News.CommentText")%>:
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtComment" TextMode="MultiLine" ValidationGroup="NewComment"
                            SkinID="NewsAddCommentCommentText"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvComment" runat="server" ControlToValidate="txtComment"
                            ErrorMessage="<% $NopResources:News.PleaseEnterCommentText %>" ToolTip="<% $NopResources:News.PleaseEnterCommentText %>"
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
                        <asp:Button runat="server" ID="btnComment" Text="<% $NopResources:News.NewCommentButton %>"
                            ValidationGroup="NewComment" OnClick="btnComment_Click" CssClass="newsitemaddcommentbutton">
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
                    <nopCommerce:NewsComment ID="ctrlNewsComment" runat="server" NewsComment='<%# Container.DataItem %>' />
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>
