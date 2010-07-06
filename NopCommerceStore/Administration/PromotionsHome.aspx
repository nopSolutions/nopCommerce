<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="PromotionsHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.PromotionsHome" %>

<%@ Register Src="Modules/PromotionsHome.ascx" TagName="PromotionsHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:PromotionsHome ID="ctrlPromotionsHome" runat="server" />
</asp:Content>
