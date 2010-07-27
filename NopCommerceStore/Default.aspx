<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Default" CodeBehind="Default.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="HomePagePoll" Src="~/Modules/HomePagePoll.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="HomePageNews" Src="~/Modules/HomePageNews.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="HomePageCategories" Src="~/Modules/HomePageCategories.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="HomePageProducts" Src="~/Modules/HomePageProducts.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="BestSellers" Src="~/Modules/BestSellers.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:Topic ID="topicHomePageText" runat="server" TopicName="HomePageText"
        OverrideSEO="false"></nopCommerce:Topic>
    <div class="clear">
    </div>
    <nopCommerce:HomePageCategories ID="ctrlHomePageCategories" runat="server" />
    <div class="clear">
    </div>
    <nopCommerce:HomePageProducts ID="ctrlHomePageProducts" runat="server" />
    <div class="clear">
    </div>
    <nopCommerce:BestSellers ID="ctrlBestSellers" runat="server" />
    <div class="clear">
    </div>
    <nopCommerce:HomePageNews ID="ctrlHomePageNews" runat="server" />
    <div class="clear">
    </div>
    <nopCommerce:HomePagePoll ID="ctrlPolls" runat="server" />
</asp:Content>
