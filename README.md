﻿
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

## クラス
<dl>
  <dt>config</dt>
  <dd>config: 設定ファイルとのやりとり</dd>
  
  <dt>info</dt>
  <dd>ChatInfo: 受信したコメントの情報</dd>
  <dd>numTaskInfo: セグメントファイルの情報</dd>
  <dd>RedistInfo: 引数から取得した情報</dd>
  <dd>TimeShiftConfig: タイムシフト録画に関する情報</dd>
  <dd>WatchingInfo: jikken情報</dd>
  
  <dt>rec</dt>
  <dd>AnotherEngineRecorder: 外部録画エンジンを使った録画</dd>
  <dd>ArgConcat: 引数にファイルが指定された場合の連結</dd>
  <dd>CookieGetter: Cookieの取得と同時にページの取得</dd>
  <dd>DeflateDecoder: jikken時の圧縮されたコメントの展開</dd>
  <dd>DropSegmentProcess: セグメント抜け時の処理</dd>
  <dd>FFMpegConcat: FFmpegを使った連結</dd>
  <dd>FFMpegRecord: FFmpegを使った録画</dd>
  <dd>FollowCommunity: コミュニティのフォロー</dd>
  <dd>Html5Recorder: html5ページからの録画</dd>
  <dd>IRecorderProcess: live2とnicocasの共通部分インターフェイス</dd>
  <dd>JikkenRecorder: nicocasに関するデータ取得など</dd>
  <dd>JikkenRecorderProcess: nicocas録画制御部分</dd>
  <dd>NotHtml5RecorderProcess: 旧配信用（廃止）</dd>
  <dd>OutputTimeShiftTsUrlList: タイムシフトURLリスト出力</dd>
  <dd>Record: ffmpegを使った録画</dd>
  <dd>RecordFromUrl: 録画開始前の放送状態の判定</dd>
  <dd>RecordingManager: 録画開始ボタンの動作定義的クラス</dd>
  <dd>RecordStateSetter: 録画状況の表示</dd>
  <dd>Reservation: 予約機能</dd>
  <dd>RtmpClient: RTMPでのタイムシフト時のデータ取得</dd>
  <dd>RtmpRecorder: RTMP録画</dd>
  <dd>ThroughFFMpeg: FFmpegでの変換処理</dd>
  <dd>TimeShiftCommentGetter: live2のタイムシフト時のコメント取得</dd>
  <dd>TimeShiftCommentGetter_jikken: nicocasのタイムシフト時のコメント取得</dd>
  <dd>WebSocketRecorder: live2のWebSocket関係</dd>
  
  <dt>util</dt>
  <dd>util: 小さなメソッド群</dd>
  <dd>RegGetter: Regexインスタンス取得</dd>
  <dd>ArgReader: 引数読み取り処理</dd>
  <dd>SourceInfoSerialize: ブラウザのCookie情報の保存と読み込み</dd>
  
  <dt>play</dt>
  <dd>Player: 視聴機能</dd>
  <dd>CommentPlayerWebSocket: 視聴機能時のコメント取得</dd>
  
  <dt>Logger</dt>
  <dd>TraceListener: ログ出力</dd>
  
  <dt>フォーム関係</dt>
  <dd>MainForm: メインフォーム</dd>
  <dd>optionForm: オプション画面のフォーム</dd>
  <dd>commentForm: 視聴機能時のコメント</dd>
  <dd>defaultFFplayController: デフォルトの視聴機能時のコントローラ</dd>
  <dd>TimeShiftOptionForm: タイムシフト設定画面</dd>
</dl>

## 開発環境
SharpDevelop4.4.2
（VisualStudioではビルドできませんでした。）
