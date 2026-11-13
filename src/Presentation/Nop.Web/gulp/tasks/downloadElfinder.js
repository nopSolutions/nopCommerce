'use strict';

import fs from 'fs';
import path from 'path';
import AdmZip from 'adm-zip';

export default async function downloadElfinder() {
  const version = '2.1.70';
  const url = `https://github.com/Studio-42/elFinder/archive/refs/tags/${version}.zip`;
  const destDir = path.resolve('node_modules/elfinder-npm');
  const tempZip = path.resolve(`elFinder-${version}.zip`);
  const pathBuilder = path.join(destDir, `.version-${version}`);

  // Check if the version is already installed
  if (fs.existsSync(pathBuilder)) {
    console.log(`elFinder ${version} already installed in node_modules.`);
    return;
  }

  console.log(`Download elFinder ${version} from GitHub...`);
  const response = await fetch(url);
  if (!response.ok) {
    throw new Error(`Failed to download elFinder: ${response.status} ${response.statusText}`);
  }

  // Save the archive to a temporary file
  const buffer = Buffer.from(await response.arrayBuffer());
  fs.writeFileSync(tempZip, buffer);

  console.log('Unzipping archive...');
  const zip = new AdmZip(tempZip);

  // In the release archive, all files are located inside the root folder elFinder-x.x.xx
  // We extract their content directly to node_modules/elfinder-npm
  zip.getEntries().forEach((entry) => {
    const relativePath = entry.entryName.replace(/^elFinder-[^/]+\//, '');
    if (!relativePath) return;

    const fullPath = path.join(destDir, relativePath);
    if (entry.isDirectory) {
      fs.mkdirSync(fullPath, { recursive: true });
    } else {
      fs.mkdirSync(path.dirname(fullPath), { recursive: true });
      fs.writeFileSync(fullPath, entry.getData());
    }
  });

  // Remove the temporary archive and set the version marker
  fs.unlinkSync(tempZip);
  fs.writeFileSync(pathBuilder, version);

  console.log(`elFinder ${version} successfully updated!`);
}
