<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Discounts"
    CodeBehind="Discounts.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Discounts" Src="Modules/Discounts.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Discounts runat="server" ID="ctrlDiscounts" />
</asp:Content>
