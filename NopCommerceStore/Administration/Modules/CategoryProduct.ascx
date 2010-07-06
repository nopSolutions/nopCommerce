<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CategoryProductControl"
    CodeBehind="CategoryProduct.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>

<script language="javascript">
    function OpenWindow(query, w, h, scroll) {
        var l = (screen.width - w) / 2;
        var t = (screen.height - h) / 2;

        winprops = 'resizable=1, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
        if (scroll) winprops += ',scrollbars=1';
        var f = window.open(query, "_blank", winprops);
    }
</script>

<table class="adminContent">
    <tr>
        <td width="100%">
            <asp:UpdatePanel ID="upMappings" runat="server">
                <ContentTemplate>
                    <nopCommerce:NopDataPagerGridView ID="gvProductCategoryMappings" runat="server" AutoGenerateColumns="false"
                        Width="100%" AllowPaging="true" OnPageIndexChanging="gvProductCategoryMappings_PageIndexChanging">
                        <Columns>
                            <asp:TemplateField HeaderText="<% $NopResources:Admin.CategoryProducts.Product %>"
                                ItemStyle-Width="60%">
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbProductInfo" runat="server" Text='<%#Server.HtmlEncode(Eval("ProductInfo").ToString()) %>'
                                        Checked='<%# Eval("IsMapped") %>' ToolTip="<% $NopResources:Admin.CategoryProducts.Mapped.Tooltip %>" />
                                    <asp:HiddenField ID="hfProductId" runat="server" Value='<%# Eval("ProductId") %>' />
                                    <asp:HiddenField ID="hfProductCategoryId" runat="server" Value='<%# Eval("ProductCategoryId") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<% $NopResources:Admin.CategoryProducts.Image %>">
                                <ItemTemplate>
                                    <asp:Image runat="server" ID="imgProduct" ImageUrl='<%# Eval("ProductImage") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<% $NopResources:Admin.CategoryProducts.View %>" HeaderStyle-HorizontalAlign="Center"
                                ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <a href='ProductDetails.aspx?ProductID=<%# Eval("ProductId") %>' title="<%#GetLocaleResourceString("Admin.CategoryProducts.View.Tooltip")%>">
                                        <%#GetLocaleResourceString("Admin.CategoryProducts.View")%></a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<% $NopResources: Admin.CategoryProducts.FeaturedProduct%>"
                                HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbFeatured" runat="server" Checked='<%# Eval("IsFeatured") %>'
                                        ToolTip="<% $NopResources:Admin.CategoryProducts.FeaturedProduct.Tooltip %>" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<% $NopResources:Admin.CategoryProducts.DisplayOrder %>"
                                HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" Width="50px" ID="txtDisplayOrder"
                                        Value='<%# Eval("DisplayOrder") %>' RequiredErrorMessage="<% $NopResources:Admin.CategoryProducts.DisplayOrder.RequiredErrorMessage %>"
                                        RangeErrorMessage="<% $NopResources:Admin.CategoryProducts.DisplayOrder.RangeErrorMessage %>"
                                        MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerSettings Visible="False" />
                    </nopCommerce:NopDataPagerGridView>
                    <div class="pager">
                        <asp:DataPager ID="pagerProductCategoryMappings" runat="server" PageSize="15" PagedControlID="gvProductCategoryMappings">
                            <Fields>
                                <asp:NextPreviousPagerField ButtonCssClass="command" FirstPageText="«" PreviousPageText="‹"
                                    RenderDisabledButtonsAsLabels="true" ShowFirstPageButton="true" ShowPreviousPageButton="true"
                                    ShowLastPageButton="false" ShowNextPageButton="false" />
                                <asp:NumericPagerField ButtonCount="7" NumericButtonCssClass="command" CurrentPageLabelCssClass="current"
                                    NextPreviousButtonCssClass="command" />
                                <asp:NextPreviousPagerField ButtonCssClass="command" LastPageText="»" NextPageText="›"
                                    RenderDisabledButtonsAsLabels="true" ShowFirstPageButton="false" ShowPreviousPageButton="false"
                                    ShowLastPageButton="true" ShowNextPageButton="true" />
                            </Fields>
                        </asp:DataPager>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upMappings">
                <ProgressTemplate>
                    <div class="progress">
                        <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/UpdateProgress.gif" AlternateText="update" />
                        <%=GetLocaleResourceString("Admin.Common.Wait...")%>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </td>
    </tr>
    <tr>
        <td width="100%">
            <asp:Button ID="btnAddNew" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.CategoryProducts.AddNewButton.Text %>" />
            <asp:Button ID="btnRefresh" runat="server" Style="display: none" CausesValidation="false"
                CssClass="adminButton" Text="Refresh" OnClick="btnRefresh_Click" ToolTip="Refresh list" />
        </td>
    </tr>
</table>
