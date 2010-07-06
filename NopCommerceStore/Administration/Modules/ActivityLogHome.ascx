<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ActivityLogHomeControl"
    CodeBehind="ActivityLogHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ActivityLogHome.ActivityLogHome")%>" />
    <%=GetLocaleResourceString("Admin.ActivityLogHome.ActivityLogHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.ActivityLogHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="activitytypes.aspx" title="<%=GetLocaleResourceString("Admin.ActivityLogHome.ActivityTypes.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ActivityLogHome.ActivityTypes.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ActivityLogHome.ActivityTypes.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="activitylog.aspx" title="<%=GetLocaleResourceString("Admin.ActivityLogHome.ActivityLog.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ActivityLogHome.ActivityLog.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ActivityLogHome.ActivityLog.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
