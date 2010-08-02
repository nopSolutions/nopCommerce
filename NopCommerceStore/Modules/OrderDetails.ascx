<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.OrderDetailsControl"
    CodeBehind="OrderDetails.ascx.cs" %>
<div class="order-details">
    <%if (!this.IsInvoice)
      { %>
    <table width="100%">
        <tr>
            <td style="text-align: left;">
                <div class="page-title">
                    <h1>
                        <%=GetLocaleResourceString("Order.OrderInformation")%></h1>
                </div>
            </td>
            <td style="text-align: right;">
                <asp:HyperLink runat="server" ID="lnkPrint" Text="<% $NopResources:Order.Print %>"
                    Target="_blank" CssClass="orderdetailsprintbutton" />
                <asp:LinkButton runat="server" ID="lbPDFInvoice" Text="<% $NopResources:Order.GetPDFInvoice %>"
                    CssClass="orderdetailsprintbutton" OnClick="lbPDFInvoice_Click" />
            </td>
        </tr>
    </table>
    <div class="clear">
    </div>
    <%} %>
    <div class="info">
        <div class="order-overview">
            <table width="100%" cellspacing="0" cellpadding="2" border="0">
                <tbody>
                    <tr>
                        <td colspan="2">
                            <b>
                                <%=GetLocaleResourceString("Order.Order#")%><asp:Label ID="lblOrderId" runat="server" />
                            </b>
                        </td>
                    </tr>
                    <tr>
                        <td class="smallText">
                            <%=GetLocaleResourceString("Order.OrderDate")%>:
                            <asp:Label ID="lblCreatedOn" runat="server"></asp:Label>
                        </td>
                        <td align="right" class="smallText">
                            <%=GetLocaleResourceString("Order.OrderTotal")%>:
                            <asp:Label ID="lblOrderTotal" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="smallText">
                            <%=GetLocaleResourceString("Order.OrderStatus")%>
                            <asp:Label ID="lblOrderStatus" runat="server"></asp:Label>
                        </td>
                        <td colspan="2">
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="clear">
        </div>
        <div class="order-details-box">
            <table width="100%" cellspacing="0" cellpadding="2" border="0">
                <tbody>
                    <tr>
                        <td width="50%" align="left" style="vertical-align:top;">
                            <b>
                                <%=GetLocaleResourceString("Order.BillingAddress")%></b>
                            <br />
                            <asp:Literal ID="lBillingFirstName" runat="server"></asp:Literal>
                            <asp:Literal ID="lBillingLastName" runat="server"></asp:Literal><br />
                            <div>
                                <%=GetLocaleResourceString("Order.Email")%>:
                                <asp:Literal ID="lBillingEmail" runat="server"></asp:Literal></div>
                            <div>
                                <%=GetLocaleResourceString("Order.Phone")%>:
                                <asp:Literal ID="lBillingPhoneNumber" runat="server"></asp:Literal></div>
                            <div>
                                <%=GetLocaleResourceString("Order.Fax")%>:
                                <asp:Literal ID="lBillingFaxNumber" runat="server"></asp:Literal></div>
                            <asp:Panel ID="pnlBillingCompany" runat="server">
                                <asp:Literal ID="lBillingCompany" runat="server"></asp:Literal></asp:Panel>
                            <div>
                                <asp:Literal ID="lBillingAddress1" runat="server"></asp:Literal></div>
                            <asp:Panel ID="pnlBillingAddress2" runat="server">
                                <asp:Literal ID="lBillingAddress2" runat="server"></asp:Literal></asp:Panel>
                            <div>
                                <asp:Literal ID="lBillingCity" runat="server"></asp:Literal>,
                                <asp:Literal ID="lBillingStateProvince" runat="server"></asp:Literal>
                                <asp:Literal ID="lBillingZipPostalCode" runat="server"></asp:Literal></div>
                            <asp:Panel ID="pnlBillingCountry" runat="server">
                                <asp:Literal ID="lBillingCountry" runat="server"></asp:Literal></asp:Panel>
                            <br />
                            <br />
                            <asp:PlaceHolder runat="server" ID="phVatNumber"><b>
                                <%=GetLocaleResourceString("Order.VATNumber")%></b> <br />
                                <asp:Literal runat="server" ID="lVatNumber"></asp:Literal>
                                <br />
                                <br />
                            </asp:PlaceHolder>
                            <b>
                                <%=GetLocaleResourceString("Order.PaymentMethod")%></b>
                            <br />
                            <asp:Literal runat="server" ID="lPaymentMethod"></asp:Literal>
                        </td>
                        <td width="50%" align="left" runat="server" id="pnlShipping" style="vertical-align:top;">
                            <b>
                                <%=GetLocaleResourceString("Order.ShippingAddress")%></b>
                            <br />
                            <asp:Literal ID="lShippingFirstName" runat="server"></asp:Literal>
                            <asp:Literal ID="lShippingLastName" runat="server"></asp:Literal><br />
                            <div>
                                <%=GetLocaleResourceString("Order.Email")%>:
                                <asp:Literal ID="lShippingEmail" runat="server"></asp:Literal></div>
                            <div>
                                <%=GetLocaleResourceString("Order.Phone")%>:
                                <asp:Literal ID="lShippingPhoneNumber" runat="server"></asp:Literal></div>
                            <div>
                                <%=GetLocaleResourceString("Order.Fax")%>:
                                <asp:Literal ID="lShippingFaxNumber" runat="server"></asp:Literal></div>
                            <asp:Panel ID="pnlShippingCompany" runat="server">
                                <asp:Literal ID="lShippingCompany" runat="server"></asp:Literal></asp:Panel>
                            <div>
                                <asp:Literal ID="lShippingAddress1" runat="server"></asp:Literal></div>
                            <asp:Panel ID="pnlShippingAddress2" runat="server">
                                <asp:Literal ID="lShippingAddress2" runat="server"></asp:Literal></asp:Panel>
                            <div>
                                <asp:Literal ID="lShippingCity" runat="server"></asp:Literal>,
                                <asp:Literal ID="lShippingStateProvince" runat="server"></asp:Literal>
                                <asp:Literal ID="lShippingZipPostalCode" runat="server"></asp:Literal></div>
                            <asp:Panel ID="pnlShippingCountry" runat="server">
                                <asp:Literal ID="lShippingCountry" runat="server"></asp:Literal></asp:Panel>
                            <br />
                            <br />
                            <b>
                                <%=GetLocaleResourceString("Order.ShippingMethod")%></b>
                            <br />
                            <asp:Label ID="lblShippingMethod" runat="server"></asp:Label>
                            <asp:PlaceHolder runat="server" ID="pnlTrackingNumber">
                                <br />
                                <b>
                                    <%=GetLocaleResourceString("Order.TrackingNumber")%></b> </asp:PlaceHolder>
                            <br />
                            <asp:Label ID="lblTrackingNumber" runat="server"></asp:Label>
                            <br />
                            <%if (!this.IsInvoice)
                              { %>
                            <br />
                                <b>
                                    <%=GetLocaleResourceString("Order.ShippedOn")%></b>
                                <br />
                                <asp:Label ID="lblShippedDate" runat="server"></asp:Label>
                                <br />
                                <b>
                                    <%=GetLocaleResourceString("Order.DeliveredOn")%></b>
                                <br />
                                <asp:Label ID="lblDeliveredOn" runat="server"></asp:Label>
                                <br />
                                <%} %>
                                <b>
                                    <%=GetLocaleResourceString("Order.Weight")%></b>
                                <br />
                                <asp:Label ID="lblOrderWeight" runat="server"></asp:Label>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="clear">
        </div>
        <div class="section-title">
            <%=GetLocaleResourceString("Order.Product(s)")%></div>
        <div class="clear">
        </div>
        <div class="products-box">
            <asp:GridView ID="gvOrderProductVariants" runat="server" AutoGenerateColumns="False"
                Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Order.ProductsGrid.SKU %>" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <div style="padding-left: 10px; padding-right: 10px;">
                                <%#Server.HtmlEncode(((OrderProductVariant)Container.DataItem).ProductVariant.SKU)%>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Order.ProductsGrid.Name %>" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <div style="padding-left: 10px; padding-right: 10px;">
                                <em><a href='<%#GetProductUrl(Convert.ToInt32(Eval("ProductVariantId")))%>'>
                                    <%#Server.HtmlEncode(GetProductVariantName(Convert.ToInt32(Eval("ProductVariantId"))))%></a></em>
                                <%#GetAttributeDescription((OrderProductVariant)Container.DataItem)%>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Order.ProductsGrid.Download %>" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <div style="padding-left: 10px; padding-right: 10px;">
                                <%#GetDownloadUrl(Container.DataItem as OrderProductVariant)%>
                            </div>
                            <div style="padding-left: 10px; padding-right: 10px;">
                                <%#GetLicenseDownloadUrl(Container.DataItem as OrderProductVariant)%>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Order.ProductsGrid.Price %>" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <div style="padding-left: 10px; padding-right: 10px;">
                                <%#GetProductVariantUnitPrice(Container.DataItem as OrderProductVariant)%>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Quantity" HeaderText="<% $NopResources:Order.ProductsGrid.Quantity %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:TemplateField HeaderText="<% $NopResources:Order.ProductsGrid.Total %>" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <div style="padding-left: 10px; padding-right: 10px;">
                                <%#GetProductVariantSubTotal(Container.DataItem as OrderProductVariant)%>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <div class="clear">
            </div>
            <div class="checkout-attributes">
                <asp:Literal runat="server" ID="lCheckoutAttributes"></asp:Literal>
            </div>
            <div class="clear">
            </div>
            <%if (!this.IsInvoice)
              { %>
            <br />
            <asp:Button runat="server" ID="btnReOrder" CssClass="reorderbutton" Text="<% $NopResources:Order.BtnReOrder.Text %>"
                ToolTip="<% $NopResources:Order.BtnReOrder.Tooltip %>" OnClick="BtnReOrder_OnClick" />
            <asp:PlaceHolder runat="server" ID="phReturnRequest">&nbsp;&nbsp;&nbsp;
                <asp:Button runat="server" ID="btnReturnItems" OnClick="btnReturnItems_Click" Text="<% $NopResources:OrderDetails.ReturnItemsButton %>"
                    CssClass="returnitemsbutton" />
            </asp:PlaceHolder>
            <%} %>
        </div>
        <div class="clear">
        </div>
        <table width="100%" cellspacing="0" cellpadding="2" border="0">
            <tbody>
                <tr>
                    <td width="100%" align="right">
                        <b>
                            <%=GetLocaleResourceString("Order.Sub-Total")%>:</b>
                    </td>
                    <td align="right">
                        <span style="white-space: nowrap;">
                            <asp:Label ID="lblOrderSubtotal" runat="server"></asp:Label>
                        </span>
                    </td>
                </tr>
                <tr runat="server" id="pnlShippingTotal">
                    <td width="100%" align="right">
                        <b>
                            <%=GetLocaleResourceString("Order.Shipping")%>:</b>
                    </td>
                    <td align="right">
                        <span style="white-space: nowrap;">
                            <asp:Label ID="lblOrderShipping" runat="server"></asp:Label>
                        </span>
                    </td>
                </tr>
                <asp:PlaceHolder runat="server" ID="phPaymentMethodAdditionalFee">
                    <tr>
                        <td width="100%" align="right">
                            <b>
                                <%=GetLocaleResourceString("Order.PaymentMethodAdditionalFee")%>:</b>
                        </td>
                        <td align="right">
                            <span style="white-space: nowrap;">
                                <asp:Label ID="lblPaymentMethodAdditionalFee" runat="server"></asp:Label>
                            </span>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:Repeater runat="server" ID="rptrTaxRates" OnItemDataBound="rptrTaxRates_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td width="100%" align="right">
                                <b>
                                    <asp:Literal runat="server" ID="lTaxRateTitle"></asp:Literal>:</b>
                            </td>
                            <td align="right">
                                <span style="white-space: nowrap;">
                                    <asp:Literal runat="server" ID="lTaxRateValue"></asp:Literal>
                                </span>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:PlaceHolder runat="server" ID="phTaxTotal">
                    <tr>
                        <td width="100%" align="right">
                            <b>
                                <%=GetLocaleResourceString("Order.Tax")%>:</b>
                        </td>
                        <td align="right">
                            <span style="white-space: nowrap;">
                                <asp:Label ID="lblOrderTax" runat="server"></asp:Label>
                            </span>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="phDiscount">
                    <tr>
                        <td width="100%" align="right">
                            <b>
                                <%=GetLocaleResourceString("Order.Discount")%>:</b>
                        </td>
                        <td align="right">
                            <span style="white-space: nowrap;">
                                <asp:Label ID="lblDiscount" runat="server"></asp:Label>
                            </span>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:Repeater runat="server" ID="rptrGiftCards" OnItemDataBound="rptrGiftCards_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td width="100%" align="right">
                                <b>
                                    <asp:Literal runat="server" ID="lGiftCard"></asp:Literal>:</b>
                            </td>
                            <td align="right">
                                <span style="white-space: nowrap;">
                                    <asp:Label ID="lblGiftCardAmount" runat="server"></asp:Label>
                                </span>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:PlaceHolder runat="server" ID="phRewardPoints">
                    <tr>
                        <td width="100%" align="right">
                            <b>
                                <asp:Literal runat="server" ID="lRewardPointsTitle"></asp:Literal>:</b>
                        </td>
                        <td align="right">
                            <span style="white-space: nowrap;">
                                <asp:Label ID="lblRewardPointsAmount" runat="server"></asp:Label>
                            </span>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td width="100%" align="right">
                        <b>
                            <%=GetLocaleResourceString("Order.OrderTotal")%>:</b>
                    </td>
                    <td align="right">
                        <b><span style="white-space: nowrap;">
                            <asp:Label ID="lblOrderTotal2" runat="server"></asp:Label>
                        </span></b>
                    </td>
                </tr>
            </tbody>
        </table>
        <div class="clear">
        </div>
        <div>
        </div>
        <%if (!this.IsInvoice)
          { %>
        <div class="clear">
        </div>
        <div class="section-title" runat="server" id="pnlOrderNotesTitle">
            <%=GetLocaleResourceString("Order.Notes")%>
        </div>
        <div class="clear">
        </div>
        <div class="ordernotes-box" runat="server" id="pnlOrderNotes">
            <asp:GridView ID="gvOrderNotes" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Order.OrderNotes.CreatedOn %>" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Order.OrderNotes.Note %>" ItemStyle-Width="70%"
                        HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <div style="padding-left: 10px; padding-right: 10px; text-align: left;">
                                <%#OrderManager.FormatOrderNoteText((string)Eval("Note"))%>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <%} %>
    </div>
</div> 