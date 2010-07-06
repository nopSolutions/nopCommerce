<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MeasureDimensionDetails.aspx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_MeasureDimensionDetails" MasterPageFile="~/Administration/main.master" %>

<%@ Register TagPrefix="nopCommerce" TagName="MeasureDimensionDetails" Src="Modules/MeasureDimensionDetails.ascx" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:MeasureDimensionDetails ID="ctrlMeasureDimensionDetails" runat="server" />
</asp:Content>
