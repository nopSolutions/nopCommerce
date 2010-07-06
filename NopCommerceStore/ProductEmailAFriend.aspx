<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.ProductEmailAFriendPage" CodeBehind="ProductEmailAFriend.aspx.cs"
    ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductEmailAFriend" Src="~/Modules/ProductEmailAFriend.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:ProductEmailAFriend ID="ctrlProductEmailAFriend" runat="server" />
</asp:Content>
