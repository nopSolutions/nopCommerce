<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductEmailAFriendButton"
    CodeBehind="ProductEmailAFriendButton.ascx.cs" %>
<asp:Button runat="server" ID="btnEmailAFriend" Text="<% $NopResources:Products.EmailAFriend %>"
    OnClick="btnEmailAFriend_Click" CausesValidation="false" CssClass="productemailafriendbutton" />
