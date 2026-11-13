'use strict'

import gulp from 'gulp';
import { prepareCldr } from './gulp/tasks/prepareCldr.js';
import copyDependencies from './gulp/tasks/copyDependencies.js';
import clean from './gulp/tasks/clean.js';
import downloadElfinder from './gulp/tasks/downloadElfinder.js';

gulp.task('downloadElfinder', downloadElfinder);

gulp.task('default', 
  gulp.series(clean, copyDependencies, prepareCldr)
);