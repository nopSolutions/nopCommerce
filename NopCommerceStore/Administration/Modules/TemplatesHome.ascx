<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.TemplatesHomeControl"
    CodeBehind="TemplatesHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.TemplatesHome.TemplatesHome")%>" />
    <%=GetLocaleResourceString("Admin.TemplatesHome.TemplatesHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.TemplatesHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="producttemplates.aspx" title="<%=GetLocaleResourceString("Admin.TemplatesHome.ProductTemplates.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.TemplatesHome.ProductTemplates.Title")%>
                    </a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.TemplatesHome.ProductTemplates.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="categorytemplates.aspx" title="<%=GetLocaleResourceString("Admin.TemplatesHome.CategoryTemplates.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.TemplatesHome.CategoryTemplates.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.TemplatesHome.CategoryTemplates.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="manufacturertemplates.aspx" title="<%=GetLocaleResourceString("Admin.TemplatesHome.ManufacturerTemplates.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.TemplatesHome.ManufacturerTemplates.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.TemplatesHome.ManufacturerTemplates.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="messagetemplates.aspx" title="<%=GetLocaleResourceString("Admin.TemplatesHome.MessageTemplates.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.TemplatesHome.MessageTemplates.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.TemplatesHome.MessageTemplates.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
