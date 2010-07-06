<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductPicturesControl"
    CodeBehind="ProductPictures.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>

<script type="text/javascript">
    function showUploadPanels() {
        if ($('#upl20').is(':hidden')) {
            $('#upl20').show();
            $('#upl21').show();
            $('#upl22').show();
        }
        else if ($('#upl30').is(':hidden')) {
            $('#upl30').show();
            $('#upl31').show();
            $('#upl32').show();
        }
        else {
            $('#<%=btnMoreUploads.ClientID %>').attr("disabled", "disabled");
        }
    }
</script>

<asp:Panel runat="server" ID="pnlData">
    <asp:GridView ID="gvwImages" runat="server" AutoGenerateColumns="false" DataKeyNames="ProductPictureId"
        OnRowDeleting="gvwImages_RowDeleting" OnRowDataBound="gvwImages_RowDataBound"
        OnRowCommand="gvwImages_RowCommand" Width="100%">
        <Columns>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductPictures.Image %>" ItemStyle-Width="50%">
                <ItemTemplate>
                    <asp:Image ID="iProductPicture" runat="server" AlternateText="pic" />
                    <asp:HiddenField ID="hfProductPictureId" runat="server" Value='<%# Eval("ProductPictureId") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductPictures.DisplayOrder %>"
                HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" Width="50px" ID="txtProductPictureDisplayOrder"
                        Value='<%# Eval("DisplayOrder") %>' RequiredErrorMessage="<% $NopResources:Admin.ProductPictures.DisplayOrder.RequiredErrorMessage %>"
                        RangeErrorMessage="<% $NopResources:Admin.ProductPictures.DisplayOrder.RangeErrorMessage %>"
                        ValidationGroup="ProductPictures" MinimumValue="-99999" MaximumValue="99999">
                    </nopCommerce:NumericTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductPictures.Update %>"
                HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Button ID="btnUpdate" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.ProductPictures.Update %>"
                        ValidationGroup="ProductPictures" CommandName="UpdateProductPicture" ToolTip="<% $NopResources:Admin.ProductPictures.Update.Tooltip %>" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductPictures.Delete %>"
                HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Button ID="btnDeletePicture" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.ProductPictures.Delete %>"
                        CausesValidation="false" CommandName="Delete" ToolTip="<% $NopResources:Admin.ProductPictures.Delete.Tooltip %>" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <p>
        <strong>
            <%=GetLocaleResourceString("Admin.ProductPictures.AddNewPicture")%>
        </strong>
    </p>
    <table class="adminContent">
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblSelectPicture1" Text="<% $NopResources:Admin.ProductPictures.SelectPicture %>"
                    ToolTip="<% $NopResources:Admin.ProductPictures.SelectPicture.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:FileUpload class="text" ID="fuProductPicture1" CssClass="adminInput" runat="server"
                    ToolTip="<% $NopResources:Admin.ProductPictures.FileUpload %>" />
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblProductDisplayOrder1" Text="<% $NopResources:Admin.ProductPictures.New.DisplayOrder %>"
                    ToolTip="<% $NopResources:Admin.ProductPictures.New.DisplayOrder.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtProductPictureDisplayOrder1"
                    Value="1" RequiredErrorMessage="<% $NopResources:Admin.ProductPictures.New.DisplayOrder.RequiredErrorMessage %>"
                    RangeErrorMessage="<% $NopResources:Admin.ProductPictures.New.DisplayOrder.RangeErrorMessage %>"
                    MinimumValue="-99999" MaximumValue="99999" ValidationGroup="UploadNewProductPicture">
                </nopCommerce:NumericTextBox>
            </td>
        </tr>
        <tr id="upl20" style="display: none;">
            <td colspan="2">
                <br />
            </td>
        </tr>
        <tr id="upl21" style="display: none;">
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblSelectPicture2" Text="<% $NopResources:Admin.ProductPictures.SelectPicture %>"
                    ToolTip="<% $NopResources:Admin.ProductPictures.SelectPicture.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:FileUpload class="text" ID="fuProductPicture2" CssClass="adminInput" runat="server"
                    ToolTip="<% $NopResources:Admin.ProductPictures.FileUpload %>" />
            </td>
        </tr>
        <tr id="upl22" style="display: none;">
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblProductDisplayOrder2" Text="<% $NopResources:Admin.ProductPictures.New.DisplayOrder %>"
                    ToolTip="<% $NopResources:Admin.ProductPictures.New.DisplayOrder.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtProductPictureDisplayOrder2"
                    Value="1" RequiredErrorMessage="<% $NopResources:Admin.ProductPictures.New.DisplayOrder.RequiredErrorMessage %>"
                    RangeErrorMessage="<% $NopResources:Admin.ProductPictures.New.DisplayOrder.RangeErrorMessage %>"
                    MinimumValue="-99999" MaximumValue="99999" ValidationGroup="UploadNewProductPicture">
                </nopCommerce:NumericTextBox>
            </td>
        </tr>
        <tr id="upl30" style="display: none;">
            <td colspan="2">
                <br />
            </td>
        </tr>
        <tr id="upl31" style="display: none;">
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblSelectPicture3" Text="<% $NopResources:Admin.ProductPictures.SelectPicture %>"
                    ToolTip="<% $NopResources:Admin.ProductPictures.SelectPicture.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:FileUpload class="text" ID="fuProductPicture3" CssClass="adminInput" runat="server"
                    ToolTip="<% $NopResources:Admin.ProductPictures.FileUpload %>" />
            </td>
        </tr>
        <tr id="upl32" style="display: none;">
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblProductDisplayOrder3" Text="<% $NopResources:Admin.ProductPictures.New.DisplayOrder %>"
                    ToolTip="<% $NopResources:Admin.ProductPictures.New.DisplayOrder.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtProductPictureDisplayOrder3"
                    Value="1" RequiredErrorMessage="<% $NopResources:Admin.ProductPictures.New.DisplayOrder.RequiredErrorMessage %>"
                    RangeErrorMessage="<% $NopResources:Admin.ProductPictures.New.DisplayOrder.RangeErrorMessage %>"
                    MinimumValue="-99999" MaximumValue="99999" ValidationGroup="UploadNewProductPicture">
                </nopCommerce:NumericTextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="left">
                <asp:Button runat="server" ID="btnMoreUploads" CssClass="adminButton" Text="+" />
                <asp:Button runat="server" ID="btnUploadProductPicture" CssClass="adminButton" Text="<% $NopResources:Admin.ProductPictures.UploadButton.Text %>"
                    ValidationGroup="UploadNewProductPicture" OnClick="btnUploadProductPicture_Click"
                    ToolTip="<% $NopResources:Admin.ProductPictures.UploadButton.Tooltip %>" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel runat="server" ID="pnlMessage">
    <%=GetLocaleResourceString("Admin.ProductPictures.AvailableAfterSaving")%>
</asp:Panel>
