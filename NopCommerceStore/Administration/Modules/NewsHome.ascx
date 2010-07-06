<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.NewsHomeControl"
    CodeBehind="NewsHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.NewsHome.NewsHome")%>" />
    <%=GetLocaleResourceString("Admin.NewsHome.NewsHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.NewsHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="newssettings.aspx" title="<%=GetLocaleResourceString("Admin.NewsHome.NewsSettings.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.NewsHome.NewsSettings.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.NewsHome.NewsSettings.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="news.aspx" title="<%=GetLocaleResourceString("Admin.NewsHome.News.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.NewsHome.News.Title")%>
                    </a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.NewsHome.News.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="newscomments.aspx" title="<%=GetLocaleResourceString("Admin.NewsHome.NewsComments.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.NewsHome.NewsComments.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.NewsHome.NewsComments.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
