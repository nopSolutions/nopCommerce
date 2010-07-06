<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_DiscountDetails"
    CodeBehind="DiscountDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="DiscountDetails" Src="Modules/DiscountDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:DiscountDetails runat="server" ID="ctrlDiscountDetails" />
</asp:Content>
