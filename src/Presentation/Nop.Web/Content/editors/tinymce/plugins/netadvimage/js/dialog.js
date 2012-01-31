

function OnDataBinding(e) {
    UpdateTree();
}

function OnExpand(e) {
    $.post("_ExpandDirectoryState", { value: GetNodeValue(e.item) });
}

function OnCollapse(e) {
    $.post("_CollapseDirectoryState", { value: GetNodeValue(e.item) });
}

function OnNodeSelect(e) {
    GetImages(e.item);
}

function OnNodeDropped(e) {
    if ($(e.destinationItem).size() > 0 && e.dropPosition === 'over') {
        MoveDirectory(GetNodeValue(e.item), GetNodeValue(e.destinationItem));
        UpdateTree();
    }
    e.preventDefault();
}

function OnNodeDrop(e) {

    if ($('.t-drag-status').hasClass('t-add')) { // NOTE: Asked Telerik for e.isValid

        var dropContainer = $(e.dropTarget).closest('.t-drop');
        if (dropContainer.size() > 0) {
            // Node dropped in target area

            // Delete
            if (dropContainer.attr('id') === 'dir-delete') {
                tinyMCEPopup.confirm('Are you sure you want to delete this directory and all of it\'s contents? This action cannot be undone.', function (s) {
                    if (s)
                        RemoveNode($(e.item));
                    else
                        e.preventDefault();
                });
            }

            // Edit
            else if (dropContainer.attr('id') === 'dir-edit') {
                EditNode(e.item);
            }

            //...
        }
        else {
            // Node dropped on another node
            var dropNode = $(e.dropTarget).closest('.t-item');
            if (dropNode.size() > 0) {
                tinyMCEPopup.confirm('Are you sure you want to move this directory? Any existing links to the images below this directory may break.', function (s) {
                    if (!s)
                        e.preventDefault();
                });
            }
        }
    }
}

function GetNodeValue(node) {
    // TODO: See if we can't get the value from the treeview object
    return $(node).closest('.t-item').find(':input[name*="Value"]').val()
}

function SetNodeValue(node, value) {
    $(node).closest('.t-item').find(':input[name*="Value"]').val(value);
}

function GetImages(node) {
    if (GetNodeValue(node)) {

        $('#prompt').fadeOut(100, 0, function () {
            $(this).hide();
        });

        $.post("_GetImages", { path: GetNodeValue(node) }, function (result) {

            $('#img-list').empty();

            if ($(result).size() > 0) {

                HideOverlays();

                $('#directory img').attr('src', nop_store_directory_root + 'Content/editors/tinymce/plugins/netadvimage/img/folder-horizontal.gif');
                $(node).closest('.t-item').find('img:first').attr('src', nop_store_directory_root + 'Content/editors/tinymce/plugins/netadvimage/img/folder-horizontal-open.gif');

                $(result).each(function (index, image) {
                    $('#img-list').append('<li><a href="#" title="' + image.FileName + '"><span><img src="' + image.Path + '" alt="" /></span>' + image.FileName + '</a><input type="hidden" value="' + image.FileName + '" /></li>');
                });
            }
            else
                ShowOverlays();
        });
    }
}

function RemoveNode(node) {
    // Delete directory
    $.post("_DeleteDirectory", { path: GetNodeValue(node) }, function (result) {
        if (result)
            alert(result);
        else
            UpdateTree();
    });
}

function UpdateTree() {
    var treeview = $('#directory').data('tTreeView');
    $.post("_GetDirectories", function (data) {
        treeview.bindTo(data);
    });
}

function MoveDirectory(path, destinationPath) {
    $.post("_MoveDirectory", { path: path, destinationPath: destinationPath }, function (result) {
        if (result)
            alert(result);
    });
}

function HideOverlays() {
    //modified by nopCommerce team (IE8 fix)
    //$('#mask, #img-upload').fadeTo(100, 0, function () {
    //    $(this).hide();
    //});
    $('#img-upload').hide();
    $('#mask').fadeTo(100, 0, function () {
        $(this).hide();
    });
}

