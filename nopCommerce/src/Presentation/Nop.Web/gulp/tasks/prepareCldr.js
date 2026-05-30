import { deleteAsync } from 'del';
import gulp from 'gulp';
import zip from 'gulp-zip';

//Clear before ZIP
function pre_del_cldr(cb) {
  return deleteAsync(['./wwwroot/lib_npm/cldr-data/main/**/*.json',
    '!./wwwroot/lib_npm/cldr-data/main/**/{ca-gregorian,currencies,numbers,timeZoneNames}.json',
    '!./wwwroot/lib_npm/cldr-data/main/*.zip']).then(() => {
      cb()
    })
}

function makeMainZip(cb) {
  deleteAsync(['./wwwroot/lib_npm/cldr-data/**/*',
    '!./wwwroot/lib_npm/cldr-data/main',
    '!./wwwroot/lib_npm/cldr-data/main/**/{ca-gregorian,currencies,numbers,timeZoneNames}.json',
    '!./wwwroot/lib_npm/cldr-data/supplemental',
    '!./wwwroot/lib_npm/cldr-data/main/*.zip']).then(() => {
      gulp.src(['./wwwroot/lib_npm/cldr-data/main/**'])
      .pipe(zip('main.zip'))
      .pipe(gulp.dest('./wwwroot/lib_npm/cldr-data/main/')).on('end', cb)
  }).catch(cb)
}

function del_cldr(cb) {
  return deleteAsync(['./wwwroot/lib_npm/cldr-data/main/**',
    '!./wwwroot/lib_npm/cldr-data/main/en',
    '!./wwwroot/lib_npm/cldr-data/main/*.zip']).then(() => {
      cb()
    })
}

export const prepareCldr = gulp.series(pre_del_cldr, makeMainZip, del_cldr);
