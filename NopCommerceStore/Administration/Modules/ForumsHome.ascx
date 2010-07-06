<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ForumsHomeControl"
    CodeBehind="ForumsHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ForumsHome.ForumsHome")%>" />
    <%=GetLocaleResourceString("Admin.ForumsHome.ForumsHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.ForumsHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="forumssettings.aspx" title="<%=GetLocaleResourceString("Admin.ForumsHome.ForumsSettings.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ForumsHome.ForumsSettings.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ForumsHome.ForumsSettings.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="forums.aspx" title="<%=GetLocaleResourceString("Admin.ForumsHome.Forums.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ForumsHome.Forums.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ForumsHomea.Forums.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
