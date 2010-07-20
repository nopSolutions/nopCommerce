<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ReturnItemsControl"
    CodeBehind="ReturnItems.ascx.cs" %>
<div class="return-request">
    <div class="info">
        <div class="page-title">
            <h1>
                <asp:Literal runat="server" ID="lTitle"></asp:Literal></h1>
        </div>
        <div class="clear">
        </div>
        <asp:PlaceHolder runat="server" ID="phInput">
            <div class="section-addinfo">
                <%=GetLocaleResourceString("ReturnItems.SelectProduct(s)")%></div>
            <div class="clear">
            </div>
            <div class="products-box">
                <asp:GridView ID="gvOrderProductVariants" runat="server" AutoGenerateColumns="False"
                    Width="100%" OnRowDataBound="gvOrderProductVariants_OnRowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="<% $NopResources:ReturnItems.ProductsGrid.Name %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="60%">
                            <ItemTemplate>
                                <div style="padding-left: 10px; padding-right: 10px;">
                                    <em><a href='<%#GetProductUrl(Convert.ToInt32(Eval("ProductVariantId")))%>'>
                                        <%#Server.HtmlEncode(GetProductVariantName(Convert.ToInt32(Eval("ProductVariantId"))))%></a></em>
                                    <%#GetAttributeDescription((OrderProductVariant)Container.DataItem)%>
                                    <asp:HiddenField runat="server" ID="hfOpvId" Value='<%# Eval("OrderProductVariantId") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:ReturnItems.ProductsGrid.Price %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="20%">
                            <ItemTemplate>
                                <div style="padding-left: 10px; padding-right: 10px;">
                                    <%#GetProductVariantUnitPrice(Container.DataItem as OrderProductVariant)%>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:ReturnItems.ProductsGrid.Quantity %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%">
                            <ItemTemplate>
                                <div style="padding-left: 10px; padding-right: 10px;">
                                    <asp:DropDownList runat="server" ID="ddlQuantity" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <div class="clear">
                </div>
                <div class="section-addinfo">
                    <%=GetLocaleResourceString("ReturnItems.WhyReturning")%></div>
                <div class="clear">
                </div>
                <div class="why">
                    <table class="table-container">
                        <tr class="row">
                            <td class="item-name">
                                <%=GetLocaleResourceString("ReturnItems.ReturnReason")%>
                            </td>
                            <td class="item-value">
                                <asp:DropDownList runat="server" ID="ddlReturnReason" />
                            </td>
                        </tr>
                        <tr class="row">
                            <td class="item-name">
                                <%=GetLocaleResourceString("ReturnItems.ReturnAction")%>
                            </td>
                            <td class="item-value">
                                <asp:DropDownList runat="server" ID="ddlReturnAction" />
                            </td>
                        </tr>
                        <tr class="row">
                            <td class="item-name">
                                <%=GetLocaleResourceString("ReturnItems.Comments")%>
                            </td>
                            <td class="item-value">
                                <asp:TextBox runat="server" ID="txtComments" TextMode="MultiLine" SkinID="ReturnRequestCommentsText"></asp:TextBox>
                            </td>
                        </tr>
                        <tr class="row">
                            <td class="item-name">
                            </td>
                            <td class="buttons">
                                <asp:Button runat="server" ID="btnSubmit" Text="<% $NopResources:ReturnItems.SubmitButton %>"
                                    OnClick="btnSubmit_Click" CssClass="submitreturnrequestbutton"></asp:Button>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="phResult">
            <div>
                <asp:Literal runat="server" ID="lResults" /></div>
        </asp:PlaceHolder>
    </div>
</div>
