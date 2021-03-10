const del = require('del');
const gulp = require('gulp');
const zip = require('gulp-zip');

//Clear before ZIP
function pre_del_cldr(cb) {
  return del(['./wwwroot/lib_npm/cldr-data/main/**/*.json',
    '!./wwwroot/lib_npm/cldr-data/main/**/{ca-gregorian,currencies,numbers,timeZoneNames}.json',
    '!./wwwroot/lib_npm/cldr-data/main/*.zip']).then(() => {
      cb()
    })
}

function makeMainZip(cb) {
  del(['./wwwroot/lib_npm/cldr-data/**/*',
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
  return del(['./wwwroot/lib_npm/cldr-data/main/**',
    '!./wwwroot/lib_npm/cldr-data/main/en',
    '!./wwwroot/lib_npm/cldr-data/main/*.zip']).then(() => {
      cb()
    })
}

exports.Execute = gulp.series(pre_del_cldr, makeMainZip, del_cldr);
