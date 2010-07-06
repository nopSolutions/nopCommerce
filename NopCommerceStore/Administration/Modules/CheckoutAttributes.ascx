<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CheckoutAttributesControl"
    CodeBehind="CheckoutAttributes.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.CheckoutAttributes.Title")%>" />
        <%=GetLocaleResourceString("Admin.CheckoutAttributes.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='checkoutattributeadd.aspx'" value="<%=GetLocaleResourceString("Admin.CheckoutAttributes.AddButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.CheckoutAttributes.AddButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvCheckoutAttributes" runat="server" AutoGenerateColumns="False"
    Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CheckoutAttributes.Name %>"
            ItemStyle-Width="40%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CheckoutAttributes.IsRequired %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:ImageCheckBox runat="server" ID="cbIsRequired" Checked='<%# Eval("IsRequired") %>'>
                </nopCommerce:ImageCheckBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CheckoutAttributes.AttributeControlType %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#CommonHelper.ConvertEnum(Eval("AttributeControlType").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CheckoutAttributes.DisplayOrder %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CheckoutAttributes.Edit %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="checkoutattributedetails.aspx?checkoutattributeid=<%#Eval("CheckoutAttributeId")%>">
                    <%#GetLocaleResourceString("Admin.CheckoutAttributes.Edit")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
