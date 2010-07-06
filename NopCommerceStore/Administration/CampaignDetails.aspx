<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CampaignDetails"
    ValidateRequest="false" CodeBehind="CampaignDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CampaignDetails" Src="Modules/CampaignDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CampaignDetails runat="server" ID="ctrlCampaignDetails" />
</asp:Content>
