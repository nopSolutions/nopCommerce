<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.WishlistEmailAFriendControl"
    CodeBehind="WishlistEmailAFriend.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="~/Modules/EmailTextBox.ascx" %>
<div class="emailafriend-box">
    <div class="page-title">
        <h1>
            <%=GetLocaleResourceString("EmailWishlist.EmailAFriend")%></h1>
    </div>
    <div class="clear">
    </div>
    <div class="send-email">
        <p>
            <asp:Label ID="lblDescription" runat="server" CssClass="description" />
        </p>
        <p>
            <asp:Label runat="server" ID="lblEmailAFriend" CssClass="confirm" />
        </p>
        <table>
            <tr runat="server" id="pnlFriendsEmail">
                <td style="width: 100px; text-align: left; vertical-align: middle;">
                    <%=GetLocaleResourceString("EmailWishlist.FriendEmail")%>
                </td>
                <td>
                    <nopCommerce:EmailTextBox runat="server" ID="txtFriendsEmail" ValidationGroup="WishlistEmail"
                        Width="250px"></nopCommerce:EmailTextBox>
                </td>
            </tr>
            <tr runat="server" id="pnlFrom">
                <td style="width: 100px; text-align: left; vertical-align: middle;">
                    <%=GetLocaleResourceString("EmailWishlist.YourEmailAddress")%>
                </td>
                <td>
                    <asp:Label runat="server" ID="lblFrom"></asp:Label>
                </td>
            </tr>
            <tr runat="server" ID="pnlPersonalMessage">
                <td style="width: 100px; text-align: left; vertical-align: middle;">
                    <%=GetLocaleResourceString("EmailWishlist.PersonalMessage")%>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtPersonalMessage" TextMode="MultiLine" Height="150px"
                        Width="250px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td style="text-align: left; vertical-align: middle;">
                    <asp:Button runat="server" ID="btnEmail" Text="<% $NopResources:EmailWishlist.EmailAFriendButton %>"
                        ValidationGroup="WishlistEmail" OnClick="btnEmail_Click" CssClass="sendemailafriendbutton" />
                </td>
            </tr>
        </table>
    </div>
</div>
