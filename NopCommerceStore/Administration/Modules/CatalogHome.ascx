<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CatalogHomeControl"
    CodeBehind="CatalogHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.CatalogHome.CatalogHome")%>" />
    <%=GetLocaleResourceString("Admin.CatalogHome.CatalogHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.CatalogHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="categories.aspx" title="<%=GetLocaleResourceString("Admin.CatalogHome.Categories.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.CatalogHome.Categories.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.CatalogHome.Categories.Description1")%>
                    </p>
                    <p>
                        <%=GetLocaleResourceString("Admin.CatalogHome.Categories.Description2")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="productshome.aspx" title="<%=GetLocaleResourceString("Admin.CatalogHome.Products.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.CatalogHome.Products.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.CatalogHome.Products.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="attributeshome.aspx" title="<%=GetLocaleResourceString("Admin.CatalogHome.Attributes.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.CatalogHome.Attributes.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.CatalogHome.Attributes.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="manufacturers.aspx" title="<%=GetLocaleResourceString("Admin.CatalogHome.Manufacturers.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.CatalogHome.Manufacturers.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.CatalogHome.Manufacturers.Description1")%>
                    </p>
                    <p>
                        <%=GetLocaleResourceString("Admin.CatalogHome.Manufacturers.Description2")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
