const del = require('del');
const gulp = require('gulp');

const targetPath = './wwwroot/lib_npm/';

function clear_folders(cb) {
  return del(targetPath + '**/{docs,nuget,test,less,scss,src,typescript,node_modules}').then(() => {
    cb()
  })
};

function clear_files(cb) {
  return del(targetPath + '**/*.{html,txt}').then(() => {
      cb()
    })
};

function clear_fontawesome(cb) {
  return del(targetPath + '@fortawesome/fontawesome-free/{js,sprites,svgs}').then(() => {
    cb()
  })
};

function clear_tinyMceLangs(cb) {
  return del(targetPath + 'tinymce-langs').then(() => {
    cb()
  })
};

exports.Execute = gulp.series(
  clear_folders,
  clear_files,
  clear_fontawesome,
  clear_tinyMceLangs
  );
