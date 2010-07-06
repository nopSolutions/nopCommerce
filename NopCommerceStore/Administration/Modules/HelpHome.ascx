<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.HelpHomeControl"
    CodeBehind="HelpHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-help.png" alt="<%=GetLocaleResourceString("Admin.HelpHome.HelpHome")%>" />
    <%=GetLocaleResourceString("Admin.HelpHome.HelpHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.HelpHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="http://www.nopcommerce.com/documentation.aspx" title="<%=GetLocaleResourceString("Admin.HelpHome.nopDocumentation.TitleDescription")%>"
                        target="_blank">
                        <%=GetLocaleResourceString("Admin.HelpHome.nopDocumentation.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.HelpHome.nopDocumentation.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="http://www.nopcommerce.com/boards/" title="<%=GetLocaleResourceString("Admin.HelpHome.nopCommunity.TitleDescription")%>"
                        target="_blank">
                        <%=GetLocaleResourceString("Admin.HelpHome.nopCommunity.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.HelpHome.nopCommunity.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
