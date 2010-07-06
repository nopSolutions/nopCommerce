<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DatePicker.ascx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.DatePicker" %>

<asp:TextBox runat="server" ID="txtDateTime" />
<asp:ImageButton runat="Server" ID="btnCalendar" ImageUrl="~/images/Calendar_scheduleHS.png" AlternateText="<% $NopResources:Admin.ShowCalendar %>" />
<br />
<ajaxToolkit:CalendarExtender runat="server" ID="ajaxCalendar" TargetControlID="txtDateTime" PopupButtonID="btnCalendar" /> 