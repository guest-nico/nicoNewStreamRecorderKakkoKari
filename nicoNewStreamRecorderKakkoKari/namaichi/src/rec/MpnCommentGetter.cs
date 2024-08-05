/*
 * Created by SharpDevelop.
 * User: ajkkh
 * Date: 2024/08/05
 * Time: 17:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using namaichi.info;
using namaichi.utility;
using Newtonsoft.Json;
using ProtoBuf;

namespace namaichi.rec
{
	/// <summary>
	/// Description of MpnCommentGetter.
	/// </summary>
	public class MpnCommentGetter
	{
		RecordingManager rm;
		RecordFromUrl rfu;
		WebSocketRecorder wr;
		
		string mpnViewUri = null;
		int vposBaseTimeUnix = 0;
		string mpnHashedUserId = null;
		
		public MpnCommentGetter(RecordingManager rm, RecordFromUrl rfu, WebSocketRecorder wr)
		{
			this.rm = rm;
			this.rfu = rfu;
			this.wr = wr;
		}
		public void get(string wsMsg) {
			util.debugWriteLine("mpn comment getter get " + wsMsg);
			if (!setMpnInfo(wsMsg)) return;
			
			wr.commentFileInit();
			if (wr.ri.si.isTimeShift && !wr.ri.isRealtimeChase) {
				//if (tscg == null) {
				if (false) {
					if (!(wr.ri.isChase && wr.chaseCommentBuf == null)) {
						/*
						tscg = new TimeShiftCommentGetter(msUri, msThread, msStoreUri, msStoreThread,                                  
								ri.userId, rm, rfu, rm.form, ri,
								ri.si.lvid, container,
								//ri.si.type, 
								this,
								//(ri.isRtmp) ? 0 : ri.timeShiftConfig.timeSeconds, 
								//(ri.isRtmp) ? false : ri.timeShiftConfig.isVposStartTime, 
								ri.isRtmp, rr, roomName, ri.timeShiftConfig,
								null, true, false, null, null);
						 tscg.save();
						 */
					} else {
						util.debugWriteLine("not tscg ischase commentbuf null");
						#if DEBUG
							rm.form.addLogText("not tscg ischase commentbuf null");
						#endif
					}
				}
				
			}
			if (!wr.ri.si.isTimeShift || wr.ri.isChase) {
				if (rfu == rm.rfu && wr.IsRetry) {
					//connectMessageServer();
					connectMessageServerMpn();
				}
			}
		}
		public bool setMpnInfo(string message) {
			mpnViewUri = util.getRegGroup(message, "\"viewUri\":\"(.+?)\"");
			if (mpnViewUri == null) {
				rm.form.addLogText("view uriが見つかりませんでした");
				return false;
			}
			var _vposBaseTime = util.getRegGroup(message, "\"vposBaseTime\":\"(.+?)\"");
			if (_vposBaseTime == null) {
				rm.form.addLogText("vposBaseTimeが見つかりませんでした");
				//return false;
				_vposBaseTime = wr.ri.si.vposBaseTime.ToString();
				util.debugWriteLine("not found vposBaseTime mpn");
			}
			vposBaseTimeUnix = util.getUnixTime(DateTime.Parse(_vposBaseTime));
			
			mpnHashedUserId = util.getRegGroup(message, "\"hashedUserId\":\"(.+?)\"");
			if (mpnHashedUserId == null) {
				util.debugWriteLine("not found hashedUserId mpn");
			}
			wr.serverTime = util.getUnixTime() - 3600 * 9;
			return true;
		}
		private void connectMessageServerMpn() {
	    	wr.addDebugBuf("connect message server mpn");
	    	wr.addDebugBuf("isretry " + wr.IsRetry + " isend " + wr.isEndProgram);
	    	try {
	    		
	    		
	    		viewProtoProcess();
	    		
	    		
	    	} catch (Exception e) {
	    		util.debugWriteLine("connectMessageServerMpn error " + e.Message + e.Source + e.StackTrace);
	    	}
		}
		private List<T> getProtoUri<T>(string uri) {
			byte[] r = null;
    		for (var i = 0; i < 3; i++) {
    			 r = util.getFileBytes(uri, null);
    			 if (r != null) continue;
    		}
    		if (r == null) {
    			rm.form.addLogText("コメント情報のuriに接続できませんでした " + uri);
    			return null;
    		}
			var protoList = getDataToProtoList<T>(r, typeof(T));
    		return protoList;
		}
		private List<T> getDataToProtoList<T>(byte[] b, Type t) {
			var protoList = new List<T>();
			
			var rList = new List<byte>(b);
			for (var i = 0; i < b.Length; i++) {
				var dataLen = 0;
				var len = VarintBitConverter.ToUInt64(rList.ToArray(), out dataLen);
				
				try {
	    			rList.AddRange(b);
	    			util.debugWriteLine("view res len " + b.Length);
					
					var rrr = rList.GetRange(dataLen, (int)len);
					var _ms = new MemoryStream(rrr.ToArray());
					var cee = Serializer.Deserialize<T>(_ms);
					util.debugWriteLine("proto deserialize " + cee);
					
					i += dataLen + (int)len;
					rList.RemoveRange(0, dataLen + (int)len);
					protoList.Add(cee);
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + " " + t.FullName);
				}
			}
			return protoList;
		}
		void viewProtoProcess() {
			string at = "now";
			while (rm.rfu == rfu && wr.IsRetry) {
				try {
					var entryList = getProtoUri<ChunkedEntry>(mpnViewUri + "?at=" + at);
		    		if (entryList == null || entryList.Count == 0) {
		    			rm.form.addLogText("コメントのview uriから正常に情報を取得できませんでした");
		    			return;
		    		}
					
					foreach (var ce in entryList) {
						if (ce.Next != null) {
							at = ce.Next.At.ToString();
							util.debugWriteLine("view at change " + at);
						}
						if (ce.Previous != null) {
							messageSegmentProcess(ce.Previous.Uri);
						}
					}
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				}
				Thread.Sleep(1000);
			}
		}
		void messageSegmentProcess(string segmentUri) {
			var segList = getProtoUri<ChunkedMessage>(segmentUri);
			util.debugWriteLine(segList);
			foreach (var cm in segList) {
				if (cm.Message != null) {
					//var msg = cm.Message;
					//var calcVpos = cm.meta.At.Seconds - vposBaseTimeUnix + 32400;
					var chatXml = getMsgProtoToXML(cm);
					var json = JsonConvert.SerializeXNode(chatXml);
					json = json.Replace("\"#text\"", "\"content\"");
					wr.onCommentMessageReceiveCore(json);
					
				}
			}
		}
		XDocument getMsgProtoToXML(ChunkedMessage msgProto) {
			var _xml = new XDocument();
			_xml.Add(new XElement("chat"));
			try {
				if (msgProto.Message.Chat != null) {
					var chat = msgProto.Message.Chat;
					var content = chat.Content != null ? chat.Content : "";
					var premium = ((int)chat.account_status).ToString();
					var modifier = chat.modifier != null ? getModifier(chat.modifier) : "";
					_xml = getXDocument(content, premium,
					        chat.HashedUserId, modifier, chat.Name,
					        chat.No.ToString(), chat.RawUserId.ToString(), 
					        chat.Vpos.ToString(), msgProto.meta.At.Seconds,
					        msgProto.meta.At.Nanos);
				} else if (msgProto.Message.SimpleNotification != null) {
					var vpos = msgProto.meta.At.Seconds * 100;
					var notification = getNotification(msgProto.Message.SimpleNotification);
					_xml = getXDocument(notification, "3",
							"", "", "",
							"", "", vpos.ToString(), 
							msgProto.meta.At.Seconds, 
							msgProto.meta.At.Nanos);
				}
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				return null;
			}
			util.debugWriteLine(_xml);
			return _xml;
			
		}
		XDocument getXDocument(string content, string premium, 
				string userId, string modifier, string name, string no,
				string raw_user_id, string vpos, long date, int date_usec) {
			var _xml = new XDocument();
			_xml.Add(new XElement("chat"));
			
			if (string.IsNullOrEmpty(userId) && 
			    	!string.IsNullOrEmpty(raw_user_id)) {
				userId = raw_user_id;
				raw_user_id = "";
			}
			_xml.Root.Add(content);
			if (!string.IsNullOrEmpty(premium))
				_xml.Root.SetAttributeValue("premium", premium);
			if (!string.IsNullOrEmpty(userId) && userId != "0")
				_xml.Root.SetAttributeValue("user_id", userId);
			if (!string.IsNullOrEmpty(modifier))
				_xml.Root.SetAttributeValue("modifier", modifier);
			if (!string.IsNullOrEmpty(name))
				_xml.Root.SetAttributeValue("name", name);
			if (!string.IsNullOrEmpty(no))
				_xml.Root.SetAttributeValue("no", no);
			if (!string.IsNullOrEmpty(raw_user_id) && raw_user_id != "0")
				_xml.Root.SetAttributeValue("raw_user_id", raw_user_id);
			if (!string.IsNullOrEmpty(vpos))
				_xml.Root.SetAttributeValue("vpos", vpos);
			_xml.Root.SetAttributeValue("date", date.ToString());
			_xml.Root.SetAttributeValue("date_usec", date_usec.ToString());
			return _xml;
		}
		string getModifier(Chat.Modifier modifier) {
			var m = new List<string>();
			if (modifier.font.ToString() != "Defont") m.Add(modifier.font.ToString());
			if (modifier.full_color != null) m.Add(modifier.full_color.ToString());
			if (modifier.NamedColor.ToString() != "White") m.Add(modifier.NamedColor.ToString());
			if (modifier.opacity.ToString() != "Normal") m.Add(modifier.opacity.ToString());
			if (modifier.Position.ToString() != "Naka") m.Add(modifier.Position.ToString());
			if (modifier.size.ToString() != "Medium") m.Add(modifier.size.ToString());
			return string.Join(" ", m.ToArray());
		}
		string getNotification(SimpleNotification n) {
			var m = new List<string>();
			if (n.Cruise != null) m.Add(n.Cruise);
			if (n.Emotion != null) m.Add(n.Emotion);
			if (n.Ichiba != null) m.Add(n.Ichiba);
			if (n.ProgramExtended != null) m.Add(n.ProgramExtended);
			if (n.Quote != null) m.Add(n.Quote);
			if (n.RankingIn != null) m.Add(n.RankingIn);
			if (n.RankingUpdated != null) m.Add(n.RankingUpdated);
			if (n.Visited != null) m.Add(n.Visited);
			return string.Join(" ", m.ToArray());
		}
	}
}
