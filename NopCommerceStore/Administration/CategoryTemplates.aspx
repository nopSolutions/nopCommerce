<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CategoryTemplates"
    CodeBehind="CategoryTemplates.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CategoryTemplates" Src="Modules/CategoryTemplates.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CategoryTemplates runat="server" ID="ctrlCategoryTemplates" />
</asp:Content>
