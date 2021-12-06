作成中<br>

vsは仮想に入れるといい。
osとダウンロード保存とinstで100Gはいる。
<br>
.csproj作り直さないと無理だけどビルドだけならdotnetも可能。1Gほどかな<br>
https://dotnet.microsoft.com/download/visual-studio-sdks<br>
sdk-5,net48でいいかな。<br>
dotnet add package cef.redist.x32 --version 86.0.24<br>
dotnet add package cef.redist.x64 --version 86.0.24<br>
dotnet add package CefSharp.Common --version 86.0.241<br>
dotnet add package CefSharp.WinForms --version 86.0.241<br>
これ忘れるとビルドできない<br>
\<PropertyGroup>\<CefSharpAnyCpuSupport>true\</CefSharpAnyCpuSupport>\</PropertyGroup><br>
<br>

flashが含まれるため<br>
実行ファイルの不特定多数配布は禁止します。<br>
ただしコード署名付、知り合いなどにサポートや責任もてるなら可能。<br>
(オレオレコード署名はもちろんダメ)<br>
自ビルドして使う分には問題ありませんが自己責任。<br>
実行ファイル公開は自らはしません。ライセンスや法律に触れ裁判や家宅捜索などなると厄介なので。<br>

実行ファイルはリネームして使うが基本。<br>
bat/ クライアントの登録<br>
ico/ 実行ファイル名と同じ、場所にすると表示される(サンプル)<br>
readme/list.txt 必要なファイル構成など<br>
readme/txt.ini 設定の説明<br>
UserData/実行ファイル名/ 自動で作成 Chromiumのデータ<br>

クライアント対応game 基本ゲソ経由<br>
League of Angels 2(air)<br>
League of Angels 3(air)<br>
World End Fantasy(flash.ocx)<br>
ドラグーン・ナイツ(air)<br>
ドラゴンアウェイクン(WebKit)<br>
戦国義風(Chromium)<br>

2021/02/11<br>
・ドラグーン・ナイツ追加<br>
・bool.Parseでエラーになりやすいので修正<br>
2021/03/11<br>
・Chromium: 87で直swfがheight=150pxになるのでbody.style.height=embed.style.height=を800以下になるようにメニューに追加<br>
embed.style.height=100%=150px<br>
・コマンドライン引数で既定のtab数で新プロセスになるようにした。<br>
 tabcont=0,1は1プロセス毎<br>
・CefSharp,Chromiumファイルはx86|x64フォルダベースですが実行ファイル名に.x86|x64-86.exeにするとそのフォルダベースに。<br>
2021/12/06<br>
・プロキシを起動中on/offできるように(ipも変えれる)
・プロセス数でUserDataを変更。(同じだと問題あるので)
・他多少修正<br>
