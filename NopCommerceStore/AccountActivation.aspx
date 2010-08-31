<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.AccountActivationPage" CodeBehind="AccountActivation.aspx.cs"
    ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="AccountActivation" Src="~/Modules/CustomerAccountActivation.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:AccountActivation ID="ctrlAccountActivation" runat="server" />
</asp:Content>
