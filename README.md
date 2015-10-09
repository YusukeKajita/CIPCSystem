# CIPCSystem

## 特徴
* CIPCシステムは、UDPを用いてあらゆるプロセス間の通信を実現します。
* UDPであるためIPを用いた遠隔通信も実現します。この際CIPCServerが中継通信を実現するため、P2P通信を行うことが可能です。
* CIPCSystemの関連ソフトを使用することでさまざまな機能を副次的に利用できます。 

## CIPCServer
　CIPCServerはクライアントから要求されると接続を保持し、クライアントとCIPC通信という独自のプロトコルによって実現される通信を行います。  
　CIPCServerは以下の二種が存在します。  
　* CIPCServer（GUIBase）
　* CIPCServer_Console
　CIPCServer(GUIBase)で実現されるCIPC通信は１．送信　２．受信の二種類になります。今後のアップデートによりそのほかの通信方式を実現する可能性があります。  
　これに対してCIPCServer_Consoleで実現されるCIPC通信は１．送信　２．受信　に加え、３．双方向　４．ダイレクト通信の４方式を使用することが可能となっております。  
　いかにその手順を記述します。  
### セットアップ方法
* 起動
　
　
