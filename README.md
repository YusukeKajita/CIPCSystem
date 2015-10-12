# CIPCSystem

## 特徴
* CIPCシステムは、UDPを用いてあらゆるプロセス間の通信を実現します。
* UDPであるためIPを用いた遠隔通信も実現します。この際CIPCServerが中継通信を実現するため、P2P通信を行うことが可能です。
* CIPCSystemの関連ソフトを使用することでさまざまな機能を副次的に利用できます。  

## 使い方  
Marge or Pull する度に毎回VisualStudio 2013以上でビルドしてください．  

## CIPCServer
　CIPCServerはクライアントから要求されると接続を保持し、クライアントとCIPC通信という独自のプロトコルによって実現される通信を行います。  
　CIPCServerは以下の二種が存在します。  
* CIPCServer（GUIBase）  
CIPCServer(GUIBase)で実現されるCIPC通信は１．送信　２．受信の二種類になります。今後のアップデートによりそのほかの通信方式を実現する可能性があります。 
* CIPCServer_Console  
　これに対してCIPCServer_Consoleで実現されるCIPC通信は１．送信　２．受信　に加え、３．双方向　４．ダイレクト通信の４方式を使用することが可能となっております。  
　以下にその通信の手順を記述します。  
* CIPCServerのセットアップ方法  
特に必要はありません。Publishフォルダの中にインストーラ（setup.exe）がありますが、releaseフォルダに入っているCentralInterProcessCommunicationServer.exeを実行することで起動することができます。  
この時、CIPCServerを起動するPCに到達することができるノードにあるクライアントのみCIPCSeverを利用することができます。  
    * 同じPC間で利用する場合  
    特別な設定は必要ありません。この場合、クライアントから見たServerのノードは127.0.0.1:(ポート番号)となります。  
    * 同じルータ間で利用する場合
    サーバーを立てているPCのローカルIPアドレスをクライアントにて指定することで接続が可能です。ローカルIPアドレスはcmd.exeでipconfig.exeを実行すると確認することができます。  
    * グローバルIPアドレスを用いた通信  
    現在記述中です。  
* CIPCServerの画面の見方  
    * 制御用ポート : CIPCServerが現在使用している制御用ポート。各クライアントはこのポートにアクセスすることで接続が確立される。  
    * 接続中のプロセス : 現在接続中のクライアントが表示されます。各クライアントに対してどのポートを割り当てているかが確認できます。  

## ArduinoCommunication
