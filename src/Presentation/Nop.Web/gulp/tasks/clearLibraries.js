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

exports.Execute = gulp.series(
  clear_folders,
  clear_files
  );
