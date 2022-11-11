'use strict'

import gulp from 'gulp';
import { prepareCldr } from './gulp/tasks/prepareCldr.js';
import copyDependencies from './gulp/tasks/copyDependencies.js';
import clean from './gulp/tasks/clean.js';

gulp.task('default', 
  gulp.series(clean, copyDependencies, prepareCldr)
);