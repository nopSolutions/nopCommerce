<%@ Page Language="C#" AutoEventWireup="true" ContentType="text/xml" MaintainScrollPositionOnPostback="false"
    EnableTheming="false" Inherits="NopSolutions.NopCommerce.Web.NewsRSSPage" Codebehind="NewsRSS.aspx.cs" %>

<head id="Head1" runat="server" visible="false">
</head>
<asp:repeater id="rptrNews" runat="server">
    <HeaderTemplate>
        <rss version="2.0">
         <channel>
            <title><![CDATA[<%# SettingManager.StoreName%>: News]]></title>
            <link><%# CommonHelper.GetStoreLocation(false)%></link>
            <description><%# SettingManager.StoreName%></description>
            <copyright>Copyright <%= DateTime.Now.Year.ToString()%> by <%# SettingManager.StoreName%></copyright>
    </HeaderTemplate>
    <ItemTemplate>
        <item>
         <title><![CDATA[<%# Eval("Title") %>]]></title>
         <author><![CDATA[<%# SettingManager.StoreName%>]]></author>
         <description><![CDATA[<%# Eval("Short") %>]]></description>
         <link><![CDATA[<%# SEOHelper.GetNewsUrl(Convert.ToInt32(Eval("NewsId"))) %>]]></link>
         <pubDate><%# string.Format("{0:R}", DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc))%></pubDate>
      </item>
    </ItemTemplate>
    <FooterTemplate>
        </channel> </rss>
    </FooterTemplate>    
</asp:repeater>
