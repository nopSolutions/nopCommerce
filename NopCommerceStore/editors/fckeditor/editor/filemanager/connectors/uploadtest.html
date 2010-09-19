<!--
 * FCKeditor - The text editor for Internet - http://www.fckeditor.net
 * Copyright (C) 2003-2010 Frederico Caldeira Knabben
 *
 * == BEGIN LICENSE ==
 *
 * Licensed under the terms of any of the following licenses at your
 * choice:
 *
 *  - GNU General Public License Version 2 or later (the "GPL")
 *    http://www.gnu.org/licenses/gpl.html
 *
 *  - GNU Lesser General Public License Version 2.1 or later (the "LGPL")
 *    http://www.gnu.org/licenses/lgpl.html
 *
 *  - Mozilla Public License Version 1.1 or later (the "MPL")
 *    http://www.mozilla.org/MPL/MPL-1.1.html
 *
 * == END LICENSE ==
 *
 * Test page for the "File Uploaders".
-->
<html>
	<head>
		<title>FCKeditor - Uploaders Tests</title>
		<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
		<script type="text/javascript">

// Automatically detect the correct document.domain (#1919).
(function()
{
	var d = document.domain ;

	while ( true )
	{
		// Test if we can access a parent property.
		try
		{
			var test = window.opener.document.domain ;
			break ;
		}
		catch( e ) {}

		// Remove a domain part: www.mytest.example.com => mytest.example.com => example.com ...
		d = d.replace( /.*?(?:\.|$)/, '' ) ;

		if ( d.length == 0 )
			break ;		// It was not able to detect the domain.

		try
		{
			document.domain = d ;
		}
		catch (e)
		{
			break ;
		}
	}
})() ;

function SendFile()
{
	var sUploaderUrl = cmbUploaderUrl.value ;

	if ( sUploaderUrl.length == 0 )
		sUploaderUrl = txtCustomUrl.value ;

	if ( sUploaderUrl.length == 0 )
	{
		alert( 'Please provide your custom URL or select a default one' ) ;
		return ;
	}

	eURL.innerHTML = sUploaderUrl ;
	txtUrl.value = '' ;

	var date = new Date()

	frmUpload.action = sUploaderUrl + '?time=' + date.getTime();
	if (document.getElementById('cmbType').value) {
		frmUpload.action = frmUpload.action + '&Type='+document.getElementById('cmbType').value;
	}
	if (document.getElementById('CurrentFolder').value) {
		frmUpload.action = frmUpload.action + '&CurrentFolder='+document.getElementById('CurrentFolder').value;
	}
	frmUpload.submit() ;
}

function OnUploadCompleted( errorNumber, fileUrl, fileName, customMsg )
{
	switch ( errorNumber )
	{
		case 0 :	// No errors
			txtUrl.value = fileUrl ;
			alert( 'File uploaded with no errors' ) ;
			break ;
		case 1 :	// Custom error
			alert( customMsg ) ;
			break ;
		case 10 :	// Custom warning
			txtUrl.value = fileUrl ;
			alert( customMsg ) ;
			break ;
		case 201 :
			txtUrl.value = fileUrl ;
			alert( 'A file with the same name is already available. The uploaded file has been renamed to "' + fileName + '"' ) ;
			break ;
		case 202 :
			alert( 'Invalid file' ) ;
			break ;
		case 203 :
			alert( "Security error. You probably don't have enough permissions to upload. Please check your server." ) ;
			break ;
		default :
			alert( 'Error on file upload. Error number: ' + errorNumber ) ;
			break ;
	}
}

		</script>
	</head>
	<body>
		<table cellSpacing="0" cellPadding="0" width="100%" border="0" height="100%">
			<tr>
				<td>
					<table cellSpacing="0" cellPadding="0" width="100%" border="0">
						<tr>
							<td nowrap>
								Select the "File Uploader" to use: <br>
								<select id="cmbUploaderUrl">
									<option selected value="asp/upload.asp">ASP</option>
									<option value="aspx/upload.aspx">ASP.Net</option>
									<option value="cfm/upload.cfm">ColdFusion</option>
									<option value="lasso/upload.lasso">Lasso</option>
									<option value="perl/upload.cgi">Perl</option>
									<option value="php/upload.php">PHP</option>
									<option value="py/upload.py">Python</option>
									<option value="">(Custom)</option>
								</select>
							</td>
						<td>
							Resource Type<br />
							<select id="cmbType" name="cmbType">
								<option value="">None</option>
								<option value="File">File</option>
								<option value="Image">Image</option>
								<option value="Flash">Flash</option>
								<option value="Media">Media</option>
								<option value="Invalid">Invalid Type (for testing)</option>
							</select>
						</td>
						<td>
						Current Folder: <br>
						<input type="text" name="CurrentFolder" id="CurrentFolder" value="/">
						</td>
							<td nowrap>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
							<td width="100%">
								Custom Uploader URL:<BR>
								<input id="txtCustomUrl" style="WIDTH: 100%; BACKGROUND-COLOR: #dcdcdc" disabled type="text">
							</td>
						</tr>
					</table>
					<br>
					<table cellSpacing="0" cellPadding="0" width="100%" border="0">
						<tr>
							<td noWrap>
								<form id="frmUpload" target="UploadWindow" enctype="multipart/form-data" action="" method="post">
									Upload a new file:<br>
									<input type="file" name="NewFile"><br>

									<input type="button" value="Send it to the Server" onclick="SendFile();">
								</form>
							</td>
							<td style="WIDTH: 16px">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
							<td vAlign="top" width="100%">
								Uploaded File URL:<br>
								<INPUT id="txtUrl" style="WIDTH: 100%" readonly type="text">
							</td>
						</tr>
					</table>
					<br>
					Post URL: <span id="eURL">&nbsp;</span>
				</td>
			</tr>
			<tr>
				<td height="100%">
					<iframe name="UploadWindow" width="100%" height="100%" src="javascript:void(0)"></iframe>
				</td>
			</tr>
		</table>
	</body>
</html>
