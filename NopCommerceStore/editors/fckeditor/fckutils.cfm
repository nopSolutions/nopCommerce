<!---
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
 * ColdFusion integration.
 * This function is used by FCKeditor module to check browser compatibility
 --->
<cfscript>
function FCKeditor_IsCompatibleBrowser()
{
	sAgent = lCase( cgi.HTTP_USER_AGENT );
	isCompatibleBrowser = false;

	// check for Internet Explorer ( >= 5.5 )
	if( find( "msie", sAgent ) and not find( "mac", sAgent ) and not find( "opera", sAgent ) )
	{
		// try to extract IE version
		stResult = reFind( "msie ([5-9]\.[0-9])", sAgent, 1, true );
		if( arrayLen( stResult.pos ) eq 2 )
		{
			// get IE Version
			sBrowserVersion = mid( sAgent, stResult.pos[2], stResult.len[2] );
			if( sBrowserVersion GTE 5.5 )
				isCompatibleBrowser = true;
		}
	}
	// check for Gecko ( >= 20030210+ )
	else if( find( "gecko/", sAgent ) )
	{
		// try to extract Gecko version date
		stResult = reFind( "gecko/([0-9]{8})", sAgent, 1, true );
		if( arrayLen( stResult.pos ) eq 2 )
		{
			// get Gecko build (i18n date)
			sBrowserVersion = mid( sAgent, stResult.pos[2], stResult.len[2] );
			if( sBrowserVersion GTE 20030210 )
				isCompatibleBrowser = true;
		}
	}
	else if( find( "opera/", sAgent ) )
	{
		// try to extract Opera version
		stResult = reFind( "opera/([0-9]+\.[0-9]+)", sAgent, 1, true );
		if( arrayLen( stResult.pos ) eq 2 )
		{
			if ( mid( sAgent, stResult.pos[2], stResult.len[2] ) gte 9.5)
				isCompatibleBrowser = true;
		}
	}
	else if( find( "applewebkit", sAgent ) )
	{
		// try to extract Gecko version date
		stResult = reFind( "applewebkit/([0-9]+)", sAgent, 1, true );
		if( arrayLen( stResult.pos ) eq 2 )
		{
			if( mid( sAgent, stResult.pos[2], stResult.len[2] ) gte 522 )
				isCompatibleBrowser = true;
		}
	}
	return isCompatibleBrowser;
}
</cfscript>
