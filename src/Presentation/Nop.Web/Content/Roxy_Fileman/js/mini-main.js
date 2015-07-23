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
var fileTypeIcons = new Object();
fileTypeIcons['3gp'] = 'file_extension_3gp.png';
fileTypeIcons['7z'] = 'file_extension_7z.png';
fileTypeIcons['ace'] = 'file_extension_ace.png';
fileTypeIcons['ai'] = 'file_extension_ai.png';
fileTypeIcons['aif'] = 'file_extension_aif.png';
fileTypeIcons['aiff'] = 'file_extension_aiff.png';
fileTypeIcons['amr'] = 'file_extension_amr.png';
fileTypeIcons['asf'] = 'file_extension_asf.png';
fileTypeIcons['asx'] = 'file_extension_asx.png';
fileTypeIcons['avi'] = 'file_extension_avi.png';
fileTypeIcons['bat'] = 'file_extension_bat.png';
fileTypeIcons['bin'] = 'file_extension_bin.png';
fileTypeIcons['bmp'] = 'file_extension_bmp.png';
fileTypeIcons['bup'] = 'file_extension_bup.png';
fileTypeIcons['cab'] = 'file_extension_cab.png';
fileTypeIcons['cbr'] = 'file_extension_cbr.png';
fileTypeIcons['cda'] = 'file_extension_cda.png';
fileTypeIcons['cdl'] = 'file_extension_cdl.png';
fileTypeIcons['cdr'] = 'file_extension_cdr.png';
fileTypeIcons['chm'] = 'file_extension_chm.png';
fileTypeIcons['dat'] = 'file_extension_dat.png';
fileTypeIcons['divx'] = 'file_extension_divx.png';
fileTypeIcons['dll'] = 'file_extension_dll.png';
fileTypeIcons['dmg'] = 'file_extension_dmg.png';
fileTypeIcons['doc'] = 'file_extension_doc.png';
fileTypeIcons['dss'] = 'file_extension_dss.png';
fileTypeIcons['dvf'] = 'file_extension_dvf.png';
fileTypeIcons['dwg'] = 'file_extension_dwg.png';
fileTypeIcons['eml'] = 'file_extension_eml.png';
fileTypeIcons['eps'] = 'file_extension_eps.png';
fileTypeIcons['exe'] = 'file_extension_exe.png';
fileTypeIcons['fla'] = 'file_extension_fla.png';
fileTypeIcons['flv'] = 'file_extension_flv.png';
fileTypeIcons['gif'] = 'file_extension_gif.png';
fileTypeIcons['gz'] = 'file_extension_gz.png';
fileTypeIcons['hqx'] = 'file_extension_hqx.png';
fileTypeIcons['htm'] = 'file_extension_htm.png';
fileTypeIcons['html'] = 'file_extension_html.png';
fileTypeIcons['ifo'] = 'file_extension_ifo.png';
fileTypeIcons['indd'] = 'file_extension_indd.png';
fileTypeIcons['iso'] = 'file_extension_iso.png';
fileTypeIcons['jar'] = 'file_extension_jar.png';
fileTypeIcons['jpeg'] = 'file_extension_jpeg.png';
fileTypeIcons['jpg'] = 'file_extension_jpg.png';
fileTypeIcons['lnk'] = 'file_extension_lnk.png';
fileTypeIcons['log'] = 'file_extension_log.png';
fileTypeIcons['m4a'] = 'file_extension_m4a.png';
fileTypeIcons['m4b'] = 'file_extension_m4b.png';
fileTypeIcons['m4p'] = 'file_extension_m4p.png';
fileTypeIcons['m4v'] = 'file_extension_m4v.png';
fileTypeIcons['mcd'] = 'file_extension_mcd.png';
fileTypeIcons['mdb'] = 'file_extension_mdb.png';
fileTypeIcons['mid'] = 'file_extension_mid.png';
fileTypeIcons['mkv'] = 'file_extension_mkv.png';
fileTypeIcons['mov'] = 'file_extension_mov.png';
fileTypeIcons['mp2'] = 'file_extension_mp2.png';
fileTypeIcons['mp3'] = 'file_extension_mp3.png';
fileTypeIcons['mp4'] = 'file_extension_mp4.png';
fileTypeIcons['mpeg'] = 'file_extension_mpeg.png';
fileTypeIcons['mpg'] = 'file_extension_mpg.png';
fileTypeIcons['msi'] = 'file_extension_msi.png';
fileTypeIcons['mswmm'] = 'file_extension_mswmm.png';
fileTypeIcons['ogg'] = 'file_extension_ogg.png';
fileTypeIcons['pdf'] = 'file_extension_pdf.png';
fileTypeIcons['png'] = 'file_extension_png.png';
fileTypeIcons['pps'] = 'file_extension_pps.png';
fileTypeIcons['ps'] = 'file_extension_ps.png';
fileTypeIcons['psd'] = 'file_extension_psd.png';
fileTypeIcons['pst'] = 'file_extension_pst.png';
fileTypeIcons['ptb'] = 'file_extension_ptb.png';
fileTypeIcons['pub'] = 'file_extension_pub.png';
fileTypeIcons['qbb'] = 'file_extension_qbb.png';
fileTypeIcons['qbw'] = 'file_extension_qbw.png';
fileTypeIcons['qxd'] = 'file_extension_qxd.png';
fileTypeIcons['ram'] = 'file_extension_ram.png';
fileTypeIcons['rar'] = 'file_extension_rar.png';
fileTypeIcons['rm'] = 'file_extension_rm.png';
fileTypeIcons['rmvb'] = 'file_extension_rmvb.png';
fileTypeIcons['rtf'] = 'file_extension_rtf.png';
fileTypeIcons['sea'] = 'file_extension_sea.png';
fileTypeIcons['ses'] = 'file_extension_ses.png';
fileTypeIcons['sit'] = 'file_extension_sit.png';
fileTypeIcons['sitx'] = 'file_extension_sitx.png';
fileTypeIcons['ss'] = 'file_extension_ss.png';
fileTypeIcons['swf'] = 'file_extension_swf.png';
fileTypeIcons['tgz'] = 'file_extension_tgz.png';
fileTypeIcons['thm'] = 'file_extension_thm.png';
fileTypeIcons['tif'] = 'file_extension_tif.png';
fileTypeIcons['tmp'] = 'file_extension_tmp.png';
fileTypeIcons['torrent'] = 'file_extension_torrent.png';
fileTypeIcons['ttf'] = 'file_extension_ttf.png';
fileTypeIcons['txt'] = 'file_extension_txt.png';
fileTypeIcons['vcd'] = 'file_extension_vcd.png';
fileTypeIcons['vob'] = 'file_extension_vob.png';
fileTypeIcons['wav'] = 'file_extension_wav.png';
fileTypeIcons['wma'] = 'file_extension_wma.png';
fileTypeIcons['wmv'] = 'file_extension_wmv.png';
fileTypeIcons['wps'] = 'file_extension_wps.png';
fileTypeIcons['xls'] = 'file_extension_xls.png';
fileTypeIcons['xpi'] = 'file_extension_xpi.png';
fileTypeIcons['zip'] = 'file_extension_zip.png';

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
    //alert(ex);
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
  ret = 'images/filetypes/unknown.png';//'images/filetypes/file_extension_' + RoxyUtils.GetFileExt(path).toLowerCase() + '.png';
  if(fileTypeIcons[RoxyUtils.GetFileExt(path).toLowerCase()]){
    ret = 'images/filetypes/' + fileTypeIcons[RoxyUtils.GetFileExt(path).toLowerCase()];
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
  url += (url.indexOf('?') > -1?'&':'?') + n + '='+encodeURIComponent(v);

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
    if(RoxyUtils.GetUrlParam('langCode')){
      langUrl = 'lang/' + RoxyUtils.GetUrlParam('langCode').substr(0, 2).toLowerCase() + '.json';
    }
    else {
      var language = window.navigator.userLanguage || window.navigator.language;
      langUrl = 'lang/' + language.substr(0, 2) + '.json';
    }
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
RoxyUtils.GetCookies = function() {
  var ret = new Object();
  var tmp = document.cookie.replace(' ','');
  tmp = tmp.split(';');

  for(i in tmp){
    var s = tmp[i].split('=');
    if(s.length > 1){
      ret[$.trim(s[0].toString())] = decodeURIComponent($.trim(s[1].toString())) || '';
    }
  }

  return ret;
}
RoxyUtils.GetCookie = function(key) {
  var tmp = RoxyUtils.GetCookies();

  return tmp[key] || '';
}
RoxyUtils.SetCookie = function(key, val, hours, path) {
  var expires = new Date();
  if(hours){
    expires.setTime(expires.getTime() + (hours * 3600 * 1000));
  }
  
  if(!path){
     path = '/';
  }
  
  document.cookie = key + '=' + encodeURIComponent(val) + '; path=' + path + (hours?'; expires=' + expires.toGMTString():'');
}
RoxyUtils.ToBool = function(val){
  var ret = false;
  val = val.toString().toLowerCase();
  if(val == 'true' || val == 'on' || val == 'yes' || val == '1')
    ret = true;
  
  return ret;
}
RoxyUtils.UnsetCookie = function(key) {
  document.cookie = key + "=; expires=Thu, 01 Jan 1972 00:00:00 UTC"; 
}

function t(tag){
  var ret = tag;
  if(RoxyLang && RoxyLang[tag])
    ret = RoxyLang[tag];
  return ret;
}


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
function File(filePath, fileSize, modTime, w, h){
  this.fullPath = filePath;
  this.type = RoxyUtils.GetFileType(filePath);
  this.name = RoxyUtils.GetFilename(filePath);
  this.ext = RoxyUtils.GetFileExt(filePath);
  this.path = RoxyUtils.GetPath(filePath);
  this.icon = RoxyUtils.GetFileIcon(filePath);
  this.bigIcon = this.icon.replace('filetypes', 'filetypes/big');
  this.image = filePath;
  this.size = (fileSize?fileSize:RoxyUtils.GetFileSize(filePath));
  this.time = modTime;
  this.width = (w? w: 0);
  this.height = (h? h: 0);
  this.Show = function(){
    html = '<li data-path="'+this.fullPath+'" data-time="'+this.time+'" data-icon="'+this.icon+'" data-w="'+this.width+'" data-h="'+this.height+'" data-size="'+this.size+'" data-icon-big="'+(this.IsImage()?this.fullPath:this.bigIcon)+'" title="'+this.name+'">';
    html += '<img src="'+this.icon+'" class="icon">';
    html += '<span class="time">'+RoxyUtils.FormatDate(new Date(this.time * 1000))+'</span>';
    html += '<span class="name">'+this.name+'</span>';
    html += '<span class="size">'+RoxyUtils.FormatFileSize(this.size)+'</span>';
    html += '</li>';
    $('#pnlFileList').append(html);
    var li = $("#pnlFileList li:last");
    if(RoxyFilemanConf.MOVEFILE){
      li.draggable({helper:makeDragFile,start:startDragFile,cursorAt: { left: 10 ,top:10},delay:200});
    }
    li.click(function(e){
       selectFile(this);
    });
    li.dblclick(function(e){
       selectFile(this);
       setFile();
    });
    li.tooltip({show:{delay:700},track: true, content:tooltipContent});

    li.bind('contextmenu', function(e) {
      e.stopPropagation();
      e.preventDefault();
      closeMenus('dir');
      selectFile(this);
      $(this).tooltip('close');
      var t = e.pageY - $('#menuFile').height();
      if(t < 0)
        t = 0;
      $('#menuFile').css({
          top: t+'px',
          left: e.pageX+'px'
      }).show();

      return false;
    });
  };
  this.GetElement = function(){
    return  $('li[data-path="'+this.fullPath+'"]');
  };
  this.IsImage = function(){
    var ret = false;
    if(this.type == 'image')
      ret = true;
    return ret;
  };
  this.Delete = function(){
    if(!RoxyFilemanConf.DELETEFILE){
      alert(t('E_ActionDisabled'));
      return;
    }
    var deleteUrl = RoxyUtils.AddParam(RoxyFilemanConf.DELETEFILE, 'f', this.fullPath);
    var item = this;
    $.ajax({
        url: deleteUrl,
        type: 'POST',
        data: {f: this.fullPath},
        dataType: 'json',
        async:false,
        success: function(data){
            if(data.res.toLowerCase() == 'ok'){
              $('li[data-path="'+item.fullPath+'"]').remove();
              var d = Directory.Parse(item.path);
              if(d){
                d.files--;
                d.Update();
                d.SetStatusBar();
              }
            }
            else{
              alert(data.msg);
            }
        },
        error: function(data){
            alert(t('E_LoadingAjax')+' '+deleteUrl);
        }
    });
  };
  this.Rename = function(newName){
    if(!RoxyFilemanConf.RENAMEFILE){
      alert(t('E_ActionDisabled'));
      return false;
    }
    if(!newName)
      return false;
    var url = RoxyUtils.AddParam(RoxyFilemanConf.RENAMEFILE, 'f', this.fullPath);
    url = RoxyUtils.AddParam(url, 'n', newName);
    var item = this;
    var ret = false;
    $.ajax({
        url: url,
        type: 'POST',
        data: {f: this.fullPath, n: newName},
        dataType: 'json',
        async:false,
        success: function(data){
            if(data.res.toLowerCase() == 'ok'){
              var newPath = RoxyUtils.MakePath(this.path, newName);
              $('li[data-path="'+item.fullPath+'"] .icon').attr('src', RoxyUtils.GetFileIcon(newName));
              $('li[data-path="'+item.fullPath+'"] .name').text(newName);
              $('li[data-path="'+newPath+'"]').attr('data-path', newPath);
              ret = true;
            }
            if(data.msg)
              alert(data.msg);
        },
        error: function(data){
          alert(t('E_LoadingAjax')+' '+url);
        }
    });
    return ret;
  };
  this.Copy = function(newPath){
    if(!RoxyFilemanConf.COPYFILE){
      alert(t('E_ActionDisabled'));
      return;
    }
    var url = RoxyUtils.AddParam(RoxyFilemanConf.COPYFILE, 'f', this.fullPath);
    url = RoxyUtils.AddParam(url, 'n', newPath);
    var item = this;
    var ret = false;
    $.ajax({
        url: url,
        type: 'POST',
        data: {f: this.fullPath, n: newPath},
        dataType: 'json',
        async:false,
        success: function(data){
            if(data.res.toLowerCase() == 'ok'){
              var d = Directory.Parse(newPath);
              if(d){
                d.files++;
                d.Update();
                d.SetStatusBar();
                d.ListFiles(true);
              }
              ret = true;
            }
            if(data.msg)
              alert(data.msg);
        },
        error: function(data){
            alert(t('E_LoadingAjax')+' '+url);
        }
    });
    return ret;
  };
  this.Move = function(newPath){
    if(!RoxyFilemanConf.MOVEFILE){
      alert(t('E_ActionDisabled'));
      return;
    }
    newFullPath = RoxyUtils.MakePath(newPath, this.name);
    var url = RoxyUtils.AddParam(RoxyFilemanConf.MOVEFILE, 'f', this.fullPath);
    url = RoxyUtils.AddParam(url, 'n', newFullPath);
    var item = this;
    var ret = false;
    $.ajax({
        url: url,
        type: 'POST',
        data: {f: this.fullPath, n: newFullPath},
        dataType: 'json',
        async:false,
        success: function(data){
            if(data.res.toLowerCase() == 'ok'){
              $('li[data-path="'+item.fullPath+'"]').remove();
              var d = Directory.Parse(item.path);
              if(d){
                d.files--;
                d.Update();
                d.SetStatusBar();
                d = Directory.Parse(newPath);
                d.files++;
                d.Update();
              }
              ret = true;
            }
            if(data.msg)
              alert(data.msg);
        },
        error: function(data){
            alert(t('E_LoadingAjax')+' '+url);
        }
    });
    return ret;
  };
}
File.Parse = function(path){
  var ret = false;
  var li = $('#pnlFileList').find('li[data-path="'+path+'"]');
  if(li.length > 0)
    ret = new File(li.attr('data-path'), li.attr('data-size'), li.attr('data-time'), li.attr('data-w'), li.attr('data-h'));

  return ret;
};

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
function Directory(fullPath, numDirs, numFiles){
  if(!fullPath) fullPath = '';
  this.fullPath = fullPath;
  this.name = RoxyUtils.GetFilename(fullPath);
  if(!this.name)
    this.name = 'My files';
  this.path = RoxyUtils.GetPath(fullPath);
  this.dirs = (numDirs?numDirs:0);
  this.files = (numFiles?numFiles:0);
  this.filesList = new Array();

  this.Show = function(){
    var html = this.GetHtml();
    var el = null;
    el = $('li[data-path="'+this.path+'"]');
    if(el.length == 0)
      el = $('#pnlDirList');
    else{
      if(el.children('ul').length == 0)
        el.append('<ul></ul>');
      el = el.children('ul');
    }
    if(el){
      el.append(html);
      this.SetEvents();
    }
  };
  this.SetEvents = function(){
    var el = this.GetElement();
    if(RoxyFilemanConf.MOVEDIR){
      el.draggable({helper:makeDragDir,start:startDragDir,cursorAt: { left: 10 ,top:10},delay:200});
    }
    el = el.children('div');
    el.click(function(e){
       selectDir(this);
    });

    el.bind('contextmenu', function(e) {
      e.stopPropagation();
      e.preventDefault();
      closeMenus('file');
      selectDir(this);
      var t = e.pageY - $('#menuDir').height();
      if(t < 0)
        t = 0;
      $('#menuDir').css({
          top: t+'px',
          left: e.pageX+'px'
      }).show();

      return false;
    });

    el.droppable({drop:moveObject,over:dragFileOver,out:dragFileOut});
    el = el.children('.dirPlus');
    el.click(function(e){
      e.stopPropagation();
      var d = Directory.Parse($(this).closest('li').attr('data-path'));
      d.Expand();
    });
  };
  this.GetHtml = function(){
     var html = '<li data-path="'+this.fullPath+'" data-dirs="'+this.dirs+'" data-files="'+this.files+'" class="directory">';
     html += '<div><img src="images/'+(this.dirs > 0?'dir-plus.png':'blank.gif')+'" class="dirPlus" width="9" height="9">';
     html += '<img src="images/folder.png" class="dir"><span class="name">'+this.name+' ('+this.files+')</span></div>';
     html += '</li>';

    return html;
  };
  this.SetStatusBar = function(){
    $('#pnlStatus').html(this.files+' '+(this.files == 1?t('file'):t('files')));
  };
  this.SetSelectedFile = function(path){
    if(path){
      var f = File.Parse(path);
      if(f){
        selectFile(f.GetElement()); 
      }
    }
  };
  this.Select = function(selectedFile){
    var el = this.GetElement();
    el.children('div').addClass('selected');
    $('#pnlDirList li[data-path!="'+this.fullPath+'"] > div').removeClass('selected');
    el.children('img.dir').prop('src', 'images/folder.png');
    this.SetStatusBar();
    var p = this.GetParent();
    while(p){
      p.Expand(true);
      p = p.GetParent();
    }
    this.Expand(true);
    this.ListFiles(true, selectedFile);
    setLastDir(this.fullPath);
  };
  this.GetElement = function(){
    return  $('li[data-path="'+this.fullPath+'"]');
  };
  this.IsExpanded = function(){
    var el = this.GetElement().children('ul');
    return (el && el.is(":visible"));
  };
  this.IsListed = function(){
    if($('#hdDir').val() == this.fullPath)
      return true;
    return false;
  };
  this.GetExpanded = function(el){
    var ret = new Array();
    if(!el)
      el = $('#pnlDirList');
    el.children('li').each(function(){
      var path = $(this).attr('data-path');
      var d = new Directory(path);
      if(d){
        if(d.IsExpanded() && path)
          ret.push(path);
        ret = ret.concat(d.GetExpanded(d.GetElement().children('ul')));
      }
    });

    return ret;
  };
  this.RestoreExpanded = function(expandedDirs){
    for(i = 0; i < expandedDirs.length; i++){
      var d = Directory.Parse(expandedDirs[i]);
      if(d)
        d.Expand(true);
    }
  };
  this.GetParent = function(){
    return Directory.Parse(this.path);
  };
  this.SetOpened = function(){
    var li = this.GetElement();
    if(li.find('li').length < 1)
      li.children('div').children('.dirPlus').prop('src', 'images/blank.gif');
    else if(this.IsExpanded())
      li.children('div').children('.dirPlus').prop('src', 'images/dir-minus.png');
    else
      li.children('div').children('.dirPlus').prop('src', 'images/dir-plus.png');
  };
  this.Update = function(newPath){
    var el = this.GetElement();
    if(newPath){
      this.fullPath = newPath;
      this.name = RoxyUtils.GetFilename(newPath);
      if(!this.name)
        this.name = 'My files';
      this.path = RoxyUtils.GetPath(newPath);
    }
    el.attr('data-path', this.fullPath);
    el.attr('data-dirs', this.dirs);
    el.attr('data-files', this.files);
    el.children('div').children('.name').html(this.name+' ('+this.files+')');
    this.SetOpened();
  };
  this.LoadAll = function(selectedDir){
    var expanded = this.GetExpanded();
    var dirListURL = RoxyFilemanConf.DIRLIST;
    if(!dirListURL){
      alert(t('E_ActionDisabled'));
      return;
    }
    $('#pnlLoadingDirs').show();
    $('#pnlDirList').hide();
    dirListURL = RoxyUtils.AddParam(dirListURL, 'type', RoxyUtils.GetUrlParam('type'));

    var dir = this;
    $.ajax({
        url: dirListURL,
        type:'POST',
        dataType: 'json',
        async: false,
        cache: false,
        success: function(dirs){
            $('#pnlDirList').children('li').remove();
            for(i = 0; i < dirs.length; i++){
              var d = new Directory(dirs[i].p, dirs[i].d, dirs[i].f);
              d.Show();
            }
            $('#pnlLoadingDirs').hide();
            $('#pnlDirList').show();
            dir.RestoreExpanded(expanded);
            var d = Directory.Parse(selectedDir);
            if(d)
              d.Select();
        },
        error: function(data){
          $('#pnlLoadingDirs').hide();
          $('#pnlDirList').show();
          alert(t('E_LoadingAjax')+' '+RoxyFilemanConf.DIRLIST);
        }
    });
  };
  this.Expand = function(show){
    var li = this.GetElement();
    var el = li.children('ul');
    if(this.IsExpanded() && !show)
      el.hide();
    else
      el.show();

    this.SetOpened();
  };
  this.Create = function(newName){
    if(!newName)
      return false;
    else if(!RoxyFilemanConf.CREATEDIR){
      alert(t('E_ActionDisabled'));
      return;
    }
    var url = RoxyUtils.AddParam(RoxyFilemanConf.CREATEDIR, 'd', this.fullPath);
    url = RoxyUtils.AddParam(url, 'n', newName);
    var item = this;
    var ret = false;
    $.ajax({
        url: url,
        type: 'POST',
        data: {d: this.fullPath, n: newName},
        dataType: 'json',
        async:false,
        cache: false,
        success: function(data){
            if(data.res.toLowerCase() == 'ok'){
              item.LoadAll(RoxyUtils.MakePath(item.fullPath, newName));
              ret = true;
            }
            else{
              alert(data.msg);
            }
        },
        error: function(data){
            alert(t('E_LoadingAjax')+' '+item.name);
        }
    });
    return ret;
  };
  this.Delete = function(){
    if(!RoxyFilemanConf.DELETEDIR){
      alert(t('E_ActionDisabled'));
      return;
    }
    var url = RoxyUtils.AddParam(RoxyFilemanConf.DELETEDIR, 'd', this.fullPath);
    var item = this;
    var ret = false;
    $.ajax({
        url: url,
        type: 'POST',
        data: {d: this.fullPath},
        dataType: 'json',
        async:false,
        cache: false,
        success: function(data){
            if(data.res.toLowerCase() == 'ok'){
              var parent = item.GetParent();
              parent.dirs--;
              parent.Update();
              parent.Select();
              item.GetElement().remove();
              ret = true;
            }
            if(data.msg)
              alert(data.msg);
        },
        error: function(data){
            alert(t('E_LoadingAjax')+' '+item.name);
        }
    });
    return ret;
  };
  this.Rename = function(newName){
    if(!newName)
      return false;
    else if(!RoxyFilemanConf.RENAMEDIR){
      alert(t('E_ActionDisabled'));
      return;
    }
    var url = RoxyUtils.AddParam(RoxyFilemanConf.RENAMEDIR, 'd', this.fullPath);
    url = RoxyUtils.AddParam(url, 'n', newName);
    var item = this;
    var ret = false;
    $.ajax({
        url: url,
        type: 'POST',
        data: {d: this.fullPath, n: newName},
        dataType: 'json',
        async:false,
        cache: false,
        success: function(data){
            if(data.res.toLowerCase() == 'ok'){
              var newPath = RoxyUtils.MakePath(item.path, newName);
              item.Update(newPath);
              item.Select();
              ret = true;
            }
            if(data.msg)
              alert(data.msg);
        },
        error: function(data){
            alert(t('E_LoadingAjax')+' '+item.name);
        }
    });
    return ret;
  };
  this.Copy = function(newPath){
    if(!RoxyFilemanConf.COPYDIR){
      alert(t('E_ActionDisabled'));
      return;
    }
    var url = RoxyUtils.AddParam(RoxyFilemanConf.COPYDIR, 'd', this.fullPath);
    url = RoxyUtils.AddParam(url, 'n', newPath);
    var item = this;
    var ret = false;
    $.ajax({
        url: url,
        type: 'POST',
        data: {d: this.fullPath, n: newPath},
        dataType: 'json',
        async:false,
        cache: false,
        success: function(data){
            if(data.res.toLowerCase() == 'ok'){
              var d = Directory.Parse(newPath);
              if(d){
                d.LoadAll(d.fullPath);
              }
              ret = true;
            }
            if(data.msg)
              alert(data.msg);
        },
        error: function(data){
            alert(t('E_LoadingAjax')+' '+url);
        }
    });
    return ret;
  };
  this.Move = function(newPath){
    if(!newPath)
      return false;
    else if(!RoxyFilemanConf.MOVEDIR){
      alert(t('E_ActionDisabled'));
      return;
    }
    var url = RoxyUtils.AddParam(RoxyFilemanConf.MOVEDIR, 'd', this.fullPath);
    url = RoxyUtils.AddParam(url, 'n', newPath);
    var item = this;
    var ret = false;
    $.ajax({
        url: url,
        type: 'POST',
        data: {d: this.fullPath, n: newPath},
        dataType: 'json',
        async:false,
        cache: false,
        success: function(data){
            if(data.res.toLowerCase() == 'ok'){
              item.LoadAll(RoxyUtils.MakePath(newPath, item.name));
              ret = true;
            }
            if(data.msg)
              alert(data.msg);
        },
        error: function(data){
            alert(t('E_LoadingAjax')+' '+item.name);
        }
    });
    return ret;
  };
  this.ListFiles = function(refresh, selectedFile){
    $('#pnlLoading').show();
    $('#pnlEmptyDir').hide();
    $('#pnlFileList').hide();
    $('#pnlSearchNoFiles').hide();
    this.LoadFiles(refresh, selectedFile);
  };
  this.FilesLoaded = function(filesList, selectedFile){
    filesList = this.SortFiles(filesList);
    $('#pnlFileList').html('');
    for(i = 0; i < filesList.length; i++){
      var f = filesList[i];
      f.Show();
    }
    $('#hdDir').val(this.fullPath);
    $('#pnlLoading').hide();
    if($('#pnlFileList').children('li').length == 0)
      $('#pnlEmptyDir').show();
    this.files = $('#pnlFileList').children('li').length;
    this.Update();
    this.SetStatusBar();
    filterFiles();
    switchView();
    $('#pnlFileList').show();
    this.SetSelectedFile(selectedFile);
  };
  this.LoadFiles = function(refresh, selectedFile){
    if(!RoxyFilemanConf.FILESLIST){
      alert(t('E_ActionDisabled'));
      return;
    }
    var ret = new Array();
    var fileURL = RoxyFilemanConf.FILESLIST;
    fileURL = RoxyUtils.AddParam(fileURL, 'd', this.fullPath);
    fileURL = RoxyUtils.AddParam(fileURL, 'type', RoxyUtils.GetUrlParam('type'));
    var item = this;
    if(!this.IsListed() || refresh){

      $.ajax({
          url: fileURL,
          type: 'POST',
          data: {d: this.fullPath, type: RoxyUtils.GetUrlParam('type')},
          dataType: 'json',
          async:true,
          cache: false,
          success: function(files){
              for(i = 0; i < files.length; i++){
                ret.push(new File(files[i].p, files[i].s, files[i].t, files[i].w, files[i].h));
              }
              item.FilesLoaded(ret, selectedFile);
          },
          error: function(data){
              alert(t('E_LoadingAjax')+' '+fileURL);
          }
      });
    }
    else{
      $('#pnlFileList li').each(function(){
        ret.push(new File($(this).attr('data-path'), $(this).attr('data-size'), $(this).attr('data-time'), $(this).attr('data-w'), $(this).attr('data-h')));
      });
      item.FilesLoaded(ret, selectedFile);
    }

    return ret;
  };

  this.SortByName = function(files, order){
     files.sort(function(a, b){
       var x = (order == 'desc'?0:2)
       a = a.name.toLowerCase();
       b = b.name.toLowerCase();
       if(a > b)
         return -1 + x;
       else if(a < b)
         return 1 - x;
       else
        return 0;
     });

     return files;
  };
  this.SortBySize = function(files, order){
     files.sort(function(a, b){
       var x = (order == 'desc'?0:2)
       a = parseInt(a.size);
       b = parseInt(b.size);
       if(a > b)
         return -1 + x;
       else if(a < b)
         return 1 - x;
       else
        return 0;
     });

     return files;
  };
  this.SortByTime = function(files, order){
     files.sort(function(a, b){
       var x = (order == 'desc'?0:2)
       a = parseInt(a.time);
       b = parseInt(b.time);
       if(a > b)
         return -1 + x;
       else if(a < b)
         return 1 - x;
       else
        return 0;
     });

     return files;
  };
  this.SortFiles = function(files){
    var order = $('#ddlOrder').val();
    if(!order)
      order = 'name';

    switch(order){
      case 'size':
        files = this.SortBySize(files, 'asc');
      break;
      case 'size_desc':
        files = this.SortBySize(files, 'desc');
      break;
      case 'time':
        files = this.SortByTime(files, 'asc');
      break;
      case 'time_desc':
        files = this.SortByTime(files, 'desc');
      break;
      case 'name_desc':
        files = this.SortByName(files, 'desc');
      break;
      default:
        files = this.SortByName(files, 'asc');
    }

    return files;
  };
}
Directory.Parse = function(path){
  var ret = false;
  var li = $('#pnlDirList').find('li[data-path="'+path+'"]');
  if(li.length > 0)
    ret = new Directory(li.attr('data-path'), li.attr('data-dirs'), li.attr('data-files'));

  return ret;
};


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

$.ajaxSetup ({cache: false});
function selectFile(item){
  $('#pnlFileList li').removeClass('selected');
  $(item).prop('class', 'selected');
  var html = RoxyUtils.GetFilename($(item).attr('data-path'));
  html += ' ('+t('Size')+': '+RoxyUtils.FormatFileSize($(item).attr('data-size'));
  if($(item).attr('data-w') > 0)
    html += ', '+t('Dimensions')+':'+$(item).attr('data-w')+'x'+$(item).attr('data-h');
  html += ')';
  $('#pnlStatus').html(html);
}
function getLastDir(){
  return RoxyUtils.GetCookie('roxyld');
}
function setLastDir(path){
  RoxyUtils.SetCookie('roxyld', path, 10);
}
function selectDir(item){
  var d = Directory.Parse($(item).parent('li').attr('data-path'));
  if(d){
    d.Select();
  }
}
function startDragDir(){

}
function startDragFile(){
  selectFile(this);
}
function dragFileOver(){
  $(this).children('img.dir').attr('src', 'images/folder-green.png');
}
function dragFileOut(){
  $('#pnlDirList').find('img.dir').attr('src', 'images/folder.png');
}
function makeDragFile(e){
  var f = new File($(e.target).closest('li').attr('data-path'));
  return '<div class="pnlDragFile" data-path="'+f.fullPath+'"><img src="'+f.bigIcon+'" align="absmiddle">&nbsp;'+f.name+'</div>';
}
function makeDragDir(e){
  var f = new Directory($(e.target).attr('data-path')?$(e.target).attr('data-path'):$(e.target).closest('li').attr('data-path'));
  return '<div class="pnlDragDir" data-path="'+f.fullPath+'"><img src="images/folder.png" align="absmiddle">&nbsp;'+f.name+'</div>';
}
function moveDir(e, ui, obj){
  var dir = Directory.Parse(ui.draggable.attr('data-path'));
  var target = Directory.Parse($(obj).parent('li').attr('data-path'));
  if(target.fullPath != dir.path)
    dir.Move(target.fullPath);
}
function moveFile(e, ui, obj){
  var f = new File(ui.draggable.attr('data-path'));
  var d = Directory.Parse($(obj).parent('li').attr('data-path'));
  var src = Directory.Parse(f.path);
  if(f.path != d.fullPath)
    f.Move(d.fullPath);
}
function moveObject(e, ui){
  e.stopPropagation();
  if(ui.draggable.hasClass('directory'))
    moveDir(e, ui, this);
  else
    moveFile(e, ui, this);
  dragFileOut();
}
function clickFirstOnEnter(elId){
  $('#'+elId).unbind('keypress');
  $('.actions input').each(function(){this.blur();});
  $('#'+elId).keypress(function(e) {
      if (e.keyCode == $.ui.keyCode.ENTER) {
        e.stopPropagation();
        $(this).parent().find('.ui-dialog-buttonset button').eq(0).trigger('click');
      }
    });
}
function addDir(){
  var f = getSelectedDir();
  if(!f)
    return;
  clickFirstOnEnter('pnlDirName');
  $('#txtDirName').val('');

  var dialogButtons = {};
  dialogButtons[t('CreateDir')] = function(){
    var newName = $.trim($('#txtDirName').val());
    if(!newName)
      alert(t('E_MissingDirName'));
    if(f.Create(newName)){
      $('#pnlDirName').dialog('close');
    }
  };
  dialogButtons[t('Cancel')] = function(){$('#pnlDirName').dialog('close');};
  $('#pnlDirName').dialog({title: t('T_CreateDir'),modal:true,buttons:dialogButtons});
}
var uploadFileList = new Array();
function showUploadList(files){
  var filesPane = $('#uploadFilesList');
  filesPane.html('');
  clearFileField();
  for(i = 0; i < files.length; i++){
    filesPane.append('<div class="fileUpload"><div class="fileName">'+files[i].name+' ('+RoxyUtils.FormatFileSize(files[i].size)+')<span class="progressPercent"></span><div class="uploadProgress"><div class="stripes"></div></div></div><a class="removeUpload" onclick="removeUpload(' + i + ')"></a></div>');
  }
  if(files.length > 0)
    $('#btnUpload').button('enable');
  else
    $('#btnUpload').button('disable');
}
function listUploadFiles(files){
  if(!window.FileList) {
    $('#btnUpload').button('enable');
  }
  else if(files.length > 0) {
    uploadFileList = new Array();
    addUploadFiles(files);
  }
}
function addUploadFiles(files){
  for(i = 0; i < files.length; i++)
    uploadFileList.push(files[i]);
  showUploadList(uploadFileList);
}
function removeUpload(i){
  var el = findUploadElement(i);
  el.remove();
  try{
    uploadFileList.splice(i, 1);
    showUploadList(uploadFileList);
  }
  catch(ex){
    //alert(ex); 
  }
}
function findUploadElement(i){
  return $('#uploadFilesList .fileUpload:eq(' + (i)+ ')');
} 
function updateUploadProgress(e, i){
  var el = findUploadElement(i);
  var percent = 99;
  if (e.lengthComputable) {
    percent = Math.floor((e.loaded / e.total) * 100);
  }
  if(percent > 99)
    percent = 99;
  el.find('.uploadProgress').css('width', percent + '%');
  el.find('.progressPercent').html(' - ' + percent + '%');
}
function uploadComplete(e, i){
  uploadFinished(e, i, 'ok');
}
function uploadError(e, i){
  setUploadError(i);
  uploadFinished(e, i, 'error');
}
function setUploadError(i){
  var el = findUploadElement(i);
  el.find('.uploadProgress').css('width', '100%').addClass('uploadError').removeClass('uploadComplete');
  el.find('.progressPercent').html(' - <span class="error">' + t('E_UploadingFile')+'</span>');
}
function setUploadSuccess(i){
  var el = findUploadElement(i);
  el.find('.uploadProgress').css('width', '100%').removeClass('uploadError').addClass('uploadComplete');
  el.find('.progressPercent').html(' - 100%');
}
function uploadCanceled(e, i){
  uploadFinished(e, i, 'error');
}
function uploadFinished(e, i, res){
  var el = findUploadElement(i);
  var httpRes = null;
  try{
    httpRes = JSON.parse(e.target.responseText);
  }
  catch(ex){}
  
  if((httpRes && httpRes.res == 'error') || res != 'ok'){
    res = 'error';
    setUploadError(i);
  }
  else{
    res = 'ok';
    setUploadSuccess(i)
  }
    
  el.attr('data-ulpoad', res);
  checkUploadResult();
}
function checkUploadResult(){
  var all = $('#uploadFilesList .fileUpload').length;
  var completed = $('#uploadFilesList .fileUpload[data-ulpoad]').length;
  var success = $('#uploadFilesList .fileUpload[data-ulpoad="ok"]').length;
  if(completed == all){
     //$('#uploadResult').html(success + ' files uploaded; '+(all - success)+' failed');
     uploadFileList = new Array();
     var d = Directory.Parse($('#hdDir').val());
     d.ListFiles(true);
     $('#btnUpload').button('disable');
  }
}
function fileUpload(f, i){
  var http = new XMLHttpRequest();
  var fData = new FormData();
  var el = findUploadElement(i);
  el.find('.removeUpload').remove();
  fData.append("action", 'upload');
  fData.append("method", 'ajax');
  fData.append("d", $('#hdDir').attr('value'));
  fData.append("files[]", f);
  http.upload.addEventListener("progress", function(e){updateUploadProgress(e, i);}, false);
  http.addEventListener("load", function(e){uploadComplete(e, i);}, false);
  http.addEventListener("error", function(e){uploadError(e, i);}, false);
  http.addEventListener("abort", function(e){uploadCanceled(e, i);}, false);
  http.open("POST", RoxyFilemanConf.UPLOAD, true);
  http.setRequestHeader("Accept", "*/*");
  http.send(fData);
}
function dropFiles(e, append){
  if(e && e.dataTransfer && e.dataTransfer.files){
    addFile();
    if(append)
      addUploadFiles(e.dataTransfer.files);
    else
      listUploadFiles(e.dataTransfer.files);
  }
  else
    addFile();
}
function clearFileField(selector){
  if(!selector)
    selector = '#fileUploads';
  try{
    $(selector).val('');
    $(selector).val(null);
  }
  catch(ex){}
}
function addFileClick(){
  $('#uploadResult').html('');
  showUploadList(new Array());
  addFile();
}
function addFile(){
  clickFirstOnEnter('dlgAddFile');
  $('#uploadResult').html('');
  clearFileField();
  var dialogButtons = {};
  dialogButtons[t('Upload')] = {id:'btnUpload', text: t('Upload'), disabled:true, click:function(){
    if(!$('#fileUploads').val() && (!uploadFileList || uploadFileList.length == 0))
      alert(t('E_SelectFiles'));
    else{
      if(!RoxyFilemanConf.UPLOAD){
        alert(t('E_ActionDisabled'));
        //$('#dlgAddFile').dialog('close');
      }
      else{
        if(window.FormData && window.XMLHttpRequest && window.FileList && uploadFileList && uploadFileList.length > 0){
          for(i = 0; i < uploadFileList.length; i++){
            fileUpload(uploadFileList[i], i);
          } 
        }
        else{
          document.forms['addfile'].action = RoxyFilemanConf.UPLOAD;
          document.forms['addfile'].submit();
        }
      }
    }
  }};
  
  dialogButtons[t('Cancel')] = function(){$('#dlgAddFile').dialog('close');};
  $('#dlgAddFile').dialog({title:t('T_AddFile'),modal:true,buttons:dialogButtons,width:400});
}
function fileUploaded(res){
  if(res.res == 'ok' && res.msg){
     $('#dlgAddFile').dialog('close');
     var d = Directory.Parse($('#hdDir').val());
     d.ListFiles(true);
     alert(res.msg);
  }
  else if(res.res == 'ok'){
     $('#dlgAddFile').dialog('close');
     var d = Directory.Parse($('#hdDir').val());
     d.ListFiles(true);
  }
  else
    alert(res.msg);
}
function renameDir(){
  var f = getSelectedDir();
  if(!f)
    return;
  if($('[data-path="'+f.fullPath+'"]').parents('li').length < 1){
     alert(t('E_CannotRenameRoot'));
     return;
  }
  clickFirstOnEnter('pnlDirName');
  $('#txtDirName').val(f.name);

  var dialogButtons = {};
  dialogButtons[t('RenameDir')] = function(){
    var newName = $.trim($('#txtDirName').val());
    if(!newName)
      alert(t('E_MissingDirName'));
    if(f.Rename(newName))
      $('#pnlDirName').dialog('close');
  };
  dialogButtons[t('Cancel')] = function(){$('#pnlDirName').dialog('close');};

  $('#pnlDirName').dialog({title:t('T_RenameDir'),modal:true,buttons:dialogButtons});
  RoxyUtils.SelectText('txtDirName', 0, new String(f.name).length);
}
function renameFile(){
  var f = getSelectedFile();
  if(!f)
    return;
  clickFirstOnEnter('pnlRenameFile');
  $('#txtFileName').val(f.name);

  var dialogButtons = {};
  dialogButtons[t('RenameFile')] = function(){
    var newName = $.trim($('#txtFileName').val());
    if(!newName)
      alert('Missing file name');
    else if(f.Rename(newName)){
      $('li[data-path="'+f.fullPath+'"] .name').text(newName);
      $('li[data-path="'+f.fullPath+'"]').attr('data-path', RoxyUtils.MakePath(f.path, newName));
      $('#pnlRenameFile').dialog('close');
    }
  };
  dialogButtons[t('Cancel')] = function(){$('#pnlRenameFile').dialog('close');};

  $('#pnlRenameFile').dialog({title:t('T_RenameFile'),modal:true,buttons:dialogButtons});
  if(f.name.lastIndexOf('.') > 0)
    RoxyUtils.SelectText('txtFileName', 0, f.name.lastIndexOf('.'));
}
function getSelectedFile(){
  var ret = null;
  if($('#pnlFileList .selected').length > 0)
    ret = new File($('#pnlFileList .selected').attr('data-path'));
  return ret;
}
function getSelectedDir(){
  var ret = null;
  if($('#pnlDirList .selected'))
    ret = Directory.Parse($('#pnlDirList .selected').closest('li').attr('data-path'));

  return ret;
}
function deleteDir(path){
  var d = null;
  if(path)
    d = Directory.Parse(path);
  else
    d = getSelectedDir();

  if(d && confirm(t('Q_DeleteFolder'))){
    d.Delete();
  }
}
function deleteFile(){
  var f = getSelectedFile();
  if(f && confirm(t('Q_DeleteFile'))){
    f.Delete();
  }
}
function previewFile(){
  var f = getSelectedFile();
  if(f){
    window.open(f.fullPath);
  }
}
function downloadFile(){
  var f = getSelectedFile();
  if(f && RoxyFilemanConf.DOWNLOAD){
    var url = RoxyUtils.AddParam(RoxyFilemanConf.DOWNLOAD, 'f', f.fullPath);
    window.frames['frmUploadFile'].location.href = url;
  }
  else if(!RoxyFilemanConf.DOWNLOAD)
    alert(t('E_ActionDisabled'));
}
function downloadDir(){
  var d = getSelectedDir();
  if(d && RoxyFilemanConf.DOWNLOADDIR){
    var url = RoxyUtils.AddParam(RoxyFilemanConf.DOWNLOADDIR, 'd', d.fullPath);
    window.frames['frmUploadFile'].location.href = url;
  }
  else if(!RoxyFilemanConf.DOWNLOAD)
    alert(t('E_ActionDisabled'));
}
function closeMenus(el){
  if(!el || el == 'dir')
    $('#menuDir').fadeOut();
  if(!el || el == 'file')
    $('#menuFile').fadeOut();
}
function selectFirst(){
  var item = $('#pnlDirList li:first').children('div').first();
  if(item.length > 0)
    selectDir(item);
  else
    window.setTimeout('selectFirst()', 300);
}
function tooltipContent(){
  if($('#menuFile').is(':visible'))
    return '';
  var html = '';
  var f = File.Parse($(this).attr('data-path'));
  if($('#hdViewType').val() == 'thumb' && f.IsImage()){
    html = f.fullPath+'<br><span class="filesize">'+t('Size')+': '+RoxyUtils.FormatFileSize(f.size) + ' '+t('Dimensions')+': '+f.width+'x'+f.height+'</span>';
  }
  else if(f.IsImage()){
    if(RoxyFilemanConf.GENERATETHUMB){
      imgUrl = RoxyUtils.AddParam(RoxyFilemanConf.GENERATETHUMB, 'f', f.fullPath);
      imgUrl = RoxyUtils.AddParam(imgUrl, 'width', RoxyFilemanConf.PREVIEW_THUMB_WIDTH);
      imgUrl = RoxyUtils.AddParam(imgUrl, 'height', RoxyFilemanConf.PREVIEW_THUMB_HEIGHT);
    }
    else
      imgUrl = f.fullPath;
    html = '<img src="'+imgUrl+'" class="imgPreview"><br>'+f.name+' <br><span class="filesize">'+t('Size')+': '+RoxyUtils.FormatFileSize(f.size) + ' '+t('Dimensions')+': '+f.width+'x'+f.height+'</span>';
  }
  else
    html = f.fullPath+' <span class="filesize">'+t('Size')+': '+RoxyUtils.FormatFileSize(f.size) + '</span>';
  return html;
}
function filterFiles(){
  var str = $('#txtSearch').val();
  $('#pnlSearchNoFiles').hide();
  if($('#pnlFileList li').length == 0)
    return;
  if(!str){
    $('#pnlFileList li').show();
    return;
  }
  var i = 0;
  $('#pnlFileList li').each(function(){
      var name = $(this).children('.name').text();
      if(name.toLowerCase().indexOf(str.toLowerCase()) > -1){
        i++;
        $(this).show();
      }
      else{
        $(this).removeClass('selected');
        $(this).hide();
      }
  });
  if(i == 0)
    $('#pnlSearchNoFiles').show();
}
function sortFiles(){
  var d = getSelectedDir();
  if(!d)
    return;
  d.ListFiles();
  filterFiles();
  switchView($('#hdViewType').val());
}
function switchView(t){
  if(t == $('#hdViewType').val())
    return;
  if(!t)
    t = $('#hdViewType').val();
  $('.btnView').removeClass('selected');
  if(t == 'thumb'){
    $('#pnlFileList .icon').attr('src', 'images/blank.gif');
    $('#pnlFileList').addClass('thumbView');
    if($('#dynStyle').length == 0){
      $('head').append('<style id="dynStyle" />');
      var rules = 'ul#pnlFileList.thumbView li{width:'+RoxyFilemanConf.THUMBS_VIEW_WIDTH+'px;}';
      rules += 'ul#pnlFileList.thumbView li{height:'+(parseInt(RoxyFilemanConf.THUMBS_VIEW_HEIGHT) + 20)+'px;}';
      rules += 'ul#pnlFileList.thumbView .icon{width:'+RoxyFilemanConf.THUMBS_VIEW_WIDTH+'px;}';
      rules += 'ul#pnlFileList.thumbView .icon{height:'+RoxyFilemanConf.THUMBS_VIEW_HEIGHT+'px;}';
      $('#dynStyle').html(rules);
    }
    $('#pnlFileList li').each(function(){
      
      //$('ul#pnlFileList.thumbView li').css('width', RoxyFilemanConf.THUMBS_VIEW_WIDTH + 'px');
      //$('ul#pnlFileList.thumbView li').css('height', (parseInt(RoxyFilemanConf.THUMBS_VIEW_HEIGHT) + 20) + 'px');
      //$('ul#pnlFileList.thumbView .icon').css('width', RoxyFilemanConf.THUMBS_VIEW_WIDTH + 'px');
      //$('ul#pnlFileList.thumbView .icon').css('height', RoxyFilemanConf.THUMBS_VIEW_HEIGHT + 'px');
      var imgUrl = $(this).attr('data-icon-big');
      if(RoxyFilemanConf.GENERATETHUMB && RoxyUtils.IsImage($(this).attr('data-path'))){
        imgUrl = RoxyUtils.AddParam(RoxyFilemanConf.GENERATETHUMB, 'f', imgUrl);
        imgUrl = RoxyUtils.AddParam(imgUrl, 'width', RoxyFilemanConf.THUMBS_VIEW_WIDTH);
        imgUrl = RoxyUtils.AddParam(imgUrl, 'height', RoxyFilemanConf.THUMBS_VIEW_HEIGHT);
      }
      $(this).children('.icon').css('background-image', 'url('+imgUrl+')');
      $(this).tooltip('option', 'show', {delay:50});
    });
    $('#btnThumbView').addClass('selected');
  }
  else{
    $('#pnlFileList').removeClass('thumbView');
    $('#pnlFileList li').each(function(){
      $(this).children('.icon').css('background-image','').attr('src', $(this).attr('data-icon'));
      $(this).tooltip('option', 'show', {delay:500});
    });
    $('#btnListView').addClass('selected');
  }
  $('#hdViewType').val(t);
  RoxyUtils.SetCookie('roxyview', t, 10);
}
var clipBoard = null;
function Clipboard(a, obj){
  this.action = a;
  this.obj = obj;
}
function cutDir(){
  var d = getSelectedDir();
  if(d){
    setClipboard('cut', d);
    d.GetElement().addClass('pale');
  }
}
function copyDir(){
  var d = getSelectedDir();
  if(d){
    setClipboard('copy', d);
  }
}
function cutFile(){
  var f = getSelectedFile();
  if(f){
    setClipboard('cut', f);
    f.GetElement().addClass('pale');
  }
}
function copyFile(){
  var f = getSelectedFile();
  if(f){
    setClipboard('copy', f);
  }
}
function pasteToFiles(e, el){
  if($(el).hasClass('pale')){
    e.stopPropagation();
    return false;
  }
  var d = getSelectedDir();
  if(!d)
    d = Directory.Parse($('#pnlDirList li:first').children('div').first());
  if(d && clipBoard && clipBoard.obj){
    if(clipBoard.action == 'copy')
      clipBoard.obj.Copy(d.fullPath);
    else{
      clipBoard.obj.Move(d.fullPath);
      clearClipboard();
    }
  }
  return true;
}
function pasteToDirs(e, el){
  if($(el).hasClass('pale')){
    e.stopPropagation();
    return false;
  }
  var d = getSelectedDir();
  if(!d)
    d = Directory.Parse($('#pnlDirList li:first').children('div').first());
  if(clipBoard && d){
    if(clipBoard.action == 'copy')
      clipBoard.obj.Copy(d.fullPath);
    else{
      clipBoard.obj.Move(d.fullPath);
      clearClipboard();
      d.ListFiles(true);
    }
  }
  else
    alert('error');
  return true;
}
function clearClipboard(){
  $('#pnlDirList li').removeClass('pale');
  $('#pnlFileList li').removeClass('pale');
  clipBoard = null;
  $('.paste').addClass('pale');
}
function setClipboard(a, obj){
  clearClipboard();
  if(obj){
    clipBoard = new Clipboard(a, obj);
    $('.paste').removeClass('pale');
  }
}
function ResizeLists(){
  var tmp = $(window).innerHeight() - $('#fileActions .actions').outerHeight() - $('.bottomLine').outerHeight();
  $('.scrollPane').css('height', tmp);
}
function removeDisabledActions(){
  if(RoxyFilemanConf.CREATEDIR == ''){
    $('#mnuCreateDir').next().remove();
    $('#mnuCreateDir').remove();
    $('#btnAddDir').remove();
  }
  if(RoxyFilemanConf.DELETEDIR == ''){
    $('#mnuDeleteDir').prev().remove();
    $('#mnuDeleteDir').remove();
    $('#btnDeleteDir').remove();
  }
  if(RoxyFilemanConf.MOVEDIR == ''){
    $('#mnuDirCut').next().remove();
    $('#mnuDirCut').remove();
  }
  if(RoxyFilemanConf.COPYDIR == ''){
    $('#mnuDirCopy').next().remove();
    $('#mnuDirCopy').remove();
  }
  if(RoxyFilemanConf.COPYDIR == '' && RoxyFilemanConf.MOVEDIR == ''){
    $('#mnuDirPaste').next().remove();
    $('#mnuDirPaste').remove();
  }
  if(RoxyFilemanConf.RENAMEDIR == ''){
    $('#mnuRenameDir').next().remove();
    $('#mnuRenameDir').remove();
    $('#btnRenameDir').remove();
  }
  if(RoxyFilemanConf.UPLOAD == ''){
    $('#btnAddFile').remove();
  }
  if(RoxyFilemanConf.DOWNLOAD == ''){
    $('#mnuDownload').next().remove();
    $('#mnuDownload').remove();
  }
  if(RoxyFilemanConf.DOWNLOADDIR == ''){
    $('#mnuDownloadDir').next().remove();
    $('#mnuDownloadDir').remove();
  }
  if(RoxyFilemanConf.DELETEFILE == ''){
    $('#mnuDeleteFile').prev().remove();
    $('#mnuDeleteFile').remove();
    $('#btnDeleteFile').remove();
  }
  if(RoxyFilemanConf.MOVEFILE == ''){
    $('#mnuFileCut').next().remove();
    $('#mnuFileCut').remove();
  }
  if(RoxyFilemanConf.COPYFILE == ''){
    $('#mnuFileCopy').next().remove();
    $('#mnuFileCopy').remove();
  }
  if(RoxyFilemanConf.COPYFILE == '' && RoxyFilemanConf.MOVEFILE == ''){
    $('#mnuFilePaste').next().remove();
    $('#mnuFilePaste').remove();
  }
  if(RoxyFilemanConf.RENAMEFILE == ''){
    $('#mnuRenameFile').next().remove();
    $('#mnuRenameFile').remove();
    $('#btnRenameFile').remove();
  }
}
function getPreselectedFile(){
  var filePath = RoxyUtils.GetUrlParam('selected');
  if(!filePath){
    switch(getFilemanIntegration()){
      case 'ckeditor':
        try{
          var dialog = window.opener.CKEDITOR.dialog.getCurrent();
          filePath = dialog.getValueOf('info', (dialog.getName() == 'link'?'url':'txtUrl'));
        }
        catch(ex){}
      break;
      case 'tinymce3':
        try{
          var win = tinyMCEPopup.getWindowArg("window");
          filePath = win.document.getElementById(tinyMCEPopup.getWindowArg("input")).value;
          if(filePath.indexOf('..') == 0)
            filePath = filePath.substr(2);
        }
        catch(ex){}
      break;
      case 'tinymce4':
        try{
          var win = (window.opener?window.opener:window.parent);
          filePath = win.document.getElementById(RoxyUtils.GetUrlParam('input')).value;
          if(filePath.indexOf('..') == 0)
            filePath = filePath.substr(2);
        }
        catch(ex){}
      break;
      default:
        filePath = GetSelectedValue();     
      break;
    }
  }
  if(RoxyFilemanConf.RETURN_URL_PREFIX){
    var prefix = RoxyFilemanConf.RETURN_URL_PREFIX;
    if(filePath.indexOf(prefix) == 0){
      if(prefix.substr(-1) == '/')
        prefix = prefix.substr(0, prefix.length - 1);
      filePath = filePath.substr(prefix.length);
    }
  }
  
  return filePath;
}
function initSelection(filePath){
  var hasSelection = false, fileSelected = true;
  if(!filePath)
    filePath = getPreselectedFile();
  if(!filePath && RoxyUtils.ToBool(RoxyFilemanConf.OPEN_LAST_DIR)){
    filePath = getLastDir();
    fileSelected = false;
  }
  if(filePath){
    var p = (fileSelected? RoxyUtils.GetPath(filePath): filePath);
    var d = tmp = Directory.Parse(p);
    do{
      if(tmp){
        tmp.Expand(true);
        hasSelection = true; 
      }
      tmp = Directory.Parse(tmp.path);
    }while(tmp);
    
    if(d){
      d.Select(filePath);
      hasSelection = true; 
    }
  }
  if(!hasSelection)
    selectFirst();
}
$(function(){
  RoxyUtils.LoadConfig();
  var d = new Directory();
  d.LoadAll();
  $('#wraper').show();
  
  window.setTimeout('initSelection()', 100);

  RoxyUtils.Translate();
  $('body').click(function(){
    closeMenus();
  });
  
  var viewType = RoxyUtils.GetCookie('roxyview');
  if(!viewType)
    viewType = RoxyFilemanConf.DEFAULTVIEW;
  if(viewType)
    switchView(viewType);
    
  ResizeLists();
  $(".actions input").tooltip({track: true});
  $( window ).resize(ResizeLists);
  
  document.oncontextmenu = function() {return false;};
  removeDisabledActions();
  $('#copyYear').html(new Date().getFullYear());
  if(RoxyFilemanConf.UPLOAD && RoxyFilemanConf.UPLOAD != ''){
    var dropZone = document.getElementById('fileActions');
    dropZone.ondragover = function () { return false; };
    dropZone.ondragend = function () { return false; };
    dropZone.ondrop = function (e) {
      e.preventDefault();
      e.stopPropagation();
      dropFiles(e);
    };
    
    dropZone = document.getElementById('dlgAddFile');
    dropZone.ondragover = function () { return false; };
    dropZone.ondragend = function () { return false; };
    dropZone.ondrop = function (e) {
      e.preventDefault();
      e.stopPropagation();
      dropFiles(e, true);
    };
  }
  
  if(getFilemanIntegration() == 'tinymce3'){
    try {
      $('body').append('<script src="js/tiny_mce_popup.js"><\/script>');
    }
    catch(ex){}
  }
});
function getFilemanIntegration(){
  var integration = RoxyUtils.GetUrlParam('integration');
  if(!integration)
    integration = RoxyFilemanConf.INTEGRATION;
    
  return integration.toLowerCase();
}
function setFile(){
  var f = getSelectedFile();
  if(!f){
    alert(t('E_NoFileSelected'));
    return;
  }
  var insertPath = f.fullPath;
  if(RoxyFilemanConf.RETURN_URL_PREFIX){
    var prefix = RoxyFilemanConf.RETURN_URL_PREFIX;
    if(prefix.substr(-1) == '/')
      prefix = prefix.substr(0, prefix.length - 1);
    insertPath = prefix + (insertPath.substr(0, 1) != '/'? '/': '') + insertPath;
  }
  switch(getFilemanIntegration()){
      case 'ckeditor':
      window.opener.CKEDITOR.tools.callFunction(RoxyUtils.GetUrlParam('CKEditorFuncNum'), insertPath);
      self.close();
    break;
    case 'tinymce3':
      var win = tinyMCEPopup.getWindowArg("window");
      win.document.getElementById(tinyMCEPopup.getWindowArg("input")).value = insertPath;
      if (typeof(win.ImageDialog) != "undefined") {
          if (win.ImageDialog.getImageData)
              win.ImageDialog.getImageData();

          if (win.ImageDialog.showPreviewImage)
              win.ImageDialog.showPreviewImage(insertPath);
      }
      tinyMCEPopup.close();
    break;
    case 'tinymce4':
      var win = (window.opener?window.opener:window.parent);
      win.document.getElementById(RoxyUtils.GetUrlParam('input')).value = insertPath;
      if (typeof(win.ImageDialog) != "undefined") {
          if (win.ImageDialog.getImageData)
              win.ImageDialog.getImageData();
          if (win.ImageDialog.showPreviewImage)
              win.ImageDialog.showPreviewImage(insertPath);
      }
      win.tinyMCE.activeEditor.windowManager.close();
    break;
    default:
      FileSelected(f);
    break;
  }
}
