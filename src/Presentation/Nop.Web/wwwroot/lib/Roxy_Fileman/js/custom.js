/*
  RoxyFileman - web based file manager. Ready to use with CKEditor, TinyMCE. 
  Can be easily integrated with any other WYSIWYG editor or CMS.

  Copyright (C) 2013, RoxyFileman.com - Lyubomir Arsov. All rights reserved.
  For licensing, see LICENSE.txt or http://RoxyFileman.com/license

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.

  Contact: Lyubomir Arsov, liubo (at) web-lobby.com
*/
function FileSelected(file) {
  if (window.opener && window.opener.ROXY_CALLBACK) {
    window.opener.ROXY_CALLBACK(file);
    window.close();
  } else if (window.ROXY_CALLBACK) {
    window.ROXY_CALLBACK(file);
    window.close();
  } else {
    alert('"' + file.fullPath + '" selected. Integration callback not found.');
  }
}

function getInsertPath(insertPath) {
  if(RoxyFilemanConf.RETURN_URL_PREFIX){
    var prefix = RoxyFilemanConf.RETURN_URL_PREFIX;
    if(prefix.substr(-1) == '/')
      prefix = prefix.substr(0, prefix.length - 1);
    insertPath = prefix + (insertPath.substr(0, 1) != '/'? '/': '') + insertPath;
  }

  return insertPath;
}

function GetSelectedValue(){
  /**
  * This function is called to retrieve selected value when custom integration is used.
  * Url parameter selected will override this value.
  */
  
  return "";
}
