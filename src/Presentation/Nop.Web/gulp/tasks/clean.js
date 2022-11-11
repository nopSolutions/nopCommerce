import { deleteAsync } from 'del';
const targetPath = './wwwroot/lib_npm/';

export default function clean(cb) {
  return deleteAsync(targetPath + '**/*').then(() => {
    cb()
  })
};