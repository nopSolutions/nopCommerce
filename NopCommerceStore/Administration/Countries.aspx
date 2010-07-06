<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Countries"
    CodeBehind="Countries.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Countries" Src="Modules/Countries.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Countries runat="server" ID="ctrlCountries" />
</asp:Content>
