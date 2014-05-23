/*
  RoxyFileman - web based file manager. Ready to use with CKEditor, TinyMCE. 
  Can be easily integrated with any other WYSIWYG editor or CMS.

  Copyright (C) 2013, RoxyFileman.com - Lyubomir Arsov. All rights reserved.
  For licensing, see LICENSE.txt or http://RoxyFileman.com/license

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.

  Contact: Lyubomir Arsov, liubo (at) web-lobby.com
*/
var fileTypeIcons = new Object();
fileTypeIcons['3gp'] = 'file_extension_3gp.png';
fileTypeIcons['7z'] = 'file_extension_7z.png';
fileTypeIcons['ace'] = 'file_extension_ace.png';
fileTypeIcons['ai'] = 'file_extension_ai.png';
fileTypeIcons['aif'] = 'file_extension_aif.png';
fileTypeIcons['aiff'] = 'file_extension_aiff.png';
fileTypeIcons['amr'] = 'file_extension_amr.png';
fileTypeIcons['asf'] = 'file_extension_asf.png';
fileTypeIcons['asx'] = 'file_extension_asx.png';
fileTypeIcons['bat'] = 'file_extension_bat.png';
fileTypeIcons['bin'] = 'file_extension_bin.png';
fileTypeIcons['bmp'] = 'file_extension_bmp.png';
fileTypeIcons['bup'] = 'file_extension_bup.png';
fileTypeIcons['cab'] = 'file_extension_cab.png';
fileTypeIcons['cbr'] = 'file_extension_cbr.png';
fileTypeIcons['cda'] = 'file_extension_cda.png';
fileTypeIcons['cdl'] = 'file_extension_cdl.png';
fileTypeIcons['cdr'] = 'file_extension_cdr.png';
fileTypeIcons['chm'] = 'file_extension_chm.png';
fileTypeIcons['dat'] = 'file_extension_dat.png';
fileTypeIcons['divx'] = 'file_extension_divx.png';
fileTypeIcons['dll'] = 'file_extension_dll.png';
fileTypeIcons['dmg'] = 'file_extension_dmg.png';
fileTypeIcons['doc'] = 'file_extension_doc.png';
fileTypeIcons['dss'] = 'file_extension_dss.png';
fileTypeIcons['dvf'] = 'file_extension_dvf.png';
fileTypeIcons['dwg'] = 'file_extension_dwg.png';
fileTypeIcons['eml'] = 'file_extension_eml.png';
fileTypeIcons['eps'] = 'file_extension_eps.png';
fileTypeIcons['exe'] = 'file_extension_exe.png';
fileTypeIcons['fla'] = 'file_extension_fla.png';
fileTypeIcons['flv'] = 'file_extension_flv.png';
fileTypeIcons['gif'] = 'file_extension_gif.png';
fileTypeIcons['gz'] = 'file_extension_gz.png';
fileTypeIcons['hqx'] = 'file_extension_hqx.png';
fileTypeIcons['htm'] = 'file_extension_htm.png';
fileTypeIcons['html'] = 'file_extension_html.png';
fileTypeIcons['ifo'] = 'file_extension_ifo.png';
fileTypeIcons['indd'] = 'file_extension_indd.png';
fileTypeIcons['iso'] = 'file_extension_iso.png';
fileTypeIcons['jar'] = 'file_extension_jar.png';
fileTypeIcons['jpeg'] = 'file_extension_jpeg.png';
fileTypeIcons['jpg'] = 'file_extension_jpg.png';
fileTypeIcons['lnk'] = 'file_extension_lnk.png';
fileTypeIcons['log'] = 'file_extension_log.png';
fileTypeIcons['m4a'] = 'file_extension_m4a.png';
fileTypeIcons['m4b'] = 'file_extension_m4b.png';
fileTypeIcons['m4p'] = 'file_extension_m4p.png';
fileTypeIcons['m4v'] = 'file_extension_m4v.png';
fileTypeIcons['mcd'] = 'file_extension_mcd.png';
fileTypeIcons['mdb'] = 'file_extension_mdb.png';
fileTypeIcons['mid'] = 'file_extension_mid.png';
fileTypeIcons['mov'] = 'file_extension_mov.png';
fileTypeIcons['mp2'] = 'file_extension_mp2.png';
fileTypeIcons['mp4'] = 'file_extension_mp4.png';
fileTypeIcons['mpeg'] = 'file_extension_mpeg.png';
fileTypeIcons['mpg'] = 'file_extension_mpg.png';
fileTypeIcons['msi'] = 'file_extension_msi.png';
fileTypeIcons['mswmm'] = 'file_extension_mswmm.png';
fileTypeIcons['ogg'] = 'file_extension_ogg.png';
fileTypeIcons['pdf'] = 'file_extension_pdf.png';
fileTypeIcons['png'] = 'file_extension_png.png';
fileTypeIcons['pps'] = 'file_extension_pps.png';
fileTypeIcons['ps'] = 'file_extension_ps.png';
fileTypeIcons['psd'] = 'file_extension_psd.png';
fileTypeIcons['pst'] = 'file_extension_pst.png';
fileTypeIcons['ptb'] = 'file_extension_ptb.png';
fileTypeIcons['pub'] = 'file_extension_pub.png';
fileTypeIcons['qbb'] = 'file_extension_qbb.png';
fileTypeIcons['qbw'] = 'file_extension_qbw.png';
fileTypeIcons['qxd'] = 'file_extension_qxd.png';
fileTypeIcons['ram'] = 'file_extension_ram.png';
fileTypeIcons['rar'] = 'file_extension_rar.png';
fileTypeIcons['rm'] = 'file_extension_rm.png';
fileTypeIcons['rmvb'] = 'file_extension_rmvb.png';
fileTypeIcons['rtf'] = 'file_extension_rtf.png';
fileTypeIcons['sea'] = 'file_extension_sea.png';
fileTypeIcons['ses'] = 'file_extension_ses.png';
fileTypeIcons['sit'] = 'file_extension_sit.png';
fileTypeIcons['sitx'] = 'file_extension_sitx.png';
fileTypeIcons['ss'] = 'file_extension_ss.png';
fileTypeIcons['swf'] = 'file_extension_swf.png';
fileTypeIcons['tgz'] = 'file_extension_tgz.png';
fileTypeIcons['thm'] = 'file_extension_thm.png';
fileTypeIcons['tif'] = 'file_extension_tif.png';
fileTypeIcons['tmp'] = 'file_extension_tmp.png';
fileTypeIcons['torrent'] = 'file_extension_torrent.png';
fileTypeIcons['ttf'] = 'file_extension_ttf.png';
fileTypeIcons['txt'] = 'file_extension_txt.png';
fileTypeIcons['vcd'] = 'file_extension_vcd.png';
fileTypeIcons['vob'] = 'file_extension_vob.png';
fileTypeIcons['wav'] = 'file_extension_wav.png';
fileTypeIcons['wma'] = 'file_extension_wma.png';
fileTypeIcons['wmv'] = 'file_extension_wmv.png';
fileTypeIcons['wps'] = 'file_extension_wps.png';
fileTypeIcons['xls'] = 'file_extension_xls.png';
fileTypeIcons['xpi'] = 'file_extension_xpi.png';
fileTypeIcons['zip'] = 'file_extension_zip.png';
