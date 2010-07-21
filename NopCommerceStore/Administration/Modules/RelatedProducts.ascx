<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.RelatedProductsControl"
    CodeBehind="RelatedProducts.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlData">

    <asp:GridView ID="gvRelatedProducts" runat="server" AutoGenerateColumns="false" Width="100%">
        <Columns>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.RelatedProducts.Product %>"
                ItemStyle-Width="60%">
                <ItemTemplate>
                    <asp:CheckBox ID="cbProductInfo2" runat="server" Text='<%# Server.HtmlEncode(Eval("ProductInfo2").ToString()) %>'
                        Checked='<%# Eval("IsMapped") %>' ToolTip="<% $NopResources:Admin.RelatedProducts.Product.Tooltip %>" />
                    <asp:HiddenField ID="hfProductId2" runat="server" Value='<%# Eval("ProductId2") %>' />
                    <asp:HiddenField ID="hfRelatedProductId" runat="server" Value='<%# Eval("RelatedProductId") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.RelatedProducts.Image %>">
                <ItemTemplate>
                    <asp:Image runat="server" ID="imgProduct" ImageUrl='<%# Eval("ProductImage") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.RelatedProducts.View %>" HeaderStyle-HorizontalAlign="Center"
                ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <a href='ProductDetails.aspx?ProductID=<%# Eval("ProductId2") %>' title="<%#GetLocaleResourceString("Admin.RelatedProducts.View.Tooltip")%>">
                        <%#GetLocaleResourceString("Admin.RelatedProducts.View")%></a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.RelatedProducts.DisplayOrder %>"
                HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" Width="50px" ID="txtDisplayOrder"
                        Value='<%# Eval("DisplayOrder") %>' RequiredErrorMessage="<% $NopResources:Admin.RelatedProducts.DisplayOrder.RequiredErrorMessage %>"
                        RangeErrorMessage="<% $NopResources:Admin.RelatedProducts.DisplayOrder.RangeErrorMessage %>"
                        MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <asp:Button ID="btnAddNew" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.RelatedProducts.AddNewButton.Text %>" />
    <asp:Button ID="btnRefresh" runat="server" Style="display: none" CausesValidation="false"
        CssClass="adminButton" Text="Refresh" OnClick="btnRefresh_Click" ToolTip="Refresh list" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlMessage">
    <%=GetLocaleResourceString("Admin.RelatedProducts.AvailableAfterSaving")%>
</asp:Panel>
