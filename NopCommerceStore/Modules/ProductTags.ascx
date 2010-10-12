<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductTagsControl"
    CodeBehind="ProductTags.ascx.cs" %>

<div class="producttags-box">
    <asp:Repeater ID="rptrProductTags" runat="server">
        <ItemTemplate>
            <a href="<%# SEOHelper.GetProductTagUrl((ProductTag)(Container.DataItem)) %>"
                class="producttag">
                <%#Server.HtmlEncode((string)Eval("Name"))%></a><%-- (<%#Eval("ProductCount")%>)--%></ItemTemplate>
        <SeparatorTemplate>
            ,
        </SeparatorTemplate>
    </asp:Repeater>
</div>
