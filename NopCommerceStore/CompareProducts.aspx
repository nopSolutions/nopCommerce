<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.CompareProductsPage" Codebehind="CompareProducts.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CompareProducts" Src="~/Modules/CompareProducts.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:CompareProducts ID="ctrlCompareProducts" runat="server" />
</asp:Content>
