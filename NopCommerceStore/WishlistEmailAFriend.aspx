<%@ Page Language="C#" MasterPageFile="~/MasterPages/ThreeColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.WishlistEmailAFriendPage" CodeBehind="WishlistEmailAFriend.aspx.cs"
     %>

<%@ Register TagPrefix="nopCommerce" TagName="WishlistEmailAFriend" Src="~/Modules/WishlistEmailAFriend.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:WishlistEmailAFriend ID="ctrlWishlistEmailAFriend" runat="server" />
</asp:Content>
