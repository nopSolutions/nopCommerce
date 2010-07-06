<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CampaignAdd"
    CodeBehind="CampaignAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CampaignAdd" Src="Modules/CampaignAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CampaignAdd runat="server" ID="ctrlCampaignAdd" />
</asp:Content>
