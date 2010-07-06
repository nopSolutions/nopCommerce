<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.nopCommerceNewsControl"
    CodeBehind="nopCommerceNews.ascx.cs" %>
<%@ Register Assembly="RssToolkit" Namespace="RssToolkit.Web.WebControls" TagPrefix="RssToolkit" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-news.gif" alt="<%=GetLocaleResourceString("Admin.nopCommerceNews.News")%>" />
        <%=GetLocaleResourceString("Admin.nopCommerceNews.News")%>
    </div>
</div>
<RssToolkit:RssDataSource ID="dsNopCommerceNews" runat="server" MaxItems="5" Url="http://www.nopCommerce.com/NewsRSS.aspx">
</RssToolkit:RssDataSource>
<asp:ListView ID="lvNopCommerceNews" runat="server" DataSourceID="dsNopCommerceNews">
    <LayoutTemplate>
        <div class="newsitem">
            <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
        </div>
    </LayoutTemplate>
    <ItemTemplate>
        <div class="newstitle">
            <a href='<%# Eval("link") %>'>
                <%# Eval("title") %></a></div>
        <div class="newsdate">
            (<%# FormatDateTime((string)Eval("pubDateParsed"))%>)
        </div>
        <div class="newsdetails">
            <%# Eval("description") %>
        </div>
    </ItemTemplate>
</asp:ListView>
