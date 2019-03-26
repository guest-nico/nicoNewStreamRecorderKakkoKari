## ニコ生 新配信録画ツール（仮）  
  
ニコ生のHLS新配信を録画するツールです。  
このツールを改良していただいても、もし参考になるところがあれば  
ソースを使って別のツールを作っていただいても構いません。  
リファクタリング大歓迎です。  
  
製作中に見つけた[ニコ生新配信の仕様](siyou.md)を記してみました。  
  
## ライセンス  
SnkLib.App.CookieGetter  
CookieGetterSharp  
<https://github.com/namoshika/SnkLib.App.CookieGetter>  
<https://github.com/namoshika/CookieGetterSharp>  
Copyright (c) 2014 halxxxx, うつろ  
Copyright (c) 2014 namoshika.  
Released under the GNU Lesser GPL  
halxxxx様、うつろ様、にょんにょん様、炬燵犬様  
  
WebSocket4Net  
<https://github.com/kerryjiang/WebSocket4Net>  
kerryjiang様  
一部編集しております  
<https://github.com/guest-nico/WebSocket4Net_deflate>  
SuperSocket Apache License 2.0  
https://github.com/kerryjiang/SuperSocket/blob/master/LICENSE  
  
ffmpeg  
<http://www.ffmpeg.org/>  
GNU Lesser General Public License (_LGPL_) version 2.1  
同梱のffplayは一部編集しております  
<https://github.com/guest-nico/settableVolumeUsingNamedPipeFFplay>  
  
Json.NET  
<https://www.newtonsoft.com/json>  
  
RtmpSharp2  
<https://github.com/ashindlecker/RtmpSharp2>  
一部編集しております  
<https://github.com/guest-nico/RtmpSharp2_connectSendData>  
  
RTMPDump  
<https://rtmpdump.mplayerhq.hu/>  
Andrej Stepanchuk, Howard Chu, The Flvstreamer Team  
  
NAudio Microsoft Public License  
https://github.com/naudio/NAudio/blob/master/license.txt  
  
## 開発環境  
SharpDevelop4.4.2  
（VisualStudioではビルドできませんでした。）  