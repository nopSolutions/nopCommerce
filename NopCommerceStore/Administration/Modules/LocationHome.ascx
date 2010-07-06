<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.LocationHomeControl"
    CodeBehind="LocationHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.LocationHome.LocationHome")%>" />
    <%=GetLocaleResourceString("Admin.LocationHome.LocationHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.LocationHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="countries.aspx" title="<%=GetLocaleResourceString("Admin.LocationHome.Countries.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.LocationHome.Countries.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.LocationHome.Countries.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="stateprovinces.aspx" title="<%=GetLocaleResourceString("Admin.LocationHome.StateProvinces.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.LocationHome.StateProvinces.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.LocationHome.StateProvinces.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="languages.aspx" title="<%=GetLocaleResourceString("Admin.LocationHome.Languages.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.LocationHome.Languages.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.LocationHome.Languages.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="currencies.aspx" title="<%=GetLocaleResourceString("Admin.LocationHome.Currencies.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.LocationHome.Currencies.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.LocationHome.Currencies.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="warehouses.aspx" title="<%=GetLocaleResourceString("Admin.LocationHome.Warehouses.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.LocationHome.Warehouses.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.LocationHome.Warehouses.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
