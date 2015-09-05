#include "CIPC_SETTINGS.h"

using namespace CIPC;

//相手先ＩＰアドレスの設定。
const char* SETTINGS::RemoteExchangerServer::IP = "192.168.11.6";
//相手先CIPSの制御ポート
const int SETTINGS::RemoteExchangerServer::Port = 12000;
//自分が使用するポート
const int SETTINGS::Port::myPort = 4002;

//自分のclientの名前
const wchar_t SETTINGS::myinfo::name[] = L"RC_KS_DATA";
const int SETTINGS::myinfo::name_length = sizeof(SETTINGS::myinfo::name) / sizeof(SETTINGS::myinfo::name[0]);
const int SETTINGS::myinfo::fps = FRAMERATE_CIPC;

//デフォルトのモード設定。送信モードではデータを送信する素子、受信モードではデータを受信する素子が利用する。
const CLIENT::MODE SETTINGS::MODE::Mode = CLIENT::MODE::Sender;



//connection comands バージョン固定の値
const int SETTINGS::CONNECTION_COMANDS::DEMANDS = 1;
const int SETTINGS::CONNECTION_COMANDS::END = 9;
const int SETTINGS::CONNECTION_COMANDS::MODE_SEND = 2;
const int SETTINGS::CONNECTION_COMANDS::MODE_RECEIVE = 3;
const int SETTINGS::CONNECTION_COMANDS::OK = 0;