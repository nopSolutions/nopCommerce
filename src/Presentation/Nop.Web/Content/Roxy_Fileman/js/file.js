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
    li.draggable({helper:makeDragFile,start:startDragFile,cursorAt: { left: 10 ,top:10},delay:200});
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