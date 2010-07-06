<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SystemHomeControl"
    CodeBehind="SystemHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-system.png" alt="<%=GetLocaleResourceString("Admin.SystemHome.SystemHome")%>" />
    <%=GetLocaleResourceString("Admin.SystemHome.SystemHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.SystemHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="logs.aspx" title="<%=GetLocaleResourceString("Admin.SystemHome.Logs.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.SystemHome.Logs.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.SystemHome.Logs.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="messagequeue.aspx" title="<%=GetLocaleResourceString("Admin.SystemHome.MessageQueue.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.SystemHome.MessageQueue.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.SystemHome.MessageQueue.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="maintenance.aspx" title="<%=GetLocaleResourceString("Admin.SystemHome.Maintenance.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.SystemHome.Maintenance.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.SystemHome.Maintenance.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
