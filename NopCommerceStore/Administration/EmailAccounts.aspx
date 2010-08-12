<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_EmailAccounts"
    CodeBehind="EmailAccounts.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="EmailAccounts" Src="Modules/EmailAccounts.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:EmailAccounts runat="server" ID="ctrlEmailAccounts" />
</asp:Content>
