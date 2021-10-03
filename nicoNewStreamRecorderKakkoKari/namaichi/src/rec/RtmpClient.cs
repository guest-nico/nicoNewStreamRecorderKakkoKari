/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/11/30
 * Time: 9:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
/*
using System;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using RtmpSharp2.Abstract;
using RtmpSharp2.Abstract.CommandMessages;

namespace namaichi.rec
{
	/// <summary>
	/// Description of RtmpClient.
	/// </summary>
	public class RtmpClient : RtmpSharp2.Abstract.ClientBase
	{
		private TcpClient client;
        private bool _connect = true;
        //private bool _sendToken = true;
        private string url;
        private string que;
        private string ticket;
//        private TwitchUsher _usher;
		private RecordingManager rm;

        public RtmpClient(string url, string que, string ticket, RecordingManager rm)
        {
        	var host = getRegGroup(url, "//(.+):");
        	var port = int.Parse(getRegGroup(url, ":(\\d+)"));
            client = new TcpClient(host, port);
            client.Client.Blocking = false;
            client.NoDelay = true;
            client.SendBufferSize = 10000;
            client.ReceiveBufferSize = 10000;
//            _usher = usher;
			this.url = url;
			this.que = que;
			this.ticket = ticket;
			this.rm = rm;
        }

        protected override void SendData(byte[] array)
        {
            client.Client.Send(array);
//            System.Diagnostics.Debug.WriteLine(Encoding.ASCII.GetString(array));
        }

        protected override bool CanReceiveData()
        {
            return client.Available > 0;
        }

        protected override byte[] ReceivedData()
        {
        	
            var buffer = new byte[client.Available];
            var ret = client.Client.Receive(buffer);
            
            return buffer;
        }

        protected override void Debug(string str)
        {
            base.Debug(str);
            util.consoleWrite(str);
            System.Diagnostics.Debug.WriteLine(str);
        }

        public override void Update()
        {
            base.Update();
            if (CurrentState == ClientStates.Handshake_Done)
            {
                if (_connect)
                {
                    _connect = false;
					
                    
                    
                    
                }
            }
        }
        protected override void ParseChunk(Chunk chunk)
        {
            base.ParseChunk(chunk);
            
        }
		/*
        private void sendToken()
        {
        	/*
            if (_usher.Streams.Count == 0)
                return;

            AmfWriter writer = new AmfWriter();

            writer.WriteString("NetStream.Authenticate.UsherToken");
            writer.WriteNumber(0);
            writer.WriteNull();
//            writer.WriteString(_usher.Streams[0].Token);

            RtmpSharp2.Abstract.CommandMessage message = new CommandMessage(writer);

            SendMessage(message);
            SendMessage(new CreateStream());
            SendMessage(new Play(_usher.Streams[0].PlayStream));
            *
        }
        /
       public static string getRegGroup(string target, string reg, int group = 1, Regex r = null) {
			if (r == null)
				 r = new Regex(reg);
			var m = r.Match(target);
	//		util.consoleWrite(m.Groups.Count +""+ m.Groups[0]);
			if (m.Groups.Count>group) {
				return m.Groups[group].ToString();
			} else return null;
		}
		public bool makeTs() {
       		while (true) {
       			try {
		       		StartHandshake();
		       	
		       		while (CurrentState != ClientStates.Handshake_Done) {
						base.Update();       			
		       			System.Threading.Thread.Sleep(500);
		            }
		       		
					SendMessage(new RtmpSharp2.Abstract.ControlMessages.SetChunkSize(10000));
		            var connect = new Connect(ticket);
		            connect.PageUrl = "http://live.nicovideo.jp/nicoliveplayer.swf";
		            connect.SwfUrl = "http://live.nicovideo.jp/nicoliveplayer.swf";
		            /*
		            connect.App = "fileorigin/01";
		            connect.ServerUrl = "rtmp://nlace02.live.nicovideo.jp:1935/fileorigin/01";         
		            connect.Data = Encoding.ASCII.GetBytes("225832:lv316983063:0:1543462925:7c12cb3f6a81c679");
		            /
		           connect.App = getRegGroup(url, "(fileorigin.+)");
		           connect.ServerUrl = url;
		//           connect.Data = Encoding.ASCII.GetBytes(ticket);
		           
		            connect.ApplyValues(ticket);
		            SendMessage(connect);
		            
		            
		            var createStream = new CreateStream();
		            createStream.ApplyValues();
		            SendMessage(createStream);
	       		} catch (Exception e) {
	       			util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
	       		}
	            
				for (var i = 0;; i++) {
	            	try {
		                var cfr = new byte[]{0x02,0x00,0x0f,0x73,0x65,0x6e,0x64,0x46,0x69,0x6c,0x65,0x52,0x65,0x71,0x75,0x65,
								0x73,0x74,0x00,0x40,0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x05,0x11,0x09,0x03,0x01,
								0x06}; //,0x6f
		                var len = (byte)0x6f;
		                //var que = Encoding.ASCII.GetBytes(getRegGroup("/content/20181128/lv316983063_200012298000_1_659aac.f4v");
		                var _que = Encoding.ASCII.GetBytes(que);
		            	var arr = new List<byte>();
		            	arr.AddRange(cfr);
		            	arr.Add(len);
		            	arr.AddRange(_que);
						
		            	var r = new RtmpSharp2.Abstract.CommandMessage(arr.ToArray());
		                
						SendData(r.ToBytes());
						System.Diagnostics.Debug.WriteLine("rtmp client a " + i + " write len " + arr.Count);
						System.Diagnostics.Debug.WriteLine(len + " " + Encoding.ASCII.GetString(_que));
						
						client.Client.Blocking = true;
						client.ReceiveTimeout = 3000;
						client.SendTimeout = 3000;
						var b = new byte[10000];
						Stream s = null;
						var t = Task.Run(() => {s = client.GetStream();});
						if (!t.Wait(2000)) 
							continue;
						
						var ii = s.Read(b, 0, b.Length);
						var ch = new char[b.Length];
						string a = "";
						System.Diagnostics.Debug.WriteLine("rtmp client b " + i);
						for (var iii = 0; iii < ii; iii++) {
							if (b[iii] < 33 || b[iii] > 126) continue;
							
							a += (char)b[iii];
		//					System.Diagnostics.Debug.WriteLine((int)ch[iii]);
		//					System.Diagnostics.Debug.Write(ch[iii]);
						}
						System.Diagnostics.Debug.WriteLine("rtmp client i " + i + " a " + a);
						if (a.IndexOf("error") > -1 && a.IndexOf("/content/") > -1)
						    return false;
							
						if (a.IndexOf("/content/") > -1 && a.IndexOf("error") == -1 && a.IndexOf("Success") == -1) {
							client.Close();
							return true;
						}
							
						if (a == "") {
							client.Close();
							return true;
						}
						System.Diagnostics.Debug.WriteLine("rtmp client c " + i);
						//System.Diagnostics.Debug.WriteLine(Encoding.ASCII.GetString(b));
						if (i == 2) rm.form.addLogText("タイムシフト動画データを取得します...");
						if (i > 1) {
							System.Threading.Thread.Sleep(1000);
						}
	            	} catch (SocketException e) {
	            		util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
	            		return false;
	            	} catch (IOException e) {
	            		util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
	            		return false;
	            	} catch (Exception e) {
	            		util.debugWriteLine(e.Message + e.Source + e.StackTrace + e.TargetSite);
	            		System.Threading.Thread.Sleep(1000);
	            		return false;
	            	}
			}
       	}
       	//return false;
	}
}
	public class Connect : CommandMessage
    {
        public string App = "app";
        public string FlashVersion = "FMSc/1.0";
        public string SwfUrl = "NONE";
        public string ServerUrl = "NONE";
        public bool Proxy = false;
        public int AudioCodecs = 0x0FFF;
        public int VideoCodecs = 0x00FF;
        public int VideoFunction = 1;
        public string PageUrl = "NONE";
        public int ObjectEncoding = 0x00; //AMF0 is only supported atm


        private const int TransactionId = 1;

        public Connect(string ticket)
        {
            ApplyValues(ticket);
        }

        public void ApplyValues(string ticket)
        {
            var writer = new AmfWriter();

            writer.WriteString("connect");
            writer.WriteNumber(TransactionId);

            var obj = new AmfObject();
            obj.Strings.Add("app", App);
            obj.Strings.Add("flashver", FlashVersion);
            obj.Strings.Add("swfUrl", SwfUrl);
            obj.Strings.Add("tcUrl", ServerUrl);
            obj.Booleans.Add("fpad", Proxy);
            obj.Numbers.Add("audioCodecs", AudioCodecs);
            obj.Numbers.Add("videoCodecs", VideoCodecs);
            obj.Numbers.Add("videoFunction", VideoFunction);
            obj.Strings.Add("pageUrl", PageUrl);
            obj.Numbers.Add("objectEncoding", ObjectEncoding);

            writer.WriteObject(obj);
            writer.WriteString(ticket);
            Data = writer.GetByteArray();
        }
    }
}
*/