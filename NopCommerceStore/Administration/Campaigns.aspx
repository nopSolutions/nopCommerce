<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Campaigns"
    CodeBehind="Campaigns.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Campaigns" Src="Modules/Campaigns.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Campaigns runat="server" ID="ctrlCampaigns" />
</asp:Content>
