<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.LoginPage" Title="Login" Codebehind="Login.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="CustomerLogin" Src="~/Modules/CustomerLogin.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:CustomerLogin ID="ctrlCustomerLogin" runat="server" />
</asp:Content>
