const del = require('del');
const targetPath = './wwwroot/lib_npm/';

function clean(cb) {
  return del(targetPath + '**/*').then(() => {
    cb()
  })
};

exports.Execute = clean;