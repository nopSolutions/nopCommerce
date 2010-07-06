<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CreditCardTypesControl"
    CodeBehind="CreditCardTypes.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.CreditCardTypes.Title")%>" />
        <%=GetLocaleResourceString("Admin.CreditCardTypes.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='CreditCardTypeAdd.aspx'" value="<%=GetLocaleResourceString("Admin.CreditCardTypes.AddButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.CreditCardTypes.AddButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvCreditCardTypes" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CreditCardTypes.Name %>" ItemStyle-Width="60%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CreditCardTypes.DisplayOrder %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CreditCardTypes.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="CreditCardTypeDetails.aspx?CreditCardTypeID=<%#Eval("CreditCardTypeId")%>"
                    title="<%#GetLocaleResourceString("Admin.CreditCardTypes.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.CreditCardTypes.Edit")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
