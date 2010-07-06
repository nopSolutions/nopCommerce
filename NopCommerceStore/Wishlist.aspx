<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.WishlistPage" CodeBehind="Wishlist.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Wishlist" Src="~/Modules/Wishlist.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="wishlist-page">
        <div class="page-title">
            <h1><asp:Literal runat="server" ID="lTitle"></asp:Literal></h1>
        </div>
        <div class="clear">
        </div>
        <div class="body">
            <nopCommerce:Wishlist ID="ctrlWishlist" runat="server" IsEditable="false"></nopCommerce:Wishlist>
            <div class="shareinfo">
                <p>
                    <asp:Label runat="server" ID="lblYourWishlistURL" Visible="false" CssClass="sharelabel" />
                </p>
                <p>
                    <asp:HyperLink runat="server" ID="lnkWishListUrl" Visible="false" CssClass="sharelink" />
                </p>
            </div>
        </div>
    </div>
</asp:Content>
