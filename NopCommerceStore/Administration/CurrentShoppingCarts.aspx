<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CurrentShoppingCarts"
    CodeBehind="CurrentShoppingCarts.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CurrentShoppingCarts" Src="Modules/CurrentShoppingCarts.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CurrentShoppingCarts runat="server" ID="ctrlCurrentShoppingCarts" />
</asp:Content>
