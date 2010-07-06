<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true" ValidateRequest="false"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ThirdPartyIntegration"
    CodeBehind="ThirdPartyIntegration.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ThirdPartyIntegration" Src="Modules/ThirdPartyIntegration.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ThirdPartyIntegration runat="server" ID="ctrlThirdPartyIntegration" />
</asp:Content>
