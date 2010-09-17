<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.CheckoutOnepagePage" CodeBehind="CheckoutOnepage.aspx.cs"
    EnableEventValidation="false"  %>

<%@ Register TagPrefix="nopCommerce" TagName="CheckoutOnePage" Src="~/Modules/CheckoutOnePage.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:CheckoutOnePage ID="ctrlCheckoutOnePage" runat="server" />
</asp:Content>
