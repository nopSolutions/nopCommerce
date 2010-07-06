<%@ Page Language="C#" MasterPageFile="~/Administration/popup.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CategoryProductAdd"
    CodeBehind="CategoryProductAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CategoryProductAdd" Src="Modules/CategoryProductAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CategoryProductAdd runat="server" ID="ctrlCategoryProductAdd" />
</asp:Content>
