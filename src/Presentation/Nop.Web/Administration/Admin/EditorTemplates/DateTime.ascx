<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DateTime?>" %>

<%= Html.Telerik().DateTimePicker()
        .Name(ViewData.TemplateInfo.GetFullHtmlFieldName(string.Empty))
        .Value(Model > DateTime.MinValue? Model : DateTime.Today)
%>