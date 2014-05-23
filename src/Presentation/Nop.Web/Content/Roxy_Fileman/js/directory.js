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
    el.draggable({helper:makeDragDir,start:startDragDir,cursorAt: { left: 10 ,top:10},delay:200});
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
  this.Select = function(){
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
    this.ListFiles(true);
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
        dataType: 'json',
        async:true,
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
        dataType: 'json',
        async:false,
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
        dataType: 'json',
        async:false,
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
        dataType: 'json',
        async:false,
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
        dataType: 'json',
        async:false,
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
        dataType: 'json',
        async:false,
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
  this.ListFiles = function(refresh){
    $('#pnlLoading').show();
    $('#pnlEmptyDir').hide();
    $('#pnlFileList').hide();
    $('#pnlSearchNoFiles').hide();
    this.LoadFiles(refresh);
  };
  this.FilesLoaded = function(filesList){
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
  };
  this.LoadFiles = function(refresh){
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
          dataType: 'json',
          async:true,
          success: function(files){
              for(i = 0; i < files.length; i++){
                ret.push(new File(files[i].p, files[i].s, files[i].t, files[i].w, files[i].h));
              }
              item.FilesLoaded(ret);
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
      item.FilesLoaded(ret);
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
