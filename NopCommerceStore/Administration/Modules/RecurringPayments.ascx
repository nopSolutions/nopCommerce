<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.RecurringPaymentsControl"
    CodeBehind="RecurringPayments.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.RecurringPayments.Title")%>" />
        <%=GetLocaleResourceString("Admin.RecurringPayments.Title")%>
    </div>
    <div class="options">
    </div>
</div>
<asp:GridView ID="gvRecurringPayments" runat="server" AutoGenerateColumns="False"
    Width="100%" OnPageIndexChanging="gvRecurringPayments_PageIndexChanging" AllowPaging="true"
    PageSize="15">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.RecurringPayments.CustomerColumn %>"
            ItemStyle-Width="15%">
            <ItemTemplate>
                <%#GetCustomerInfo((RecurringPayment)Container.DataItem)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.RecurringPayments.CycleInfoColumn %>"
            ItemStyle-Width="15%">
            <ItemTemplate>
                <%#GetCycleInfo((RecurringPayment)Container.DataItem)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.RecurringPayments.IsActiveColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:ImageCheckBox runat="server" ID="cbIsActive" Checked='<%# Eval("IsActive") %>'>
                </nopCommerce:ImageCheckBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.RecurringPayments.StartDateColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("StartDate"), DateTimeKind.Utc).ToString()%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.RecurringPayments.NextPaymentColumn %>"
            ItemStyle-Width="15%">
            <ItemTemplate>
                <%#GetNextPaymentInfo((RecurringPayment)Container.DataItem)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="TotalCycles" HeaderText="<% $NopResources:Admin.RecurringPayments.TotalCyclesColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="CyclesRemaining" HeaderText="<% $NopResources:Admin.RecurringPayments.CyclesRemainingColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.RecurringPayments.EditColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="RecurringPaymentDetails.aspx?RecurringPaymentID=<%#Eval("RecurringPaymentId")%>">
                    <%#GetLocaleResourceString("Admin.RecurringPayments.EditColumn")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<br />
<asp:Label runat="server" ID="lblNoRecurringPayments" Text="<% $NopResources: Admin.RecurringPayments.NoRecurringPaymentsFound %>"
    Visible="false"></asp:Label>