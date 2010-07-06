<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Categories"
    CodeBehind="Categories.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Categories" Src="Modules/Categories.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Categories runat="server" ID="ctrlCategories" />
</asp:Content>
