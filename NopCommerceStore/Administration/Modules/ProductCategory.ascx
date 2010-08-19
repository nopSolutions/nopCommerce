<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductCategoryControl"
    CodeBehind="ProductCategory.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<asp:UpdatePanel ID="upCat" runat="server">
    <ContentTemplate>
        <asp:GridView ID="gvCategoryMappings" runat="server" AutoGenerateColumns="false"
            Width="100%" OnPageIndexChanging="gvCategoryMappings_PageIndexChanging" AllowPaging="true"
            PageSize="20">
            <Columns>
                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductCategory.Category %>"
                    ItemStyle-Width="60%">
                    <ItemTemplate>
                        <asp:CheckBox ID="cbCategoryInfo" runat="server" Text='<%# Server.HtmlEncode(Eval("CategoryInfo").ToString()) %>'
                            Checked='<%# Eval("IsMapped") %>' ToolTip="<% $NopResources:Admin.ProductCategory.Category.Tooltip %>" />
                        <asp:HiddenField ID="hfCategoryId" runat="server" Value='<%# Eval("CategoryId") %>' />
                        <asp:HiddenField ID="hfProductCategoryId" runat="server" Value='<%# Eval("ProductCategoryId") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductCategory.View %>" HeaderStyle-HorizontalAlign="Center"
                    ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <a href='CategoryDetails.aspx?CategoryID=<%# Eval("CategoryId") %>' title="<%#GetLocaleResourceString("Admin.ProductCategory.View.Tooltip")%>">
                            <%#GetLocaleResourceString("Admin.ProductCategory.View")%></a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductCategory.FeaturedProduct %>"
                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:CheckBox ID="cbFeatured" runat="server" Checked='<%# Eval("IsFeatured") %>'
                            ToolTip="<% $NopResources:Admin.ProductCategory.FeaturedProduct.Tooltip %>" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductCategory.DisplayOrder %>"
                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" Width="50px" ID="txtDisplayOrder"
                            Value='<%# Eval("DisplayOrder") %>' RequiredErrorMessage="<% $NopResources:Admin.ProductCategory.DisplayOrder.RequiredErrorMessage %>"
                            RangeErrorMessage="<% $NopResources:Admin.ProductCategory.DisplayOrder.RangeErrorMessage %>"
                            MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
        </asp:GridView>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upCat">
    <ProgressTemplate>
        <div class="progress">
            <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/UpdateProgress.gif"
                AlternateText="update" />
            <%=GetLocaleResourceString("Admin.Common.Wait...")%>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
