Type.registerNamespace("Sys.Extended.UI.HTMLEditor.CustomToolbarButton");

Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertPicture = function(element) {
Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertPicture.initializeBase(this, [element]);
}

Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertPicture.prototype = {
    callMethod: function() {
        if (!Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertPicture.callBaseMethod(this, "callMethod")) return false;
        var editor = this._designPanel;
        var editPanel = this._editPanel;
        
        externaleditorcontainer = editor; //set global continer when opening popup
        externaleditorpanel = editPanel; //set global continer when opening popup

        try {
            // For 'Undo'
            editor._saveContent();

            // What to do - insert date at current selection
            //---------------------------------------------------
            var picturewindow = PictureBrowserOpener();
            //---------------------------------------------------

            // Notify Editor about content changed and update toolbars linked to the edit panel
            // setTimeout(function() { editor.onContentChanged(); editPanel.updateToolbar(); }, 0);
            // Ensure focus in design panel
            //editor.focusEditor();
            
            return true;
        } catch (e) {}
    }
}

Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertPicture.registerClass("Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertPicture", Sys.Extended.UI.HTMLEditor.ToolbarButton.MethodButton);
Sys.Application.notifyScriptLoaded();


function PictureBrowserOpener()
{
  var picturebrowser = window.open('PictureBrowser.aspx', 
      'PictureBrowser', 
      'width=800, \
       height=600, \
       directories=no, \
       location=no, \
       menubar=no, \
       resizable=yes, \
       scrollbars=1, \
       status=no, \
       toolbar=no'); 
  return picturebrowser;
}

function InsertPictureFromWindow(url)
{
    if (externaleditorcontainer!=undefined && externaleditorpanel!=undefined)
    {
    
    
     try {
            // For 'Undo'
            externaleditorcontainer._saveContent();

            // What to do - insert date at current selection
            //---------------------------------------------------
            externaleditorcontainer.insertHTML("<img src=\"" + url + "\" alt=\"pic\" />");
            //---------------------------------------------------

            // Ensure focus in design panel
            externaleditorcontainer.focusEditor();
            
            setTimeout(function() { externaleditorcontainer.onContentChanged(); externaleditorpanel.updateToolbar(); }, 0);
            
            return true;
        } catch (e) {}
    }
}

/* GLOBAL HOLDERS FOR EDITORS! */
var externaleditorcontainer = undefined;
var externaleditorpanel = undefined;


