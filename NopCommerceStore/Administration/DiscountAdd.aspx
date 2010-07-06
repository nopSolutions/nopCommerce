<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_DiscountAdd"
    CodeBehind="DiscountAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="DiscountAdd" Src="Modules/DiscountAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:DiscountAdd runat="server" ID="ctrlDiscountAdd" />
</asp:Content>
