<%@ Page Language="C#" MasterPageFile="~/MasterPages/ThreeColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.RegisterPage" CodeBehind="Register.aspx.cs"
     %>

<%@ Register TagPrefix="nopCommerce" TagName="CustomerRegister" Src="~/Modules/CustomerRegister.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:CustomerRegister ID="ctrlCustomerRegister" runat="server" />
</asp:Content>