function ShowOverlays() {
    //modified by nopCommerce team (IE8 fix)
    //$('#img-upload').fadeTo(100, 1);
    //$('#mask').fadeTo(100, .9);
    //$('.qq-upload-list').empty();
    $('#img-upload').show();
    $('#mask').fadeTo(100, .9);
    $('.qq-upload-list').empty();
}

function EditNode(node) {

    var value = GetNodeValue(node);
    var $item = $(node).find('.t-in:first');

    // Add selected class
    if (!$item.hasClass('t-state-selected'))
        $item.addClass('t-state-selected');

    $item.addClass('t-state-edit').html('<input class="t-edit" type="text" value="' + value.substr(value.lastIndexOf('\\') + 1) + '" />');
    $item.find('.t-edit').select();
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

var ImageDialog = {

    preInit: function () {
        var url;
        tinyMCEPopup.requireLangPack();
    },

    init: function (ed) {

        var f = document.forms['mce'],
                nl = f.elements,
                ed = tinyMCEPopup.editor,
                dom = ed.dom,
                n = ed.selection.getNode(),
                uploader = new qq.FileUploader({
                    element: document.getElementById('upload'),
                    action: nop_store_directory_root + 'Content/editors/tinymce/plugins/netadvimage/index',
                    params: {
                        path: null
                    },
                    onComplete: function (id, fileName, responseJSON) {
                        //HideOverlays();
                        GetImages($('.t-state-selected').first());
                    }
                });

        ///////////////////

        // Directory options
        $('#dir-delete').click(function (e) {
            if ($('.t-state-selected').size() > 0) {
                tinyMCEPopup.confirm('Are you sure you want to delete this directory and all of it\'s contents? This action cannot be undone.', function (s) {
                    if (s)
                        RemoveNode($('.t-state-selected').first());
                });
            }
            else
                alert('You must select a directory to delete.');
            e.preventDefault();
        });

        // Add directory
        $('#dir-add').click(function (e) {
            if ($('.t-state-selected').size() > 0) {
                // Add directory
                $.post("_AddDirectory", { path: GetNodeValue($('.t-state-selected')) }, function (result) {
                    if (result)
                        alert(result);
                    else
                        UpdateTree();
                });
            }
            else {
                var cont = confirm('No folder was selected. Do you want to add the new folder to the root?');
                if (!cont)
                    return false;
                else {
                    // Add directory to root
                    $.post("_AddDirectory", { path: '' }, function (result) {
                        if (result)
                            alert(result);
                        else
                            UpdateTree();
                    });
                }
            }
            e.preventDefault();
        });

        // Edit directory
        $('#dir-edit').click(function (e) {
            if ($('.t-state-selected').size() > 0) {
                EditNode($('.t-state-selected').closest('.t-item'));
            }
            else
                alert('You must select a directory to edit.');
            e.preventDefault();
        });
        $('input.t-edit').live('blur', function (e) {

            var newText = $(this).val();
            var currentPath = GetNodeValue($(this));

            $.post("_RenameDirectory", { path: currentPath, name: newText }, function (result) {
                if (result) {
                    alert(result);
                    UpdateTree();
                }
                else
                    UpdateTree();
            });
        });

        // Upload image
        $('#upload').live('click', function (e) {
            if ($('.t-state-selected').size() > 0) {
                uploader.setParams({
                    path: GetNodeValue($('.t-state-selected'))
                });
            }
            else {
                alert('You must select an upload directory.');
                e.preventDefault();
            }
        });

        // External link
        //$('.img-ext').live('click', function (e) {
        //    mcTabs.displayTab('general_tab', 'general_panel');
        //    $('#src').focus().select();
        //});

        // Upload image
        $('.img-upload').click(function (e) {
            ShowOverlays();
            e.preventDefault();
        });

        // Delete image
        $('#img-delete').click(function (e) {

            if ($('.t-state-selected').size() == 0) {
                alert('You must select a directory.');
                e.preventDefault();
                return;
            }

            if ($('#img-list .selected').size() == 0) {
                alert('You must select an image to delete.');
                e.preventDefault();
                return;
            }

            var answer = confirm('Are you sure you want to delete this image? This action cannot be undone.');
            if (answer) {
                $.post("_DeleteImage", { path: GetNodeValue($('.t-state-selected').first()), name: $('#img-list .selected:first').parent().find('input').val() }, function (result) {
                    if (result)
                        alert(result);
                    else
                        $('#img-list .selected:first').fadeOut(100, function () {
                            GetImages($('.t-state-selected').first());
                        });
                });
            }

            e.preventDefault();

        });

        // Overlay clicks
        $('#mask').live('click', function () {
            if ($('#img-list li').size() > 0)
                HideOverlays();
        });

        // Splitter setup
        splitter = $('#upload_panel').layout({
            west__minSize: parseInt(ed.getParam("netadvimage_west_pane_minsize", 100)),
            west__size: parseInt(ed.getParam("netadvimage_west_pane_size", 200))
        });

        // Image select
        $('#img-list a').live('click', function (e) {
            if (!$(this).hasClass('selected')) {
                var src = $(this).find('img').attr('src');
                $('#img-list a').removeClass('selected');
                $(this).addClass('selected');
                $('#src').val(src);
                ImageDialog.showPreviewImage(src);
            }
            e.preventDefault();
        });

        // Image selection
        $('#img-list a').live('dblclick', function (e) {
            var src = $(this).find('img').attr('src');
            mcTabs.displayTab('general_tab', 'general_panel');
            $('#src').val(src);
            ImageDialog.showPreviewImage(src);
            e.preventDefault();
        });

        // Directory selection
        $('#directory a').click(function (e) {

            if (!$(this).hasClass('selected')) {

                $('.directory-list a').removeClass('selected');
                $(this).addClass('selected');

                GetImagesByPath($(this).attr('href'));
            }
            e.preventDefault();
        });

        ///////////////////

        tinyMCEPopup.resizeToInnerSize();
        this.fillClassList('class_list');
        TinyMCE_EditableSelects.init();

        if (n.nodeName == 'IMG') {

            nl.src.value = dom.getAttrib(n, 'src');
            nl.width.value = dom.getAttrib(n, 'width');
            nl.height.value = dom.getAttrib(n, 'height');
            nl.alt.value = dom.getAttrib(n, 'alt');
            nl.title.value = dom.getAttrib(n, 'title');
            nl.vspace.value = this.getAttrib(n, 'vspace');
            nl.hspace.value = this.getAttrib(n, 'hspace');
            nl.border.value = this.getAttrib(n, 'border');
            selectByValue(f, 'align', this.getAttrib(n, 'align'));
            selectByValue(f, 'class_list', dom.getAttrib(n, 'class'), true, true);
            nl.style.value = dom.getAttrib(n, 'style');
            nl.id.value = dom.getAttrib(n, 'id');
            nl.dir.value = dom.getAttrib(n, 'dir');
            nl.lang.value = dom.getAttrib(n, 'lang');
            nl.usemap.value = dom.getAttrib(n, 'usemap');
            nl.longdesc.value = dom.getAttrib(n, 'longdesc');
            nl.insert.value = ed.getLang('update');

            if (ed.settings.inline_styles) {
                // Move attribs to styles
                if (dom.getAttrib(n, 'align'))
                    this.updateStyle('align');

                if (dom.getAttrib(n, 'hspace'))
                    this.updateStyle('hspace');

                if (dom.getAttrib(n, 'border'))
                    this.updateStyle('border');

                if (dom.getAttrib(n, 'vspace'))
                    this.updateStyle('vspace');
            }

            mcTabs.displayTab('general_tab', 'general_panel');
        }


        // If option enabled default contrain proportions to checked
        if (ed.getParam("netadvimage_constrain_proportions", true))
            f.constrain.checked = true;

        this.changeAppearance();
        this.showPreviewImage(nl.src.value, 1);

    },

    insert: function (file, title) {

        var ed = tinyMCEPopup.editor, t = this, f = document.forms['mce'];

        if (f.src.value === '') {
            if (ed.selection.getNode().nodeName == 'IMG') {
                ed.dom.remove(ed.selection.getNode());
                ed.execCommand('mceRepaint');
            }

            tinyMCEPopup.close();
            return;
        }

        if (tinyMCEPopup.getParam("accessibility_warnings", 1)) {
            if (!f.alt.value) {
                tinyMCEPopup.confirm('Are you sure you want to continue without including an Image Description? Without it the image may not be accessible to some users with disabilities, or to those using a text browser, or browsing the Web with images turned off.', function (s) {
                    if (s)
                        t.insertAndClose();
                });
                return;
            }
        }

        t.insertAndClose();
    },

    insertAndClose: function () {

        var ed = tinyMCEPopup.editor, f = document.forms['mce'], nl = f.elements, v, args = {}, el;

        tinyMCEPopup.restoreSelection();

        // Fixes crash in Safari
        if (tinymce.isWebKit)
            ed.getWin().focus();

        if (!ed.settings.inline_styles) {
            args = {
                vspace: nl.vspace.value,
                hspace: nl.hspace.value,
                border: nl.border.value,
                align: getSelectValue(f, 'align')
            };
        } else {
            // Remove deprecated values
            args = {
                vspace: '',
                hspace: '',
                border: '',
                align: ''
            };
        }

        tinymce.extend(args, {
            src: nl.src.value,
            width: nl.width.value,
            height: nl.height.value,
            alt: nl.alt.value,
            title: nl.title.value,
            'class': getSelectValue(f, 'class_list'),
            style: nl.style.value,
            id: nl.id.value,
            dir: nl.dir.value,
            lang: nl.lang.value,
            usemap: nl.usemap.value,
            longdesc: nl.longdesc.value
        });

        el = ed.selection.getNode();

        if (el && el.nodeName == 'IMG') {
            ed.dom.setAttribs(el, args);
        } else {
            ed.execCommand('mceInsertContent', false, '<img id="__mce_tmp" />', { skip_undo: 1 });
            ed.dom.setAttribs('__mce_tmp', args);
            ed.dom.setAttrib('__mce_tmp', 'id', '');
            ed.undoManager.add();
        }

        tinyMCEPopup.close();
    },

    getAttrib: function (e, at) {
        var ed = tinyMCEPopup.editor, dom = ed.dom, v, v2;

        if (ed.settings.inline_styles) {
            switch (at) {
                case 'align':
                    if (v = dom.getStyle(e, 'float'))
                        return v;

                    if (v = dom.getStyle(e, 'vertical-align'))
                        return v;

                    break;

                case 'hspace':
                    v = dom.getStyle(e, 'margin-left')
                    v2 = dom.getStyle(e, 'margin-right');

                    if (v && v == v2)
                        return parseInt(v.replace(/[^0-9]/g, ''));

                    break;

                case 'vspace':
                    v = dom.getStyle(e, 'margin-top')
                    v2 = dom.getStyle(e, 'margin-bottom');
                    if (v && v == v2)
                        return parseInt(v.replace(/[^0-9]/g, ''));

                    break;

                case 'border':
                    v = 0;

                    tinymce.each(['top', 'right', 'bottom', 'left'], function (sv) {
                        sv = dom.getStyle(e, 'border-' + sv + '-width');

                        // False or not the same as prev
                        if (!sv || (sv != v && v !== 0)) {
                            v = 0;
                            return false;
                        }

                        if (sv)
                            v = sv;
                    });

                    if (v)
                        return parseInt(v.replace(/[^0-9]/g, ''));

                    break;
            }
        }

        if (v = dom.getAttrib(e, at))
            return v;

        return '';
    },

    fillClassList: function (id) {

        var dom = tinyMCEPopup.dom, lst = dom.get(id), v, cl;

        if (v = tinyMCEPopup.getParam('theme_advanced_styles')) {
            cl = [];

            tinymce.each(v.split(';'), function (v) {
                var p = v.split('=');

                cl.push({ 'title': p[0], 'class': p[1] });
            });
        } else
            cl = tinyMCEPopup.editor.dom.getClasses();

        if (cl.length > 0) {
            lst.options.length = 0;
            lst.options[lst.options.length] = new Option('-- not set --', '');

            tinymce.each(cl, function (o) {
                lst.options[lst.options.length] = new Option(o.title || o['class'], o['class']);
            });
        } else
            dom.remove(dom.getParent(id, 'tr'));
    },

    resetImageData: function () {

        var f = document.forms['mce'];
        f.elements.width.value = f.elements.height.value = '';

    },

    updateImageData: function (img, st) {
        var f = document.forms['mce'];

        if (!st) {
            f.elements.width.value = img.width;
            f.elements.height.value = img.height;
        }

        this.preloadImg = img;
    },

    changeAppearance: function () {
        var ed = tinyMCEPopup.editor, f = document.forms['mce'], img = document.getElementById('alignSampleImg');

        if (img) {
            if (ed.getParam('inline_styles')) {
                ed.dom.setAttrib(img, 'style', f.style.value);
            } else {
                img.align = f.align.value;
                img.border = f.border.value;
                img.hspace = f.hspace.value;
                img.vspace = f.vspace.value;
            }
        }
    },

    changeHeight: function () {
        var f = document.forms['mce'], tp, t = this;

        if (!f.constrain.checked || !t.preloadImg) {
            return;
        }

        if (f.width.value == "" || f.height.value == "")
            return;

        tp = (parseInt(f.width.value) / parseInt(t.preloadImg.width)) * t.preloadImg.height;
        f.height.value = tp.toFixed(0);
    },

    changeWidth: function () {
        var f = document.forms['mce'], tp, t = this;

        if (!f.constrain.checked || !t.preloadImg) {
            return;
        }

        if (f.width.value == "" || f.height.value == "")
            return;

        tp = (parseInt(f.height.value) / parseInt(t.preloadImg.height)) * t.preloadImg.width;
        f.width.value = tp.toFixed(0);
    },

    updateStyle: function (ty) {
        var dom = tinyMCEPopup.dom, st, v, f = document.forms['mce'], img = dom.create('img', { style: dom.get('style').value });

        if (tinyMCEPopup.editor.settings.inline_styles) {
            // Handle align
            if (ty == 'align') {
                dom.setStyle(img, 'float', '');
                dom.setStyle(img, 'vertical-align', '');

                v = getSelectValue(f, 'align');
                if (v) {
                    if (v == 'left' || v == 'right')
                        dom.setStyle(img, 'float', v);
                    else
                        img.style.verticalAlign = v;
                }
            }

            // Handle border
            if (ty == 'border') {
                dom.setStyle(img, 'border', '');

                v = f.border.value;
                if (v || v == '0') {
                    if (v == '0')
                        img.style.border = '0';
                    else
                        img.style.border = v + 'px solid black';
                }
            }

            // Handle hspace
            if (ty == 'hspace') {
                dom.setStyle(img, 'marginLeft', '');
                dom.setStyle(img, 'marginRight', '');

                v = f.hspace.value;
                if (v) {
                    img.style.marginLeft = v + 'px';
                    img.style.marginRight = v + 'px';
                }
            }

            // Handle vspace
            if (ty == 'vspace') {
                dom.setStyle(img, 'marginTop', '');
                dom.setStyle(img, 'marginBottom', '');

                v = f.vspace.value;
                if (v) {
                    img.style.marginTop = v + 'px';
                    img.style.marginBottom = v + 'px';
                }
            }

            // Merge
            dom.get('style').value = dom.serializeStyle(dom.parseStyle(img.style.cssText), 'img');
        }
    },

    showPreviewImage: function (u, st) {
        if (!u) {
            tinyMCEPopup.dom.setHTML('prev', '');
            return;
        }

        if (!st && tinyMCEPopup.getParam("netadvimage_update_dimensions_onchange", true))
            this.resetImageData();

        u = tinyMCEPopup.editor.documentBaseURI.toAbsolute(u);

        if (!st)
            tinyMCEPopup.dom.setHTML('prev', '<img id="previewImg" src="' + u + '" border="0" onload="ImageDialog.updateImageData(this);" onerror="ImageDialog.resetImageData();" />');
        else
            tinyMCEPopup.dom.setHTML('prev', '<img id="previewImg" src="' + u + '" border="0" onload="ImageDialog.updateImageData(this, 1);" />');
    }
};

ImageDialog.preInit();
tinyMCEPopup.onInit.add(ImageDialog.init, ImageDialog);
