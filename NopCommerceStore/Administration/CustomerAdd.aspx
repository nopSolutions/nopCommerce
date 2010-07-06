<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CustomerAdd"
    CodeBehind="CustomerAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CustomerAdd" Src="Modules/CustomerAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CustomerAdd runat="server" ID="ctrlCustomerAdd" />
</asp:Content>
