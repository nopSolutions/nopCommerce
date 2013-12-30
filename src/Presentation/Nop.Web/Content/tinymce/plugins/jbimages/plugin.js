/**
 * Justboil.me - a TinyMCE image upload plugin
 * jbimages/plugin.js
 *
 * Released under Creative Commons Attribution 3.0 Unported License
 *
 * License: http://creativecommons.org/licenses/by/3.0/
 * Plugin info: http://justboil.me/
 * Author: Viktor Kuzhelnyi
 *
 * Version: 2.3 released 23/06/2013
 */

tinymce.PluginManager.add('jbimages', function(editor, url) {
	
	function jbBox() {
		editor.windowManager.open({
			title: 'Upload an image',
			file : url + '/dialog-v4.htm',
			width : 350,
			height: 135,
			buttons: [{
				text: 'Upload',
				classes:'widget btn primary first abs-layout-item',
				disabled : true,
				onclick: 'close'
			},
			{
				text: 'Close',
				onclick: 'close'
			}]
		});
	}
	
	// Add a button that opens a window
	editor.addButton('jbimages', {
		tooltip: 'Upload an image',
		icon : 'image',
		text: 'Upload',
		onclick: jbBox
	});

	// Adds a menu item to the tools menu
	editor.addMenuItem('jbimages', {
		text: 'Upload image',
		icon : 'image',
		context: 'insert',
		onclick: jbBox
	});
});