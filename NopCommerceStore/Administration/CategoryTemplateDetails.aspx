<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CategoryTemplateDetails"
    CodeBehind="CategoryTemplateDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CategoryTemplateDetails" Src="Modules/CategoryTemplateDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CategoryTemplateDetails runat="server" ID="ctrlCategoryTemplateDetails" />
</asp:Content>
