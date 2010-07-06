<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="PromotionProviders.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.PromotionProviders" %>

<%@ Register Src="Modules/PromotionProviders.ascx" TagName="PromotionProviders"
    TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:PromotionProviders ID="ctrlPromotionProviders" runat="server" />
</asp:Content>
