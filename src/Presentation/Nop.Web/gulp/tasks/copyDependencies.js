import gulp from 'gulp';
import filter from 'gulp-filter';
import { readFileSync } from 'fs';
import merge from 'merge2';
import rename from 'gulp-rename';

const pkg = JSON.parse(readFileSync(new URL('../../package.json', import.meta.url), 'utf-8')); 
const nodeModules = './node_modules/';
const targetPath = './wwwroot/lib_npm/';

export default function copyDependencies()
{
  return merge(
    [
    //common dependencies
    gulp
      .src(`${nodeModules}/**`)
      .pipe(filter(Object.keys(pkg.dependencies).map(module => `${nodeModules}${module}/dist/**/*.min*`)))
      .pipe(rename(function (path) {
        path.dirname = path.dirname.replace(/\/dist/, '').replace(/\\dist/, '');
      }))
      .pipe(gulp.dest(targetPath)),

    //Font Awesome
    gulp
      .src(nodeModules + '@fortawesome/fontawesome-free/**')
      .pipe(filter(['**/css/*.min*', '**/webfonts/*', 'attribution.js']))
      .pipe(gulp.dest(targetPath + '/@fortawesome/fontawesome-free')),

    //datatables.net
    gulp
      .src(nodeModules + '{datatables.net,datatables.net-bs4,datatables.net-buttons,datatables.net-buttons-bs4}/**')
      .pipe(filter(['**/{css,js}/*.min*', '**/swf/*']))
      .pipe(gulp.dest(targetPath)),

    //CLDR (unicode.org)
    gulp
      .src(nodeModules + 'cldrjs/dist/**')
      .pipe(filter(['**/*.js', '!**/.build']))
      .pipe(gulp.dest(targetPath + '/cldrjs')),
      
    gulp
      .src(nodeModules + 'cldr-data/{main,segments,supplemental}/**')
      .pipe(gulp.dest(targetPath + '/cldr-data')),

    //Moment.js  
    gulp
      .src(`${nodeModules}moment/min/moment-with-locales.min.js*`)
      .pipe(gulp.dest(targetPath + '/moment/min')),
    gulp
      .src(`${nodeModules}moment/dist/**`)
      .pipe(gulp.dest(targetPath + '/moment')),

    //Ionicons
    gulp
      .src(`${nodeModules}ionicons/{css,fonts,png}/**`)
      .pipe(gulp.dest(targetPath + '/ionicons')),

    //Tiny MCE
    gulp
      .src(`${nodeModules}tinymce/**/*.min.*`)
      .pipe(gulp.dest(targetPath + '/tinymce')),

    gulp
      .src(nodeModules + 'tinymce-langs/langs/*')
      .pipe(gulp.dest(targetPath + '/tinymce/langs')),

    //OverlayScrollbars
    gulp
      .src(`${nodeModules}overlayscrollbars/**/*.min.*`)
      .pipe(gulp.dest(`${targetPath}overlayscrollbars`)),

    //Swiper
    gulp
      .src(nodeModules + 'swiper/swiper-bundle.min.{css,js,js.map}')
      .pipe(gulp.dest(targetPath + '/swiper')),

    //Shepherd.js
    gulp
      .src(nodeModules + 'shepherd.js/dist/**/shepherd.{css,min.js}')
      .pipe(gulp.dest(targetPath + '/shepherd.js')),

    //JsRender
    gulp
      .src(`${nodeModules}jsrender/*`)
      .pipe(filter(['*.min.*']))
      .pipe(gulp.dest(`${targetPath}jsrender`)),

    //jquery-ui-dist
    gulp
      .src(`${nodeModules}jquery-ui-dist/**`)
      .pipe(filter(['*.min.{css,js}', '**/images/*']))
      .pipe(gulp.dest(`${targetPath}jquery-ui-dist`)),

    //jquery-ui-touch-punch
    gulp
      .src(`${nodeModules}jquery-ui-touch-punch/**`)
      .pipe(filter(['*.min.*']))
      .pipe(gulp.dest(`${targetPath}jquery-ui-touch-punch`)),

    //Globalize
    gulp
      .src(`${nodeModules}globalize/dist/**`)
      .pipe(gulp.dest(`${targetPath}globalize`)),

    //jQuery-tagEditor
    gulp
      .src(`${nodeModules}jquery-tageditor/*.{min.js,css}`)
      .pipe(gulp.dest(`${targetPath}jquery-tageditor`)),

    //Farbtastic
    gulp
      .src(`${nodeModules}farbstastic/*.{min.js,css,png}`)
      .pipe(gulp.dest(`${targetPath}farbstastic`)),

    //Magnific Popup
    gulp
      .src(nodeModules + 'magnific-popup/dist/**/*.{css,min.js}')
      .pipe(gulp.dest(`${targetPath}magnific-popup`)),

    //Admin LTE plugins: select2
    gulp
    .src(nodeModules + 'admin-lte/plugins/select2/**/*.{css,min.js}')
    .pipe(gulp.dest(`${targetPath}admin-lte/plugins/select2`)),

    //Chart.js
    gulp
      .src(nodeModules + 'chart.js/dist/chart.umd.{js,js.map}')
      .pipe(rename({
        suffix: '.min'
      }))
        .pipe(gulp.dest(`${targetPath}chart.js`)),

    //jQuery Migrate
    gulp
      .src(nodeModules + 'jquery-migrate/dist/*.{js,js.map}')
      .pipe(gulp.dest(`${targetPath}jquery-migrate`)),

    ]);
}