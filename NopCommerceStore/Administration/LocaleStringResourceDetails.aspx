<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    ValidateRequest="false" Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_LocaleStringResourceDetails"
    CodeBehind="LocaleStringResourceDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="LocaleStringResourceDetails" Src="Modules/LocaleStringResourceDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:LocaleStringResourceDetails runat="server" ID="ctrlLocaleStringResourceDetails" />
</asp:Content>
