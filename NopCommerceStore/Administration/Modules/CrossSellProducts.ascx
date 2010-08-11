<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CrossSellProductsControl"
    CodeBehind="CrossSellProducts.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlData">

    <asp:GridView ID="gvCrossSellProducts" runat="server" AutoGenerateColumns="false" Width="100%">
        <Columns>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.CrossSellProducts.Product %>"
                ItemStyle-Width="70%">
                <ItemTemplate>
                    <asp:CheckBox ID="cbProductInfo2" runat="server" Text='<%# Server.HtmlEncode(Eval("ProductInfo2").ToString()) %>'
                        Checked='<%# Eval("IsMapped") %>' ToolTip="<% $NopResources:Admin.CrossSellProducts.Product.Tooltip %>" />
                    <asp:HiddenField ID="hfProductId2" runat="server" Value='<%# Eval("ProductId2") %>' />
                    <asp:HiddenField ID="hfCrossSellProductId" runat="server" Value='<%# Eval("CrossSellProductId") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.CrossSellProducts.Image %>">
                <ItemTemplate>
                    <asp:Image runat="server" ID="imgProduct" ImageUrl='<%# Eval("ProductImage") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.CrossSellProducts.View %>" HeaderStyle-HorizontalAlign="Center"
                ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <a href='ProductDetails.aspx?ProductID=<%# Eval("ProductId2") %>' title="<%#GetLocaleResourceString("Admin.CrossSellProducts.View.Tooltip")%>">
                        <%#GetLocaleResourceString("Admin.CrossSellProducts.View")%></a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <asp:Button ID="btnAddNew" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.CrossSellProducts.AddNewButton.Text %>" />
    <asp:Button ID="btnRefresh" runat="server" Style="display: none" CausesValidation="false"
        CssClass="adminButton" Text="Refresh" OnClick="btnRefresh_Click" ToolTip="Refresh list" />
</asp:Panel>
<asp:Panel runat="server" ID="pnlMessage">
    <%=GetLocaleResourceString("Admin.CrossSellProducts.AvailableAfterSaving")%>
</asp:Panel>
