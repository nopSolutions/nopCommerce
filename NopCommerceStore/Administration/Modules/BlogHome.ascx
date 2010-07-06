<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlogHomeControl"
    CodeBehind="BlogHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.BlogHome.BlogHome")%>" />
    <%=GetLocaleResourceString("Admin.BlogHome.BlogHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.BlogHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="blogsettings.aspx" title="<%=GetLocaleResourceString("Admin.BlogHome.BlogSettings.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.BlogHome.BlogSettings.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.BlogHome.BlogSettings.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="blog.aspx" title="<%=GetLocaleResourceString("Admin.BlogHome.Blog.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.BlogHome.Blog.Title")%>
                    </a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.BlogHome.Blog.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="blogcomments.aspx" title="<%=GetLocaleResourceString("Admin.BlogHome.BlogComments.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.BlogHome.BlogComments.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.BlogHome.BlogComments.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
