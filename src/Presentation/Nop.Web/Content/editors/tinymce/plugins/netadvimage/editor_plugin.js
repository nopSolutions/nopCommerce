/**
* editor_plugin_src.js
*
* Copyright 2009, Moxiecode Systems AB
* Released under LGPL License.
*
* License: http://tinymce.moxiecode.com/license
* Contributing: http://tinymce.moxiecode.com/contributing
*/

(function () {
    tinymce.create('tinymce.plugins.NetAdvImagePlugin', {
        init: function (ed, url) {
            // Register commands
            ed.addCommand('mceNetAdvImage', function () {

                // Internal image object like a flash placeholder
                if (ed.dom.getAttrib(ed.selection.getNode(), 'class').indexOf('mceItem') != -1)
                    return;

                ed.windowManager.open({
                    file: url + '/index',
                    width: 700 + parseInt(ed.getLang('netadvimage.delta_width', 0)),
                    height: 500 + parseInt(ed.getLang('netadvimage.delta_height', 0)),
                    inline: 1,
                    translate_i18n: false // Need for Telerik components to work
                }, {
                    plugin_url: url
                });
            });

            // Register buttons
            ed.addButton('netadvimage', {
                title: 'Insert/Edit Image',
                cmd: 'mceNetAdvImage',
                image: url + '/img/tinymce-insertimage.png'
            });
        },

        getInfo: function () {
            return {
                longname: '.NET Advanced Image',
                author: 'LayerWorks LLC (base plugin by Moxiecode Systems AB)',
                authorurl: 'http://layerworks.com',
                infourl: 'http://layerworks.com',
                version: tinymce.majorVersion + "." + tinymce.minorVersion
            };
        }
    });

    // Register plugin
    tinymce.PluginManager.add('netadvimage', tinymce.plugins.NetAdvImagePlugin);

})();