/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2018/09/05
 * Time: 4:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.IO.Compression;

namespace namaichi.rec
{
	/// <summary>
	/// Description of DeflateDecoder.
	/// </summary>
	public class DeflateDecoder {
		private MemoryStream ms = new MemoryStream();
		//private DeflateStream ds;
		private StreamReader br;
		public DeflateDecoder() {
			var ds = new DeflateStream(ms, CompressionMode.Decompress);
			br = new StreamReader(ds, System.Text.Encoding.UTF8);
			
		}
		public string decode(byte[] b) {
			ms.Position = 0;
			ms.SetLength(0);
			
			ms.Write(b, 0, b.Length);
			/*
			if (b.Length > 100)
				ms.Write(b, 4, b.Length - 4);
			else 
				ms.Write(b, 2, b.Length - 2);
			*/
			ms.Write(new byte[]{0,0,255,255}, 0, 4);
			if (b.Length % 13 != 0 && false) {
				var n = (((int)(b.Length / 13)) + 1) * 13 - b.Length;
				ms.Write(new byte[n], 0, n);
			}
			
			
			ms.Flush();
			//ms.Position -= b.Length;
			ms.Position = 0;
			
//			util.debugWriteLine(string.Join(" ", ms.ToArray()));
			//byte[] o = new byte[int.MaxValue];
			try {
				//ds.Read(o, 0, o.Length);
				//o = br.ReadBytes(int.MaxValue);
				//return System.Text.Encoding.UTF8.GetString(br.ReadBytes(int.MaxValue));
				return br.ReadToEnd();
			} catch (Exception e) {
				util.debugWriteLine("decode exception " + e.Message + e.StackTrace + b);
				//util.debugWriteLine("exception " + o);
				return null;
			}

		}
	}
}
