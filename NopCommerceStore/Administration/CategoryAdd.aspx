<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CategoryAdd"
    CodeBehind="CategoryAdd.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="CategoryAdd" Src="Modules/CategoryAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CategoryAdd runat="server" ID="ctrlCategoryAdd" />
</asp:Content>
