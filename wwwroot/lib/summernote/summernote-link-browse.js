/* Summernote Link Dialog Button Enhancement
 * Adds a Browse button to the Link insertion dialog */

(function() {
    'use strict';
  function setupLinkDialogPatch() {
    $(document).on('shown.bs.modal', '.note-modal', function () {
      addBrowseButtonToLinkDialog($(this));
    });
  }

  function addBrowseButtonToLinkDialog($modal) {
    // Find URL input - using the correct selector for Summernote
    var $urlInput = $modal.find('input.note-link-url:visible');

    if (!$urlInput.length ||
      $urlInput.siblings('.note-link-browse-btn').length) {
      return;
    }

    // Create Browse button
    var $browseBtn = $('<button/>')
        .addClass('btn btn-primary btn-info note-link-browse-btn')
        .attr('type', 'button')
        .attr('title', 'Browse files')
        .html('<i class="fas fa-folder-open"></i>');

    $browseBtn.on('click', function(e) {
        e.preventDefault();
        e.stopPropagation();

        // Save reference to the input field BEFORE opening dialog
        var $currentUrlInput = $('input.note-link-url');
        var $currentLinkInput = $('input.note-link-text');

        // Pass callback that will be called when file is selected
        openElFinderDialogForLink(function(files) {
            // This callback is executed after file selection
            // Set the URL in the saved input reference
            if ($currentUrlInput && $currentUrlInput.length) {
                $currentUrlInput.val(files.url).trigger('change').focus();
            }

            // Set the link text in the saved input reference
            if ($currentLinkInput && $currentLinkInput.length) {
              if ($currentLinkInput.val() == '') {
                $currentLinkInput.val(files.name).trigger('input').focus();
              } else {
                $currentLinkInput.trigger('input').focus();
              }
            }
        });
    });

    // Insert button after URL input
    $urlInput.after($browseBtn);
    };

  $(setupLinkDialogPatch);
})();
