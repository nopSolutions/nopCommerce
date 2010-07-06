<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_AffiliateAdd"
    CodeBehind="AffiliateAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="AffiliateAdd" Src="Modules/AffiliateAdd.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:AffiliateAdd runat="server" ID="ctrlAffiliateAdd" />
</asp:Content>
