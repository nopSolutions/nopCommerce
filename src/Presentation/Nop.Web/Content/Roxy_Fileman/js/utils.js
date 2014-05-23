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
var FileTypes = new Array();
FileTypes['image'] = new Array('jpg', 'jpeg', 'png', 'gif');
FileTypes['media'] = new Array('avi', 'flv', 'swf', 'wmv', 'mp3', 'wma', 'mpg','mpeg');
FileTypes['document'] = new Array('doc', 'docx', 'txt', 'rtf', 'pdf', 'xls', 'mdb','html','htm','db');
function RoxyUtils(){}
RoxyUtils.FixPath = function(path){
  if(!path)
    return '';
  var ret = path.replace(/\\/g, '');
  ret = ret.replace(/\/\//g, '/');
  ret = ret.replace(':/', '://');

  return ret;
};
RoxyUtils.FormatDate = function(date){
  var ret = '';
  try{
    ret = $.format.date(date, RoxyFilemanConf.DATEFORMAT);
  }
  catch(ex){
    alert(ex);
    ret = date.toString();
    ret = ret.substr(0, ret.indexOf('UTC'));
  }
  return ret;
};
RoxyUtils.GetPath = function(path){
  var ret = '';
  path = RoxyUtils.FixPath(path);
  if(path.indexOf('/') > -1)
    ret = path.substring(0, path.lastIndexOf('/'));

  return ret;
};
RoxyUtils.GetUrlParam = function(varName, url){
  var ret = '';
  if(!url)
    url = self.location.href;
  if(url.indexOf('?') > -1){
     url = url.substr(url.indexOf('?') + 1);
     url = url.split('&');
     for(i = 0; i < url.length; i++){
       var tmp = url[i].split('=');
       if(tmp[0] && tmp[1] && tmp[0] == varName){
         ret = tmp[1];
         break;
       }
     }
  }

  return ret;
};
RoxyUtils.GetFilename = function(path){
  var ret = path;
  path = RoxyUtils.FixPath(path);
  if(path.indexOf('/') > -1){
    ret = path.substring(path.lastIndexOf('/')+1);
  }

  return ret;
};
RoxyUtils.MakePath = function(){
  ret = '';
  if(arguments && arguments.length > 0){
    for(var i = 0; i < arguments.length; i++){
      ret += ($.isArray(arguments[i])?arguments[i].join('/'):arguments[i]);
      if(i < (arguments.length - 1))
        ret += '/';
    }
    ret = RoxyUtils.FixPath(ret);
  }

  return ret;
};
RoxyUtils.GetFileExt = function(path){
  var ret = '';
  path = RoxyUtils.GetFilename(path);
  if(path.indexOf('.') > -1){
    ret = path.substring(path.lastIndexOf('.') + 1);
  }

  return ret;
};
RoxyUtils.FileExists = function(path) {
  var ret = false;

  $.ajax({
      url: path,
      type: 'HEAD',
      async: false,
      dataType:'text',
      success:function(){ret = true;}
  });

  return ret;
};
RoxyUtils.GetFileIcon = function(path){
  ret = 'images/filetypes/file_extension_' + RoxyUtils.GetFileExt(path).toLowerCase() + '.png';
  if(!fileTypeIcons[RoxyUtils.GetFileExt(path).toLowerCase()]){
    ret = 'images/filetypes/unknown.png';
  }

  return ret;
};
RoxyUtils.GetFileSize = function(path){
  var ret = 0;
  $.ajax({
      url: path,
      type: 'HEAD',
      async: false,
      success:function(d,s, xhr){
        ret = xhr.getResponseHeader('Content-Length');
      }
  });
  if(!ret)
    ret = 0;

  return ret;
};
RoxyUtils.GetFileType = function(path){
  var ret = RoxyUtils.GetFileExt(path).toLowerCase();
  if(ret == 'png' || ret == 'jpg' || ret == 'gif' || ret == 'jpeg')
    ret = 'image';

  return ret;
};
RoxyUtils.IsImage = function(path){
  var ret = false;
  if(RoxyUtils.GetFileType(path) == 'image')
    ret = true;

  return ret;
};
RoxyUtils.FormatFileSize = function(x){
  var suffix = 'B';
  if(!x)
    x = 0;
  if(x > 1024){
    x = x / 1024;
    suffix = 'KB';
  }
  if(x > 1024){
    x = x / 1024;
    suffix = 'MB';
  }
  x = new Number(x);
  return x.toFixed(2) + ' ' + suffix;
};
RoxyUtils.AddParam = function(url, n, v){
  url += (url.indexOf('?') > -1?'&':'?') + n + '='+escape(v);

  return url;
};
RoxyUtils.SelectText = function(field_id, start, end) {
  try{
    var field = document.getElementById(field_id);
    if( field.createTextRange ) {
        var selRange = field.createTextRange();
        selRange.collapse(true);
        selRange.moveStart('character', start);
        selRange.moveEnd('character', end-start);
        selRange.select();
    } else if( field.setSelectionRange ) {
        field.setSelectionRange(start, end);
    } else if( field.selectionStart ) {
        field.selectionStart = start;
        field.selectionEnd = end;
    }
    field.focus();
  }
  catch(ex){}
};
function RoxyFilemanConf(){}
RoxyUtils.LoadConfig = function(){
  $.ajax({
      url: 'conf.json',
      dataType: 'json',
      async:false,
      success: function(data){
        RoxyFilemanConf = data;
      },
      error: function(data){
        alert(t('E_LoadingConf'));
      }
  });
};
function RoxyLang(){}
RoxyUtils.LoadLang = function(){
  var langUrl = '';
  if(RoxyFilemanConf.LANG && RoxyFilemanConf.LANG.toLowerCase() == 'auto'){
    var language = window.navigator.userLanguage || window.navigator.language;
    langUrl = 'lang/' + language.substr(0, 2) + '.json';
    if(!RoxyUtils.FileExists(langUrl))
      langUrl = '';
  }
  else{
    if(RoxyFilemanConf.LANG){
      langUrl = 'lang/' + RoxyFilemanConf.LANG.substr(0, 2).toLowerCase() + '.json';
      if(!RoxyUtils.FileExists(langUrl))
        langUrl = '';
      }
  }
  if(!langUrl)
    langUrl = 'lang/en.json';

  $.ajax({
      url: langUrl,
      dataType: 'json',
      async:false,
      success: function(data){
        RoxyLang = data;
      },
      error: function(data){
        alert('Error loading language file');
      }
  });
};
RoxyUtils.Translate = function(){
  RoxyUtils.LoadLang();

  $('[data-lang-t]').each(function(){
    var key = $(this).attr('data-lang-t');
    $(this).prop('title', t(key));
  });
  $('[data-lang-v]').each(function(){
    var key = $(this).attr('data-lang-v');
    $(this).prop('value', t(key));
  });
  $('[data-lang]').each(function(){
    var key = $(this).attr('data-lang');
    $(this).html(t(key));
  });
};
function t(tag){
  var ret = tag;
  if(RoxyLang && RoxyLang[tag])
    ret = RoxyLang[tag];
  return ret;
}
