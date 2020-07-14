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

  //jquery.validate.globalize
  gulp.src(nodeModules + "jquery-validation-globalize/jquery.validate.globalize.min.js.map").pipe(gulp.dest(targetPath + "/jquery-validation-globalize"));

  //bootstrap
  gulp.src(nodeModules + "bootstrap/dist/css/bootstrap.min.css.map").pipe(gulp.dest(targetPath + "/bootstrap/css"));

  //jsrender
  gulp.src(nodeModules + "jsrender/jsrender.min.js.map").pipe(gulp.dest(targetPath + "/jsrender"));
  cb();
}

exports.jquery_DevTools = jquery_DevTools;

exports.Execute = gulp.parallel(copyDependencies, jquery_DevTools);