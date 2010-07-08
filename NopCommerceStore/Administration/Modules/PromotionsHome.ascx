<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PromotionsHomeControl"
    CodeBehind="PromotionsHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.PromotionsHome.PromotionsHome")%>" />
    <%=GetLocaleResourceString("Admin.PromotionsHome.PromotionsHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.PromotionsHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="affiliates.aspx" title="<%=GetLocaleResourceString("Admin.PromotionsHome.Affiliates.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.PromotionsHome.Affiliates.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.PromotionsHome.Affiliates.Description1")%></p>
                    <p>
                        <%=GetLocaleResourceString("Admin.PromotionsHome.Affiliates.Description2")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="newslettersubscribers.aspx" title="<%=GetLocaleResourceString("Admin.PromotionsHome.NewsletterSubscribers.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.PromotionsHome.NewsletterSubscribers.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.PromotionsHome.NewsletterSubscribers.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="campaigns.aspx" title="<%=GetLocaleResourceString("Admin.PromotionsHome.Campaigns.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.PromotionsHome.Campaigns.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.PromotionsHome.Campaigns.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="discounts.aspx" title="<%=GetLocaleResourceString("Admin.PromotionsHome.Discounts.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.PromotionsHome.Discounts.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.PromotionsHome.Discounts.Description1")%>
                    </p>
                    <p>
                        <%=GetLocaleResourceString("Admin.PromotionsHome.Discounts.Description2")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="pricelist.aspx" title="<%=GetLocaleResourceString("Admin.PromotionsHome.Pricelist.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.PromotionsHome.Pricelist.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.PromotionsHome.Pricelist.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="promotionproviders.aspx" title="<%=GetLocaleResourceString("Admin.PromotionsHome.PromotionProviders.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.PromotionsHome.PromotionProviders.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.PromotionsHome.PromotionProviders.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
