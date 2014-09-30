using System;
using System.Drawing.Imaging;
namespace MoMA.Helpers
{
	public class FileHelper
	{
		public static ImageFormat GetImageFormat(string file)
		{
			ImageFormat result = null;
			string text = file.Remove(0, file.LastIndexOf("."));
			string a;
			if ((a = text.ToLower()) != null)
			{
				if (!(a == ".gif"))
				{
					if (!(a == ".jpeg"))
					{
						if (!(a == ".jpg"))
						{
							if (a == ".png")
							{
								result = ImageFormat.Png;
							}
						}
						else
						{
							result = ImageFormat.Jpeg;
						}
					}
					else
					{
						result = ImageFormat.Jpeg;
					}
				}
				else
				{
					result = ImageFormat.Gif;
				}
			}
			return result;
		}
		public static string MimeType(string file)
		{
			string text = file.Remove(0, file.LastIndexOf("."));
			string key;
			string result;
			switch (key = text.ToLower())
			{
			case ".3dm":
				result = "x-world/x-3dmf";
				return result;
			case ".3dmf":
				result = "x-world/x-3dmf";
				return result;
			case ".a":
				result = "application/octet-stream";
				return result;
			case ".aab":
				result = "application/x-authorware-bin";
				return result;
			case ".aam":
				result = "application/x-authorware-map";
				return result;
			case ".aas":
				result = "application/x-authorware-seg";
				return result;
			case ".abc":
				result = "text/vnd.abc";
				return result;
			case ".acgi":
				result = "text/html";
				return result;
			case ".afl":
				result = "video/animaflex";
				return result;
			case ".ai":
				result = "application/postscript";
				return result;
			case ".aif":
				result = "audio/aiff";
				return result;
			case ".aifc":
				result = "audio/aiff";
				return result;
			case ".aiff":
				result = "audio/aiff";
				return result;
			case ".aim":
				result = "application/x-aim";
				return result;
			case ".aip":
				result = "text/x-audiosoft-intra";
				return result;
			case ".ani":
				result = "application/x-navi-animation";
				return result;
			case ".aos":
				result = "application/x-nokia-9000-communicator-add-on-software";
				return result;
			case ".aps":
				result = "application/mime";
				return result;
			case ".arc":
				result = "application/octet-stream";
				return result;
			case ".arj":
				result = "application/arj";
				return result;
			case ".art":
				result = "image/x-jg";
				return result;
			case ".asf":
				result = "video/x-ms-asf";
				return result;
			case ".asm":
				result = "text/x-asm";
				return result;
			case ".asp":
				result = "text/asp";
				return result;
			case ".asx":
				result = "video/x-ms-asf";
				return result;
			case ".au":
				result = "audio/basic";
				return result;
			case ".avi":
				result = "video/avi";
				return result;
			case ".avs":
				result = "video/avs-video";
				return result;
			case ".bcpio":
				result = "application/x-bcpio";
				return result;
			case ".bin":
				result = "application/octet-stream";
				return result;
			case ".bm":
				result = "image/bmp";
				return result;
			case ".bmp":
				result = "image/bmp";
				return result;
			case ".boo":
				result = "application/book";
				return result;
			case ".book":
				result = "application/book";
				return result;
			case ".boz":
				result = "application/x-bzip2";
				return result;
			case ".bsh":
				result = "application/x-bsh";
				return result;
			case ".bz":
				result = "application/x-bzip";
				return result;
			case ".bz2":
				result = "application/x-bzip2";
				return result;
			case ".c":
				result = "text/plain";
				return result;
			case ".c++":
				result = "text/plain";
				return result;
			case ".cat":
				result = "application/vnd.ms-pki.seccat";
				return result;
			case ".cc":
				result = "text/plain";
				return result;
			case ".ccad":
				result = "application/clariscad";
				return result;
			case ".cco":
				result = "application/x-cocoa";
				return result;
			case ".cdf":
				result = "application/cdf";
				return result;
			case ".cer":
				result = "application/pkix-cert";
				return result;
			case ".cha":
				result = "application/x-chat";
				return result;
			case ".chat":
				result = "application/x-chat";
				return result;
			case ".class":
				result = "application/java";
				return result;
			case ".com":
				result = "application/octet-stream";
				return result;
			case ".conf":
				result = "text/plain";
				return result;
			case ".cpio":
				result = "application/x-cpio";
				return result;
			case ".cpp":
				result = "text/x-c";
				return result;
			case ".cpt":
				result = "application/x-cpt";
				return result;
			case ".crl":
				result = "application/pkcs-crl";
				return result;
			case ".crt":
				result = "application/pkix-cert";
				return result;
			case ".csh":
				result = "application/x-csh";
				return result;
			case ".css":
				result = "text/css";
				return result;
			case ".cxx":
				result = "text/plain";
				return result;
			case ".dcr":
				result = "application/x-director";
				return result;
			case ".deepv":
				result = "application/x-deepv";
				return result;
			case ".def":
				result = "text/plain";
				return result;
			case ".der":
				result = "application/x-x509-ca-cert";
				return result;
			case ".dif":
				result = "video/x-dv";
				return result;
			case ".dir":
				result = "application/x-director";
				return result;
			case ".dl":
				result = "video/dl";
				return result;
			case ".doc":
				result = "application/msword";
				return result;
			case ".dot":
				result = "application/msword";
				return result;
			case ".dp":
				result = "application/commonground";
				return result;
			case ".drw":
				result = "application/drafting";
				return result;
			case ".dump":
				result = "application/octet-stream";
				return result;
			case ".dv":
				result = "video/x-dv";
				return result;
			case ".dvi":
				result = "application/x-dvi";
				return result;
			case ".dwf":
				result = "model/vnd.dwf";
				return result;
			case ".dwg":
				result = "image/vnd.dwg";
				return result;
			case ".dxf":
				result = "image/vnd.dwg";
				return result;
			case ".dxr":
				result = "application/x-director";
				return result;
			case ".el":
				result = "text/x-script.elisp";
				return result;
			case ".elc":
				result = "application/x-elc";
				return result;
			case ".env":
				result = "application/x-envoy";
				return result;
			case ".eps":
				result = "application/postscript";
				return result;
			case ".es":
				result = "application/x-esrehber";
				return result;
			case ".etx":
				result = "text/x-setext";
				return result;
			case ".evy":
				result = "application/envoy";
				return result;
			case ".exe":
				result = "application/octet-stream";
				return result;
			case ".f":
				result = "text/plain";
				return result;
			case ".f77":
				result = "text/x-fortran";
				return result;
			case ".f90":
				result = "text/plain";
				return result;
			case ".fdf":
				result = "application/vnd.fdf";
				return result;
			case ".fif":
				result = "image/fif";
				return result;
			case ".fli":
				result = "video/fli";
				return result;
			case ".flo":
				result = "image/florian";
				return result;
			case ".flx":
				result = "text/vnd.fmi.flexstor";
				return result;
			case ".fmf":
				result = "video/x-atomic3d-feature";
				return result;
			case ".for":
				result = "text/x-fortran";
				return result;
			case ".fpx":
				result = "image/vnd.fpx";
				return result;
			case ".frl":
				result = "application/freeloader";
				return result;
			case ".funk":
				result = "audio/make";
				return result;
			case ".g":
				result = "text/plain";
				return result;
			case ".g3":
				result = "image/g3fax";
				return result;
			case ".gif":
				result = "image/gif";
				return result;
			case ".gl":
				result = "video/gl";
				return result;
			case ".gsd":
				result = "audio/x-gsm";
				return result;
			case ".gsm":
				result = "audio/x-gsm";
				return result;
			case ".gsp":
				result = "application/x-gsp";
				return result;
			case ".gss":
				result = "application/x-gss";
				return result;
			case ".gtar":
				result = "application/x-gtar";
				return result;
			case ".gz":
				result = "application/x-gzip";
				return result;
			case ".gzip":
				result = "application/x-gzip";
				return result;
			case ".h":
				result = "text/plain";
				return result;
			case ".hdf":
				result = "application/x-hdf";
				return result;
			case ".help":
				result = "application/x-helpfile";
				return result;
			case ".hgl":
				result = "application/vnd.hp-hpgl";
				return result;
			case ".hh":
				result = "text/plain";
				return result;
			case ".hlb":
				result = "text/x-script";
				return result;
			case ".hlp":
				result = "application/hlp";
				return result;
			case ".hpg":
				result = "application/vnd.hp-hpgl";
				return result;
			case ".hpgl":
				result = "application/vnd.hp-hpgl";
				return result;
			case ".hqx":
				result = "application/binhex";
				return result;
			case ".hta":
				result = "application/hta";
				return result;
			case ".htc":
				result = "text/x-component";
				return result;
			case ".htm":
				result = "text/html";
				return result;
			case ".html":
				result = "text/html";
				return result;
			case ".htmls":
				result = "text/html";
				return result;
			case ".htt":
				result = "text/webviewhtml";
				return result;
			case ".htx":
				result = "text/html";
				return result;
			case ".ice":
				result = "x-conference/x-cooltalk";
				return result;
			case ".ico":
				result = "image/x-icon";
				return result;
			case ".idc":
				result = "text/plain";
				return result;
			case ".ief":
				result = "image/ief";
				return result;
			case ".iefs":
				result = "image/ief";
				return result;
			case ".iges":
				result = "application/iges";
				return result;
			case ".igs":
				result = "application/iges";
				return result;
			case ".ima":
				result = "application/x-ima";
				return result;
			case ".imap":
				result = "application/x-httpd-imap";
				return result;
			case ".inf":
				result = "application/inf";
				return result;
			case ".ins":
				result = "application/x-internett-signup";
				return result;
			case ".ip":
				result = "application/x-ip2";
				return result;
			case ".isu":
				result = "video/x-isvideo";
				return result;
			case ".it":
				result = "audio/it";
				return result;
			case ".iv":
				result = "application/x-inventor";
				return result;
			case ".ivr":
				result = "i-world/i-vrml";
				return result;
			case ".ivy":
				result = "application/x-livescreen";
				return result;
			case ".jam":
				result = "audio/x-jam";
				return result;
			case ".jav":
				result = "text/plain";
				return result;
			case ".java":
				result = "text/plain";
				return result;
			case ".jcm":
				result = "application/x-java-commerce";
				return result;
			case ".jfif":
				result = "image/jpeg";
				return result;
			case ".jfif-tbnl":
				result = "image/jpeg";
				return result;
			case ".jpe":
				result = "image/jpeg";
				return result;
			case ".jpeg":
				result = "image/jpeg";
				return result;
			case ".jpg":
				result = "image/jpeg";
				return result;
			case ".jps":
				result = "image/x-jps";
				return result;
			case ".js":
				result = "application/x-javascript";
				return result;
			case ".jut":
				result = "image/jutvision";
				return result;
			case ".kar":
				result = "audio/midi";
				return result;
			case ".ksh":
				result = "application/x-ksh";
				return result;
			case ".la":
				result = "audio/nspaudio";
				return result;
			case ".lam":
				result = "audio/x-liveaudio";
				return result;
			case ".latex":
				result = "application/x-latex";
				return result;
			case ".lha":
				result = "application/octet-stream";
				return result;
			case ".lhx":
				result = "application/octet-stream";
				return result;
			case ".list":
				result = "text/plain";
				return result;
			case ".lma":
				result = "audio/nspaudio";
				return result;
			case ".log":
				result = "text/plain";
				return result;
			case ".lsp":
				result = "application/x-lisp";
				return result;
			case ".lst":
				result = "text/plain";
				return result;
			case ".lsx":
				result = "text/x-la-asf";
				return result;
			case ".ltx":
				result = "application/x-latex";
				return result;
			case ".lzh":
				result = "application/octet-stream";
				return result;
			case ".lzx":
				result = "application/octet-stream";
				return result;
			case ".m":
				result = "text/plain";
				return result;
			case ".m1v":
				result = "video/mpeg";
				return result;
			case ".m2a":
				result = "audio/mpeg";
				return result;
			case ".m2v":
				result = "video/mpeg";
				return result;
			case ".m3u":
				result = "audio/x-mpequrl";
				return result;
			case ".man":
				result = "application/x-troff-man";
				return result;
			case ".map":
				result = "application/x-navimap";
				return result;
			case ".mar":
				result = "text/plain";
				return result;
			case ".mbd":
				result = "application/mbedlet";
				return result;
			case ".mc$":
				result = "application/x-magic-cap-package-1.0";
				return result;
			case ".mcd":
				result = "application/mcad";
				return result;
			case ".mcf":
				result = "text/mcf";
				return result;
			case ".mcp":
				result = "application/netmc";
				return result;
			case ".me":
				result = "application/x-troff-me";
				return result;
			case ".mht":
				result = "message/rfc822";
				return result;
			case ".mhtml":
				result = "message/rfc822";
				return result;
			case ".mid":
				result = "audio/midi";
				return result;
			case ".midi":
				result = "audio/midi";
				return result;
			case ".mif":
				result = "application/x-mif";
				return result;
			case ".mime":
				result = "message/rfc822";
				return result;
			case ".mjf":
				result = "audio/x-vnd.audioexplosion.mjuicemediafile";
				return result;
			case ".mjpg":
				result = "video/x-motion-jpeg";
				return result;
			case ".mm":
				result = "application/base64";
				return result;
			case ".mme":
				result = "application/base64";
				return result;
			case ".mod":
				result = "audio/mod";
				return result;
			case ".moov":
				result = "video/quicktime";
				return result;
			case ".mov":
				result = "video/quicktime";
				return result;
			case ".movie":
				result = "video/x-sgi-movie";
				return result;
			case ".mp2":
				result = "audio/mpeg";
				return result;
			case ".mp3":
				result = "audio/mpeg";
				return result;
			case ".mpa":
				result = "audio/mpeg";
				return result;
			case ".mpc":
				result = "application/x-project";
				return result;
			case ".mpe":
				result = "video/mpeg";
				return result;
			case ".mpeg":
				result = "video/mpeg";
				return result;
			case ".mpg":
				result = "video/mpeg";
				return result;
			case ".mpga":
				result = "audio/mpeg";
				return result;
			case ".mpp":
				result = "application/vnd.ms-project";
				return result;
			case ".mpt":
				result = "application/vnd.ms-project";
				return result;
			case ".mpv":
				result = "application/vnd.ms-project";
				return result;
			case ".mpx":
				result = "application/vnd.ms-project";
				return result;
			case ".mrc":
				result = "application/marc";
				return result;
			case ".ms":
				result = "application/x-troff-ms";
				return result;
			case ".mv":
				result = "video/x-sgi-movie";
				return result;
			case ".my":
				result = "audio/make";
				return result;
			case ".mzz":
				result = "application/x-vnd.audioexplosion.mzz";
				return result;
			case ".nap":
				result = "image/naplps";
				return result;
			case ".naplps":
				result = "image/naplps";
				return result;
			case ".nc":
				result = "application/x-netcdf";
				return result;
			case ".ncm":
				result = "application/vnd.nokia.configuration-message";
				return result;
			case ".nif":
				result = "image/x-niff";
				return result;
			case ".niff":
				result = "image/x-niff";
				return result;
			case ".nix":
				result = "application/x-mix-transfer";
				return result;
			case ".nsc":
				result = "application/x-conference";
				return result;
			case ".nvd":
				result = "application/x-navidoc";
				return result;
			case ".o":
				result = "application/octet-stream";
				return result;
			case ".oda":
				result = "application/oda";
				return result;
			case ".omc":
				result = "application/x-omc";
				return result;
			case ".omcd":
				result = "application/x-omcdatamaker";
				return result;
			case ".omcr":
				result = "application/x-omcregerator";
				return result;
			case ".p":
				result = "text/x-pascal";
				return result;
			case ".p10":
				result = "application/pkcs10";
				return result;
			case ".p12":
				result = "application/pkcs-12";
				return result;
			case ".p7a":
				result = "application/x-pkcs7-signature";
				return result;
			case ".p7c":
				result = "application/pkcs7-mime";
				return result;
			case ".p7m":
				result = "application/pkcs7-mime";
				return result;
			case ".p7r":
				result = "application/x-pkcs7-certreqresp";
				return result;
			case ".p7s":
				result = "application/pkcs7-signature";
				return result;
			case ".part":
				result = "application/pro_eng";
				return result;
			case ".pas":
				result = "text/pascal";
				return result;
			case ".pbm":
				result = "image/x-portable-bitmap";
				return result;
			case ".pcl":
				result = "application/vnd.hp-pcl";
				return result;
			case ".pct":
				result = "image/x-pict";
				return result;
			case ".pcx":
				result = "image/x-pcx";
				return result;
			case ".pdb":
				result = "chemical/x-pdb";
				return result;
			case ".pdf":
				result = "application/pdf";
				return result;
			case ".pfunk":
				result = "audio/make";
				return result;
			case ".pgm":
				result = "image/x-portable-greymap";
				return result;
			case ".pic":
				result = "image/pict";
				return result;
			case ".pict":
				result = "image/pict";
				return result;
			case ".pkg":
				result = "application/x-newton-compatible-pkg";
				return result;
			case ".pko":
				result = "application/vnd.ms-pki.pko";
				return result;
			case ".pl":
				result = "text/plain";
				return result;
			case ".plx":
				result = "application/x-pixclscript";
				return result;
			case ".pm":
				result = "image/x-xpixmap";
				return result;
			case ".pm4":
				result = "application/x-pagemaker";
				return result;
			case ".pm5":
				result = "application/x-pagemaker";
				return result;
			case ".png":
				result = "image/png";
				return result;
			case ".pnm":
				result = "application/x-portable-anymap";
				return result;
			case ".pot":
				result = "application/vnd.ms-powerpoint";
				return result;
			case ".pov":
				result = "model/x-pov";
				return result;
			case ".ppa":
				result = "application/vnd.ms-powerpoint";
				return result;
			case ".ppm":
				result = "image/x-portable-pixmap";
				return result;
			case ".pps":
				result = "application/vnd.ms-powerpoint";
				return result;
			case ".ppt":
				result = "application/vnd.ms-powerpoint";
				return result;
			case ".ppz":
				result = "application/vnd.ms-powerpoint";
				return result;
			case ".pre":
				result = "application/x-freelance";
				return result;
			case ".prt":
				result = "application/pro_eng";
				return result;
			case ".ps":
				result = "application/postscript";
				return result;
			case ".psd":
				result = "application/octet-stream";
				return result;
			case ".pvu":
				result = "paleovu/x-pv";
				return result;
			case ".pwz":
				result = "application/vnd.ms-powerpoint";
				return result;
			case ".py":
				result = "text/x-script.phyton";
				return result;
			case ".pyc":
				result = "applicaiton/x-bytecode.python";
				return result;
			case ".qcp":
				result = "audio/vnd.qcelp";
				return result;
			case ".qd3":
				result = "x-world/x-3dmf";
				return result;
			case ".qd3d":
				result = "x-world/x-3dmf";
				return result;
			case ".qif":
				result = "image/x-quicktime";
				return result;
			case ".qt":
				result = "video/quicktime";
				return result;
			case ".qtc":
				result = "video/x-qtc";
				return result;
			case ".qti":
				result = "image/x-quicktime";
				return result;
			case ".qtif":
				result = "image/x-quicktime";
				return result;
			case ".ra":
				result = "audio/x-pn-realaudio";
				return result;
			case ".ram":
				result = "audio/x-pn-realaudio";
				return result;
			case ".ras":
				result = "application/x-cmu-raster";
				return result;
			case ".rast":
				result = "image/cmu-raster";
				return result;
			case ".rexx":
				result = "text/x-script.rexx";
				return result;
			case ".rf":
				result = "image/vnd.rn-realflash";
				return result;
			case ".rgb":
				result = "image/x-rgb";
				return result;
			case ".rm":
				result = "application/vnd.rn-realmedia";
				return result;
			case ".rmi":
				result = "audio/mid";
				return result;
			case ".rmm":
				result = "audio/x-pn-realaudio";
				return result;
			case ".rmp":
				result = "audio/x-pn-realaudio";
				return result;
			case ".rng":
				result = "application/ringing-tones";
				return result;
			case ".rnx":
				result = "application/vnd.rn-realplayer";
				return result;
			case ".roff":
				result = "application/x-troff";
				return result;
			case ".rp":
				result = "image/vnd.rn-realpix";
				return result;
			case ".rpm":
				result = "audio/x-pn-realaudio-plugin";
				return result;
			case ".rt":
				result = "text/richtext";
				return result;
			case ".rtf":
				result = "text/richtext";
				return result;
			case ".rtx":
				result = "text/richtext";
				return result;
			case ".rv":
				result = "video/vnd.rn-realvideo";
				return result;
			case ".s":
				result = "text/x-asm";
				return result;
			case ".s3m":
				result = "audio/s3m";
				return result;
			case ".saveme":
				result = "application/octet-stream";
				return result;
			case ".sbk":
				result = "application/x-tbook";
				return result;
			case ".scm":
				result = "application/x-lotusscreencam";
				return result;
			case ".sdml":
				result = "text/plain";
				return result;
			case ".sdp":
				result = "application/sdp";
				return result;
			case ".sdr":
				result = "application/sounder";
				return result;
			case ".sea":
				result = "application/sea";
				return result;
			case ".set":
				result = "application/set";
				return result;
			case ".sgm":
				result = "text/sgml";
				return result;
			case ".sgml":
				result = "text/sgml";
				return result;
			case ".sh":
				result = "application/x-sh";
				return result;
			case ".shar":
				result = "application/x-shar";
				return result;
			case ".shtml":
				result = "text/html";
				return result;
			case ".sid":
				result = "audio/x-psid";
				return result;
			case ".sit":
				result = "application/x-sit";
				return result;
			case ".skd":
				result = "application/x-koan";
				return result;
			case ".skm":
				result = "application/x-koan";
				return result;
			case ".skp":
				result = "application/x-koan";
				return result;
			case ".skt":
				result = "application/x-koan";
				return result;
			case ".sl":
				result = "application/x-seelogo";
				return result;
			case ".smi":
				result = "application/smil";
				return result;
			case ".smil":
				result = "application/smil";
				return result;
			case ".snd":
				result = "audio/basic";
				return result;
			case ".sol":
				result = "application/solids";
				return result;
			case ".spc":
				result = "text/x-speech";
				return result;
			case ".spl":
				result = "application/futuresplash";
				return result;
			case ".spr":
				result = "application/x-sprite";
				return result;
			case ".sprite":
				result = "application/x-sprite";
				return result;
			case ".src":
				result = "application/x-wais-source";
				return result;
			case ".ssi":
				result = "text/x-server-parsed-html";
				return result;
			case ".ssm":
				result = "application/streamingmedia";
				return result;
			case ".sst":
				result = "application/vnd.ms-pki.certstore";
				return result;
			case ".step":
				result = "application/step";
				return result;
			case ".stl":
				result = "application/sla";
				return result;
			case ".stp":
				result = "application/step";
				return result;
			case ".sv4cpio":
				result = "application/x-sv4cpio";
				return result;
			case ".sv4crc":
				result = "application/x-sv4crc";
				return result;
			case ".svf":
				result = "image/vnd.dwg";
				return result;
			case ".svr":
				result = "application/x-world";
				return result;
			case ".swf":
				result = "application/x-shockwave-flash";
				return result;
			case ".t":
				result = "application/x-troff";
				return result;
			case ".talk":
				result = "text/x-speech";
				return result;
			case ".tar":
				result = "application/x-tar";
				return result;
			case ".tbk":
				result = "application/toolbook";
				return result;
			case ".tcl":
				result = "application/x-tcl";
				return result;
			case ".tcsh":
				result = "text/x-script.tcsh";
				return result;
			case ".tex":
				result = "application/x-tex";
				return result;
			case ".texi":
				result = "application/x-texinfo";
				return result;
			case ".texinfo":
				result = "application/x-texinfo";
				return result;
			case ".text":
				result = "text/plain";
				return result;
			case ".tgz":
				result = "application/x-compressed";
				return result;
			case ".tif":
				result = "image/tiff";
				return result;
			case ".tiff":
				result = "image/tiff";
				return result;
			case ".tr":
				result = "application/x-troff";
				return result;
			case ".tsi":
				result = "audio/tsp-audio";
				return result;
			case ".tsp":
				result = "application/dsptype";
				return result;
			case ".tsv":
				result = "text/tab-separated-values";
				return result;
			case ".turbot":
				result = "image/florian";
				return result;
			case ".txt":
				result = "text/plain";
				return result;
			case ".uil":
				result = "text/x-uil";
				return result;
			case ".uni":
				result = "text/uri-list";
				return result;
			case ".unis":
				result = "text/uri-list";
				return result;
			case ".unv":
				result = "application/i-deas";
				return result;
			case ".uri":
				result = "text/uri-list";
				return result;
			case ".uris":
				result = "text/uri-list";
				return result;
			case ".ustar":
				result = "application/x-ustar";
				return result;
			case ".uu":
				result = "application/octet-stream";
				return result;
			case ".uue":
				result = "text/x-uuencode";
				return result;
			case ".vcd":
				result = "application/x-cdlink";
				return result;
			case ".vcs":
				result = "text/x-vcalendar";
				return result;
			case ".vda":
				result = "application/vda";
				return result;
			case ".vdo":
				result = "video/vdo";
				return result;
			case ".vew":
				result = "application/groupwise";
				return result;
			case ".viv":
				result = "video/vivo";
				return result;
			case ".vivo":
				result = "video/vivo";
				return result;
			case ".vmd":
				result = "application/vocaltec-media-desc";
				return result;
			case ".vmf":
				result = "application/vocaltec-media-file";
				return result;
			case ".voc":
				result = "audio/voc";
				return result;
			case ".vos":
				result = "video/vosaic";
				return result;
			case ".vox":
				result = "audio/voxware";
				return result;
			case ".vqe":
				result = "audio/x-twinvq-plugin";
				return result;
			case ".vqf":
				result = "audio/x-twinvq";
				return result;
			case ".vql":
				result = "audio/x-twinvq-plugin";
				return result;
			case ".vrml":
				result = "application/x-vrml";
				return result;
			case ".vrt":
				result = "x-world/x-vrt";
				return result;
			case ".vsd":
				result = "application/x-visio";
				return result;
			case ".vst":
				result = "application/x-visio";
				return result;
			case ".vsw":
				result = "application/x-visio";
				return result;
			case ".w60":
				result = "application/wordperfect6.0";
				return result;
			case ".w61":
				result = "application/wordperfect6.1";
				return result;
			case ".w6w":
				result = "application/msword";
				return result;
			case ".wav":
				result = "audio/wav";
				return result;
			case ".wb1":
				result = "application/x-qpro";
				return result;
			case ".wbmp":
				result = "image/vnd.wap.wbmp";
				return result;
			case ".web":
				result = "application/vnd.xara";
				return result;
			case ".wiz":
				result = "application/msword";
				return result;
			case ".wk1":
				result = "application/x-123";
				return result;
			case ".wmf":
				result = "windows/metafile";
				return result;
			case ".wml":
				result = "text/vnd.wap.wml";
				return result;
			case ".wmlc":
				result = "application/vnd.wap.wmlc";
				return result;
			case ".wmls":
				result = "text/vnd.wap.wmlscript";
				return result;
			case ".wmlsc":
				result = "application/vnd.wap.wmlscriptc";
				return result;
			case ".word":
				result = "application/msword";
				return result;
			case ".wp":
				result = "application/wordperfect";
				return result;
			case ".wp5":
				result = "application/wordperfect";
				return result;
			case ".wp6":
				result = "application/wordperfect";
				return result;
			case ".wpd":
				result = "application/wordperfect";
				return result;
			case ".wq1":
				result = "application/x-lotus";
				return result;
			case ".wri":
				result = "application/mswrite";
				return result;
			case ".wrl":
				result = "application/x-world";
				return result;
			case ".wrz":
				result = "x-world/x-vrml";
				return result;
			case ".wsc":
				result = "text/scriplet";
				return result;
			case ".wsrc":
				result = "application/x-wais-source";
				return result;
			case ".wtk":
				result = "application/x-wintalk";
				return result;
			case ".xbm":
				result = "image/x-xbitmap";
				return result;
			case ".xdr":
				result = "video/x-amt-demorun";
				return result;
			case ".xgz":
				result = "xgl/drawing";
				return result;
			case ".xif":
				result = "image/vnd.xiff";
				return result;
			case ".xl":
				result = "application/excel";
				return result;
			case ".xla":
				result = "application/vnd.ms-excel";
				return result;
			case ".xlb":
				result = "application/vnd.ms-excel";
				return result;
			case ".xlc":
				result = "application/vnd.ms-excel";
				return result;
			case ".xld":
				result = "application/vnd.ms-excel";
				return result;
			case ".xlk":
				result = "application/vnd.ms-excel";
				return result;
			case ".xll":
				result = "application/vnd.ms-excel";
				return result;
			case ".xlm":
				result = "application/vnd.ms-excel";
				return result;
			case ".xls":
				result = "application/vnd.ms-excel";
				return result;
			case ".xlt":
				result = "application/vnd.ms-excel";
				return result;
			case ".xlv":
				result = "application/vnd.ms-excel";
				return result;
			case ".xlw":
				result = "application/vnd.ms-excel";
				return result;
			case ".xm":
				result = "audio/xm";
				return result;
			case ".xml":
				result = "application/xml";
				return result;
			case ".xmz":
				result = "xgl/movie";
				return result;
			case ".xpix":
				result = "application/x-vnd.ls-xpix";
				return result;
			case ".xpm":
				result = "image/xpm";
				return result;
			case ".x-png":
				result = "image/png";
				return result;
			case ".xsr":
				result = "video/x-amt-showrun";
				return result;
			case ".xwd":
				result = "image/x-xwd";
				return result;
			case ".xyz":
				result = "chemical/x-pdb";
				return result;
			case ".z":
				result = "application/x-compressed";
				return result;
			case ".zip":
				result = "application/zip";
				return result;
			case ".zoo":
				result = "application/octet-stream";
				return result;
			case ".zsh":
				result = "text/x-script.zsh";
				return result;
			}
			result = "application/octet-stream";
			return result;
		}
	}
}
