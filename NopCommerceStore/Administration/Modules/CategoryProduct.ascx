<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CategoryProductControl"
    CodeBehind="CategoryProduct.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>

<table class="adminContent">
    <tr>
        <td width="100%">
            <asp:UpdatePanel ID="upMappings" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="gvProductCategoryMappings" runat="server" AutoGenerateColumns="false"
                        Width="100%" AllowPaging="true" PageSize="15" OnPageIndexChanging="gvProductCategoryMappings_PageIndexChanging">
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
                        <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
                    </asp:GridView>
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
