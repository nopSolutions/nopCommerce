<%@ Page Language="C#" MasterPageFile="~/Administration/popup.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_RelatedProductAdd"
    CodeBehind="RelatedProductAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="RelatedProductAdd" Src="Modules/RelatedProductAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:RelatedProductAdd runat="server" ID="ctrlRelatedProductAdd" />
</asp:Content>
