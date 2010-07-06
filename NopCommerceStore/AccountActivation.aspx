<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.AccountActivationPage" Codebehind="AccountActivation.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="AccountActivation" Src="~/Modules/CustomerAccountActivation.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:AccountActivation ID="ctrlAccountActivation" runat="server" />
</asp:Content>
