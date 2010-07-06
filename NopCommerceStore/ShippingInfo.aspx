<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.ShippingInfoPage"
    CodeBehind="ShippingInfo.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:Topic ID="topicShippingInfo" runat="server" TopicName="ShippingInfo">
    </nopCommerce:Topic>
</asp:Content>
