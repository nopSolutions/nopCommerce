<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.AttributesHomeControl"
    CodeBehind="AttributesHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.AttributesHome.AttributesHome")%>" />
    <%=GetLocaleResourceString("Admin.AttributesHome.AttributesHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.AttributesHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="productattributes.aspx" title="<%=GetLocaleResourceString("Admin.AttributesHome.ProductAttributes.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.AttributesHome.ProductAttributes.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.AttributesHome.ProductAttributes.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="specificationattributes.aspx" title="<%=GetLocaleResourceString("Admin.AttributesHome.SpecificationAttributes.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.AttributesHome.SpecificationAttributes.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.AttributesHome.SpecificationAttributes.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="checkoutattributes.aspx" title="<%=GetLocaleResourceString("Admin.AttributesHome.CheckoutAttributes.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.AttributesHome.CheckoutAttributes.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.AttributesHome.CheckoutAttributes.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
