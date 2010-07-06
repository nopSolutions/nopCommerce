<%@ Page Language="C#" AutoEventWireup="true" ContentType="text/xml" MaintainScrollPositionOnPostback="false"
    EnableTheming="false" Inherits="NopSolutions.NopCommerce.Web.BlogRSSPage" CodeBehind="BlogRSS.aspx.cs" %>

<head id="Head1" runat="server" visible="false">
</head>
<asp:repeater id="rptrBlogPosts" runat="server">
    <HeaderTemplate>
        <rss version="2.0">
         <channel>
            <title><![CDATA[<%# SettingManager.StoreName%>: Blog]]></title>
            <link><%# CommonHelper.GetStoreLocation(false)%></link>
            <description><%# SettingManager.StoreName%></description>
            <copyright>Copyright <%= DateTime.Now.Year.ToString()%> by <%# SettingManager.StoreName%></copyright>
    </HeaderTemplate>
    <ItemTemplate>
        <item>
         <title><![CDATA[<%# Eval("BlogPostTitle") %>]]></title>
         <author><![CDATA[<%# SettingManager.StoreName%>]]></author>
         <description><![CDATA[<%# Eval("BlogPostBody") %>]]></description>
         <link><![CDATA[<%# SEOHelper.GetBlogPostUrl(Convert.ToInt32(Eval("BlogPostId"))) %>]]></link>
         <pubDate><%# string.Format("{0:R}", DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc))%></pubDate>
      </item>
    </ItemTemplate>
    <FooterTemplate>
        </channel> </rss>
    </FooterTemplate>    
</asp:repeater>
