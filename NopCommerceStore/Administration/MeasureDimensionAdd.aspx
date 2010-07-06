<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MeasureDimensionAdd.aspx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_MeasureDimensionAdd" MasterPageFile="~/Administration/main.master" %>

<%@ Register TagPrefix="nopCommerce" TagName="MeasureDimensionAdd" Src="Modules/MeasureDimensionAdd.ascx" %>
<asp:Content ID="mainContent" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:MeasureDimensionAdd ID="ctrlMeasureDimensionAdd" runat="server" />
</asp:Content>
