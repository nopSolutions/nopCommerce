<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.TaxHomeControl"
    CodeBehind="TaxHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.TaxHome.TaxHome")%>" />
    <%=GetLocaleResourceString("Admin.TaxHome.TaxHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.TaxHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="taxsettings.aspx" title="<%=GetLocaleResourceString("Admin.TaxHome.TaxSettings.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.TaxHome.TaxSettings.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.TaxHome.TaxSettings.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="taxproviders.aspx" title="<%=GetLocaleResourceString("Admin.TaxHome.TaxProviders.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.TaxHome.TaxProviders.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.TaxHome.TaxProviders.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="taxcategories.aspx" title="<%=GetLocaleResourceString("Admin.TaxHome.TaxCategories.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.TaxHome.TaxCategories.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.TaxHome.TaxCategories.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
