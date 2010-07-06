<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.ConditionsInfoPage"
    CodeBehind="ConditionsInfo.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:Topic ID="topicConditionsInfo" runat="server" TopicName="ConditionsOfUse">
    </nopCommerce:Topic>
</asp:Content>
