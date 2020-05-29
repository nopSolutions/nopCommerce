'use strict'

//plugins
const gulp = require('gulp');

//const
const prepareCldr = require('./gulp/tasks/prepareCldr');
const copyDependencies = require('./gulp/tasks/copyDependencies');
const clean = require('./gulp/tasks/clean');
const clearLibraries = require('./gulp/tasks/clearLibraries');

//init task
exports.Step_1_install_dependencies = gulp.series(
  clean.Execute,
  copyDependencies.Execute
);

exports.Step_2_prepareCldr = prepareCldr.Execute;
exports.Step_3_clearLibraries = clearLibraries.Execute;
