<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductManufacturerControl"
    CodeBehind="ProductManufacturer.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<asp:UpdatePanel ID="upMan" runat="server">
    <ContentTemplate>
        <asp:GridView ID="gvManufacturerMappings" runat="server" AutoGenerateColumns="false"
            Width="100%" OnPageIndexChanging="gvManufacturerMappings_PageIndexChanging" AllowPaging="true"
            PageSize="20">
            <Columns>
                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductManufacturer.Manufacturer %>"
                    ItemStyle-Width="60%">
                    <ItemTemplate>
                        <asp:CheckBox ID="cbManufacturerInfo" runat="server" Text='<%# Server.HtmlEncode(Eval("ManufacturerInfo").ToString()) %>'
                            Checked='<%# Eval("IsMapped") %>' ToolTip="<% $NopResources:Admin.ProductManufacturer.Manufacturer.Tooltip %>" />
                        <asp:HiddenField ID="hfManufacturerId" runat="server" Value='<%# Eval("ManufacturerId") %>' />
                        <asp:HiddenField ID="hfProductManufacturerId" runat="server" Value='<%# Eval("ProductManufacturerId") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductManufacturer.View %>"
                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <a href='ManufacturerDetails.aspx?ManufacturerID=<%# Eval("ManufacturerId") %>' title="<%#GetLocaleResourceString("Admin.ProductManufacturer.View.Tooltip")%>">
                            <%#GetLocaleResourceString("Admin.ProductManufacturer.View")%></a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductManufacturer.FeaturedProduct %>"
                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:CheckBox ID="cbFeatured" runat="server" Checked='<%# Eval("IsFeatured") %>'
                            ToolTip="<% $NopResources:Admin.ProductManufacturer.FeaturedProduct.Tooltip %>" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductManufacturer.DisplayOrder %>"
                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" Width="50px" ID="txtDisplayOrder"
                            Value='<%# Eval("DisplayOrder") %>' RequiredErrorMessage="<% $NopResources:Admin.ProductManufacturer.DisplayOrder.RequiredErrorMessage %>"
                            RangeErrorMessage="<% $NopResources:Admin.ProductManufacturer.DisplayOrder.RangeErrorMessage %>"
                            MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
        </asp:GridView>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upMan">
    <ProgressTemplate>
        <div class="progress">
            <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/UpdateProgress.gif"
                AlternateText="update" />
            <%=GetLocaleResourceString("Admin.Common.Wait...")%>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
