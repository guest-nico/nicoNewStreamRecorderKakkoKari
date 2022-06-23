/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2022/05/06
 * Time: 19:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace namaichi.utility
{
	/// <summary>
	/// Description of Curl.
	/// </summary>
	public class Curl
	{
		private IntPtr curlM = IntPtr.Zero;
		public Curl()
		{
			try {
				curlM = curl_multi_init();
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
			if (curlM == IntPtr.Zero) {
				Debug.WriteLine("curl_multi_init zero");
				return;
			}
			curl_multi_setopt(curlM, CURLMoption.CURLMOPT_PIPELINING, (int)CURLpipe.CURLPIPE_MULTIPLEX);
		}
		public bool isInitialized() {
			return curlM != IntPtr.Zero;
		}
		public byte[] getBytes(string url, Dictionary<string, string> headers, CurlHttpVersion httpVer, string method = "GET", string postData = "", bool isAddHeader = false) {
			try {
				var urlList = new List<string>(){url};
				var r = get(urlList, headers, httpVer, method, postData, isAddHeader);
				return r.Count != 0 ? r[0].Value : null;
			} catch (Exception e) {
				Debug.WriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return null;
			}
		}
		public string getStr(string url, Dictionary<string, string> headers, CurlHttpVersion httpVer, string method = "GET", string postData = "", bool isAddHeader = false) {
			try {
				var urlList = new List<string>(){url};
				var r = get(urlList, headers, httpVer, method, postData, isAddHeader);
				if (r.Count == 0) return null;
				return r[0].Value == null ? null : Encoding.UTF8.GetString(r[0].Value);
			} catch (Exception e) {
				Debug.WriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
				return null;
			}
		}
		
		public List<KeyValuePair<string, byte[]>> get(List<string> urlList, Dictionary<string, string> headers, CurlHttpVersion httpVer, string method = "GET", string postData = "", bool isAddHeader = false) {
			var easyDataList = new List<easyData>();
			var _postData = postData == null ? IntPtr.Zero : Marshal.StringToHGlobalAnsi(postData);
			try {
				Debug.WriteLine("curl urlList count " + urlList.Count);
				foreach (var u in urlList) {
					var easyD = new easyData(u);
					Debug.WriteLine("curl access " + u);
					if (!easyD.setEasy(u, headers, httpVer, method, _postData)) {
						Debug.WriteLine("set easy error " + u);
						easyDataList.Add(null);
						continue;
					}
					easyDataList.Add(easyD);
					curl_multi_add_handle(curlM, easyD.easyPtr);
				}
				
				
				int still_running = 1;
				var mc = CURLMcode.CURLM_OK;
				while(still_running != 0) {
					mc = curl_multi_perform(curlM, out still_running);
					
					if(still_running != 0)
						mc = curl_multi_poll(curlM, IntPtr.Zero, 0, 10000, IntPtr.Zero);
					
					if(mc != CURLMcode.CURLM_OK)
						break;
				}
				int msgs_left = 0;
				IntPtr _msg = IntPtr.Zero;
				
				while(true) {
					_msg = curl_multi_info_read(curlM, out msgs_left);
					if (_msg == IntPtr.Zero) break;
					try {
						var msg = (CURLMsg)Marshal.PtrToStructure(_msg, typeof(CURLMsg));
						if(msg.msg == CURLMSG.CURLMSG_DONE) {
							easyData easyD = null;
							foreach (var _easyD in easyDataList) {
								if (_easyD.easyPtr == msg.easy_handle) {
									easyD = _easyD;
									break;
								}
							}
							if (easyD == null) continue;
							
							var _chunk = (MemoryStruct)Marshal.PtrToStructure(easyD.chunkPtr, typeof(MemoryStruct));
							
							byte[] dHeader = new Byte[_chunk.sizeHeader];
							Marshal.Copy(_chunk.memoryHeader, dHeader, 0, _chunk.sizeHeader - 0);
							
							byte[] d = new Byte[_chunk.size + (isAddHeader ? _chunk.sizeHeader : 0)];
							easyD.res = d;
							Marshal.Copy(_chunk.memory, d, isAddHeader ? _chunk.sizeHeader : 0, _chunk.size);
							//Debug.WriteLine(_chunk + " " + _chunk.size + " " + _chunk.sizeHeader);
							if (isAddHeader) Marshal.Copy(_chunk.memoryHeader, d, 0, _chunk.sizeHeader);
						}
						
					} catch (Exception eee) {
						Debug.WriteLine(eee.Message + eee.Source + eee.StackTrace);
					}
					
				}
				foreach (var easyD in easyDataList) {
					curl_multi_remove_handle(curlM, easyD.easyPtr);
					curl_easy_cleanup_data(easyD.easyPtr, easyD.chunkPtr, easyD.headerPtr);
					Marshal.FreeHGlobal(easyD.chunkPtr);
				}
				
			} catch (Exception ee) {
				Debug.WriteLine(ee.Message + ee.Source + ee.StackTrace);
			}
			try {
				if (_postData != IntPtr.Zero)
					Marshal.FreeHGlobal(_postData);
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
			return easyDataList.Select(x => new KeyValuePair<string, byte[]>(x.url, x.res)).ToList();
		}
		
		//private static int writeCallBack(byte[] ptr, int sz, int nmemb, object userdata) {
		/*
		private  IntPtr writeCallBack(IntPtr ptr, IntPtr sz, IntPtr nmemb, IntPtr userdata) {
			//Debug.WriteLine(Encoding.UTF8.GetString(ptr));
			return (IntPtr)(sz.ToInt32() * nmemb.ToInt32());
			//return 0;
		}
		*/
		public struct MemoryStruct {
			public IntPtr memory;
			public int size;
			public IntPtr memoryHeader;
			public int sizeHeader;
			public CURLcode ret;
		}
		[DllImport("libcurl.dll")]
		public static extern IntPtr curl_easy_init();
		[DllImport("libcurl.dll")]
		public static extern IntPtr curl_multi_init();
		[DllImport("libcurl.dll", CharSet = CharSet.Ansi)]
		public static extern IntPtr curl_easy_setopt(IntPtr curl, CURLoption opt, string s);
		[DllImport("libcurl.dll")]
		public static extern IntPtr curl_easy_setopt(IntPtr curl, CURLoption opt, int n);
		[DllImport("libcurl.dll")]
		public static extern IntPtr curl_easy_setopt(IntPtr curl, CURLoption opt, IntPtr cb);
		[DllImport("libcurl.dll")]
		public static extern IntPtr curl_easy_setopt_header(IntPtr curl, CURLoption opt, IntPtr slist);
		[DllImport("libcurl.dll")]//MemoryStruct*
		public static extern IntPtr curl_easy_set_write_data(IntPtr curl, IntPtr chunk);
		[DllImport("libcurl.dll")]
		public static extern CURLMcode curl_multi_setopt(IntPtr curl, CURLMoption opt, int n);
		[DllImport("libcurl.dll")]
		public static extern CURLMcode curl_multi_remove_handle(IntPtr multi, IntPtr easy);
		
		//[DllImport("libcurl.dll")]
		//public static extern IntPtr curl_easy_setopt(IntPtr curl, CURLoption opt, write_callback cb);
		[DllImport("libcurl.dll")]
		public static extern CURLcode curl_easy_perform(IntPtr curl);
		[DllImport("libcurl.dll")]
		//public static extern MemoryStruct curl_easy_perform_data(IntPtr curl, MemoryStruct chunk);
		public static extern MemoryStruct curl_easy_perform_data(IntPtr curl);
		[DllImport("libcurl.dll")]
		public static extern CURLMcode curl_multi_perform(IntPtr curlMulti, out int still_running);
		[DllImport("libcurl.dll")]
		public static extern CURLMcode curl_multi_add_handle(IntPtr curlMulti, IntPtr easy);
		[DllImport("libcurl.dll")]
		public static extern CURLMcode curl_multi_poll(IntPtr multi_handle,
                                      IntPtr extra_fds,
                                      int extra_nfds,
                                      int timeout_ms,
                                      IntPtr ret);
		[DllImport("libcurl.dll")]//ret CURLMsg
		public static extern IntPtr curl_multi_info_read(IntPtr multi_handle, out int msgs_in_queue);
		
		[DllImport("libcurl.dll")]
		public static extern void curl_easy_cleanup(IntPtr curl);
		[DllImport("libcurl.dll")]
		public static extern void curl_easy_cleanup_data(IntPtr curl, IntPtr chunkPtr, IntPtr headerList);
		//public delegate int write_callback(byte[] ptr, int sz, int nmemb, object userdata);
		//public delegate IntPtr write_callback(IntPtr ptr, IntPtr sz, IntPtr nmemb, IntPtr userdata);
		[DllImport("libcurl.dll")]
		public static extern void curl_slist_free_all(IntPtr list);
		[DllImport("libcurl.dll")]
		public static extern void memoryFree(IntPtr data);
		[DllImport("libcurl.dll")]
		public static extern IntPtr createSlist();
		[DllImport("libcurl.dll")]
		public static extern IntPtr addSlist(IntPtr slist, string data);
		
		private class easyData {
			public string url = null;
			public IntPtr easyPtr = IntPtr.Zero;
			public IntPtr headerPtr = IntPtr.Zero;
			public IntPtr chunkPtr = IntPtr.Zero;
			public byte[] res = null;
			public easyData (string url) {
				this.url = url;
			}
			public bool setEasy(string url, Dictionary<string, string> headers, CurlHttpVersion httpVer, string method, IntPtr postData) {
				easyPtr = curl_easy_init();
				if (easyPtr == IntPtr.Zero) {
					Debug.WriteLine("curl easy init no");
					return false;
				}
				Debug.WriteLine(easyPtr);
				
				curl_easy_setopt(easyPtr, CURLoption.CURLOPT_URL, url);
				curl_easy_setopt(easyPtr, CURLoption.CURLOPT_SSL_VERIFYPEER, 0);
				curl_easy_setopt(easyPtr, CURLoption.CURLOPT_HTTP_VERSION, (int)httpVer);
				curl_easy_setopt(easyPtr, CURLoption.CURLOPT_TIMEOUT, 20);
				curl_easy_setopt(easyPtr, CURLoption.CURLOPT_TIMEOUT, 20);
				if (method == "POST") {
					curl_easy_setopt(easyPtr, CURLoption.CURLOPT_POSTFIELDS, postData);
				}
				headerPtr = getHeaderSlist(headers);
				curl_easy_setopt_header(easyPtr, CURLoption.CURLOPT_HTTPHEADER, headerPtr);
				var chunk = new MemoryStruct();
				chunkPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MemoryStruct)));
				Marshal.StructureToPtr(chunk, chunkPtr, true);
				curl_easy_set_write_data(easyPtr, chunkPtr);
				return true;
			}
			private IntPtr getHeaderSlist(Dictionary<string, string> headers) {
				var l = createSlist();
				foreach (var h in headers) {
					var _h = h.Key + ": " + h.Value;
					l = addSlist(l, _h);
				}
				return l;
			}
		}
		public static Dictionary<string, string> getDefaultHeaders(string origin = "https://nicochannel.jp") {
			var h = new Dictionary<string, string>();
			h.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:101.0) Gecko/20100101 Firefox/102.0");
			h.Add("Accept", "*/*");
			h.Add("Accept-Language", "ja,en-US;q=0.7,en;q=0.3");
			h.Add("Accept-Encoding", "gzip, deflate, br");
			h.Add("Origin", origin); //"https://nicochannel.jp");
			h.Add("Connection", "keep-alive");
			h.Add("Referer", origin + "/"); //"https://nicochannel.jp/");
			h.Add("Sec-Fetch-Dest", "empty");
			h.Add("Sec-Fetch-Mode", "cors");
			h.Add("Sec-Fetch-Site", "cross-site");
			h.Add("TE", "trailers");
			return h;
		}
	}
	public enum CURLoption {
		CURLOPT_URL = 10002,
		CURLOPT_SSL_VERIFYPEER = 64,
		CURLOPT_WRITEFUNCTION = 20011,
		CURLOPT_WRITEDATA = 10001,
		CURLOPT_HEADER = 42,
		CURLOPT_HTTPHEADER = 10023,
		CURLOPT_HTTP_VERSION = 84,
		CURLOPT_TIMEOUT = 13,
		CURLOPT_POSTFIELDS = 10015,
		CURLOPT_POSTFIELDSIZE = 60
	}
	public enum CURLMoption {
		CURLMOPT_PIPELINING = 3
	}
	public enum CURLpipe {
		CURLPIPE_NOTHING = 0,
		CURLPIPE_HTTP1 = 1,
		CURLPIPE_MULTIPLEX = 2
	}
	public enum CurlHttpVersion
        {
            CURL_HTTP_VERSION_NONE, /* setting this means we don't care, and that we'd
                             like the library to choose the best possible
                             for us! */
			CURL_HTTP_VERSION_1_0,  /* please use HTTP 1.0 in the request */
			CURL_HTTP_VERSION_1_1,  /* please use HTTP 1.1 in the request */
			CURL_HTTP_VERSION_2_0,  /* please use HTTP 2 in the request */
			CURL_HTTP_VERSION_2TLS, /* use version 2 for HTTPS, version 1.1 for HTTP */
			CURL_HTTP_VERSION_2_PRIOR_KNOWLEDGE,  /* please use HTTP 2 without HTTP/1.1
													Upgrade */
			CURL_HTTP_VERSION_3 = 30, /* Makes use of explicit HTTP/3 without fallback.
								Use CURLOPT_ALTSVC to enable HTTP/3 upgrade */
			CURL_HTTP_VERSION_LAST /* *ILLEGAL* http version */
  	
        };
	public enum CURLcode {
		CURLE_OK = 0,
		CURLE_UNSUPPORTED_PROTOCOL,    /* 1 */
		CURLE_FAILED_INIT,             /* 2 */
		CURLE_URL_MALFORMAT,           /* 3 */
		CURLE_NOT_BUILT_IN,            /* 4 - [was obsoleted in August 2007 for
		                            7.17.0, reused in April 2011 for 7.21.5] */
		CURLE_COULDNT_RESOLVE_PROXY,   /* 5 */
		CURLE_COULDNT_RESOLVE_HOST,    /* 6 */
		CURLE_COULDNT_CONNECT,         /* 7 */
		CURLE_WEIRD_SERVER_REPLY,      /* 8 */
		CURLE_REMOTE_ACCESS_DENIED,    /* 9 a service was denied by the server
		                            due to lack of access - when login fails
		                            this is not returned. */
		CURLE_FTP_ACCEPT_FAILED,       /* 10 - [was obsoleted in April 2006 for
		                            7.15.4, reused in Dec 2011 for 7.24.0]*/
		CURLE_FTP_WEIRD_PASS_REPLY,    /* 11 */
		CURLE_FTP_ACCEPT_TIMEOUT,      /* 12 - timeout occurred accepting server
		                            [was obsoleted in August 2007 for 7.17.0,
		                            reused in Dec 2011 for 7.24.0]*/
		CURLE_FTP_WEIRD_PASV_REPLY,    /* 13 */
		CURLE_FTP_WEIRD_227_FORMAT,    /* 14 */
		CURLE_FTP_CANT_GET_HOST,       /* 15 */
		CURLE_HTTP2,                   /* 16 - A problem in the http2 framing layer.
		                            [was obsoleted in August 2007 for 7.17.0,
		                            reused in July 2014 for 7.38.0] */
		CURLE_FTP_COULDNT_SET_TYPE,    /* 17 */
		CURLE_PARTIAL_FILE,            /* 18 */
		CURLE_FTP_COULDNT_RETR_FILE,   /* 19 */
		CURLE_OBSOLETE20,              /* 20 - NOT USED */
		CURLE_QUOTE_ERROR,             /* 21 - quote command failure */
		CURLE_HTTP_RETURNED_ERROR,     /* 22 */
		CURLE_WRITE_ERROR,             /* 23 */
		CURLE_OBSOLETE24,              /* 24 - NOT USED */
		CURLE_UPLOAD_FAILED,           /* 25 - failed upload "command" */
		CURLE_READ_ERROR,              /* 26 - couldn't open/read from file */
		CURLE_OUT_OF_MEMORY,           /* 27 */
		CURLE_OPERATION_TIMEDOUT,      /* 28 - the timeout time was reached */
		CURLE_OBSOLETE29,              /* 29 - NOT USED */
		CURLE_FTP_PORT_FAILED,         /* 30 - FTP PORT operation failed */
		CURLE_FTP_COULDNT_USE_REST,    /* 31 - the REST command failed */
		CURLE_OBSOLETE32,              /* 32 - NOT USED */
		CURLE_RANGE_ERROR,             /* 33 - RANGE "command" didn't work */
		CURLE_HTTP_POST_ERROR,         /* 34 */
		CURLE_SSL_CONNECT_ERROR,       /* 35 - wrong when connecting with SSL */
		CURLE_BAD_DOWNLOAD_RESUME,     /* 36 - couldn't resume download */
		CURLE_FILE_COULDNT_READ_FILE,  /* 37 */
		CURLE_LDAP_CANNOT_BIND,        /* 38 */
		CURLE_LDAP_SEARCH_FAILED,      /* 39 */
		CURLE_OBSOLETE40,              /* 40 - NOT USED */
		CURLE_FUNCTION_NOT_FOUND,      /* 41 - NOT USED starting with 7.53.0 */
		CURLE_ABORTED_BY_CALLBACK,     /* 42 */
		CURLE_BAD_FUNCTION_ARGUMENT,   /* 43 */
		CURLE_OBSOLETE44,              /* 44 - NOT USED */
		CURLE_INTERFACE_FAILED,        /* 45 - CURLOPT_INTERFACE failed */
		CURLE_OBSOLETE46,              /* 46 - NOT USED */
		CURLE_TOO_MANY_REDIRECTS,      /* 47 - catch endless re-direct loops */
		CURLE_UNKNOWN_OPTION,          /* 48 - User specified an unknown option */
		CURLE_SETOPT_OPTION_SYNTAX,    /* 49 - Malformed setopt option */
		CURLE_OBSOLETE50,              /* 50 - NOT USED */
		CURLE_OBSOLETE51,              /* 51 - NOT USED */
		CURLE_GOT_NOTHING,             /* 52 - when this is a specific error */
		CURLE_SSL_ENGINE_NOTFOUND,     /* 53 - SSL crypto engine not found */
		CURLE_SSL_ENGINE_SETFAILED,    /* 54 - can not set SSL crypto engine as
		                            default */
		CURLE_SEND_ERROR,              /* 55 - failed sending network data */
		CURLE_RECV_ERROR,              /* 56 - failure in receiving network data */
		CURLE_OBSOLETE57,              /* 57 - NOT IN USE */
		CURLE_SSL_CERTPROBLEM,         /* 58 - problem with the local certificate */
		CURLE_SSL_CIPHER,              /* 59 - couldn't use specified cipher */
		CURLE_PEER_FAILED_VERIFICATION, /* 60 - peer's certificate or fingerprint
		                             wasn't verified fine */
		CURLE_BAD_CONTENT_ENCODING,    /* 61 - Unrecognized/bad encoding */
		CURLE_OBSOLETE62,              /* 62 - NOT IN USE since 7.82.0 */
		CURLE_FILESIZE_EXCEEDED,       /* 63 - Maximum file size exceeded */
		CURLE_USE_SSL_FAILED,          /* 64 - Requested FTP SSL level failed */
		CURLE_SEND_FAIL_REWIND,        /* 65 - Sending the data requires a rewind
		                            that failed */
		CURLE_SSL_ENGINE_INITFAILED,   /* 66 - failed to initialise ENGINE */
		CURLE_LOGIN_DENIED,            /* 67 - user, password or similar was not
		                            accepted and we failed to login */
		CURLE_TFTP_NOTFOUND,           /* 68 - file not found on server */
		CURLE_TFTP_PERM,               /* 69 - permission problem on server */
		CURLE_REMOTE_DISK_FULL,        /* 70 - out of disk space on server */
		CURLE_TFTP_ILLEGAL,            /* 71 - Illegal TFTP operation */
		CURLE_TFTP_UNKNOWNID,          /* 72 - Unknown transfer ID */
		CURLE_REMOTE_FILE_EXISTS,      /* 73 - File already exists */
		CURLE_TFTP_NOSUCHUSER,         /* 74 - No such user */
		CURLE_CONV_FAILED,             /* 75 - conversion failed */
		CURLE_OBSOLETE76,              /* 76 - NOT IN USE since 7.82.0 */
		CURLE_SSL_CACERT_BADFILE,      /* 77 - could not load CACERT file, missing
		                            or wrong format */
		CURLE_REMOTE_FILE_NOT_FOUND,   /* 78 - remote file not found */
		CURLE_SSH,                     /* 79 - error from the SSH layer, somewhat
		                            generic so the error message will be of
		                            interest when this has happened */
		
		CURLE_SSL_SHUTDOWN_FAILED,     /* 80 - Failed to shut down the SSL
		                            connection */
		CURLE_AGAIN,                   /* 81 - socket is not ready for send/recv,
		                            wait till it's ready and try again (Added
		                            in 7.18.2) */
		CURLE_SSL_CRL_BADFILE,         /* 82 - could not load CRL file, missing or
		                            wrong format (Added in 7.19.0) */
		CURLE_SSL_ISSUER_ERROR,        /* 83 - Issuer check failed.  (Added in
		                            7.19.0) */
		CURLE_FTP_PRET_FAILED,         /* 84 - a PRET command failed */
		CURLE_RTSP_CSEQ_ERROR,         /* 85 - mismatch of RTSP CSeq numbers */
		CURLE_RTSP_SESSION_ERROR,      /* 86 - mismatch of RTSP Session Ids */
		CURLE_FTP_BAD_FILE_LIST,       /* 87 - unable to parse FTP file list */
		CURLE_CHUNK_FAILED,            /* 88 - chunk callback reported error */
		CURLE_NO_CONNECTION_AVAILABLE, /* 89 - No connection available, the
		                            session will be queued */
		CURLE_SSL_PINNEDPUBKEYNOTMATCH, /* 90 - specified pinned public key did not
		                             match */
		CURLE_SSL_INVALIDCERTSTATUS,   /* 91 - invalid certificate status */
		CURLE_HTTP2_STREAM,            /* 92 - stream error in HTTP/2 framing layer
		                            */
		CURLE_RECURSIVE_API_CALL,      /* 93 - an api function was called from
		                            inside a callback */
		CURLE_AUTH_ERROR,              /* 94 - an authentication function returned an
		                            error */
		CURLE_HTTP3,                   /* 95 - An HTTP/3 layer problem */
		CURLE_QUIC_CONNECT_ERROR,      /* 96 - QUIC connection error */
		CURLE_PROXY,                   /* 97 - proxy handshake error */
		CURLE_SSL_CLIENTCERT,          /* 98 - client-side certificate required */
		CURL_LAST /* never use! */
	}
	public enum CURLMcode {
		CURLM_CALL_MULTI_PERFORM = -1, /* please call curl_multi_perform() or
                                    curl_multi_socket*() soon */
		CURLM_OK,
		CURLM_BAD_HANDLE,      /* the passed-in handle is not a valid CURLM handle */
		CURLM_BAD_EASY_HANDLE, /* an easy handle was not good/valid */
		CURLM_OUT_OF_MEMORY,   /* if you ever get this, you're in deep sh*t */
		CURLM_INTERNAL_ERROR,  /* this is a libcurl bug */
		CURLM_BAD_SOCKET,      /* the passed in socket argument did not match */
		CURLM_UNKNOWN_OPTION,  /* curl_multi_setopt() with unsupported option */
		CURLM_ADDED_ALREADY,   /* an easy handle already added to a multi handle was
				attempted to get added - again */
		CURLM_RECURSIVE_API_CALL, /* an api function was called from inside a
				callback */
		CURLM_WAKEUP_FAILURE,  /* wakeup is unavailable or failed */
		CURLM_BAD_FUNCTION_ARGUMENT, /* function called with a bad parameter */
		CURLM_ABORTED_BY_CALLBACK,
		CURLM_LAST
	}
	
	public struct CURLMsg {
		public CURLMSG msg;       /* what this message means */
		public IntPtr easy_handle; /* the handle it concerns */
		public curlMsgData data;
	}
	public enum CURLMSG {
		CURLMSG_NONE, /* first, not used */
		CURLMSG_DONE, /* This easy handle has completed. 'result' contains
		   	        the CURLcode of the transfer */
		CURLMSG_LAST /* last, not used */
	}
	public struct curlMsgData {
		public IntPtr whatever;    /* message-specific data */
		public CURLcode result;   /* return code for transfer */
	}
	
		
}
