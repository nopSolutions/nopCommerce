'use strict';

const index = require('./index-d64a26b7.js');
const patch = require('./patch-60245faa.js');

patch.patchBrowser().then(options => {
  return index.bootstrapLazy([["ion-icon.cjs",[[1,"ion-icon",{"mode":[1025],"color":[1],"ariaLabel":[1537,"aria-label"],"ariaHidden":[513,"aria-hidden"],"ios":[1],"md":[1],"flipRtl":[4,"flip-rtl"],"name":[1],"src":[1],"icon":[8],"size":[1],"lazy":[4],"sanitize":[4],"svgContent":[32],"isVisible":[32]}]]]], options);
});
