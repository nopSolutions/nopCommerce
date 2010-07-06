<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CategoryTemplateAdd"
    CodeBehind="CategoryTemplateAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CategoryTemplateAdd" Src="Modules/CategoryTemplateAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CategoryTemplateAdd runat="server" ID="ctrlCategoryTemplateAdd" />
</asp:Content>
