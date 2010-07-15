<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CustomerDownloadableProductsControl"
    CodeBehind="CustomerDownloadableProducts.ascx.cs" %>
<div class="downloable-products">
    <asp:Panel runat="server" ID="pnlProducts" CssClass="info">
        <div class="products-box">
            <asp:GridView ID="gvOrderProductVariants" runat="server" AutoGenerateColumns="False"
                Width="100%" EnableViewState="false">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Account.DownloadableProducts.ProductsGrid.Order %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%">
                        <ItemTemplate>
                            <div style="padding-left: 10px; padding-right: 10px;">
                                <%#GetOrderUrl(Container.DataItem as OrderProductVariant)%>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Account.DownloadableProducts.ProductsGrid.Date %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%">
                        <ItemTemplate>
                            <div style="padding-left: 10px; padding-right: 10px;">
                                <%#GetOrderDate(Container.DataItem as OrderProductVariant)%>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Account.DownloadableProducts.ProductsGrid.Name %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="45%">
                        <ItemTemplate>
                            <div style="padding-left: 10px; padding-right: 10px;">
                                <em><a href='<%#GetProductUrl(Convert.ToInt32(Eval("ProductVariantId")))%>'>
                                    <%#Server.HtmlEncode(GetProductVariantName(Convert.ToInt32(Eval("ProductVariantId"))))%></a></em>
                                <%#GetAttributeDescription((OrderProductVariant)Container.DataItem)%>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Account.DownloadableProducts.ProductsGrid.Download %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="25%">
                        <ItemTemplate>
                            <div style="padding-left: 10px; padding-right: 10px;">
                                <%#GetDownloadUrl(Container.DataItem as OrderProductVariant)%>
                            </div>
                            <div style="padding-left: 10px; padding-right: 10px;">
                                <%#GetLicenseDownloadUrl(Container.DataItem as OrderProductVariant)%>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlMessage" CssClass="info" EnableViewState="false">
        <%=GetLocaleResourceString("Account.DownloadableProducts.NoProducts")%>
    </asp:Panel>
</div>
