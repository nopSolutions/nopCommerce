/**
 * Copyright (c) Tiny Technologies, Inc. All rights reserved.
 * Licensed under the LGPL or a commercial license.
 * For LGPL see License.txt in the project root for license information.
 * For commercial licenses see https://www.tiny.cloud/
 *
 * Version: 5.4.2 (2020-08-17)
 */
!function(){"use strict";var n=tinymce.util.Tools.resolve("tinymce.PluginManager");!function o(){n.add("hr",function(n){var o,t;(o=n).addCommand("InsertHorizontalRule",function(){o.execCommand("mceInsertContent",!1,"<hr />")}),(t=n).ui.registry.addButton("hr",{icon:"horizontal-rule",tooltip:"Horizontal line",onAction:function(){return t.execCommand("InsertHorizontalRule")}}),t.ui.registry.addMenuItem("hr",{icon:"horizontal-rule",text:"Horizontal line",onAction:function(){return t.execCommand("InsertHorizontalRule")}})})}()}();