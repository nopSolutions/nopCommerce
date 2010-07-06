<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ContentManagementHomeControl"
    CodeBehind="ContentManagementHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ContentManagementHome.ContentManagementHome")%>" />
    <%=GetLocaleResourceString("Admin.ContentManagementHome.ContentManagementHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.ContentManagementHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="polls.aspx" title="<%=GetLocaleResourceString("Admin.ContentManagementHome.Polls.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.Polls.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.Polls.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="newshome.aspx" title="<%=GetLocaleResourceString("Admin.ContentManagementHome.NewsHome.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.NewsHome.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.NewsHome.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="bloghome.aspx" title="<%=GetLocaleResourceString("Admin.ContentManagementHome.BlogHome.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.BlogHome.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.BlogHome.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="topics.aspx" title="<%=GetLocaleResourceString("Admin.ContentManagementHome.Topics.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.Topics.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.Topics.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="forums.aspx" title="<%=GetLocaleResourceString("Admin.ContentManagementHome.Forums.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.Forums.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.Forums.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="templateshome.aspx" title="<%=GetLocaleResourceString("Admin.ContentManagementHome.TemplatesHome.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.TemplatesHome.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.TemplatesHome.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="localestringresources.aspx" title="<%=GetLocaleResourceString("Admin.ContentManagementHome.LocaleStringResources.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.LocaleStringResources.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ContentManagementHome.LocaleStringResources.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
