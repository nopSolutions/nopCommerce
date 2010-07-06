<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PaymentMethodsControl" CodeBehind="PaymentMethods.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="PaymentMethodsFilterControl" Src="PaymentMethodsFilterControl.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.PaymentMethods.Title")%>" />
        <%=GetLocaleResourceString("Admin.PaymentMethods.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.PaymentMethods.BtnSave.Text %>" CssClass="adminButtonBlue" ID="btnSave" OnClick="BtnSave_OnClick" ToolTip="<% $NopResources:Admin.PaymentMethods.BtnSave.Tooltip %>" />
        <input type="button" onclick="location.href='PaymentMethodAdd.aspx'" value="<%=GetLocaleResourceString("Admin.PaymentMethods.AddButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.PaymentMethods.AddButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvPaymentMethods" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PaymentMethods.Name %>" ItemStyle-Width="30%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PaymentMethods.VisibleName %>"
            ItemStyle-Width="30%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("VisibleName").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PaymentMethods.DisplayOrder %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PaymentMethods.IsActive %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:ImageCheckBox runat="server" ID="cbPublished" Checked='<%# Eval("IsActive") %>'>
                </nopCommerce:ImageCheckBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.PaymentMethods.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="PaymentMethodDetails.aspx?PaymentMethodID=<%#Eval("PaymentMethodId")%>"
                    title="<%#GetLocaleResourceString("Admin.PaymentMethods.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.PaymentMethods.Edit")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<br />
<nopCommerce:PaymentMethodsFilterControl runat="server" ID="ctrlPaymentMethodsFilterControl" />
