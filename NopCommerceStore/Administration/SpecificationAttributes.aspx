<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_SpecificationAttributes"
    CodeBehind="SpecificationAttributes.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="SpecificationAttributes" Src="Modules/SpecificationAttributes.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:SpecificationAttributes runat="server" ID="ctrlSpecificationAttributes" />
</asp:Content>
