<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Modules.NumericTextBox" Codebehind="NumericTextBox.ascx.cs" %>
 
<asp:TextBox ID="txtValue" runat="server" SkinID="NumericTextBoxText"></asp:TextBox>
<asp:RequiredFieldValidator ID="rfvValue" ControlToValidate="txtValue"
    Font-Name="verdana" Font-Size="9pt" runat="server" Display="Dynamic"></asp:RequiredFieldValidator>
<asp:RangeValidator ID="rvValue" runat="server" ControlToValidate="txtValue"
    Type="Integer" Display="Dynamic"></asp:RangeValidator>
