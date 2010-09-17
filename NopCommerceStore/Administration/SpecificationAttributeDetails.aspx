<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_SpecificationAttributeDetails"
    CodeBehind="SpecificationAttributeDetails.aspx.cs"  %>

<%@ Register TagPrefix="nopCommerce" TagName="SpecificationAttributeDetails" Src="Modules/SpecificationAttributeDetails.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:SpecificationAttributeDetails runat="server" ID="ctrlSpecificationAttributeDetails" />
</asp:Content>
