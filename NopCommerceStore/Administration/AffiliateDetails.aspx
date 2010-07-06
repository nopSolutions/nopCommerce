<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_AffiliateDetails"
    CodeBehind="AffiliateDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="AffiliateDetails" Src="Modules/AffiliateDetails.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:AffiliateDetails runat="server" ID="ctrlAffiliateDetails" />
</asp:Content>
