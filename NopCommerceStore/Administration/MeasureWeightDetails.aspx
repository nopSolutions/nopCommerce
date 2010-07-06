<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MeasureWeightDetails.aspx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_MeasureWeightDetails" MasterPageFile="~/Administration/main.master" %>

<%@ Register TagPrefix="nopCommerce" TagName="MeasureWeightDetails" Src="Modules/MeasureWeightDetails.ascx" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:MeasureWeightDetails ID="ctrlMeasureWeightDetails" runat="server" />
</asp:Content>
