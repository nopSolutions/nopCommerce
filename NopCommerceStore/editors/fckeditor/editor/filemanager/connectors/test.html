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
 * Test page for the File Browser connectors.
-->
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>FCKeditor - Connectors Tests</title>
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

function BuildBaseUrl( command )
{
	var sUrl =
		document.getElementById('cmbConnector').value +
		'?Command=' + command +
		'&Type=' + document.getElementById('cmbType').value +
		'&CurrentFolder=' + encodeURIComponent(document.getElementById('txtFolder').value) ;

	return sUrl ;
}

function SetFrameUrl( url )
{
	document.getElementById('eRunningFrame').src = url ;

	document.getElementById('eUrl').innerHTML = url ;
}

function GetFolders()
{
	SetFrameUrl( BuildBaseUrl( 'GetFolders' ) ) ;
	return false ;
}

function GetFoldersAndFiles()
{
	SetFrameUrl( BuildBaseUrl( 'GetFoldersAndFiles' ) ) ;
	return false ;
}

function CreateFolder()
{
	var sFolder = prompt( 'Type the folder name:', 'Test Folder' ) ;

	if ( ! sFolder )
		return false ;

	var sUrl = BuildBaseUrl( 'CreateFolder' ) ;
	sUrl += '&NewFolderName=' + encodeURIComponent( sFolder ) ;

	SetFrameUrl( sUrl ) ;
	return false ;
}

function OnUploadCompleted( errorNumber, fileName )
{
	switch ( errorNumber )
	{
		case 0 :
			alert( 'File uploaded with no errors' ) ;
			break ;
		case 201 :
			GetFoldersAndFiles() ;
			alert( 'A file with the same name is already available. The uploaded file has been renamed to "' + fileName + '"' ) ;
			break ;
		case 202 :
			alert( 'Invalid file' ) ;
			break ;
		default :
			alert( 'Error on file upload. Error number: ' + errorNumber ) ;
			break ;
	}
}

this.frames.frmUpload = this ;

function SetAction()
{
	var sUrl = BuildBaseUrl( 'FileUpload' ) ;
	document.getElementById('eUrl').innerHTML = sUrl ;
	document.getElementById('frmUpload').action = sUrl ;
}

	</script>
</head>
<body>
	<table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td>
				<table cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td>
							Connector:<br />
							<select id="cmbConnector" name="cmbConnector">
								<option value="asp/connector.asp" selected="selected">ASP</option>
								<option value="aspx/connector.aspx">ASP.Net</option>
								<option value="cfm/connector.cfm">ColdFusion</option>
								<option value="lasso/connector.lasso">Lasso</option>
								<option value="perl/connector.cgi">Perl</option>
								<option value="php/connector.php">PHP</option>
								<option value="py/connector.py">Python</option>
							</select>
						</td>
						<td>
							&nbsp;&nbsp;&nbsp;</td>
						<td>
							Current Folder<br />
							<input id="txtFolder" type="text" value="/" name="txtFolder" /></td>
						<td>
							&nbsp;&nbsp;&nbsp;</td>
						<td>
							Resource Type<br />
							<select id="cmbType" name="cmbType">
								<option value="File" selected="selected">File</option>
								<option value="Image">Image</option>
								<option value="Flash">Flash</option>
								<option value="Media">Media</option>
								<option value="Invalid">Invalid Type (for testing)</option>
							</select>
						</td>
					</tr>
				</table>
				<br />
				<table cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td valign="top">
							<a href="#" onclick="GetFolders();">Get Folders</a></td>
						<td>
							&nbsp;&nbsp;&nbsp;</td>
						<td valign="top">
							<a href="#" onclick="GetFoldersAndFiles();">Get Folders and Files</a></td>
						<td>
							&nbsp;&nbsp;&nbsp;</td>
						<td valign="top">
							<a href="#" onclick="CreateFolder();">Create Folder</a></td>
						<td>
							&nbsp;&nbsp;&nbsp;</td>
						<td valign="top">
							<form id="frmUpload" action="" target="eRunningFrame" method="post" enctype="multipart/form-data">
								File Upload<br />
								<input id="txtFileUpload" type="file" name="NewFile" />
								<input type="submit" value="Upload" onclick="SetAction();" />
							</form>
						</td>
					</tr>
				</table>
				<br />
				URL: <span id="eUrl"></span>
			</td>
		</tr>
		<tr>
			<td height="100%" valign="top">
				<iframe id="eRunningFrame" src="javascript:void(0)" name="eRunningFrame" width="100%"
					height="100%"></iframe>
			</td>
		</tr>
	</table>
</body>
</html>
