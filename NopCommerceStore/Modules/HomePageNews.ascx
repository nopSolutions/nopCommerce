<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.HomePageNewsControl"
    CodeBehind="HomePageNews.ascx.cs" %>
<div class="newslist">
    <div class="title">
        <table style="width: 100%;">
            <tr>
                <td style="text-align: left; vertical-align: middle;">
                    <%=GetLocaleResourceString("News.News")%>
                </td>
                <td style="text-align: right; vertical-align: middle;">
                    <a href="<%= GetNewsRSSUrl()%>">
                        <asp:Image ID="imgRSS" runat="server" ImageUrl="~/images/icon_rss.gif" ToolTip="<% $NopResources:NewsRSS.Tooltip %>"
                            AlternateText="RSS" EnableViewState="false" /></a>
                </td>
            </tr>
        </table>
    </div>
    <div class="clear">
    </div>
    <div class="newsitems">
        <asp:Repeater ID="rptrNews" runat="server" EnableViewState="false">
            <ItemTemplate>
                <div class="item">
                    <a class="newstitle" href="<%#SEOHelper.GetNewsUrl(Convert.ToInt32(Eval("NewsId")))%>">
                        <%#Server.HtmlEncode(Eval("Title").ToString())%></a> <span class="newsdate">-
                            <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString("D")%>
                        </span>
                    <div class="newsdetails">
                        <%#Eval("Short")%>
                    </div>
                    <a href="<%#SEOHelper.GetNewsUrl(Convert.ToInt32(Eval("NewsId")))%>" class="readmore">
                        <%=GetLocaleResourceString("News.MoreInfo")%></a>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="viewall">
            <a href="<%=CommonHelper.GetStoreLocation()%>newsarchive.aspx">
                <%=GetLocaleResourceString("News.ViewAll")%></a>
        </div>
    </div>
</div>
