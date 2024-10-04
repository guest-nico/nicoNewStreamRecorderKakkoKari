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
using System.Linq;
using System.Net;
using System.Text;
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
		List<string> gotCommentIDList = new List<string>();
		List<KeyValuePair<double, string>> gotPastCommentBuf = null;
		List<KeyValuePair<double, string>> gotRealTimeCommentBuf = null;
		List<string> gotBackwardUrl = null;
		bool isPastCommentMode = false;
		
		string at = "now";
		public bool isEnd = false;
		object messageLock = new object();
		
		public MpnCommentGetter(RecordingManager rm, RecordFromUrl rfu, WebSocketRecorder wr)
		{
			this.rm = rm;
			this.rfu = rfu;
			this.wr = wr;
			
			if (wr.ri.si.isTimeShift && !wr.ri.isRealtimeChase) {
				gotPastCommentBuf = new List<KeyValuePair<double, string>>();
				gotRealTimeCommentBuf = new List<KeyValuePair<double, string>>();
				gotBackwardUrl = new List<string>();
			}
		}
		public void get(string wsMsg) {
			util.debugWriteLine("mpn comment getter get " + wsMsg);
			if (!setMpnInfo(wsMsg)) return;
			
			wr.commentFileInit();
			
			if (rfu == rm.rfu && wr.IsRetry) {
				wr.addDebugBuf("connect message server mpn");
		    	wr.addDebugBuf("isretry " + wr.IsRetry + " isend " + wr.isEndProgram);
		    	try {
		    		viewProtoProcess();
		    	} catch (Exception e) {
		    		util.debugWriteLine("connectMessageServerMpn error " + e.Message + e.Source + e.StackTrace);
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
		private bool receiveFromProtoUri<T>(string uri, bool isUsingStream) {
			byte[] r = new Byte[1000000];
			var ms = new MemoryStream();
			
			if (isUsingStream) {
				HttpWebRequest req = null;
				Exception webReqE = null;
				
				try {
					HttpWebResponse res = null;
					for (var i = 0; i < 3; i++) {
						res = util.sendRequest(uri, null, null, "GET", null, out req, out webReqE);
						if (res == null) {
							Thread.Sleep(1000);
							continue;
						}
						if (i != 0) {
							util.debugWriteLine("get after retry uri " + uri);
						}
						break;
					}
					if (res == null) {
						util.debugWriteLine("not get after retry uri " + uri);
		    			rm.form.addLogText("コメント情報のuriに接続できませんでした。" + uri);
		    			if (webReqE != null) {
		    				rm.form.addLogText("webReqE error " + webReqE.Message + " " + webReqE.Source + webReqE.StackTrace);
		    				util.debugWriteLine("webReqE error " + webReqE.Message + " " + webReqE.Source + webReqE.StackTrace);
		    			}
		    			return false;
		    		}
					
					using (var rs = res.GetResponseStream()) {
						var readNum = rs.Read(r, 0, r.Length);
						while (readNum != 0) {
							ms.Write(r, 0, readNum);
							ms.Flush();
							
							var a = ms.ToArray();
							var protoList = getDataToProtoList<T>(ms.ToArray(), typeof(T));
							if (protoList != null && protoList.Count > 0) {
								passToHandler<T>(protoList);
								ms.SetLength(0);
							}
							
							readNum = rs.Read(r, 0, r.Length);
						}
					}
				} catch (Exception e) {
					util.debugWriteLine("protoUri stream read error " + uri + " " + e.Message + e.Source + e.StackTrace);
				} finally {
					try {
						if (req != null) req.Abort();
					} catch (Exception e) {
						util.debugWriteLine(e.Message + e.Source + e.StackTrace);
					}
				}
			} else {
				var beforeT = DateTime.Now;
				var res = util.getFileBytes(uri, null);
				var afterT = DateTime.Now;
				if (res == null) {
					return false;
				}
				var protoList = getDataToProtoList<T>(res, typeof(T));
				if (protoList != null && protoList.Count > 0) {
					passToHandler(protoList);
				}
			}
    		
			return true;
		}
		void passToHandler<T>(List<T> protoList) {
			if (protoList is List<ChunkedEntry>)
				onChunkedEntryReceived(protoList as List<ChunkedEntry>);
			else if (protoList is List<ChunkedMessage>)
				onChunkedMessageReceived(protoList as List<ChunkedMessage>);
			else if (protoList is List<PackedSegment>)
				onPackedSegmentReceived(protoList as List<PackedSegment>);
		}
		private List<T> getDataToProtoList<T>(byte[] b, Type t) {
			var protoList = new List<T>();
			
			var rList = new List<byte>(b);
			var dataLen = 0;
			ulong len;
			List<byte> rrr = null;
			for (var i = 0; i < b.Length && rList.Count != 0; i++) {
				dataLen = 0;
				len = 0;
				try {
					if (t == typeof(PackedSegment)) {
						dataLen = 0;
						len = (ulong)b.Length;
					} else {
						len = VarintBitConverter.ToUInt64(rList.ToArray(), out dataLen);
						if (len == 0) break;
					}
					
	    			util.debugWriteLine("view res len " + b.Length + " rlistLen " + rList.Count + " datalen " + dataLen + " len " + len);
					
					rrr = rList.GetRange(dataLen, (int)len);
					var _ms = new MemoryStream(rrr.ToArray());
					
					var cee = Serializer.Deserialize<T>(_ms);
					
					if (t == typeof(ChunkedMessage)) {
						util.debugWriteLine("des ok " + cee);
						
						rrr = rList.GetRange(dataLen, (int)len);
						_ms = new MemoryStream(rrr.ToArray());
					}
					
					i += dataLen + (int)len;
					rList.RemoveRange(0, dataLen + (int)len);
					protoList.Add(cee);
					
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace + " " + t.FullName);
					if (dataLen != 0 || len != 0) {
						i += dataLen + (int)len;
						rList.RemoveRange(0, dataLen + (int)len);
					}
				}
			}
			if (t == typeof(ChunkedEntry)) {
				util.debugWriteLine("entry ret ");
			}
			return protoList;
		}
		void viewProtoProcess() {
			while (rm.rfu == rfu && wr.IsRetry && !isEnd) {
				var isOk = false;
				for (var i = 0; i < 5; i++) {
					isOk = receiveFromProtoUri<ChunkedEntry>(mpnViewUri + "?at=" + at, i == 0);
					if (!isOk) {
						Thread.Sleep(1000);
						continue;
					}
					break;
				}
				if (!isOk) {
					rm.form.addLogText("コメントのView Uriから正常に情報を取得できませんでした " + mpnViewUri + "?at=" + at);
	    			Thread.Sleep(1000);
	    			at = "now";
	    			continue;
	    		}
			}
		}
		void onChunkedEntryReceived(List<ChunkedEntry> l) {
			foreach (var ce in l) {
				if (ce.Next != null) {
					var newAt = ce.Next.At.ToString();
					if (at == newAt) Thread.Sleep(1000);
					at = newAt;
					util.debugWriteLine("view at change " + at);
				}
				if (ce.Segment != null) {
					messageSegmentProcess<ChunkedMessage>(ce.Segment.Uri);
				}
				if (ce.Previous != null && gotCommentIDList.Count == 0) {
					messageSegmentProcess<ChunkedMessage>(ce.Previous.Uri);
				}
				if (ce.Backward != null && gotPastCommentBuf != null && !isPastCommentMode) {
					isPastCommentMode = true;
					gotBackwardUrl.Add(ce.Backward.Segment.Uri);
					messageSegmentProcess<PackedSegment>(ce.Backward.Segment.Uri);
					saveCommentBuf();
				}
				
			}
		}
		void onChunkedMessageReceived(List<ChunkedMessage> l) {
			foreach (var cm in l) {
				lock(messageLock) {
					if (cm.Message != null || cm.State != null) {
						if (gotCommentIDList.IndexOf(cm.meta.Id) > -1 &&
						    	!string.IsNullOrEmpty(cm.meta.Id))
						    continue;
						
						var chatXml = getMsgProtoToXML(cm);
						if (chatXml == null) 
							continue;
						var json = JsonConvert.SerializeXNode(chatXml);
						json = json.Replace("\"#text\"", "\"content\"")
								.Replace("\\u0000", "")
								.Replace("\\u0010", "");
						
						if (gotRealTimeCommentBuf != null)
							addCommentBuf(cm.meta.At, json, gotRealTimeCommentBuf);
						else wr.onCommentMessageReceiveCore(json, true);
						gotCommentIDList.Add(cm.meta.Id);
						
						var isTimeshiftRec = wr.ri.si.isTimeShift && !wr.ri.isChase; 
						if (isTimeshiftRec && gotCommentIDList.Count % 1000 == 0)
							rm.form.addLogText(gotCommentIDList.Count + "件のコメントを取得しました");
					}
				}
			}
		}
		void onPackedSegmentReceived(List<PackedSegment> l) {
			var isWrite = gotBackwardUrl.IndexOf("end") > -1;
			
			foreach (var ps in l) {
				if (ps.Messages != null && isWrite) {
					foreach (var cm in ps.Messages) {
						try {
							if (cm.Message != null && cm.Message.Chat != null && cm.Message.Chat.No == 161250)
								util.debugWriteLine("aaa");
							if (wr.ri.isChase) {
							if (gotCommentIDList.IndexOf(cm.meta.Id) > -1 &&
							    	!string.IsNullOrEmpty(cm.meta.Id))
							    continue;
							}
							
							var chatXml = getMsgProtoToXML(cm);
							if (chatXml == null)
								continue;
							var json = JsonConvert.SerializeXNode(chatXml);
							json = json.Replace("\"#text\"", "\"content\"")
									.Replace("\\u0000", "")
									.Replace("\\u0010", "");
							
							//addCommentBuf(cm.meta.At, json, gotPastCommentBuf);
							wr.onCommentMessageReceiveCore(json, false);
							gotCommentIDList.Add(cm.meta.Id);
							
							if (gotCommentIDList.Count % 1000 == 0)
								rm.form.addLogText(gotCommentIDList.Count + "件のコメントを取得しました");
						} catch (Exception e) {
							util.debugWriteLine(e.Message + e.Source + e.StackTrace);
						}
					}
				}
				if (ps.next != null) {
					if (!isWrite) {
						gotBackwardUrl.Add(ps.next.Uri);
						receiveFromProtoUri<PackedSegment>(ps.next.Uri, false);
					}
				}
			}
			if (l.FirstOrDefault(x => x.next != null) == null && 
			    	gotBackwardUrl.IndexOf("end") == -1)
				gotBackwardUrl.Add("end");
		}
		void messageSegmentProcess<T>(string segmentUri) {
			var isOk = false;
			for (var i = 0; i < 5; i++) {
				isOk = receiveFromProtoUri<T>(segmentUri, i == 0 && segmentUri.IndexOf("/backward") == -1);
				if (!isOk) {
					Thread.Sleep(1000);
					continue;
				}
				break;
			}
			util.debugWriteLine("messageSegmentProcess " + isOk);
			if (!isOk) {
    			rm.form.addLogText("コメントのSegment Uriから正常に情報を取得できませんでした " + segmentUri);
    			at = "now";
    			return;
    		}
		}
		XDocument getMsgProtoToXML(ChunkedMessage msgProto) {
			var _xml = new XDocument();
			_xml.Add(new XElement("chat"));
			try {
				if (msgProto.Message != null && 
				    	(msgProto.Message.Chat != null || msgProto.Message.OverflowedChat != null)) {
					var msg = msgProto.Message;
					var chat = msg.Chat != null ? 
							msg.Chat : msg.OverflowedChat;
					var content = chat.Content != null ? chat.Content : "";
					var premium = ((int)chat.account_status).ToString();
					var modifier = chat.modifier != null ? getModifier(chat.modifier) : "";
					msgProto.Message.Chat.Vpos -= ((int)wr.ri.si._openTime - vposBaseTimeUnix + 32400) * 100;
					
					_xml = getXDocument(content, premium,
					        chat.HashedUserId, modifier, chat.Name,
					        chat.No.ToString(), chat.RawUserId.ToString(), 
					        chat.Vpos.ToString(), msgProto.meta.At.Seconds,
					        msgProto.meta.At.Nanos);
				} else {
					var vpos = (msgProto.meta.At.Seconds - wr.ri.si._openTime) * 100;
					var content = "undefined";
					if (msgProto.Message != null && msgProto.Message.SimpleNotification != null) {
						content = getNotification(msgProto.Message.SimpleNotification);
					} if (msgProto.State != null) {
						content = getStateComment(msgProto.State);
					} else if (msgProto.Message != null && 
				           msgProto.Message.Nicoad != null) {
						content = getNicoAd(msgProto.Message.Nicoad);
					} else if (msgProto.Message != null && 
					           msgProto.Message.Gift != null) {
						content = getGift(msgProto.Message.Gift);
					} else if (msgProto.Message != null && 
					           msgProto.Message.GameUpdate != null) {
						content = "Game Update";
					} else if (msgProto.Message != null &&
					           msgProto.Message.ModeratorUpdated != null) {
						content = getModeratorUpdated(msgProto.Message.ModeratorUpdated);
					} else if (msgProto.Message != null &&
					           msgProto.Message.SsngUpdated != null) {
						content = getSSNGUpdated(msgProto.Message.SsngUpdated);
					} else if (msgProto.Message != null &&
					           msgProto.Message.TagUpdated != null) {
						content = getTagUpdated(msgProto.Message.TagUpdated);
					}
					_xml = getXDocument(content, "3",
							"", "", "",
							"", "", vpos.ToString(), 
							msgProto.meta.At.Seconds, 
							msgProto.meta.At.Nanos);
				} 
				
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
				return null;
			}
			if (!isPastCommentMode)
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
				_xml.Root.SetAttributeValue("mail", modifier.ToLower());
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
			if (modifier.full_color != null) {
				var color = modifier.full_color;
				m.Add("#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"));
			}
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
		string getStateComment(NicoliveState s) {
			var l = new List<string>();
			if (s.Marquee != null) {
				if (s.Marquee.display != null && 
				    	s.Marquee.display.OperatorComment != null) {
					var opComment = s.Marquee.display.OperatorComment;
					l.Add(getOperatorCommentStr(opComment));
				}
				if (s.Marquee.display == null)
					l.Add("Hide Marquee");
			}
			if (s.Enquete != null) {
				if (!string.IsNullOrEmpty(s.Enquete.Question)) {
					if (s.Enquete.status.ToString() == "Poll")
						l.Add(s.Enquete.Question);
					l.Add(string.Join(" ", s.Enquete.Choices.Select(x => "[" + x.Description + (s.Enquete.status.ToString() == "Result" ? (" " + (x.PerMille / 10.0) + "%") : "") + "]").ToArray()));
					if (!isPastCommentMode)
						util.debugWriteLine(l);
				}
				if(s.Enquete.status.ToString() == "Closed") l.Add("Enquete");
				l.Add(s.Enquete.status.ToString());
				
			}
			if (s.ProgramStatus != null) {
				l.Add(s.ProgramStatus.state.ToString());
			}
			if (s.CommentLock != null) {
				l.Add(s.CommentLock.status.ToString());
			}
			if (s.CommentMode != null) {
				l.Add(s.CommentMode.layout.ToString());
			}
			if (s.ModerationAnnouncement != null)
				l.Add(s.ModerationAnnouncement.Message);
			if (s.MoveOrder != null) {
				var m = s.MoveOrder;
				if (m.Jump != null)
					l.Add(m.Jump.Message + " " + m.Jump.Content);
				if (m.Redirect != null)
					l.Add(m.Redirect.Message + " " + m.Redirect.Uri);
			}
			if (s.Statistics != null) {
				var st = s.Statistics;
				//l.Add("来場者:" + st.Viewers);
				//l.Add("コメント:" + st.Comments);
				if (st.AdPoints != 0) l.Add("広告:" + st.AdPoints);
				if (st.GiftPoints != 0) l.Add("ギフト:" + st.GiftPoints);
			}
			if (s.TrialPanel != null) {
				l.Add(s.TrialPanel.panel.ToString());
				l.Add(s.TrialPanel.UnqualifiedUser.ToString());
			}
			return string.Join(" ", l.ToArray());
		}
		string getOperatorCommentStr(OperatorComment c) {
			var m = new List<string>();
			if (!string.IsNullOrEmpty(c.Name)) m.Add(c.Name);
			if (!string.IsNullOrEmpty(c.Content)) m.Add(c.Content);
			if (!string.IsNullOrEmpty(c.Link)) m.Add("(" + c.Link + ")");
			if (c.Modifier != null) m.Add(getModifier(c.Modifier));
			return string.Join(" ", m.ToArray());
		}
		string getNicoAd(Nicoad n) {
			var m = new List<string>();
			if (n.v0 != null) m.Add(n.v0.latest.Message + " Total:" + n.v0.TotalPoint);
			if (n.v1 != null) m.Add(n.v1.Message + " Total:" + n.v1.TotalAdPoint.ToString());
			return string.Join(" ", m.ToArray());
		}
		string getGift(Gift n) {
			var m = new List<string>();
			if (n.ContributionRank != 0) m.Add("【ギフト貢献" + n.ContributionRank + "位】");
			if (n.AdvertiserName != null) {
				m.Add(n.AdvertiserName);
				if (n.AdvertiserUserId != 0) m.Add("(" + n.AdvertiserUserId + ")");
				m.Add("さんが");
			}
			if (n.ItemName != null) m.Add("ギフト「" + n.ItemName + " ID:" + n.ItemId + "(" + n.Point + "pt)」を贈りました");
			return string.Join(" ", m.ToArray());
		}
		string getModeratorUpdated(ModeratorUpdated n) {
			var m = new List<string>();
			m.Add("Moderator " + n.Operation.ToString());
			if (n.Operator != null && n.Operator.Nickname != null) m.Add(n.Operator.Nickname);
			if (n.Operator != null && n.Operator.UserId != 0) m.Add(n.Operator.UserId.ToString());
			return string.Join(" ", m.ToArray());
		}
		string getSSNGUpdated(SSNGUpdated n) {
			var m = new List<string>();
			m.Add("NG updated");
			m.Add(n.Operation.ToString());
			if (n.Operator != null) m.Add(n.Operator.Nickname + " " + n.Operator.UserId);
			if (n.Source != null) m.Add(n.Source);
			if (n.SsngId != 0) m.Add("NGID:" + n.SsngId);
			m.Add(n.Type.ToString());
			return string.Join(" ", m.ToArray());
		}
		string getTagUpdated(TagUpdated n) {
			var m = new List<string>();
			m.Add("タグが更新されました");
			if (n.OwnerLocked) m.Add("ロック");
			m.Add(string.Join(",", n.Tags.Select(x => x.Text).ToArray()));
			return string.Join(" ", m.ToArray());
		}
		void saveCommentBuf() {
			rm.form.addLogText("コメントの保存を開始します");
			util.debugWriteLine("sumPastComment " + (gotPastCommentBuf != null ? gotPastCommentBuf.Count.ToString() : "null"));
			try {
				gotPastCommentBuf = gotPastCommentBuf.OrderBy(x => x.Key).ToList();
			} catch (Exception e) {
				util.debugWriteLine(e.Message + e.Source + e.StackTrace);
			}
			saveFromBackwardUrlList();
			//saveCommentBufCore(gotPastCommentBuf);
			gotPastCommentBuf = null;
			gotBackwardUrl = null;
			
			saveCommentBufCore(gotRealTimeCommentBuf);
			gotRealTimeCommentBuf= null;
			isPastCommentMode = false;
			
			if (wr.ri.si.isTimeShift && !wr.ri.isChase) {
				isEnd = true;
				wr.closeWscProcess();
				rm.form.addLogText("コメントの保存を完了しました");
				
			}
		}
		void saveCommentBufCore(List<KeyValuePair<double, string>> l) {
			while (l != null && 
			       l.Count != 0) {
				try {
					var c = l[0];
					wr.onCommentMessageReceiveCore(c.Value, false);
					l.Remove(c);
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace);
					Thread.Sleep(1000);
				}
			}
		}
		void addCommentBuf(Timestamp at, string json, List<KeyValuePair<double, string>> l) {
			var seconds = at.Seconds;
			var nano = double.Parse("0." + at.Nanos);
			l.Add(new KeyValuePair<double, string>(
					seconds + nano, json));
		}
		void saveFromBackwardUrlList() {
			var l = gotBackwardUrl;
			var a = gotBackwardUrl.Count;
			while (l != null && 
			       !(l.Count == 0 || 
			         l[0] == "end") && rm.rfu == rfu) {
				
				try {
					var c = l[l.Count - 2];
					if (c == "end") continue;
					
					if (c != null) 
						receiveFromProtoUri<PackedSegment>(c, false);
					l.Remove(c);
				} catch (Exception e) {
					util.debugWriteLine(e.Message + e.Source + e.StackTrace);
					Thread.Sleep(1000);
				}
			}
			util.debugWriteLine("saveFromBackwardUrlList end");
		}
	}
}
