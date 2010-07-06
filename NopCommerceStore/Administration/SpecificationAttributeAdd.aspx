<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_SpecificationAttributeAdd"
    CodeBehind="SpecificationAttributeAdd.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="SpecificationAttributeAdd" Src="Modules/SpecificationAttributeAdd.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:SpecificationAttributeAdd runat="server" ID="ctrlSpecificationAttributeAdd" />
</asp:Content>
