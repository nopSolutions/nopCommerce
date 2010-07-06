<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CategoryDetails"
    CodeBehind="CategoryDetails.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="CategoryDetails" Src="Modules/CategoryDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CategoryDetails runat="server" ID="ctrlCategoryDetails" />
</asp:Content>
