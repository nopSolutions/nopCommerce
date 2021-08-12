const gulp = require('gulp');
const npmDist = require('gulp-npm-dist');
const rename = require('gulp-rename');

const nodeModules = './node_modules/';
const targetPath = './wwwroot/lib_npm/';

function copyDependencies(cb) {
  gulp.src(npmDist({
    excludes: ['nuget/**/*']
  }), { base: nodeModules })
    .pipe(rename(function (path) {
      path.dirname = path.dirname.replace(/\/dist/, '').replace(/\\dist/, '');
    }))
    .pipe(gulp.dest(targetPath))
  cb();
};

function jquery_DevTools(cb){
  //jquery-migrate
  gulp.src(nodeModules + "jquery-migrate/dist/jquery-migrate.js").pipe(gulp.dest(targetPath + "/jquery-migrate"));
  gulp.src(nodeModules + "jquery-migrate/dist/jquery-migrate.min.map").pipe(gulp.dest(targetPath + "/jquery-migrate"));

  //bootstrap
  gulp.src(nodeModules + "bootstrap/dist/css/bootstrap.min.css.map").pipe(gulp.dest(targetPath + "/bootstrap/css"));

  //jsrender
  gulp.src(nodeModules + "jsrender/jsrender.min.js.map").pipe(gulp.dest(targetPath + "/jsrender"));
  cb();
}

function copyMomentJS(cb) {
  //for ver. ^2.29.1
  gulp.src(nodeModules + "moment/min/moment-with-locales.min.js").pipe(gulp.dest(targetPath + "/moment/min"));
  cb();
}

function copyTinyMceLangs(cb) {
  gulp.src(nodeModules + "tinymce-langs/langs/*").pipe(gulp.dest(targetPath + "/tinymce/langs"));
  cb();
}

exports.jquery_DevTools = jquery_DevTools;

exports.Execute = gulp.parallel(copyDependencies, jquery_DevTools, copyMomentJS, copyTinyMceLangs);